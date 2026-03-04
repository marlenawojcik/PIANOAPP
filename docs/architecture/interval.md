
# Architektura modułu: Moduł Interwałów (IntervalWindow)

## 1. Cel modułu

Moduł Interwałów (Interval Trainer) służy do nauki i rozpoznawania interwałów muzycznych ze słuchu. Umożliwia użytkownikowi osłuchanie się z brzmieniem różnych odległości między dźwiękami (od prymy do oktawy) i sprawdzenie swojej wiedzy. Moduł oferuje dwa tryby: "Learn" (nauka/zgadywanie) i "Listen" (swobodne odsłuchiwanie).

## 2. Zakres funkcjonalny (powiązanie z User Stories)

- **US-05** — Jako użytkownik chcę wybrać tryb nauki interwałów (Learn), w którym po odsłuchaniu interwału mogę wybrać jego nazwę z listy i sprawdzić, czy mam rację.
- **US-06** — Jako użytkownik chcę w trybie Learn zobaczyć statystyki moich poprawnych i błędnych odpowiedzi.
- **US-07** — Jako użytkownik w trybie Learn chcę móc ponownie odsłuchać ten sam interwał, jeśli go nie zapamiętałem.
- **US-08** — Jako użytkownik w trybie Learn chcę móc odsłuchać interwał w wersji "rozłożonej" (arpeggio), aby lepiej go przeanalizować.
- **US-09** — Jako użytkownik chcę wybrać tryb "Listen", w którym po kliknięciu na nazwę interwału mogę go odsłuchać, aby po prostu poznawać ich brzmienie.
- **US-10** — Jako użytkownik w trybie Listen chcę mieć te same opcje ponownego odsłuchania (Play Again, Play Spread Version).

## 3. Granice modułu (co wchodzi / co nie wchodzi)

### 3.1 Moduł odpowiada za
- Wyświetlenie interfejsu z listą interwałów (pryma, sekunda mała, sekunda wielka itd.).
- Implementację dwóch trybów pracy: "Learn" (`IntervalLearnWindow`) i "Listen" (`IntervalListenWindow`).
- Generowanie losowych interwałów w trybie Learn (pierwszy dźwięk losowy, drugi o zadany interwal wyżej).
- Odtwarzanie interwałów (sekwencyjnie lub rozłożone) za pomocą `ISoundStrategy`.
- Zliczanie poprawnych i błędnych odpowiedzi w trybie Learn.
- Przechowywanie tymczasowych danych ostatnio odtworzonego interwału (pierwszy dźwięk, rozmiar interwału).

### 3.2 Moduł nie odpowiada za
- Grę na klawiaturze (to rola modułu klawiatury).
- Wizualizację nut na pięciolinii.

## 4. Struktura kodu modułu

- `IntervalWindow.xaml`: Definiuje wspólny układ okna dla obu trybów, zawierający dwa panele z przyciskami interwałów.
- `IntervalWindow.xaml.cs`: Abstrakcyjna klasa bazowa dla okien interwałowych. Zawiera wspólne elementy, takie jak: `midiOut`, `SoundStrategy`, metody `OnPlayAgainIntervalClick`, `OnPlayAgainSpreadIntervalClick` oraz wirtualną metodę `OnIntervalButtonClick`.
- `IntervalLearnWindow.cs`: Klasa pochodna implementująca tryb nauki. Dodaje do interfejsu przyciski "Play Interval", "Correct Answer", "Check Statistics" oraz implementuje ich logikę.
- `IntervalListenWindow.cs`: Klasa pochodna implementująca tryb słuchania. Nadpisuje metodę `OnIntervalButtonClick`, aby od razu odtwarzać interwał po kliknięciu na jego nazwę.
- `IntervalSelectionWindow.cs`: Okno wyboru trybu (Learn/Listen), które po wyborze otwiera odpowiednie okno.

## 5. Zewnętrzne API wykorzystywane przez moduł

Moduł nie korzysta z zewnętrznych API. Do generowania dźwięku używa `ISoundStrategy` i `MidiManager`.

## 6. Model danych modułu

### 6.1 Encje bazodanowe (tabele)

Moduł nie posiada własnych encji bazodanowych. Wykorzystuje encję `Note` z biblioteki `NoteLibrary`.

### 6.2 Obiekty domenowe (bez tabel w bazie)

- **`intervals` (`Dictionary<string, int>`)**: Statyczny słownik mapujący nazwy interwałów wyświetlane na przyciskach (np. "2>") na ich wartości w półtonach (np. 1). Używany do konwersji między reprezentacją tekstową a numeryczną.
- **`copyMidi` (`int`)**: Przechowuje numer MIDI pierwszego dźwięku ostatnio wygenerowanego interwału.
- **`copyInterval` (`int`)**: Przechowuje rozmiar (w półtonach) ostatnio wygenerowanego interwału. Inicjalizowany wartością `-1`, co sygnalizuje brak aktywnego interwału.
- **`goodAns` / `badAns` (`int`)**: Liczniki poprawnych i błędnych odpowiedzi w trybie Learn, utrzymywane w pamięci przez cały czas życia okna.

### 6.3 Relacje i przepływ danych

1.  **Tryb Learn – generowanie interwału**: Użytkownik klika "Play Interval". Metoda `OnPlayIntervalClick` losuje `midi` (np. 60) i `interval` (np. 4). Tworzy nuty `firstNote` i `secondNote` za pomocą `NoteLibrary.GetNote(midi)` i `NoteLibrary.GetNote(midi + interval)`. Zapisuje te wartości w `copyMidi` i `copyInterval`. Następnie odtwarza dźwięk.
2.  **Tryb Learn – odpowiedź użytkownika**: Użytkownik klika na jeden z przycisków interwałów (np. "3 - Major Third"). Wywoływana jest metoda `OnIntervalButtonClick` (z klasy bazowej). Pobiera ona tekst przycisku, wyciąga z niego pierwsze dwa znaki (np. "3 ") i szuka w słowniku `intervals` odpowiadającej mu wartości w półtonach (4). Porównuje tę wartość z `copyInterval`. Jeśli są zgodne – `goodAns++`, w przeciwnym razie `badAns++`. Wyświetlany jest odpowiedni komunikat.
3.  **Tryb Listen – odsłuch interwału**: Użytkownik klika na przycisk interwału (np. "5 - Perfect Fifth"). Wywoływana jest nadpisana metoda `OnIntervalButtonClick` z `IntervalListenWindow`. Pobiera ona wartość interwału ze słownika `intervals` (dla "5 " jest to 7), losuje `midi`, tworzy nuty i odtwarza je. Nie zapisuje wyniku ani nie oczekuje odpowiedzi.

## 7. Przepływ danych w module

**Scenariusz (US-05, US-07, US-08): Nauka interwałów w trybie Learn.**

1.  Użytkownik w oknie `IntervalSelectionWindow` klika "Learn". Otwiera się `IntervalLearnWindow`.
2.  Użytkownik klika "Play Interval". (Przepływ jak w punkcie 7.3.1). Zostaje odtworzony losowy interwał (np. tercja wielka w górę od C4).
3.  Użytkownik nie jest pewien odpowiedzi i klika "Play Again". Wywoływana jest metoda `OnPlayAgainIntervalClick` (z klasy bazowej), która odtwarza ten sam interwał (korzystając z zapisanych `copyMidi` i `copyInterval`).
4.  Użytkownik nadal nie jest pewien i klika "Play Spread Version". Wywoływana jest `OnPlayAgainSpreadIntervalClick`, która odtwarza obie nuty jednocześnie.
5.  Użytkownik klika "Correct Answer". Wywoływana jest metoda `OnCorrectAnswerClick`, która wyświetla komunikat z prawidłową nazwą interwału (np. "Correct interval is: 3 ").
6.  Użytkownik klika na przycisk interwału (np. "3 - Major Third").
    - `OnIntervalButtonClick` porównuje wartość z przycisku (4) z `copyInterval` (4) – są zgodne.
    - `goodAns` jest inkrementowane.
    - Wyświetlany jest komunikat "Correct Answer".
7.  Użytkownik klika "Check Statistics". Wywoływana jest `OnCheckClick`, która wyświetla okno z liczbą `goodAns` i `badAns` oraz procentem poprawnych odpowiedzi.

## 8.  Ograniczenia, ryzyka, dalszy rozwój
Ograniczenia: Interwały są zawsze odtwarzane w górę od pierwszego dźwięku. Brak możliwości odtwarzania interwałów w dół.

Ryzyka: Użycie Thread.Sleep(2000) w metodach odtwarzania blokuje wątek UI na czas trwania pauzy między dźwiękami, co może chwilowo zawiesić aplikację.

Dalszy rozwój:

Dodanie możliwości wyboru zakresu dźwięków dla pierwszego tonu.

Dodanie opcji odtwarzania interwałów w dół.

Zastąpienie Thread.Sleep asynchronicznym Task.Delay, aby nie blokować UI.

Wzbogacenie trybu Learn o wizualizację interwału na pięciolinii.

Zapis statystyk do pliku, aby można było śledzić postępy w czasie.