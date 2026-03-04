# Architektura aplikacji (wspólna) — T
---

## 1. Cel i zakres architektury

Niniejszy dokument opisuje wspólne elementy architektury aplikacji PIANOAPP. Zawiera informacje o ogólnej strukturze systemu, zastosowanym stosie technologicznym, konwencjach przyjętych w kodzie oraz wspólnych mechanizmach, takich jak obsługa dźwięku MIDI. Dokument ten stanowi punkt wyjścia do zrozumienia całości projektu, podczas gdy szczegółowe opisy poszczególnych modułów funkcjonalnych znajdują się w dedykowanych plikach w katalogu `doc/architecture/`.

## 2. Widok systemu jako całości

1.  **Warstwa Prezentacji (UI)**: Zdefiniowana w plikach `.xaml`. Okna takie jak `MainWindow`, `PianoKeyboardWindow` i ich pochodne stanowią interfejs użytkownika.
2.  **Warstwa Logiki Aplikacji (Code-behind)**: Pliki `.xaml.cs` zawierają logikę sterującą widokami, obsługę zdarzeń użytkownika oraz komunikację z warstwą domenową.
3.  **Warstwa Domeny**: Klasy takie jak `Song`, `Note`, `Pause`, `MusicStaff`, `ScoringManager` reprezentują kluczowe pojęcia dziedziny (muzyka, utwory, ocena postępów) i zawierają logikę biznesową.
4.  **Warstwa Dostępu do Danych (symulowana)**: Aplikacja nie korzysta z zewnętrznej bazy danych. Utwory domyślne są przechowywane w statycznej liście `SavedMelodiesCollection.DefaultSongs`. Nagrane melodie przechowywane są w sesji aplikacji w `SavedMelodiesCollection.SavedMelodies`.
5.  **Integracja Zewnętrzna (MIDI)**: Komunikacja z syntezatorem MIDI systemu Windows odbywa się za pomocą biblioteki **NAudio**. Menedżer `MidiManager` udostępnia współdzielone połączenie MIDI.

---

## 3. Konwencje i standardy w repozytorium

### 3.1 Struktura katalogów 
- `/`: Główny katalog repozytorium zawierający pliki `.csproj`, rozwiązanie `.sln` oraz kod źródłowy (`.cs`, `.xaml`).
- `/Images/`: Katalog z zasobami graficznymi (ikony, tła).
- `/doc/`: Główny katalog dokumentacji projektu.
    - `/doc/architecture/`: Dokumentacja poszczególnych modułów.
    - `/doc/assets/`: Zasoby do dokumentacji (screenshoty, diagramy, raporty).



### 3.2 Wspólne biblioteki / utilities

- **`MidiManager`**: Klasa statyczna zarządzająca współdzielonym obiektem `MidiOut`, zapewniająca, że w całej aplikacji używane jest to samo połączenie MIDI.
- **`NoteLibrary`**: Statyczna fabryka i repozytorium nut. Inicjalizuje wszystkie nuty w zakresie MIDI (0-9 oktawa) i udostępnia je na podstawie nazwy lub numeru MIDI. Stanowi centralne miejsce do pozyskiwania obiektów `Note`.
- **`DesignerClass`**: Klasa pomocnicza zawierająca statyczne metody do tworzenia spójnych wizualnie elementów interfejsu (np. stylów, przycisków) w kodzie C#.
- **`SavedMelodiesCollection`**: Statyczna kolekcja przechowująca domyślne utwory (`DefaultSongs`) oraz utwory nagrane przez użytkownika w bieżącej sesji (`SavedMelodies`).



## 4. Przepływ danych (Data Flow)
Ogólny przepływ danych w aplikacji można opisać następująco:

Interakcja użytkownika: Użytkownik klika w interfejsie (np. klawisz pianina, przycisk "Play").

Obsługa zdarzenia: Kod w pliku xaml.cs przechwytuje zdarzenie.

Logika domenowa: Wywoływane są odpowiednie metody na obiektach domenowych (np. soundStrategy.PlayTone(note), scoringManager.AddPressedNote(note)).

Pobranie danych: W razie potrzeby dane są pobierane z repozytoriów, takich jak NoteLibrary (dla nut) lub SavedMelodiesCollection (dla utworów).

Aktualizacja stanu: Stan aplikacji jest aktualizowany (np. lista clickedButton w klawiaturze).

Aktualizacja widoku: Interfejs użytkownika jest odświeżany (np. rysowanie nuty na staffCanvas).

Synchronizacja (opcjonalnie): W przypadku trybu Waterfall, w tle uruchamiane są zadania (Task.Run) i timery (DispatcherTimer) do animacji i odtwarzania dźwięku, które komunikują się z głównym wątkiem UI w celu aktualizacji pozycji bloczków.

---

## 5. Model danych (część wspólna)



### 5.1 Zakres modelu danych wspólnego

Wspólny model danych w PIANOAPP obejmuje encje, które są fundamentalne dla działania aplikacji i wykorzystywane przez wiele jej modułów.


### 5.2 Encje wspólne i ich odpowiedzialność

Encja: MusicElement (klasa abstrakcyjna)

Rola: Bazowa klasa dla wszystkich elementów muzycznych (nut i pauz). Definiuje wspólne właściwości, takie jak czas trwania (Duration), pozycja na pięciolinii (Position), typ elementu (Type) i nazwę (Name).

Wykorzystanie: Jest podstawą dla list Elements w utworze (Song). Dzięki polimorfizmowi, utwór może przechowywać mieszankę nut i pauz.

Relacje: Dziedziczą po niej Note i Pause.

Encja: Note

Rola: Reprezentuje pojedynczą nutę. Zawiera numer MIDI (MidiNum), który jest kluczowy do generowania dźwięku. Jej instancje są tworzone przez NoteLibrary.

Wykorzystanie: Używana przez moduł klawiatury (PianoKeyboardWindow) do identyfikacji klikniętego klawisza, przez ISoundStrategy do odtwarzania dźwięku, przez MusicStaff do rysowania na pięciolinii, przez WaterfallKeyboardWindow do tworzenia spadających bloczków oraz przez ScoringManager do oceny gry.

Encja: Song

Rola: Kontener dla sekwencji MusicElement (utworu). Przechowuje tytuł (Title), listę elementów (Elements), całkowitą długość (Length) oraz strategię dźwięku (Strategy) i nazwę instrumentu (Instrument), które mogą być powiązane z utworem.

Wykorzystanie: Stanowi podstawową jednostkę danych dla modułów SongWindow, KeyboardWithSongWindow i WaterfallKeyboardWindow. Jest przechowywana w SavedMelodiesCollection.

Kolekcja: SavedMelodiesCollection

Rola: Statyczne repozytorium w pamięci dla wszystkich utworów w aplikacji.

Wykorzystanie: DefaultSongs dostarcza predefiniowane utwory do nauki. SavedMelodies przechowuje utwory nagrane przez użytkownika w bieżącej sesji. Moduły odczytują stąd listę utworów do wyświetlenia.

## 6. Cross-cutting concerns (wspólne aspekty)

### 6.1 Konfiguracja i sekrety 
Aplikacja nie korzysta z zewnętrznych plików konfiguracyjnych ani zmiennych środowiskowych. Wszystkie ustawienia (np. wybór instrumentu, tempo) są zarządzane w pamięci podczas działania aplikacji.

### 6.2 Obsługa błędów i logowanie

Obsługa błędów opiera się głównie na blokach try-catch w kluczowych miejscach, takich jak wysyłanie komunikatów MIDI (PianoStrategy, OrganStrategy). Błędy są logowane do konsoli (Console.WriteLine), co jest wystarczające na etapie rozwoju. W przyszłości można rozważyć implementację bardziej zaawansowanego logowania do pliku.

### 6.3 Bezpieczeństwo (minimum)

Aplikacja jest aplikacją desktopową uruchamianą lokalnie, więc nie występują w niej typowe problemy bezpieczeństwa aplikacji webowych. Główne aspekty bezpieczeństwa sprowadzają się do:

Walidacji danych wejściowych (np. sprawdzanie, czy nazwa nagrywanej melodii nie jest pusta).

Braku przechowywania wrażliwych danych.

## 7. Decyzje architektoniczne (ADR-lite)

Decyzja: Użycie statycznej klasy MidiManager do zarządzania połączeniem MIDI.
Powód: Zapewnienie, że w całej aplikacji używany jest ten sam kanał MIDI, co zapobiega konfliktom i oszczędza zasoby systemowe. Wzorzec Singletona.
Konsekwencje: Ułatwia zarządzanie zasobem, ale utrudnia testowanie jednostkowe (wprowadza stan globalny).

Decyzja: Przechowywanie danych w pamięci (statyczne listy) zamiast w bazie danych.
Powód: Uproszczenie architektury aplikacji na etapie prototypu i rozwoju. Aplikacja nie wymaga trwałego przechowywania dużej ilości danych między sesjami (poza nagranymi melodiami, które są przechowywane tylko w bieżącej sesji).
Konsekwencje: Nagrane melodie nie są trwale zapisywane na dysku i znikają po zamknięciu aplikacji. W przyszłości można łatwo dodać serializację do pliku JSON lub bazę danych SQLite.

Decyzja: Wykorzystanie wzorca Strategia (ISoundStrategy) do implementacji różnych instrumentów.
Powód: Umożliwia łatwe dodawanie nowych instrumentów bez modyfikacji istniejącego kodu klienta (np. klasy PianoKeyboardWindow). Klient deleguje zadanie odtwarzania dźwięku do wybranej strategii.
Konsekwencje: Kod jest bardziej elastyczny i spełnia zasadę Open/Closed. Izoluje logikę specyficzną dla danego instrumentu.

Decyzja: Zastosowanie wzorca Fabryka (DrawerFactory) do tworzenia odpowiedniego rysownika (IDrawer) dla elementów muzycznych.
Powód: Oddzielenie logiki rysowania różnych typów elementów (nota, pauza) od logiki iterującej po elementach utworu w klasie MusicStaff. Ułatwia to dodawanie nowych typów elementów w przyszłości.
Konsekwencje: Kod klasy MusicStaff jest czystszy i bardziej skoncentrowany na swojej głównej odpowiedzialności.


## 8. Powiązanie architektury z modułami

Moduł Klawiatury: doc/architecture/keyboard.md

Moduł Waterfall: doc/architecture/waterfall.md

Moduł Interwałów: doc/architecture/interval.md

## 9. Ograniczenia, ryzyka i dalszy rozwój

Ograniczenia: Brak trwałego przechowywania nagranych melodii. Aplikacja działa wyłącznie w systemie Windows. Jakość dźwięku zależy od syntezatora MIDI zainstalowanego w systemie.

Ryzyka: Użycie Thread.Sleep w zadaniach (Task) do odtwarzania dźwięku może blokować wątki i wpływać na responsywność aplikacji przy bardziej złożonych utworach. 

Dalszy rozwój:

Implementacja trwałego zapisu i odczytu utworów (np. do plików XML lub JSON).

Dodanie możliwości wyboru instrumentu dla całego utworu przed jego odtworzeniem.

Rozszerzenie biblioteki nut o więcej oktaw i bardziej zaawansowane oznaczenia.

Implementacja bardziej zaawansowanego systemu oceny gry, uwzględniającego czas i dynamikę.

Dodanie możliwości wczytywania własnych plików MIDI.