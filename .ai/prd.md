# Dokument wymagań produktu (PRD) - HomeStorageApp (MVP)

## 1. Przegląd produktu

HomeStorageApp (MVP) to aplikacja mobilna zaprojektowana do zarządzania domową apteczką w ramach jednego gospodarstwa domowego. Aplikacja umożliwia użytkownikom śledzenie posiadanych leków, ich dat ważności, hierarchicznych lokalizacji oraz zarządzanie procesem ich przyjmowania. Głównym celem MVP jest scentralizowanie informacji o lekach, redukcja marnotrawstwa (przeterminowanie) oraz poprawa bezpieczeństwa i regularności poprzez system przypomnień o dawkach i terminach ważności.

## 2. Problem użytkownika

Użytkownicy w gospodarstwach domowych mają trudności z efektywnym zarządzaniem swoimi lekami. Główne problemy, które rozwiązuje aplikacja, to:

* Brak centralnej kontroli nad datami przydatności, co prowadzi do marnowania leków i ryzyka zażycia przeterminowanych produktów.
* Trudność w zlokalizowaniu konkretnego leku w domu (np. w różnych szafkach, pudełkach, lodówce).
* Zapominanie o regularnym przyjmowaniu dawek, szczególnie przy wielu domownikach i różnych harmonogramach leczenia.
* Niepewność co do aktualnego stanu zapasów, co prowadzi do niepotrzebnych zakupów lub braku leku w krytycznym momencie.
* Brak prostego narzędzia do przypisywania leków i dawek konkretnym domownikom (np. dzieciom, seniorom).
* Trudność w zarządzaniu zapasami, gdy ten sam lek występuje w różnych opakowaniach (np. blistry, pudełka, pojedyncze sztuki), co utrudnia przeliczanie całkowitego stanu.
* Niespójne nazewnictwo jednostek (np. "tab.", "tabletka", "szt."), co utrudnia sumowanie zapasów i zarządzanie nimi.

## 3. Wymagania funkcjonalne

### WF-1: Zarządzanie Lekami
Pełen cykl CRUD (Tworzenie, Odczyt, Aktualizacja, Usuwanie) dla definicji leków. Obejmuje to definicję leku, powiązanie z *systemową definicją jednostki głównej* (np. 'tabletka') oraz definicję jednostek przeliczeniowych (np. blister, opakowanie). Obejmuje to także funkcje "Archiwizuj" (dezaktywacja) i "Aktywuj" (przywrócenie).

### WF-2: Zarządzanie Lokalizacjami
Tworzenie, edycja i usuwanie (archiwizacja) nielimitowanej, hierarchicznej struktury lokalizacji (np. Szafka -> Półka -> Pudełko) prezentowanej w formie "drzewka".

### WF-3: Identyfikacja Lokalizacji
Możliwość przypisania unikalnego "symbolu" (definiowanego jako tekstowy identyfikator, np. "SZAFKA_LAZ") do lokalizacji w celu jej szybkiej identyfikacji (przez wpisanie lub skanowanie kodu QR wygenerowanego przez aplikację dla tego symbolu).

### WF-4: Zarządzanie Zapasami (Partiami)
Rejestrowanie przyjmowania dawek, ręczne zużywanie, utylizacja oraz ręczna korekta stanów magazynowych dla konkretnych partii (opakowań) leków. Wszystkie operacje (dodawanie, zużywanie) muszą być możliwe do wykonania w dowolnej zdefiniowanej dla leku jednostce, z automatycznym przeliczeniem na jednostkę główną.

### WF-5: Inwentaryzacja
Funkcja pozwalająca na weryfikację stanu magazynowego: użytkownik wybiera lokalizację, aplikacja wyświetla listę oczekiwanych leków/partii, a użytkownik ręcznie potwierdza lub koryguje ich stan (wyrażony w jednostce głównej).

### WF-6: Zarządzanie Domownikami
Tworzenie, edycja i usuwanie prostych profili domowników (np. "Tomek", "Zosia") w celu przypisywania im leków i harmonogramów dawkowania. Profile te nie są osobnymi kontami użytkowników.

### WF-7: Harmonogramy i Dawkowanie
Definiowanie prostych harmonogramów (raz dziennie o X, wiele razy dziennie o X, w wybrane dni tygodnia) i powiązanie ich z konkretnym domownikiem i lekiem. Dawka jest definiowana jako ilość w jednej ze zdefiniowanych jednostek (np. '1 tabletka').

### WF-8: Obsługa Kodów Kreskowych / QR
* Możliwość powiązania skanowanego kodu z nowym lub istniejącym lekiem (jako jego główny identyfikator).
* Automatyczne uzupełnianie zapasu po zeskanowaniu kodu leku już istniejącego w bazie (dodanie nowej partii w domyślnej jednostce zakupu).

### WF-9: Powiadomienia i Dashboard
* Dostarczanie powiadomień push o nadchodzących terminach brania leków (z informacją dla kogo) oraz o zbliżających się datach ważności (30, 7 i 0 dni przed).
* Główny ekran (dashboard) priorytetyzujący nadchodzące dawkowanie oraz szybkie akcje (np. "Dodaj lek").

### WF-10: Uwierzytelnianie
Prosty mechanizm autentykacji (np. email/hasło lub logowanie przez dostawcę tożsamości) dla "Gospodarza domu", aby chronić dane gospodarstwa domowego. MVP obsługuje only jedno konto "Gospodarza".

### WF-11: Wymagania Techniczne
Aplikacja w wersji MVP wymaga stałego połączenia z internetem do wykonywania operacji zapisu (dodawanie, edycja, zużycie). Dane mogą być buforowane lokalnie do odczytu w trybie offline.

### WF-12: Zarządzanie Jednostkami Miar Leku
System musi pozwalać na to, aby dla każdego leku:
1.  Powiązać go z jedną *systemową definicją jednostki głównej* (w której prowadzony jest stan magazynowy, np. 'tabletka', 'ml').
2.  Zdefiniować wiele *własnych jednostek pochodnych*, specyficznych dla tego leku (np. 'blister', 'opakowanie') wraz z ich przelicznikami (np. 1 blister = 10 tabletek; 1 opakowanie = 5 blistrów).

### WF-13: Systemowe Definicje Jednostek
Aplikacja musi posiadać *predefiniowaną, niemodyfikowalną przez użytkownika (w MVP)* listę definicji jednostek podstawowych (np. 'sztuka', 'tabletka', 'kapsułka', 'ml', 'g', 'saszetka'), do których użytkownik mapuje swoje leki podczas ich tworzenia.

## 4. Granice produktu

Następujące funkcje i możliwości nie wchodzą w zakres wersji MVP:

* Zarządzanie separacją danych na poziomie wielu gospodarstw domowych.
* System zaproszeń, tworzenie i przyznawanie dostępu do gospodarstwa domowego innym użytkownikom (innym kontom).
* Integracja z zewnętrznymi, publicznymi bazami danych leków w celu automatycznego pobierania informacji o lekach po zeskanowaniu kodu kreskowego.
* Konfigurowalne przez użytkownika czasy alertów o dacie ważności (ustawione na sztywno: 30, 7 i 0 dni).
* Zaawansowane zarządzanie uprawnieniami (poza jednym "Gospodarzem" i profilami "Domowników").
* Pełny tryb offline z synchronizacją konfliktów (wymagane jest połączenie do zapisu).
* Zaawansowane raportowanie i eksport danych.
* Modyfikowanie przez użytkownika systemowych definicji jednostek (WF-13).

## 5. Historyjki użytkowników

### Moduł: Uwierzytelnianie i Konfiguracja (Auth)

* ID: US-001
* Tytuł: Rejestracja konta Gospodarza domu
* Opis: Jako nowy użytkownik, chcę móc założyć konto (Gospodarza domu) przy użyciu adresu e-mail i hasła, aby móc zacząć korzystać z aplikacji i zabezpieczyć swoje dane.
* Kryteria akceptacji:
    1.  Użytkownik może wprowadzić e-mail i hasło (z potwierdzeniem).
    2.  System waliduje format adresu e-mail.
    3.  System waliduje siłę hasła (np. min. 8 znaków).
    4.  Po pomyślnej rejestracji użytkownik jest automatycznie logowany.
    5.  Konto jest tworzone jako jedyne konto "Gospodarza domu" dla tego gospodarstwa domowego.

* ID: US-002
* Tytuł: Logowanie do aplikacji
* Opis: Jako powracający Gospodarz domu, chcę móc zalogować się na swoje konto przy użyciu e-maila i hasła, aby uzyskać dostęp do moich danych.
* Kryteria akceptacji:
    1.  Użytkownik może wprowadzić e-mail i hasło.
    2.  System waliduje poprawność danych.
    3.  W przypadku błędnych danych wyświetlany jest stosowny komunikat.
    4.  Po pomyślnym zalogowaniu użytkownik widzi główny Dashboard.

* ID: US-003
* Tytuł: Wylogowanie z aplikacji
* Opis: Jako Gospodarz domu, chcę móc się wylogować z aplikacji, aby zabezpieczyć dostęp do danych na moim urządzeniu.
* Kryteria akceptacji:
    1.  W menu aplikacji dostępna jest opcja "Wyloguj".
    2.  Po wybraniu opcji sesja użytkownika jest kończona, a aplikacja przenosi do ekranu logowania.

### Moduł: Zarządzanie Domownikami (Profiles)

* ID: US-101
* Tytuł: Tworzenie profilu domownika
* Opis: Jako Gospodarz domu, chcę móc stworzyć prosty profil dla każdego domownika (np. "Dziecko Tomek"), aby móc przypisywać mu leki i harmonogramy.
* Kryteria akceptacji:
    1.  W ustawieniach aplikacji istnieje sekcja "Domownicy".
    2.  Użytkownik może dodać nowego domownika, podając jego nazwę (pole wymagane).
    3.  Nowo utworzony domownik pojawia się na liście.
    4.  Profile te nie mają danych logowania.

* ID: US-102
* Tytuł: Zarządzanie profilami domowników
* Opis: Jako Gospodarz domu, chcę móc edytować nazwę lub usuwać profile domowników, których już nie ma lub których dane się zmieniły.
* Kryteria akceptacji:
    1.  Użytkownik może wybrać profil z listy i zmienić jego nazwę.
    2.  Użytkownik może usunąć profil domownika.
    3.  Usunięcie profilu domownika powoduje usunięcie powiązanych z nim harmonogramów dawkowania (lub prosi o potwierdzenie tej akcji).

### Moduł: Zarządzanie Lokalizacjami (Locations)

* ID: US-201
* Tytuł: Tworzenie lokalizacji najwyższego poziomu
* Opis: Jako Gospodarz domu, chcę móc zdefiniować główne lokalizacje przechowywania leków (np. "Szafka w łazience", "Lodówka"), aby zacząć budować strukturę.
* Kryteria akceptacji:
    1.  W sekcji "Lokalizacje" użytkownik może dodać nową lokalizację.
    2.  Podczas dodawania podaje jej nazwę.
    3.  Lokalizacja pojawia się na najwyższym poziomie drzewka.

* ID: US-202
* Tytuł: Tworzenie zagnieżdżonych lokalizacji (hierarchia)
* Opis: Jako Gospodarz domu, chcę móc dodawać pod-lokalizacje do już istniejących (np. "Górna półka" w "Szafka w łazience"), aby precyzyjnie odwzorować miejsca przechowywania.
* Kryteria akceptacji:
    1.  Użytkownik może wybrać istniejącą lokalizację z drzewka.
    2.  Użytkownik ma opcję "Dodaj pod-lokalizację".
    3.  Nowa lokalizacja jest dodawana jako "dziecko" wybranej lokalizacji.
    4.  System wspiera nielimitowany poziom zagnieżdżenia.

* ID: US-203
* Tytuł: Zarządzanie lokalizacjami (Edycja, Archiwizacja)
* Opis: Jako Gospodarz domu, chcę móc zmieniać nazwy lokalizacji oraz je archiwizować (dezaktywować), gdy przestaję z nich korzystać, zachowując historię.
* Kryteria akceptacji:
    1.  Użytkownik może edytować nazwę dowolnej lokalizacji.
    2.  Użytkownik może "Zarchiwizować" (Dezaktywować) lokalizację.
    3.  Zarchiwizowana lokalizacja znika z aktywnego drzewka wyboru.
    4.  Archiwizacja lokalizacji powoduje archiwizację wszystkich jej pod-lokalizacji.
    5.  Leki znajdujące się w zarchiwizowanej lokalizacji pozostają w systemie, ale ich lokalizacja jest oznaczona jako archiwalna.

* ID: US-204
* Tytuł: Przypisywanie "symbolu" tekstowego do lokalizacji
* Opis: Jako Gospodarz domu, chcę móc przypisać unikalny tekstowy ID (symbol, np. "LODOWKA_LEKI") do lokalizacji, aby móc ją później szybko zidentyfikować.
* Kryteria akceptacji:
    1.  Podczas edycji lokalizacji istnieje opcjonalne pole "Symbol (ID)".
    2.  System sprawdza, czy wprowadzony symbol jest unikalny w ramach gospodarstwa domowego.
    3.  Symbol jest zapisywany.

* ID: US-205
* Tytuł: Generowanie i identyfikacja lokalizacji przez kod QR
* Opis: Jako Gospodarz domu, chcę aby aplikacja wygenerowała dla mnie kod QR bazujący na "symbolu" lokalizacji, abym mógł go wydrukować i nakleić, a następnie skanować, by szybko zobaczyć zawartość tej lokalizacji.
* Kryteria akceptacji:
    1.  W szczegółach lokalizacji, która ma przypisany "symbol", dostępna jest opcja "Pokaż kod QR".
    2.  Aplikacja generuje kod QR kodujący ten "symbol" (np. "HSAPP_LOC:LODOWKA_LEKI").
    3.  W aplikacji (np. na Dashboardzie) jest przycisk "Skanuj lokalizację".
    4.  Po zeskanowaniu tego kodu QR, aplikacja przechodzi bezpośrednio do widoku tej lokalizacji i jej zawartości.

### Moduł: Zarządzanie Lekami i Jednostkami Miar (Drugs & Units)

* ID: US-301
* Tytuł: Definiowanie nowego leku i jego jednostki głównej
* Opis: Jako Gospodarz domu, chcę móc zdefiniować nowy lek, podając jego nazwę oraz wybierając jego podstawową (główną) jednostkę miary z systemowej listy (np. 'tabletka', 'ml'), w której będzie prowadzony cały stan magazynowy.
* Kryteria akceptacji:
    1.  Formularz "Dodaj lek" wymaga podania "Nazwy" leku.
    2.  Formularz wymaga *wybrania* "Definicji jednostki głównej" z predefiniowanej, systemowej listy (np. 'tabletka', 'ml', 'g', 'sztuka', 'kapsułka').
    3.  Ta wybrana definicja staje się jednostką magazynową leku.
    4.  Po zapisaniu tworzona jest "Definicja leku" gotowa do dalszej konfiguracji.

* ID: US-302
* Tytuł: Definiowanie jednostek przeliczeniowych (opakowań) i kodu kreskowego
* Opis: Jako Gospodarz domu, po zdefiniowaniu leku, chcę móc dodać własne jednostki pochodne (np. 'blister', 'opakowanie') i ich przeliczniki (np. 1 blister = 10 tabletek, 1 opakowanie = 5 blistrów), a także powiązać kod kreskowy.
* Kryteria akceptacji:
    1.  W widoku edycji leku mogę zarządzać "Jednostkami przeliczeniowymi".
    2.  Mogę dodać nową jednostkę przeliczeniową, podając jej *nazwę* (np. "blister" lub "opakowanie").
    3.  Muszę zdefiniować jej przelicznik: 1 [nowa nazwa, np. 'blister'] = [ilość] [jednostka bazowa].
    4.  Lista "Jednostek bazowych" zawiera *jednostkę główną* (wybraną w US-301, np. 'tabletka') oraz *inne, już zdefiniowane jednostki przeliczeniowe* dla tego leku (np. 1 'opakowanie' = 5 'blistrów').
    5.  System automatycznie oblicza i zapisuje ostateczny przelicznik do jednostki głównej (np. 1 opakowanie = 5 * 10 = 50 tabletek).
    6.  Mogę oznaczyć jedną z jednostek (np. "opakowanie") jako "domyślną jednostkę zakupu" na potrzeby skanowania (US-403).
    7.  W tym samym widoku edycji mogę zeskanować i zapisać kod kreskowy/QR powiązany z tą definicją leku.

* ID: US-302-A
* Tytuł: Dodawanie partii (opakowania) leku do stanu
* Opis: Jako Gospodarz domu, chcę móc dodać nową partię (opakowanie) leku do magazynu, podając jej ilość w *dowolnej* ze zdefiniowanych jednostek (np. '1 opakowanie' lub '5 blistrów'), a także jej datę ważności i lokalizację.
* Kryteria akceptacji:
    1.  W widoku leku jest przycisk "Dodaj partię" (lub "Dodaj zapas").
    2.  Formularz wymaga podania "Ilości" (liczba) i "Jednostki" (wybór z listy zdefiniowanych jednostek dla tego leku, np. [jednostka główna: 'tabletka'], 'blister', 'opakowanie').
    3.  Formularz wymaga podania "Daty ważności".
    4.  Formularz pozwala opcjonalnie wybrać "Lokalizację".
    5.  Po zapisaniu, system przelicza podaną ilość na jednostkę główną (np. 1 opakowanie -> 50 tabletek) i tworzy nową partię z tą obliczoną ilością.

* ID: US-303
* Tytuł: Przeglądanie listy leków
* Opis: Jako Gospodarz domu, chcę widzieć listę wszystkich moich aktywnych leków wraz z ich sumarycznym stanem magazynowym i najkrótszą datą ważności.
* Kryteria akceptacji:
    1.  Aplikacja posiada listę wszystkich leków.
    2.  Przy każdym leku widoczna jest jego nazwa, łączna ilość (suma ze wszystkich partii) wyrażona w *jednostce głównej* (np. '250 tabletek') oraz najkrótsza data ważności.
    3.  Leki zarchiwizowane (dezaktywowane) nie są widoczne na tej liście.

* ID: US-304
* Tytuł: Przeglądanie szczegółów leku (partie)
* Opis: Jako Gospodarz domu, chcę móc kliknąć na lek z listy, aby zobaczyć wszystkie jego partie (opakowania), ich indywidualne ilości, daty ważności i lokalizacje.
* Kryteria akceptacji:
    1.  Po kliknięciu na lek (z US-303) użytkownik widzi ekran szczegółów.
    2.  Ekran ten listuje wszystkie aktywne partie tego leku.
    3.  Każda partia na liście pokazuje: Ilość (w *jednostce głównej*, np. '40 tabletek'), Datę ważności, Lokalizację (np. "Szafka -> Półka").

* ID: US-305
* Tytuł: Edycja definicji leku
* Opis: Jako Gospodarz domu, chcę móc edytować nazwę leku, powiązany kod kreskowy oraz zarządzać jego jednostkami przeliczeniowymi.
* Kryteria akceptacji:
    1.  W szczegółach leku (US-304) dostępna jest opcja "Edytuj definicję".
    2.  Użytkownik może zmienić Nazwę leku.
    3.  Użytkownik może zarządzać jednostkami przeliczeniowymi (dodawać/edytować przeliczniki) zgodnie z US-302.
    4.  Użytkownik może zmienić lub dodać powiązany kod kreskowy/QR.
    5.  Edycja wybranej "Definicji jednostki głównej" (z US-301) jest zablokowana, jeśli istnieją już partie tego leku w magazynie.

* ID: US-306
* Tytuł: Archiwizacja (Dezaktywacja) leku
* Opis: Jako Gospodarz domu, chcę móc zarchiwizować definicję leku, którego już nie używam i nie planuję kupować, aby ukryć go z aktywnych list.
* Kryteria akceptacji:
    1.  W szczegółach leku dostępna jest opcja "Archiwizuj".
    2.  Po potwierdzeniu, lek (wraz ze wszystkimi swoimi partiami) znika z głównych list i dashboardu.
    3.  Historia zużycia leku jest zachowana.
    4.  Aplikacja posiada osobną sekcję "Archiwum", gdzie można przeglądać zarchiwizowane leki.

* ID: US-307
* Tytuł: Aktywacja (Przywrócenie) leku
* Opis: Jako Gospodarz domu, chcę móc przywrócić zarchiwizowany lek, jeśli ponownie zacznę go używać.
* Kryteria akceptacji:
    1.  W sekcji "Archiwum" użytkownik może wybrać lek.
    2.  Dostępna jest opcja "Aktywuj" (Przywróć).
    3.  Lek (wraz ze swoimi partiami) wraca na aktywne listy.

### Moduł: Obsługa Kodów Kreskowych / QR (Scanning)

* ID: US-401
* Tytuł: Skanowanie kodu podczas definiowania leku
* Opis: Jako Gospodarz domu, edytując definicję leku (US-302), chcę móc zeskanować jego kod kreskowy, aby automatycznie powiązać ten kod z tym lekiem.
* Kryteria akceptacji:
    1.  W formularzu edycji definicji leku jest przycisk "Skanuj kod".
    2.  Po naciśnięciu otwiera się aparat.
    3.  Zeskanowany kod jest zapisywany w polu "Powiązany kod".

* ID: US-402
* Tytuł: Skanowanie nieznanego kodu (szybkie dodawanie)
* Opis: Jako Gospodarz domu, chcę móc zeskanować kod leku, którego nie ma w bazie, aby aplikacja zapytała mnie, czy chcę go dodać.
* Kryteria akceptacji:
    1.  Użytkownik używa ogólnej funkcji "Skanuj" (np. z Dashboardu).
    2.  System skanuje kod i sprawdza, czy jest powiązany z jakimkolwiek lekiem w bazie.
    3.  Jeśli kod nie jest znany, aplikacja wyświetla komunikat "Nie znaleziono leku. Czy chcesz dodać nowy lek z tym kodem?".
    4.  Po wybraniu "Tak", użytkownik jest przenoszony do formularza "Dodaj lek" (US-301), a zeskanowany kod jest tymczasowo zapamiętany, aby można go było dodać w kroku US-302.

* ID: US-403
* Tytuł: Automatyczne uzupełnianie zapasu po zeskanowaniu kodu istniejącego leku
* Opis: Jako Gospodarz domu, po zeskanowaniu kodu kreskowego leku, który już istnieje w mojej bazie, chcę, aby aplikacja automatycznie dodała jedną partię tego leku w "domyślnej jednostce zakupu" (np. 1 opakowanie).
* Kryteria akceptacji:
    1.  Użytkownik używa ogólnej funkcji "Skanuj".
    2.  System skanuje kod i identyfikuje powiązany z nim lek (np. "Apap").
    3.  System pobiera "Domyślną jednostkę zakupu" zdefiniowaną dla tego leku (z US-302), np. 'opakowanie'.
    4.  System automatycznie tworzy nową partię (opakowanie) dla leku "Apap".
    5.  Nowa partia ma ilość '1' i jednostkę równą "domyślnej jednostce zakupu" (np. 'opakowanie'). System od razu przelicza to na jednostkę główną (np. 50 tabletek) i tak zapisuje stan partii.
    6.  Nowa partia ma *pustą* datę ważności oraz *pustą* lokalizację.
    7.  Aplikacja *nie* zadaje użytkownikowi żadnych pytań (o ilość, datę czy lokalizację).
    8.  Aplikacja wyświetla krótkie potwierdzenie "Dodano 1 [domyślna jednostka zakupu]. Uzupełnij datę ważności." (Patrz US-706).

### Moduł: Zarządzanie Zapasami (Stock)

* ID: US-501
* Tytuł: Ręczna korekta stanu (zużycie lub dodanie)
* Opis: Jako Gospodarz domu, chcę móc ręcznie skorygować stan partii, rejestrując "zużycie" (np. rozlanie) lub "dodanie", podając ilość w dowolnej zdefiniowanej jednostce.
* Kryteria akceptacji:
    1.  W widoku partii jest opcja "Koryguj stan".
    2.  Użytkownik wybiera akcję: "Dodaj" lub "Zużyj".
    3.  Użytkownik wprowadza "Ilość" (liczba) i wybiera "Jednostkę" (z listy zdefiniowanych, np. 'tabletka', 'blister').
    4.  System przelicza tę ilość na jednostkę główną i odpowiednio dodaje lub odejmuje ją od stanu partii.
    5.  Nie można zużyć więcej niż aktualny stan partii.

* ID: US-502
* Tytuł: Rejestrowanie utylizacji
* Opis: Jako Gospodarz domu, chcę móc oznaczyć przeterminowaną partię leku jako "Zutylizowana", aby zniknęła z moich aktywnych zapasów.
* Kryteria akceptacji:
    1.  W widoku szczegółów partii jest opcja "Utylizuj".
    2.  Po potwierdzeniu, stan partii jest zerowany i jest ona oznaczana jako zutylizowana (usuwana z aktywnego stanu magazynowego).

* ID: US-503
* Tytuł: Inwentaryzacja lokalizacji
* Opis: Jako Gospodarz domu, chcę móc wybrać lokalizację (np. "Lodówka") i rozpocząć proces inwentaryzacji, aby zobaczyć listę leków, które powinny się tam znajdować.
* Kryteria akceptacji:
    1.  W aplikacji dostępna jest funkcja "Inwentaryzacja".
    2.  Użytkownik wybiera lokalizację z drzewka.
    3.  Aplikacja wyświetla listę wszystkich partii leków, które są obecnie przypisane do tej lokalizacji.
    4.  Przy każdej partii widoczna jest nazwa leku, oczekiwana ilość (w jednostce głównej) i data ważności.

* ID: US-504
* Tytuł: Potwierdzanie i korygowanie stanu podczas inwentaryzacji
* Opis: Jako Gospodarz domu, podczas inwentaryzacji, chcę móc przejrzeć listę i dla każdego leku potwierdzić jego obecność lub skorygować ilość (w jednostce głównej).
* Kryteria akceptacji:
    1.  Na liście inwentaryzacyjnej (z US-503) użytkownik może:
        * Potwierdzić (np. "Zgadza się").
        * Wprowadzić skorygowaną ilość (ilość jest wprowadzana w *jednostce głównej*, np. '46' tabletek).
        * Oznaczyć partię jako "Nie znaleziono" (co np. zeruje jej stan lub pozwala przenieść do lokalizacji "Zgubione").

* ID: US-505
* Tytuł: Przenoszenie leku między lokalizacjami
* Opis: Jako Gospodarz domu, chcę móc łatwo przenieść partię leku z jednej lokalizacji do drugiej.
* Kryteria akceptacji:
    1.  W widoku szczegółów partii leku użytkownik może zmienić jej lokalizację.
    2.  Użytkownik wybiera nową lokalizację z drzewka.
    3.  Partia zostaje przypisana do nowej lokalizacji.

### Moduł: Dawkowanie i Harmonogramy (Dosing)

* ID: US-601
* Tytuł: Tworzenie harmonogramu dawkowania
* Opis: Jako Gospodarz domu, chcę móc zdefiniować harmonogram dawkowania dla leku, określając domownika, lek, dawkę (ilość i jednostkę) oraz częstotliwość.
* Kryteria akceptacji:
    1.  Użytkownik może stworzyć nowy "Harmonogram".
    2.  Użytkownik wybiera Domownika (z listy US-101).
    3.  Użytkownik wybiera Lek (z listy US-303).
    4.  Użytkownik definiuje dawkę jako "Ilość" (np. '1') oraz "Jednostkę" (wybór z listy zdefiniowanych jednostek dla tego leku, np. 'tabletka', 'ml').
    5.  Użytkownik wybiera typ harmonogramu:
        * Raz dziennie (o godz. X).
        * Wiele razy dziennie (np. 8:00, 14:00, 20:00).
        * W wybrane dni tygodnia (np. Wt, Czw o 10:00).
    6.  Harmonogram jest zapisywany.

* ID: US-602
* Tytuł: Rejestrowanie wzięcia dawki (z powiadomienia lub dashboardu)
* Opis: Jako Gospodarz domu, gdy nadejdzie czas podania leku, chcę móc kliknąć przycisk "Weź dawkę" (lub "Podaj dawkę"), aby potwierdzić akcję.
* Kryteria akceptacji:
    1.  Nadchodzące dawki są widoczne na Dashboardzie (US-701).
    2.  Powiadomienia push (US-703) zawierają przycisk akcji (np. "Potwierdź wzięcie").
    3.  Użytkownik klika "Weź dawkę".
    4.  System rejestruje to zdarzenie w historii.

* ID: US-603
* Tytuł: Automatyczna redukcja stanu magazynowego po wzięciu dawki
* Opis: Jako Gospodarz domu, po potwierdzeniu wzięcia dawki (US-602), chcę, aby aplikacja automatycznie zmniejszyła stan magazynowy powiązanego leku o zdefiniowaną dawkę.
* Kryteria akceptacji:
    1.  Po akcji "Weź dawkę" system identyfikuje powiązany lek i dawkę (np. '1 tabletka' lub '5 ml').
    2.  System automatycznie odejmuje zdefiniowaną dawkę, przeliczając ją (jeśli to konieczne) na jednostkę główną magazynową, od partii tego leku.
    3.  System priorytetowo zużywa partię z najkrótszą datą ważności.
    4.  Jeśli stan partii spadnie do 0, partia jest oznaczana jako pusta (zużyta).

* ID: US-604
* Tytuł: Zarządzanie harmonogramami (Edycja, Usuwanie)
* Opis: Jako Gospodarz domu, chcę móc edytować lub usuwać istniejące harmonogramy, gdy zmieni się leczenie.
* Kryteria akceptacji:
    1.  Użytkownik może wyświetlić listę wszystkich aktywnych harmonogramów.
    2.  Użytkownik może edytować dowolny parametr harmonogramu (lek, dawkę, czas, domownika).
    3.  Użytkownik może usunąć (zakończyć) harmonogram.

### Moduł: Dashboard i Powiadomienia (Alerts)

* ID: US-701
* Tytuł: Dashboard - Nadchodzące dawkowanie
* Opis: Jako Gospodarz domu, po otwarciu aplikacji chcę na głównym ekranie (Dashboard) widzieć listę dawek do wzięcia "Dzisiaj", posortowaną chronologicznie.
* Kryteria akceptacji:
    1.  Dashboard jest głównym ekranem aplikacji.
    2.  Najwyższy priorytet ma sekcja "Nadchodzące dawkowanie (Dzisiaj)".
    3.  Lista pokazuje czas (np. 8:00), nazwę leku, dawkę (np. '1 tabletka') oraz dla kogo (Domownik).
    4.  Obok każdej pozycji znajduje się przycisk "Weź dawkę" (US-602).

* ID: US-702
* Tytuł: Dashboard - Szybkie akcje
* Opis: Jako Gospodarz domu, chcę mieć na Dashboardzie łatwo dostępne przyciski do najczęstszych akcji, takich jak "Dodaj lek", "Skanuj kod" i "Znajdź lek".
* Kryteria akceptacji:
    1.  Na Dashboardzie, poniżej listy dawek, widoczna jest sekcja "Szybkie akcje".
    2.  Dostępne są co najmniej przyciski "Dodaj lek" i "Skanuj kod".

* ID: US-703
* Tytuł: Powiadomienie push o terminie wzięcia leku
* Opis: Jako Gospodarz domu, chcę otrzymywać powiadomienie push, gdy nadejdzie czas podania leku, z informacją, jaki to lek i dla kogo.
* Kryteria akceptacji:
    1.  System wysyła powiadomienie push o godzinie zdefiniowanej w harmonogramie (US-601).
    2.  Treść powiadomienia zawiera: "Czas na lek! [Nazwa Leku] dla [Nazwa Domownika]".
    3.  Powiadomienie zawiera akcję "Weź dawkę".

* ID: US-704
* Tytuł: Powiadomienie push o zbliżającej się dacie ważności (30, 7, 0 dni)
* Opis: Jako Gospodarz domu, chcę otrzymywać powiadomienia push, gdy data ważności któregoś z moich leków zbliża się ku końcowi.
* Kryteria akceptacji:
    1.  System codziennie skanuje daty ważności aktywnych partii leków.
    2.  System wysyła powiadomienie push, gdy partia leku ma datę ważności za 30 dni.
    3.  System wysyła powiadomienie push, gdy partia leku ma datę ważności za 7 dni.
    4.  System wysyła powiadomienie push w dniu upłynięcia daty ważności.
    5.  Powiadomienia są grupowane (np. "3 leki wkrótce tracą ważność").

* ID: US-705
* Tytuł: Dashboard - Alerty o datach ważności
* Opis: Jako Gospodarz domu, chcę widzieć na Dashboardzie sekcję informującą mnie o lekach, które wkrótce tracą ważność lub są przeterminowane.
* Kryteria akceptacji:
    1.  Na Dashboardzie widoczna jest sekcja "Alerty o ważności".
    2.  Sekcja ta listuje leki/partie, które tracą ważność (w ciągu 30 dni) lub są przeterminowane.
    3.  Użytkownik może łatwo przejść z tej listy do szczegółów partii (np. w celu utylizacji US-502).

* ID: US-706
* Tytuł: Dashboard - Alerty o brakujących danych
* Opis: Jako Gospodarz domu, chcę widzieć na Dashboardzie informację, jeśli dodałem opakowanie leku (np. przez US-403), ale nie uzupełniłem jego daty ważności.
* Kryteria akceptacji:
    1.  Na Dashboardzie widoczna jest sekcja "Wymagane działania" lub podobna.
    2.  Sekcja ta listuje wszystkie partie leków, które mają pustą datę ważności.
    3.  Użytkownik może kliknąć na pozycję, aby przejść do edycji partii i uzupełnić brakujące dane.

## 6. Metryki sukcesu

Zgodnie z decyzjami projektowymi, formalne, mierzalne wskaźniki sukcesu (KPI) oraz szczegółowe scenariusze akceptacyjne (Acceptance Criteria) zostały świadomie pominięte na etapie MVP, gdyż jest to projekt indywidualny.

Głównym kryterium sukcesu jest wdrożenie i poprawne działanie wszystkich funkcjonalności opisanych w niniejszym dokumencie. Sukcesem jest dostarczenie działającego produktu MVP, który pozwala "Gospodarzowi domu" efektywnie zarządzać zakupionymi lekami i procesem ich używania z poziomu aplikacji mobilnej, rozwiązując tym samym problemy użytkownika zdefiniowane w sekcji 2.