# Architektura modułu: Moduł Klawiatury (PianoKeyboardWindow)

## 1. Cel modułu

Moduł Klawiatury jest centralnym punktem aplikacji, odpowiedzialnym za interakcję użytkownika z wirtualnym instrumentem. Jego główne zadania to: wyświetlenie interaktywnej klawiatury, generowanie dźwięku po naciśnięciu klawisza (przy użyciu strategii dźwięku), wizualizacja naciskanych klawiszy na pięciolinii (w klasie pochodnej `DefaultKeyboardWindow`) oraz zarządzanie nagrywaniem prostych melodii.

## 2. Zakres funkcjonalny (powiązanie z User Stories)

- **US-01** — Jako użytkownik chcę widzieć interaktywną klawiaturę pianina, abym mógł na niej grać myszką.
- **US-02** — Jako użytkownik chcę słyszeć dźwięk po naciśnięciu klawisza, aby symulować grę na prawdziwym instrumencie.
- **US-03** — Jako użytkownik chcę móc zmienić instrument (np. na organy lub skrzypce), aby urozmaicić brzmienie.
- **US-04** — Jako użytkownik chcę nagrać prostą melodię graną na klawiaturze, abym mógł ją później odtworzyć.
- **US-17** — Jako użytkownik chcę widzieć zapis nutowy granych przeze mnie dźwięków na pięciolinii (w trybie domyślnym), aby uczyć się czytania nut.

## 3. Granice modułu (co wchodzi / co nie wchodzi)

### 3.1 Moduł odpowiada za
- Generowanie i wyświetlanie klawiszy pianina dla zakresu 7 oktaw.
- Obsługę zdarzeń naciśnięcia i puszczenia klawisza myszą.
- Inicjowanie i zatrzymywanie dźwięku poprzez interfejs `ISoundStrategy`.
- Przechowywanie tymczasowej listy klikniętych nut (`clickedButton`).
- Nagrywanie sekwencji zdarzeń naciśnięcia/zwolnienia klawiszy w celu utworzenia obiektu `Song`.
- (W klasie `DefaultKeyboardWindow`) Rysowanie naciskanych nut na pięciolinii.

### 3.2 Moduł nie odpowiada za
- Odtwarzanie całych utworów z wyznaczonym tempem (to rola przycisków "Play" w klasach pochodnych).
- Zarządzanie listą dostępnych utworów (to rola `SavedMelodiesCollection` i `SongWindow`).
- Wizualizację w trybie Waterfall (to rola `WaterfallKeyboardWindow`).
- Logikę trenera interwałów (to rola `IntervalWindow`).

## 4. Struktura kodu modułu

- `PianoKeyboardWindow.xaml`: Definiuje układ okna zawierający panel klawiatury (`KeyboardPanel`), canvas na pięciolinię (`staffCanvas`) oraz panel przycisków sterujących.
- `PianoKeyboardWindow.xaml.cs`: Główna klasa bazowa dla okien z klawiaturą. Zawiera logikę generowania klawiatury, obsługę zdarzeń myszy, nagrywanie oraz metody wirtualne do nadpisania w klasach pochodnych (np. `OnPlayButtonClick`).
- `DefaultKeyboardWindow.cs`: Klasa pochodna, która implementuje domyślne zachowanie po naciśnięciu klawisza – rysowanie nuty na pięciolinii (`AddElementsToStaff`).

## 5. Interfejs modułu

Moduł nie udostępnia publicznego API w postaci endpointów sieciowych. Jego interfejs stanowią publiczne metody i właściwości klas, które mogą być wykorzystywane przez inne moduły.

## 6. Zewnętrzne API wykorzystywane przez moduł

Moduł nie korzysta z zewnętrznych API sieciowych. Komunikuje się bezpośrednio z systemowym syntezatorem MIDI za pośrednictwem biblioteki **NAudio**, która jest opakowaniem dla niskopoziomowych sterowników MIDI w systemie Windows.

## 7. Model danych modułu

### 7.1 Encje bazodanowe (tabele)

Moduł nie posiada własnych encji bazodanowych. Wykorzystuje encje wspólne, takie jak `Note` i `Song`.

### 7.2 Obiekty domenowe (bez tabel w bazie)

- **`clickedButton` (`List<Note>`)**: Tymczasowa lista przechowująca nuty naciśnięte przez użytkownika w bieżącej sesji (np. od momentu otwarcia okna). Służy do wizualizacji w `DefaultKeyboardWindow`.
- **`recordedElements` (`List<MusicElement>`)**: Prywatna lista w `PianoKeyboardWindow`, używana do gromadzenia nut i pauz podczas nagrywania. Na jej podstawie tworzony jest nowy obiekt `Song`.

### 7.3 Relacje i przepływ danych

1.  **Naciśnięcie klawisza**: Użytkownik klika `Button` reprezentujący nutę. Zdarzenie `OnKeyMouseDown` pobiera obiekt `Note` z właściwości `Tag` przycisku.
2.  **Generowanie dźwięku**: Obiekt `Note` jest przekazywany do metody `StartTone` aktualnej strategii `soundStrategy`.
3.  **Nagrywanie**: Jeśli flaga `isRecording` jest `true`, czas naciśnięcia (`DateTime.Now`) jest zapisywany w `keyDownTimes`. Różnice czasowe między kolejnymi zdarzeniami są wykorzystywane do tworzenia obiektów `Note` i `Pause`, które trafiają do `recordedElements`.
4.  **Rysowanie na pięciolinii (w `DefaultKeyboardWindow`)**: Po każdym naciśnięciu, nuta dodawana jest do listy `clickedButton`. Następnie wywoływana jest metoda `AddElementsToStaff`, która czyści canvas i rysuje wszystkie nuty z listy `clickedButton`, pobierając ich pozycję Y z `GetNoteYPosition`.

## 8. Przepływ danych w module

**Scenariusz (US-04): Użytkownik nagrywa krótką melodię.**

1.  Użytkownik klika przycisk "Record". Rozpoczyna się nagrywanie (`isRecording = true`), a lista `recordedElements` jest czyszczona.
2.  Użytkownik naciska klawisz `C4`. Zdarzenie `OnKeyMouseDown` jest wywoływane.
    - Odtwarzany jest dźwięk `C4`.
    - Do listy `keyDownTimes` dodawany jest bieżący czas.
    - Sprawdzana jest różnica czasu od ostatniego `keyUpTimes` – jeśli jest większa od 0, do `recordedElements` dodawana jest nowa `Pause` o tym czasie.
3.  Użytkownik puszcza klawisz `C4`. Zdarzenie `OnKeyMouseUp` jest wywoływane.
    - Dźwięk `C4` jest zatrzymywany.
    - Do listy `keyUpTimes` dodawany jest bieżący czas.
    - Obliczana jest różnica czasu od ostatniego `keyDownTimes` (dla `C4`) i na jej podstawie tworzony jest nowy obiekt `Note` (`C4` o odpowiednim `Duration`), który jest dodawany do `recordedElements`. Wpis `keyDownTimes` dla `C4` jest usuwany.
4.  Użytkownik powtarza kroki 2-3 dla kolejnych nut (`E4`, `G4`).
5.  Użytkownik ponownie klika przycisk "Record". Nagrywanie zostaje zakończone (`isRecording = false`).
6.  Pojawia się okno dialogowe z prośbą o podanie nazwy melodii.
7.  Po podaniu nazwy, tworzony jest nowy obiekt `Song` (z tytułem i listą `recordedElements`), który jest dodawany do `SavedMelodiesCollection.SavedMelodies`. Wyświetlany jest komunikat o sukcesie.

## 8. Ograniczenia, ryzyka, dalszy rozwój

Ograniczenia: Rysowanie na pięciolinii w DefaultKeyboardWindow jest bardzo proste – wyświetla jedynie wypełnione nuty (ćwierćnuty) bez uwzględniania ich rzeczywistej długości trwania. Nie obsługuje pauz.

Ryzyka: Nagrywanie oparte na różnicach czasu DateTime.Now może być niedokładne przy bardzo szybkim graniu ze względu na ograniczenia rozdzielczości timera systemowego.

Dalszy rozwój:

Wzbogacenie wizualizacji na pięciolinii w trybie domyślnym o poprawne rysowanie różnych wartości rytmicznych (całe nuty, półnuty, ósemki itp.) oraz pauz.

Ulepszenie mechanizmu nagrywania o precyzyjniejszy timer (np. Stopwatch).

Dodanie możliwości odtwarzania nagranej melodii bezpośrednio z poziomu okna klawiatury.