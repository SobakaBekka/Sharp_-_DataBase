README: Online Supermarket Project
Přehled projektu
Cílem tohoto projektu je vytvořit webovou aplikaci pro online supermarket, která umožní správu uživatelů, produktů, objednávek a kategorií produktů s použitím technologií ASP.NET MVC a Oracle SQL. Projekt je zaměřen na splnění všech specifických požadavků stanovených v zadání předmětů BDATS2 a Programování v .NET a C# II, včetně správy databáze, implementace rolí, práv přístupu a obchodní logiky zpracování objednávek.

Členové týmu
Nazar: Správa databáze (Oracle SQL Developer, Data Modeler)
Pavlo: Vývoj webové aplikace (ASP.NET MVC)
Pracovní plán a pokyny pro kroky
Krok 1: Počáteční nastavení databáze a struktury ASP.NET MVC projektu
Zadání pro Nazara:

Vytvořit základní databázové tabulky pro uživatele, kategorie, produkty a role. Tyto tabulky budou základem pro další vývoj a logiku aplikace. Součástí bude také vytvoření dvou číselníkových tabulek, například pro role uživatelů.
Připravit SQL skripty, které definují tabulky, jejich primární a cizí klíče a sekvence pro generování hodnot primárních klíčů.
Zadání pro Pavla:

Vytvořit ASP.NET MVC projekt ve Visual Studio, nastavit základní konfiguraci pro připojení k Oracle SQL databázi a implementovat základní strukturu aplikace.
Definovat modely pro jednotlivé databázové tabulky, které vytvořil Nazar, a zajistit, aby modely správně reprezentovaly strukturu databáze.
Krok 2: Implementace funkcí a uložených procedur pro práci s daty
Zadání pro Nazara:

Navrhnout a vytvořit několik uložených funkcí a procedur v SQL pro zpracování dat. Tyto funkce budou sloužit k provádění běžných operací, jako je například získání počtu produktů v kategorii, počtu uživatelů v každé roli nebo výpočet průměrné ceny produktů.
Zajistit, aby každá funkce a procedura měla unikátní logiku a výstup.
Zadání pro Pavla:

V ASP.NET MVC volat uložené funkce a procedury v kontrolerech pro implementaci základní funkčnosti aplikace, jako je práce s produkty a uživatelskými rolemi.
Zajistit správné zpracování vstupů a výstupů při volání SQL funkcí a procedur.
Krok 3: Implementace triggerů pro automatizaci zpracování dat a logování
Zadání pro Nazara:

Vytvořit triggery, které budou automatizovat určité úlohy a logovat změny v databázi. Triggery budou například aktualizovat pole s datem poslední změny při úpravě produktu nebo logovat změny rolí uživatelů.
Triggery musí být dostatečně komplexní a sloužit konkrétnímu účelu pro zachování integrity dat.
Zadání pro Pavla:

Integrovat a testovat triggery přes ASP.NET MVC aplikaci, zajistit, že se triggery spouštějí správně při operacích jako je úprava produktu nebo změna uživatelské role.
Krok 4: Práce s binárními daty a zobrazení obrázků produktů
Zadání pro Nazara:

Vytvořit tabulku a strukturu pro ukládání binárních dat (například obrázků produktů) spolu s dalšími informacemi, jako je název, typ a datum nahrání. Tato data budou ukládána do databáze a zobrazována v aplikaci.
Definovat uložené procedury pro vkládání, úpravu a mazání binárních dat.
Zadání pro Pavla:

Implementovat uživatelské rozhraní v ASP.NET MVC pro nahrávání, zobrazení a mazání obrázků produktů. Zajistit správné zpracování binárních dat při jejich ukládání a načítání z databáze.
Krok 5: Implementace formulářů, validací a práv přístupu
Zadání pro Nazara:

Vytvořit uložené procedury, které budou kontrolovat práva přístupu uživatelů a umožní vkládání a úpravu dat pouze oprávněným uživatelům. Zajistit, aby aplikace byla uživatelsky přívětivá a používala například comboboxy pro číselníkové hodnoty.
Připravit kontrolní mechanismy, které zajistí, že data jsou spravována v souladu s právy uživatele.
Zadání pro Pavla:

Vytvořit formuláře v ASP.NET MVC pro přidávání, úpravu a mazání záznamů s implementací validací a kontrol přístupu. Formuláře musí být přístupné pouze uživatelům s odpovídajícími právy.
Krok 6: Implementace vyhledávání, filtrování a zobrazení dat
Zadání pro Nazara:

Vytvořit pohledy a hierarchické dotazy, které umožní efektivní vyhledávání a filtrování dat, například pro prohlížení kategorií a podkategorií produktů.
Ujistit se, že uživatelé mohou zobrazit relevantní informace, aniž by potřebovali přístup k primárním klíčům.
Zadání pro Pavla:

Implementovat rozhraní pro vyhledávání a filtrování produktů a kategorií v ASP.NET MVC, s důrazem na uživatelskou přívětivost a bez zobrazení ID záznamů.
Krok 7: Implementace rolí, omezení přístupu a emulace uživatelského profilu
Zadání pro Nazara:

Definovat a implementovat role a přístupová práva v databázi, včetně funkce emulace profilu uživatele pro administrátory. Nastavit výchozí roli pro nové uživatele a omezit možnost změny role pouze pro administrátory.
Zadání pro Pavla:

V ASP.NET MVC aplikaci implementovat emulaci profilu uživatele pro administrátory, umožnit přihlášení novým uživatelům s výchozí rolí a zajistit, že změnu role může provést pouze administrátor.
Krok 8: Implementace logování a historie změn
Zadání pro Nazara:

Vytvořit triggery a procedury pro logování důležitých akcí a uchování historie změn dat, zejména pro operace prováděné administrátorem. Logované záznamy musí být dostupné pouze administrátorům.
Zadání pro Pavla:

Implementovat logování akcí uživatelů ve webové aplikaci, aby administrátoři mohli sledovat důležité události a změny provedené uživateli.
Krok 9: Implementace transakcí a zajištění integrity dat
Zadání pro Nazara:

Zajistit správu transakcí pro kritické operace, jako je přidání objednávky a aktualizace skladových zásob. Použít kurzory v PL/SQL kódu pro efektivní zpracování výsledků dotazů a udržení integrity dat.
Zadání pro Pavla:

V ASP.NET MVC aplikaci volat transakční procedury a implementovat kontrolu výjimek, aby se při zjištění chyb transakce správně vracely zpět.
Krok 10: Optimalizace, testování a dokumentace
Zadání pro Nazara:

Optimalizovat databázové dotazy, naplnit tabulky realistickými daty pro závěrečné testování a připravit dotaz pro zobrazení všech databázových objektů použitých v aplikaci.
Vytvořit finální dokumentaci k databázi, včetně přehledu použitých databázových objektů.
Zadání pro Pavla:

Projít závěrečné testování aplikace, včetně všech formulářů, přístupových práv a transakcí. Otestovat výkon a uživatelskou přívětivost aplikace.
Připravit závěrečnou dokumentaci s popisem architektury, funkcionality a návodem na použití aplikace.
