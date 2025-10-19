\<conversation\_summary\>
\<decisions\>

1.  **Model Użytkownika:** MVP będzie obsługiwać jedno gospodarstwo domowe. Zostaną zaimplementowane proste profile ("Gospodarz domu", "Domownicy") do przypisywania leków, bez skomplikowanego systemu uprawnień.
2.  **Lokalizacje (Hierarchia):** Aplikacja musi wspierać nielimitowaną, hierarchiczną strukturę lokalizacji (np. Szafka -\> Półka -\> Pudełko). Zarządzanie tą strukturą odbywać się będzie poprzez prosty interfejs "drzewka".
3.  **Lokalizacje (Identyfikacja):** Zamiast "Ulubionych", lokalizacje będą identyfikowane przez "symbole", które użytkownik może skanować, wybierać z listy lub wprowadzać ręcznie (wymaga to dalszego doprecyzowania).
4.  **Dodawanie Leków (Ręczne):** Proces dodawania leku musi być uproszczony. Pola *wymagane* to: **Nazwa**, **Ilość początkowa** oraz **Data ważności**.
5.  **Obsługa Jednostek:** Pole "Ilość początkowa" musi wspierać wiele jednostek (np. szt., ml, g).
6.  **Dodawanie Leków (Kody Kreskowe):** Skanowanie *nowego* kodu służy do ręcznego powiązania go z lekiem. Skanowanie kodu leku *już istniejącego* w bazie ma *automatycznie uzupełniać zapas* (np. dodawać opakowanie) bez dodatkowego pytania użytkownika.
7.  **Dawkowanie:** MVP obsłuży proste harmonogramy (raz dziennie o X, wiele razy dziennie o X, w wybrane dni tygodnia). Harmonogramy są powiązane z konkretnym profilem "Domownika".
8.  **Raportowanie i Alerty:** Alerty (o datach ważności i terminach brania leków) będą dostarczane przez **powiadomienia push** oraz prezentowane na głównym **dashboardzie** aplikacji.
9.  **Timing Alertów (Data Ważności):** Alerty o dacie ważności będą domyślne i niekonfigurowalne w MVP: 30 dni przed, 7 dni przed i w dniu terminu.
10. **Dashboard:** Główny ekran aplikacji będzie priorytetyzował: (1) Nadchodzące dawkowanie (na dziś), (2) Szybkie akcje (np. Dodaj lek, Znajdź lek).
11. **Śledzenie Zużycia:** Odnotowanie wzięcia leku (przycisk "Weź dawkę") automatycznie redukuje stan magazynowy o zdefiniowaną dawkę. Możliwa będzie też ręczna korekta stanu.
12. **Inwentaryzacja:** Proces polega na wybraniu lokalizacji, wyświetleniu listy oczekiwanych leków i ręcznym potwierdzeniu/skorygowaniu ich stanu przez użytkownika.
13. **Archiwizacja:** Funkcje "Dezaktywacja" (leku lub lokalizacji) będą działać jako "Archiwizacja" – ukryją element z aktywnych list, ale zachowają jego historię.
14. **Tryb Offline:** MVP będzie wymagać stałego połączenia z internetem do wykonywania operacji zapisu (dodawanie, edycja, zużycie). Dane będą mogły być buforowane do odczytu offline.
15. **Kryteria Sukcesu (KPI):** Formalne wskaźniki KPI oraz scenariusze akceptacyjne zostają *pominięte* na tym etapie (projekt indywidualny).

\</decisions\>

\<matched\_recommendations\>

1.  MVP skupi się na jednym, wspólnym koncie z prostymi profilami domowników (Zaakceptowane).
2.  Skanowanie kodu w MVP służy jedynie jako identyfikator do szybkiego znalezienia lub ręcznego powiązania (Zaakceptowane, ale zmodyfikowane dla istniejących leków).
3.  MVP skupi się na powiadomieniach push i pulpicie (dashboard) do raportowania alertów (Zaakceptowane).
4.  Proces "brania leku" będzie prostą akcją ("Weź dawkę"), która automatycznie redukuje stan magazynowy (Zaakceptowane).
5.  "Dezaktywacja" będzie działać jako "Archiwizacja" (Zaakceptowane).
6.  Proces dodawania leku będzie wymagał absolutnego minimum pól (Zaakceptowane i doprecyzowane).
7.  MVP będzie wymagało stałego połączenia z internetem do przeprowadzania akcji zapisu (Zaakceptowane).
8.  Interfejs zarządzania hierarchią lokalizacji będzie prostą listą "drzewka" (Zaakceptowane).
9.  Funkcja "Ulubione lokalizacje" zostanie zastąpiona przez system "symboli" (Zmodyfikowane).
10. Główny ekran (dashboard) będzie priorytetyzował "Nadchodzące dawkowanie" oraz "Szybkie akcje" (Zaakceptowane).
11. Pole ilości początkowej musi obsługiwać różne jednostki (Zaakceptowane).
12. Harmonogramy dawkowania w MVP będą proste (dzienne, wielokrotne, wybrane dni tygodnia) (Zaakceptowane).
13. Proces "Inwentaryzacji" będzie polegał na ręcznej weryfikacji listy leków w wybranej lokalizacji (Zaakceptowane).
14. Skanowanie istniejącego leku powinno pytać użytkownika – (Odrzucone, zmienione na automatyczne uzupełnianie).
15. Domyślne, niekonfigurowalne alerty o dacie ważności (30, 7, 0 dni) (Zaakceptowane).
16. Harmonogram dawkowania będzie powiązany z profilem domownika, a powiadomienia będą to odzwierciedlać (Zaakceptowane).
    \</matched\_recommendations\>

\<prd\_planning\_summary\>

### Główne wymagania funkcjonalne produktu

1.  **Zarządzanie Lekami:** Pełen cykl CRUD (Create, Read, Update) dla leków, w tym funkcje "Archiwizuj" (Dezaktywuj) i "Aktywuj".
    * *Wymagane pola przy dodawaniu:* Nazwa, Ilość początkowa, Jednostka (np. szt., ml), Data ważności.
    * *Pola opcjonalne:* Lokalizacja, Powiązany kod kreskowy/QR, Przypisanie do domownika, Harmonogram dawkowania.
2.  **Zarządzanie Lokalizacjami:**
    * Tworzenie i zarządzanie *nielimitowaną* hierarchią lokalizacji (interfejs "drzewka").
    * Możliwość przypisania "symbolu" (do dalszego zdefiniowania: kod QR, ikona, tekst) do lokalizacji w celu jej szybkiej identyfikacji (skanowanie, wybór z listy, wpisanie).
3.  **Zarządzanie Zapasami:**
    * Rejestrowanie zużycia leku poprzez akcję "Weź dawkę" (automatyczna redukcja stanu) lub ręczną korektę ilości.
    * Funkcja "Inwentaryzacja" pozwalająca na weryfikację stanu magazynowego w wybranej lokalizacji.
4.  **Obsługa Kodów Kreskowych / QR:**
    * Skanowanie kodu nowego leku pozwala na ręczne powiązanie go z rekordem w aplikacji.
    * Skanowanie kodu leku już istniejącego *automatycznie* uzupełnia jego zapas (np. dodaje 1 opakowanie).
5.  **Zarządzanie Domownikami i Dawkowaniem:**
    * Tworzenie prostych profili domowników (bez osobnych kont).
    * Definiowanie prostych harmonogramów dawkowania (dzienne, wielokrotne, wybrane dni) i powiązanie ich z konkretnym domownikiem.
6.  **Powiadomienia i Dashboard:**
    * Wysyłanie powiadomień push o nadchodzących terminach brania leków (z informacją, dla kogo) oraz o zbliżających się datach ważności (30, 7 i 0 dni przed).
    * Dashboard prezentujący priorytetowo nadchodzące dawkowanie oraz szybkie akcje.
7.  **Wymagania Techniczne:** Aplikacja w wersji MVP wymaga aktywnego połączenia z internetem do operacji zapisu (synchronizacji danych).

### Kluczowe historie użytkownika i ścieżki korzystania

* **Persona:** Główną personą jest "Gospodarz domu" odpowiedzialny za zarządzanie domową apteczką i podawanie leków pozostałym "Domownikom".
* **User Story 1 (Dodawanie i lokalizacja):** "Jako Gospodarz domu, chcę móc dodać nowy lek, podając tylko jego nazwę, ilość i datę ważności, a następnie przypisać go do głęboko zagnieżdżonej lokalizacji (np. 'Szafka w łazience' -\> 'Górna półka' -\> 'Pudełko niebieskie'), aby dokładnie wiedzieć, gdzie się znajduje."
* **User Story 2 (Dawkowanie):** "Jako Gospodarz domu, chcę ustawić przypomnienie dla 'Dziecko Tomek' na 'Syrop X' dwa razy dziennie (8:00 i 20:00), aby otrzymać powiadomienie push, gdy nadejdzie czas podania leku."
* **User Story 3 (Uzupełnianie zapasów):** "Jako Gospodarz domu, po powrocie z apteki chcę zeskanować kod kreskowy nowego opakowania leku, który już mam, aby aplikacja automatycznie zwiększyła mój stan zapasów."
* **User Story 4 (Identyfikacja lokalizacji):** "Jako Gospodarz domu, chcę móc oznaczyć 'Apteczkę podróżną' unikalnym symbolem (np. kodem QR), aby móc szybko ją zeskanować i zobaczyć jej zawartość."
* **User Story 5 (Inwentaryzacja):** "Jako Gospodarz domu, chcę wybrać lokalizację 'Lodówka', aby zobaczyć listę wszystkich leków, które powinny się tam znajdować, i móc ręcznie potwierdzić lub skorygować ich ilości."

### Ważne kryteria sukcesu i sposoby ich mierzenia

* Zgodnie z decyzją projektową, formalne, mierzalne wskaźniki sukcesu (KPI) oraz szczegółowe scenariusze akceptacyjne (Acceptance Criteria) **zostały świadomie pominięte** na etapie MVP, gdyż jest to projekt indywidualny. Sukcesem będzie wdrożenie opisanych funkcjonalności.
  \</prd\_planning\_summary\>

\<unresolved\_issues\>

1.  **Definicja "Symboli" dla Lokalizacji:** Decyzja (pkt 3) o "przydzielaniu symboli" jest niejasna. Należy doprecyzować:
    * Czym dokładnie są te "symbole"? Czy są to kody QR generowane przez aplikację? Dowolne kody kreskowe/QR? Ikony/Emoji wybierane z listy? Ręcznie wpisywany tekst/ID?
    * Jak wygląda ścieżka użytkownika (UX) dla skanowania/wybierania/wprowadzania symbolu w celu identyfikacji lokalizacji?
2.  **Logika "Automatycznego Uzupełniania" Zapasów:** Decyzja (pkt 6) o automatycznym uzupełnianiu zapasów po zeskanowaniu istniejącego kodu jest ryzykowna. Należy wyjaśnić:
    * Co dokładnie oznacza "uzupełnianie"? Czy dodaje +1 do licznika opakowań, czy sumuje ilości (np. 100ml + 100ml)?
    * Co się dzieje, jeśli nowe opakowanie ma *inną datę ważności* niż te już posiadane? Automatyczne uzupełnienie bez pytania może prowadzić do utraty kluczowej informacji o najkrótszej dacie ważności.
    * Rekomenduje się ponowne rozważenie wyświetlenia prostego pytania (np. "Dodać nowe opakowanie z nową datą?"), aby uniknąć błędów w danych.
      \</unresolved\_issues\>
      \</conversation\_summary\>