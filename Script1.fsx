// 1-10 egész számokat négyzetre emeljük
// Amit használunk:
// * egyszeresen láncolt lista
// * range expression - gyors számtani sorozat létrehozás
// * pipe operátor: |> egy paramétert átad jobb oldalra, láncoláshoz hasznos
// * List.map - lista leképezése. Magasabb rendű függvény: első paramétere egy függvény
// * lambda kifejezés (fun): névtelen függvény definiálása helyben
[ 1 .. 10 ] |> List.map (fun x -> x * x)


// pipe nélkül
List.map (fun x -> x * x) [ 1 .. 10 ]

// ugyanez tömbbel
[| 1 .. 10 |] |> Array.map (fun x -> x * x)

// listát elemenként így definiálhatunk 
[ 1 ; 3; 5 ]

// sortörés esetén a ; elhagyható
[
    1
    2
    3
    2 + 2
]

// értékadás. nem változó, hacsak nincs "mutable"
let x = 1 + 1

// függvény definiálás. "x" típusa automatikusan kikövetkeztetve "int"
let f x = x + 1

// függvény hívás, mindettő ok
f 4
f(4)

// a definiált függvényünk segítségével képezünk le egy listát
// nincs szükség lambda kifejezésre, az "f" függvényt direkt módon is átadhatjuk paraméterként
[ 1 .. 10 ] |> List.map f //(fun x -> f x)

// kétváltozós függvény
let add x y = x + y

// részleges applikáció: ha egy "int -> int -> int" függvény egy paramétert kap, egy "int -> int" függvény az eredmény 
add 1

// ezt használhatjuk, ezek ekvivalensek:
[ 1 .. 10 ] |> List.map (fun x -> add x 3)
[ 1 .. 10 ] |> List.map (fun x -> add 3 x)
[ 1 .. 10 ] |> List.map (add 3)
[ 1 .. 10 ] |> List.map ((+) 3)

let add3 = add 3
[ 1 .. 10 ] |> List.map add3

// operátor definiálás
let (++) x y = x + y + 1

2 ++ 2
(++) 2 2

// shadowing (árnyékolás)
let result =
    // ha az F# Power Tools telepítve van, a Visual Studio kijelölés mutatja a szemantikai azonosságot
    let x = 2
    let f () = x
    let x = 3
    f() + x

// let kifejezés egy sorba átírva
let resultOneLine =
    let x = 2 in let f () = x in let x = 3 in f() + x

1, "hello", true, 3.2 //tuple

// függvény zárójelezett (tuple) paraméterrel
let mult (a, b) = a * b

mult (3, 3)

// típus alias
type int2 = int * int

let p : int2 = 4, 5

mult p
fst p
snd p
let (p1, p2) = p

// rekord típus deklaráció
type Person =
    {
        Name : string
        Age  : int option
    }

// rekord érték deklaráció
let bela =
    {
        Name = "Béla"
        Age  = Some 42
    }
// egy sorba is írható
//    { Name = "Béla"; Age  = 42 }

let ageOneYear someone =
    let newAge =
//        match someone.Age with
//        | Some a -> Some (a + 1)
//        | _ -> None //failwith "unknown age"
        someone.Age |> Option.map ((+) 1)                
    { someone with Age = newAge }

// példák option értékre
None
Some 1

// |> eredeti definíciója
//let (|>) x f = f x

// unió típus deklaráció
type Name =
    | Nickname of string
    | Fullname of firstname: string * lastname: string
    | Anonymous

// példák az általunk definiált típusú értékekre:
Nickname "Józsi"
Fullname ("Bartók", "Béla")
Anonymous

// függvény, ami stringgé alakít egy Name típusú értéket
// pattern matching példa
let nameToString (name: Name) : string =
    match name with
    | Nickname a -> a
    | Fullname (a, b) -> a + " " + b
    | Anonymous -> ""



