
## 1. Wymagania systemowe

- **System operacyjny:** Windows 10 lub Windows 11.
- **Zainstalowane środowisko uruchomieniowe:** [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) lub nowsze.
- **Dysk:** Minimum 100 MB wolnego miejsca.
- **Pamięć RAM:** Minimum 512 MB (zalecane 1 GB).

## 2. Instalacja lokalna (uruchomienie z kodu źródłowego)

### 2.1 Pobranie repozytorium

Sklonuj repozytorium za pomocą komendy Git:

```bash
git clone https://github.com/[twoja-organizacja]/PIANOAPP.git
cd PIANOAPP

###2.2 Otwarcie projektu
Otwórz plik rozwiązania PIANOAPP.sln przy użyciu programu Microsoft Visual Studio 2022 (lub nowszego). Jeśli nie posiadasz Visual Studio, możesz użyć lekkiego edytora, takiego jak Visual Studio Code z zainstalowanym rozszerzeniem C# Dev Kit, a następnie uruchomić aplikację za pomocą .NET CLI.

### 2.3 Przywrócenie pakietów NuGet
Wszystkie niezbędne pakiety (NAudio, MathNet.Numerics, System.Speech) zostaną automatycznie przywrócone podczas pierwszego budowania projektu. W razie potrzeby można to zrobić ręcznie w konsoli Menedżera pakietów:
Update-Package -reinstall
lub za pomocą .NET CLI:
dotnet restore

###2.4 Konfiguracja projektu startowego
Upewnij się, że projekt PIANOAPP jest ustawiony jako projekt startowy. W Visual Studio można to zrobić, klikając prawym przyciskiem myszy na projekt w Eksploratorze rozwiązań i wybierając "Ustaw jako projekt startowy".

### 2.5 Uruchomienie aplikacji
Z poziomu Visual Studio: Naciśnij klawisz F5 (Debug) lub Ctrl + F5 (Uruchom bez debugowania).

Z poziomu .NET CLI: W katalogu głównym projektu (zawierającym plik PIANOAPP.csproj) wykonaj komendę:
dotnet run

###3. Konfiguracja aplikacji
Aplikacja nie wymaga żadnych plików konfiguracyjnych (takich jak .env) ani zmiennych środowiskowych do swojego podstawowego działania. Wszystkie ustawienia (np. wybór instrumentu, tempo) są konfigurowane z poziomu interfejsu użytkownika i przechowywane w pamięci podczas trwania sesji.

###4. Różnice środowisk
Aplikacja jest projektowana i testowana wyłącznie na systemach Windows. Nie jest przewidziana do uruchamiania na innych systemach operacyjnych (Linux, macOS) ze względu na wykorzystanie technologii WPF oraz bibliotek zależnych od Windows (np. System.Speech).

### 5. Typowe problemy i rozwiązywanie
Problem: Po uruchomieniu aplikacji nie słychać dźwięku.

Rozwiązanie 1: Sprawdź, czy głośniki/słuchawki są podłączone i włączone.

Rozwiązanie 2: Upewnij się, że syntezator MIDI w systemie Windows jest włączony. W Panelu sterowania przejdź do "Sprzęt i dźwięk" -> "Dźwięk" -> karta "MIDI". Powinieneś mieć wybrane jakieś urządzenie (np. Microsoft GS Wavetable Synth).

Problem: Aplikacja nie uruchamia się, wyświetlając błąd o braku .NET.

Rozwiązanie: Pobierz i zainstaluj .NET 8.0 Runtime lub .NET 8.0 SDK z oficjalnej strony Microsoft.

Problem: Podczas kompilacji występują błędy związane z brakującymi pakietami NuGet.

Rozwiązanie: Wykonaj ręczne przywracanie pakietów, jak opisano w punkcie 2.3.