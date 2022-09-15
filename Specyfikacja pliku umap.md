# Specyfikacja pliku .umap (.0):

## Opis:
Format działa na zasadzie struktury drzewiastej, podobnie jak pliki .xml i .json.

## Znaki specjalne:
`#;|\"() `
### Aliasy:
`:?`

## Zarys algorytmu interpretującego:
Podczas interpretacji algorytm czyta wszystkie linijki pliku jedna po drugiej, na jedną linijkę przypada jeden element .xml'a (wyjątkiem są modyfikatory zapisywane w nawiasach kwadratowych, o nich później), oraz następnie dzieli linijkę po spacjach w taki sposób, że spacje oddzielają kolejne argumenty. Żeby zapobiec rozpoznaniu string'a ze spacjami takiego jak np. `FA (1)` należy go zamknąć w cudzysłowach: `"FA (1)"`:
```umap
Element1 # Pusty node bez wartości
    Element2 Jakaś_wartość # Dziecko Element1 o wartości `Jakaś_wartość`
    Element3 "Jakaś wartość" # Dziecko Element1 o wartości `Jakaś wartość`
    Element4 "\"Coś co chcemy by było w cytacie\"" # Dziecko Element1 o wartości `"Coś co chcemy by było w cytacie"`
        # Jeśli chcemy by w string'u znajdował się cudzysłów wtedy przed nim należy postawić `\`
    Element5 "Jakaś" wartość" # BŁĄD: Nieparzysta ilość cudzysłowów
    Element6 "Jakaś" wartość"" # Ta linijka zostanie zintepretowana jako nazwa elementu i 3 string'i {`Jakaś`, `wartość`, ``}
        # Taka akurat konfiguracja doprowadza do błędu, ponieważ zwykły node przyjmuje tylko 2 argumenty, nazwę element'u i ewentualnie wartość.
```

## Ustalanie hierarchi:
Hierarchia elementów ustalana jest przy pomocy ilości tab'ów lub spacji (4 spacje), rodzicem elementu jest pierwszy element z mniejszą ilością tabów, to znaczy, że jeśli element `WariantFormularza` ma 2 wcięcia, a element `Naglowek` jest pierwszym od góry elementem który ma mniej niż 2 wcięcia, to `WariantFormularza` staje się dzieckiem `Naglowek`.  
Przykład:  
```umap
Element1 # Rodzic całej hierarchi
        Element2 # Dziecko Element1
    Element3 # Dziecko Element1
        Element4 # Dziecko Element3
```

<!-- NON MVP -->
## Koniec linii:
Linię kończy znak końca linii `\n` lub średnik `;`.

<!-- NON MVP -->
## Zasada pomijania nawiasów `()`:
Nawiasy są kompletnie zbędne, można je pomijać, służą jedynie do jawnego zaznaczenia ilości argumentów podawanych do konkretnej funkcji, ponieważ inaczej brana jest najmniejsza liczba.
Dla funkcji `|SUM` która potencjalnie przyjmuje od jednego do nieskończoności argumentów zostaje wzięty jedynie jeden (wyjątkiem jest sytuacja w której funkcja jest najwyższego rzędu w linijce, to znaczy, że stoi bezpośrednio po nazwie elementu lub modyfikatora).  
Przykład należy rozumieć jako grupy linijek które są sobie równe:
```umap
Element1 |SUM :"path1" :"path2" :"path3"
Element1 |SUM(:"path1" :"path2" :"path3")
Element1(|SUM(:"path1" :"path2" :"path3"))

# Błąd. Poniższy kod wyrzuci błąd, ponieważ `|NUM_F` przyjmuje od 1 do 2 argumentów.
Element1 |NUM_F |SUM :"path1" :"path2" "format"
Element1 |NUM_F |SUM(:"path1") :"path2" "format"
Element1 |NUM_F(|SUM(:"path1") :"path2" "format")

[list] "path1" Element1 |NUM_F |SUM(:"path2" :"path3") "format"
[list]("path1") Element1 |NUM_F |SUM(:"path2" :"path3") "format"

# Błąd. `[relative]` przyjmuje 0..1 argumentów, tak więc tutaj nie przyjmuje żadnego, interpretacja linijki załamuje się przy `|NUM_F` ponieważ interpreter nie spodziewał się kolejnego argumentu po wartości node'a.
# Interpreter zaczyna to czytać, jako node którego lokalna ścieżka jest zresetowana o nazwie `path1` i wartości `Element1`, następnie wyrzuci błąd ponieważ następnie spodziewa się jedynie znaku końca linii.
[relative] "path1" Element1 |NUM_F |SUM(:"path2" :"path3") "format"
[relative] "path1" Element1 |NUM_F |SUM(:"path2" :"path3") "format"
```

<!-- NON MVP -->
## Komentarze:
Komentarze zaczynamy przy pomocy znaku `#`, komentarz sprawia, że wszystko po nim w danej linijce jest ignorowane. Znak komentarza musi stać na początku linijki bądź być poprzedzony tylko pustym znakiem, lub tab'em w przypadku string'u `abc#` ten zostanie normalnie zinterpretowany jako argument.

## Funkcje:
Funkcja jest rozumiana jako argument który zaczyna się od znaku `|`.

- `|XML({arg})` alias `:{arg}` - zwraca wartość ze ścieżki `arg` w mapowanym .xml'u.
- `|DATE({arg} {format})` - interpretuje `arg` jako datę i zwraca go sformatowanego przy pomocy `format`
- `|IF({func} {argT} {argF})` - `func` funkcja zwracająca `true` albo `false`, `argT` argument ewaluowany i zwracany przez funkcję jeśli `func` zwróciło `true`, `argF` - argument ewaluowany i zwracany przez funkcję jeśli `func` zwróciło `false`
- `|_EQ({args}[2..])` - funkcja specjalna zwracająca `true` albo `false` jeśli wszystkie argumenty są sobie równe, jest to argument specjlany
- `|SUM({args}[1..])` - intepretuje każdy z `args`'ów jako liczbę i sumuje je ze sobą zwracając na koniec ją w formacie `F2`.
- `|NUM_F({arg} {format}[0..1])` - interpretuje `arg` jako liczbę i formatuje go przy pomocy `format`, domyślnie `format` jest ustawiony na `[default]NUMBER_FORMAT`.
- `|SPLIT_AND_TAKE({arg} {separator} {idx})` - interpretuje `arg` jako string, dzieli go przy pomocy `separator` i zwraca string o indeksie `idx`, `idx` działa na zasadzie indeksu w pythonie, więc przyjmuje także ujemne wartości.
- `|SPLIT_AND_REMOVE({arg} {separator} {idx})` - interpretuje `arg` jako string, dzieli go przy pomocy `separator` i zwraca string o indeksie `idx`, `idx` działa na zasadzie indeksu w pythonie, więc przyjmuje także ujemne wartości.

## Modyfikatory:
Modyfikatory zawsze znajdują się przed nazwą elementu. Modyfikatory które nie mają po sobie nazwy elementu aplikowane są do najbliższego elementu poniżej ich.

- `[attr]` - ten element zostaje zinterpretowany jako atrybut swojego rodzica.
    ```umap
    Element1
        [attr] NazwaAtrybutu "Wartość atrybutu"
    ```
    Powyższy .umap wytwarza poniższy .xml
    ```xml
    <Element1 NazwaAtrybutu="Wartość atrybutu">
    </Element1>
    ```
- `[list]({path})` - interpreter wie, że elementów z takim modyfikatorem może być więcej niż jeden. Modyfikator `[list]` także od razu zapewnia funkcjonalność podobną do `[relative]`. 
    ```umap
    Element1
        [list]("Root") ListElement :"./El_1"
    ```
    Powyższy .umap wytwarza poniższy .xml
    ```xml
    <Element1>
        <ListElement>123</ListElement>
        <ListElement>223</ListElement>
    </Element1>
    ```
    z poniższego .xml
    ```xml
    <Root>
        <El_1>123</El_1>
        <El_1>223</El_1>
    </Root>
    ```
- `[relative]({path}[0..1])` - interpreter ustawia lokalną ścieżkę dla wszystkich dzieci tego elementu na `{path}`. Wtedy użycie na początku ścieżki `.` sprawia, że wszystkie ścieżki są interpretowane jako `{path}/{arg}`. Jeśli nie podamy argumentu `path` wtedy lokalna ścieżka jest resetowana.
    ```umap
    [relative]("El_1")
    Element1
        Element2 :"./El_2"
        Element3 :"El_1/El_2"
        Element4 |SUM(:"El_1/El_2" :"./El_3")
    ```
    Powyższy .umap wytwarza poniższy .xml
    ```xml
    <Element1>
        <Element2>2</Element2>
        <Element3>3</Element3>
        <Element4>5</Element4>
    </Element1>
    ```
    z poniższego .xml
    ```xml
    <El_1>
        <El_2>2</El_2>
        <El_3>3</El_3>
    </El_1>
    ```
- `[try]` alias `?` - w przypadku jakiegoś błędu podczas przypisywania tego elementu, ten element jest jedynie ignorowany, a mapowanie reszty dokumentu przebiega dalej.
    ```umap
    Element1
        ? Element2 :"./El_2"
        ? Element3 :"El_1/El_2"
    ```
    Powyższy .umap wytwarza poniższy .xml
    ```xml
    <Element1>
        <Element3>3</Element3>
    </Element1>
    ```
    z poniższego .xml
    ```xml
    <El_1>
        <El_2>2</El_2>
    </El_1>
    ```
- `[default]` - używany do ustawiania domyślnych wartości używanych przez interpreter.
    Użycie:
    ```umap
    [default] NUMBER_FORMAT "F2"
    ```
    Dla jasności, NUMBER_FORMAT nie jest odtwarzany w hierarchi xml'owej.
- `[version]` - linijka zaczynająca się tym modyfikatorem imnterpretowana jest w inny sposób od reszty pliku, w tej linijce nie może znajdować się nic poza informacją o wersji. Linijka z tym modyfikatorem musi znajdować się przed jakimkolwiek innym elementem. Sam numer wersji składa się z 2 liczb `{major}.{minor} f{fixes}`.

<!-- NON MVP -->
## System wersji:
Format: `{major}.{minor}.{fixes}`</br>
W zapisie można pomijać wersję major jeśli jest 0 zapisując `.0` == `0.0`.
Domyślnie także wersja fixes jest wersją nieustaloną, jeśli nie zostanie ustalona interpreter ją pomija.
### Przykłady skracania:
`.12` == `0.12.x`
`.12.0` == `0.12.0`
`0.12` == `0.12.x`
`0.12.0` == `0.12.0`

### Rozbieżności wersji:
- Rozbieżność wersji `major`
  > Pomiędzy wersjami `major` dozwolone jest usuwanie funkcjonalności uznanych za zbędne, oraz nawet kompletna zmiana zasady działania mapper'a.
  - Interpreter wyrzuci błąd i nie będzie interpretował pliku.
- Rozbieżność wersji `minor`
  > Pomiędzy wersjami `minor` dozwolone jest dodawania nowych funkcjonalności.
  - W przypadku kiedy interpreter ma wyższą wersję, wtedy nie powinno się nic stać.
  - W przeciwnym przypadku interpreter spróbuje zinterpretować pliki
    - Jeśli mu się nie uda z powodu na napotkanie kodu którego nie zrozumie (np. funkcji dodanej w nowej wersji), wyrzuci błąd.
    - Jeśli mu się uda wtedy wyrzuci ostrzerzenie sugerujące by rozważyć obniżenie wersji api pliku, bo jest potencjalnie nie potrzebnie zbyt wysoka.
- Rozbieżność wersji `fixes`
  > Pomiędzy wersjami `fixes` dozwolone jest jedynie łatanie błędów.
  - W przypadku kiedy interpreter ma wyższą wersję, wtedy wyrzuci ostrzerzenie, że mapowanie może zawieść, jeśli użytkownik korzystał w nim z funkcjonalności która wynikała z błędu.
  - W przeciwnym przypadku interpreter wyrzuci ostrzerzenie by użytkownik zaktualizował wersję interpretera zawierające bug fix'y.
  > Ostrzerzenia są wyrzucane z powodu tego, że użytkownik sobie tego jawnie zażyczył w deklaracji pliku ustawiając wersję `fixes`.
