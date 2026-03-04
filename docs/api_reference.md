
*(Niniejsza sekcja jest pusta, ponieważ projekt jest aplikacją desktopową WPF i nie udostępnia zewnętrznego API sieciowego w postaci endpointów HTTP. Funkcjonalności są dostępne bezpośrednio poprzez interfejs użytkownika i zdarzenia w kodzie.)*

## 1. Informacje ogólne

Projekt **PIANOAPP** jest samodzielną aplikacją desktopową. W związku z tym nie posiada on warstwy API sieciowego, która mogłaby być opisana w standardowym pliku `api_reference.md`. Wszelka komunikacja odbywa się wewnątrz procesu aplikacji, pomiędzy jej komponentami.

## 2. Odpowiednik API – Główne interfejsy programistyczne

Zamiast endpointów HTTP, architektura aplikacji definiuje kluczowe interfejsy i klasy abstrakcyjne, które pełnią rolę "kontraktów" między modułami. Poniżej znajduje się ich zestawienie.

| Interfejs / Klasa | Moduł | Opis (rola w systemie) |
|---|---|---|
| `ISoundStrategy` | Wspólny | Definiuje kontrakt dla strategii generowania dźwięku. Klasy implementujące (np. `PianoStrategy`) muszą dostarczyć metody do grania nut i pauz. Jest to główny punkt rozszerzalności dla dodawania nowych instrumentów. |
| `IDrawer` | Pięciolinia (`MusicStaff`) | Definiuje kontrakt dla rysowania elementów muzycznych na pięciolinii. Fabryka `DrawerFactory` zwraca odpowiednią implementację (`NoteDrawer`, `PauseDrawer`) w zależności od typu elementu. |
| `PianoKeyboardWindow` (klasa abstrakcyjna) | Klawiatura | Stanowi bazę dla wszystkich okien zawierających klawiaturę. Definiuje wirtualne metody (`OnPlayButtonClick`, `OnTempoButtonClick`, itp.), które są nadpisywane w klasach pochodnych (`WaterfallKeyboardWindow`, `KeyboardWithSongWindow`) w celu zapewnienia specyficznego zachowania. |

## 3. Podsumowanie

Ze względu na charakter aplikacji, niniejszy plik nie zawiera typowej specyfikacji endpointów API. Szczegółowy opis działania poszczególnych modułów i ich interfejsów znajduje się w dedykowanych plikach w katalogu `doc/architecture/`.