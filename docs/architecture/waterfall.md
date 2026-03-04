
# Architektura modułu: Moduł Waterfall (WaterfallKeyboardWindow)

## 1. Cel modułu

Moduł Waterfall implementuje interaktywny tryb nauki gry, znany jako "spadający bloczek" lub "sight-reading". Jego celem jest wizualne przedstawienie utworu w postaci kolorowych prostokątów (bloczków), które przesuwają się z góry ekranu w dół, w kierunku klawiatury. Użytkownik musi nacisnąć odpowiedni klawisz w momencie, gdy bloczek dotrze do klawiatury. Moduł integruje się z systemem oceny (`ScoringManager`), aby śledzić postępy użytkownika.

## 2. Zakres funkcjonalny (powiązanie z User Stories)

- **US-11** — Jako użytkownik chcę wybrać utwór i uruchomić go w trybie "Waterfall", aby uczyć się go wizualnie.
- **US-12** — Jako użytkownik chcę widzieć "spadające" bloczki reprezentujące nuty, aby wiedzieć, który klawisz nacisnąć.
- **US-13** — Jako użytkownik chcę, aby prędkość spadania bloczków była zsynchronizowana z tempem utworu.
- **US-14** — Jako użytkownik chcę móc zmienić tempo utworu (przyspieszyć/zwolnić), aby ćwiczyć w dogodnym dla siebie rytmie.
- **US-15** — Jako użytkownik po zakończeniu utworu chcę zobaczyć mój wynik (np. procent poprawnie trafionych nut).

## 3. Granice modułu (co wchodzi / co nie wchodzi)

### 3.1 Moduł odpowiada za
- Oczyszczenie canvas i utworzenie bloczków dla wszystkich elementów utworu na podstawie danych z `currentSong`.
- Obliczenie opóźnienia (`delayMs`) dla każdego bloczka na podstawie czasu rozpoczęcia nuty w utworze i aktualnego tempa (`change`), aby bloczki pojawiały się w odpowiednim momencie.
- Animację bloczków (przesuwanie w dół) za pomocą `DispatcherTimer` dla każdego bloczka.
- Synchronizację animacji z odtwarzaniem dźwięku utworu (w metodzie `OnPlayButtonClick`).
- Przekazanie naciśniętych nut do `scoringManager` w celu oceny.

### 3.2 Moduł nie odpowiada za
- Zarządzanie listą utworów (to rola `SongWindow`).
- Logikę rysowania nut na statycznej pięciolinii (to rola `MusicStaff` w `KeyboardWithSongWindow`).
- Samo odtwarzanie dźwięku – deleguje to do `soundStrategy` w pętli odtwarzania.

## 4. Struktura kodu modułu

- `WaterfallKeyboardWindow.cs`: Klasa pochodna po `PianoKeyboardWindow`. Zawiera nadpisane metody obsługi zdarzeń przycisków (`OnStartButtonClick`, `OnPlayButtonClick`, itp.) oraz kluczową metodę `CreateFallingBlock`, która odpowiada za generowanie i animację bloczków.


## 5. Zewnętrzne API wykorzystywane przez moduł

Moduł nie korzysta z zewnętrznych API.

## 6. Model danych modułu

### 6.1 Encje bazodanowe (tabele)

Moduł nie wprowadza własnych encji bazodanowych. Wykorzystuje encje wspólne: `Song`, `Note` oraz `ScoringManager`.

### 6.2 Obiekty domenowe (bez tabel w bazie)

- **Bloczek (`Grid`)**: Wizualna reprezentacja nuty na canvas. Jest to kontener WPF (`Grid`) zawierający `Rectangle` (tło) i `TextBlock` (nazwa nuty). Jego wysokość jest uzależniona od długości nuty (`note.Duration`), a pozycja pozioma od nazwy nuty (wyznaczana przez `GetNoteXPosition`).
- **Timer animacji (`DispatcherTimer`)**: Każdy bloczek posiada swój własny timer, który w regularnych odstępach czasu (co 5 ms) aktualizuje jego pozycję pionową na canvas. Timer zatrzymuje się i usuwa bloczek, gdy ten opuści obszar canvas.
- **`delayMs` (zmienna lokalna)**: Obliczone opóźnienie dla każdego bloczka, które określa, po jakim czasie od uruchomienia metody `CreateFallingBlock` bloczek ma się pojawić i rozpocząć animację. Jest to kluczowe dla synchronizacji z dźwiękiem.

### 6.3 Relacje i przepływ danych

1.  **Start**: Użytkownik klika "Play" lub "Start". Wywoływana jest metoda `CreateFallingBlock`, która iteruje po `currentSong.Elements`.
2.  **Tworzenie bloczka**: Dla każdego elementu typu `Note` tworzony jest bloczek. Jego początkowa pozycja Y jest tuż nad górną krawędzią canvas (`-block.Height`), a pozycja X zależy od nazwy nuty.
3.  **Obliczenie opóźnienia**: Opóźnienie jest obliczane jako `song.StartingTime(elements)/change`, co zapewnia, że bloczek pojawi się w momencie, w którym nuta powinna zostać zagrana, uwzględniając zmienione tempo.
4.  **Uruchomienie animacji**: Dla każdego bloczka tworzony i konfigurowany jest `DispatcherTimer`. Jeśli `delayMs > 0`, timer uruchamiany jest asynchronicznie po upływie tego opóźnienia (`Task.Delay`). W przeciwnym razie timer startuje od razu.
5.  **Animacja**: W zdarzeniu `Tick` timera aktualizowana jest właściwość `Canvas.Top` bloczka (`currentY += step`). `step` zależy od prędkości (`speed`) i interwału timera.
6.  **Usunięcie bloczka**: Gdy `currentY` przekroczy `canvas.ActualHeight`, timer jest zatrzymywany, a bloczek usuwany z canvas.
7.  **Ocena**: Równolegle do animacji, w nadpisanej metodzie `OnKeyMouseDown`, naciśnięte nuty są przekazywane do `scoringManager`. Po zakończeniu utworu (lub ręcznym kliknięciu "Finish") `scoringManager` oblicza i wyświetla wynik.

## 7. Przepływ danych w module

**Scenariusz (US-11, US-13, US-15): Uruchomienie utworu w trybie Waterfall i uzyskanie wyniku.**

1.  Użytkownik wybiera utwór i w oknie wyboru trybu klika "Waterfall". Otwiera się `WaterfallKeyboardWindow` z wybranym utworem jako `currentSong`.
2.  Użytkownik klika przycisk "Play".
    - Nadpisana metoda `OnPlayButtonClick` ustawia `isPlaying = true`.
    - Wywołuje `CreateFallingBlock`, która tworzy bloczki i uruchamia ich animacje z opóźnieniami dopasowanymi do tempa.
    - Równolegle, w tle uruchamiane jest zadanie, które iteruje po `currentSong.Elements` i odtwarza dźwięk (z klasy bazowej `PianoKeyboardWindow`), również z uwzględnieniem tempa (`change`).
3.  Użytkownik naciska klawisze, starając się trafić w moment, gdy bloczek dotrze do dołu ekranu.
    - Nadpisana metoda `OnKeyMouseDown` odtwarza dźwięk (wywołując `base.OnKeyMouseDown`) i dodaje naciśniętą nutę do `scoringManager`.
4.  Po zakończeniu utworu (lub gdy użytkownik kliknie "Finish") wywoływana jest metoda `OnFinishButtonClick` (z klasy bazowej).
    - `scoringManager.ComparePressedNotes(currentSong)` porównuje listę naciśniętych nut z nutami w utworze.
    - `scoringManager.CalculateMatch(currentSong)` oblicza procent zgodności.
    - Wyświetlane jest okno `MessageBox` z wynikiem.

## 8. Ograniczenia, ryzyka, dalszy rozwój
Ograniczenia: Użycie osobnego timera (DispatcherTimer) dla każdego bloczka może być nieefektywne przy bardzo długich utworach z dużą liczbą nut. Lepszym rozwiązaniem byłby jeden globalny timer, który aktualizowałby pozycje wszystkich bloczków.


Ryzyka: Obliczenia prędkości (speed) i kroku (step) oparte na stałej 1000.0*change oraz stałym interwale timera (5ms) mogą prowadzić do niedokładności w synchronizacji, zwłaszcza przy ekstremalnych zmianach tempa.

Dalszy rozwój:
Optymalizacja animacji poprzez użycie pojedynczego timera i Storyboard lub CompositionTarget.Rendering.

Dodanie wizualnego wskaźnika (np. linii) na dole ekranu, który precyzyjnie wskazuje moment, w którym należy nacisnąć klawisz.

Ulepszenie systemu oceny o informację zwrotną na temat timing'u (czy nuta została naciśnięta za wcześnie, za późno, czy idealnie).

Możliwość przewinięcia animacji do tyłu lub zatrzymania jej w dowolnym momencie.