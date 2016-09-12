// bonyolultabb verzió
type Name =
    | Nickname of string
    | Fullname of 
        firstname: string * 
        middleInitial: string option * 
        lastname: string
    | Anonymous

// öszetett pattern match
let nameToString (name: Name) : string =
    match name with
    | Nickname n -> n
    | Fullname (f, None, l) -> f + " " + l
    | Fullname (f, Some i, l) -> 
        sprintf "%s %s. %s" f i l
        //f + " " + i + ". " + l
    | Anonymous -> ""

// nyomtatás konzolra
// teljes printf dokumentáció: https://msdn.microsoft.com/en-us/visualfsharpdocs/conceptual/core.printf-module-%5Bfsharp%5D?f=255&MSPPError=-2147217396
printfn "Hello %d!" 333

// újból a rekord típusunk
type Person =
    {
        Name : Name
        Age  : int option
    }

let isUnknown (p: Person) =
    match p with
    | { Name = Anonymous; Age = None } -> true
    | _ -> false

// Fizzbuzz feladat eredmény típus
type Fizzbuzz =
    | Fizz // oszható 3-al
    | Buzz // 5-tel
    | FizzBuzz // 15-tel
    | Number of int // számot magát adod vissza becsomagolva, ha egyik se

// első megoldás if/then/else
let fizzbuzz n : Fizzbuzz =
    if n % 15 = 0 then
        FizzBuzz
    elif n % 3 = 0 then
        Fizz
    elif n % 5 = 0 then
        Buzz
    else
        Number n    

// második megoldás, pattern match tuple-ön
let fizzbuzz2 n : Fizzbuzz =
    match n % 3, n % 5 with
    | 0, 0 -> FizzBuzz 
    | 0, _ -> Fizz
    | _, 0 -> Buzz
    |_ -> Number n


// tesztelhetjük
[ 1 .. 5 ] |> List.map fizzbuzz =
    [Number 1; Number 2; Fizz; Number 4; Buzz]

// egyelemű lista kétféleképp
// a :: operátor egy új elemet ad fejként a láncolt listához
[1]
1 :: []

let l = [1, 2; 3, 4]

match l with
| [] -> printfn "it's empty"
| h :: r -> printfn "not empty"
| [_, _] -> printfn "2 items" // ezt az esetet már lefedtük az előzővel, jelzi a fordító
| _ -> () // szintén

// faktoriális először, reduce segédvfüggvénnyel
let factorial1 (n: bigint) =
    [ 1I .. n ] |> List.reduce (*) //(fun a b -> a * b)

factorial1 2343I

// faktoriális másodszor, naiv rekurzív implementáció
let rec factorial2 (n: bigint) =
    if n > 1I then
        factorial2 (n - 1I) * n 
    else 1I

factorial2 53I

// faktoriális harmadszor, tail rekurzív implementáció
// nem okoz StackOverflow hibát nagy értékre se
// amit figyelni kell: a rekurzív függvényhívás után már ne legyen további művelet a visszatérési értékkel
// a trükk: egy akkumuláló paraméterben gyűjtjük a részeredményt, a végén ezt adjuk vissza
let factorial (n: bigint) =
    let rec f acc n =
        if n > 1I then
            f (acc * n) (n - 1I)
        else acc
    f 1I n

// nincs Stack Overflow, igaz, memória túlcsoldulást okoz, ha kivárod és nincs pár petabyte memóriád :P 
factorial 435465467475678I

// List.map újraimplementálva
// nem tail rekurzív
let rec listmap (f: 'a -> 'b) (l: 'a list) : 'b list =
    match l with
    | [] -> []
    | h :: t -> f h :: listmap f t 

[ 1 .. 10 ] |> listmap (fun x -> x * x)

// List.map újraimplementálva
// ez már tail rekurzív
// a végére kell a List.rev, mert az acc-ban fordított sorrendben tudjuk csak összegyűjteni a mappelt elemeket
let listmap2 (f: 'a -> 'b) (l: 'a list) : 'b list =
    let rec map acc l =
        match l with
        | [] -> acc
        | h :: t -> map (f h :: acc) t  //f h :: listmap f t
    map [] l |> List.rev 

[ 1 .. 10 ] |> listmap2 (fun x -> x * x)

// szabvány .NET-es konzolra nyomtatás, és F#-specifikus verzió
System.Console.WriteLine("Hello {0}!", "world")
printfn "Hello %s!" "world"

// kétféle for ciklus: gyűjteményre, vagy lépegetős
for i in 1 .. 10 do
    printfn "Line %d" i

for i = 10 to 1 do
    printfn "Line %d" i

// while ciklus triviális példa
let mutable go = true 
while go do
    go <- false

let mylist = [ 1 .. 10 ]
// ez egy szekvencia, nem a memóriában tárolt értékek gyűjteménye, csak egy objektum,
// ami igény szerint tud újabb értékeket előállítani 
let myseq = seq { 1 .. 10 }

// a myseq első öt eleme lesz így csak lekérdezve és négyzetre emelve
// ha listával csinálnánk hasonlót, a List.map azonnal feldolgozná az egész listát
myseq |> Seq.map (fun x -> x * x) |> Seq.take 5 |> Seq.sum

// F# interaktívnak van pár beállítása, pl kikapcsolható a szekvenciák első pár elemének automatikus kiírása
fsi.ShowIEnumerable <- false

// "végtelen" szekvencia: nem gond, ha nem akarunk végigmenni rajta
Seq.initInfinite id |> Seq.take 10 |> Seq.toList

// fold, "hajtogatás" függvény, mindig egy aktuális állapot és egy új elem felhasználásával csinál következő állapotot
seq { 1 .. 9 } |> Seq.fold (fun s i -> s * 10 + i) 0

// szekvencia kifejezés, fibonacci sorozat imperatív módon
// a "while true" miatt végtelen sorozat, de nem okoz gondot, mert ez a kódblokk nem fut le egyben,
// a "yield" mindig visszaad egy elemet, és addig várakozik a futása, amíg következő elemre nincs szükség 
let fibonacci =
    seq {
        let mutable a = 1
        let mutable b = 1
        while true do
            yield a
            let c = a + b
            a <- b
            b <- c
    }

fibonacci |> Seq.take 10 |> List.ofSeq

// minden egyes kiértékelésnél ellépked újra a 40. elemig 
fibonacci |> Seq.item 40

// cache-elt verzió: ha már az első valahány elemet ismeri, azokat nem számolja újra
let fibCached = Seq.cache fibonacci

// első kiértékelés után már cache-ből szedi az értékeket, így gyorsabban megvan a 40. elem
fibCached |> Seq.item 40

// lusta érték, hasonló annyiban a cache-elt szekvenciához, hogy a belső számítás nem történik
// meg azonnal, csak igény szerint, de onnantól már a fix értéket adja vissza
let lazy2 = 
    lazy (1 + 1)
    //System.Lazy(fun () -> 1 + 1)

// lusta kifejezés értékének lekérdezése
lazy2.Value

// a modulok függvények kényelmes csoportosítására jók
// "kibővíthetjük" a Seq modult azzal, hogy ugyanolyan néven csinálunk egy saját modult
module Seq =
    // az unfold mintájára egy segédfüggvény, ami végtelen szekvenciát csinál, az unfoldot használjuk belül
    let unfoldInfinite generator state =
    //    state |> Seq.unfold (fun x -> x |> generator |> Some)
        // lambda helyett hasznos a >> (függvény kompozíció) operátor is
        state |> Seq.unfold (generator >> Some)

// fibonacci az unfoldunkkal, nincs mutable
let fibonacci2 =
    (1, 1) 
    |> Seq.unfoldInfinite (fun (a, b) -> 
        a, (b, a + b)
    )

// 50-nél kisebb Fibonacci számokat rakjátok listába
fibonacci2 |> Seq.takeWhile (fun x -> x < 50) |> Seq.toList   

// még egy fibonacci, rekurzív szekvencia definíció
// a "yield!" egy belső szekvencia összes elemét továbbadja
let fibonacci3 =
    let rec fib a b =
        seq {
            yield a
            yield! fib b (a + b)
        }     
    fib 1 1

fibonacci3 |> Seq.take 10

// Seq.take helyett néha a truncate jön jól, a take hibát dob, ha nincs adott számú elem,
// a truncate csak levágja a szekvenciát maximum adott számú elemnél
[ 1 .. 10 ] |> Seq.truncate 15 |> List.ofSeq

