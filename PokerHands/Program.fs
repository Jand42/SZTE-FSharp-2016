// opcionális, az utolsó kódfájt automatikusan egy modulba rakja, ha nincs jelölve
// a module engedi, hogy "let" deklarációink legyenek, namespace-ben, csak
// type és module lehetséges
module Program 

// típusok a kártyalapok reprezentálására
type Suit =
    | Hearts
    | Spades
    | Clubs
    | Diamonds

type Rank =
    | Number of int
    | Jack
    | Queen 
    | King 
    | Ace

type Card =
    {
        Rank: Rank
        Suit: Suit
    }

// segéd operátor
let (--) r s =
    {
        Rank = r
        Suit = s
    }

let suits =
    [
        Hearts
        Spades
        Clubs
        Diamonds
    ]

let ranks =
    ([ 2 .. 10 ] |> List.map Number)
    @ [
        Jack
        Queen 
        King 
        Ace
    ]  

// egy teljes pakli immutábilis halmazként
let fullDeck =
    Set [
        for r in ranks do
            for s in suits do
                yield r -- s
    ]

// legegyszerűbb .NET véletlengenerátor
// komoly titkosítási vagy szerencsejáték célra nagyon nem ajánlott,
// de nekünk most megfelel
let rnd = System.Random()
// egy lap véletlen húzása, a maradék paklit is visszaadjuk
let draw deck =
    if Set.isEmpty deck then
        None
    else
        let i = rnd.Next(Set.count deck)
        let c = deck |> Seq.item i
        Some (c, deck |> Set.remove c)

let printCard (card: Card) =
    match card.Rank with
    | Number n ->
        sprintf "%d of %A" n card.Suit
    | _ ->
        sprintf "%A of %A" card.Rank card.Suit

//teszt
//printCard (Number 4 -- Diamonds)

// öt lapot húz a teljes pakliból
// 
let drawNewHand() =
    fullDeck |> Seq.unfold draw |> Seq.take 5 |> List.ofSeq

// segédfüggvények
let getSuit x = x.Suit
let getRank x = x.Rank

// korábbi verziók:
//let isFlush (hand: Card list) : bool =
//    hand.Tail |> List.forall (fun x -> x.Suit = hand.Head.Suit)    
//    hand |> List.map getSuit |> List.distinct |> List.length = 1
//    hand |> List.countBy getSuit |> List.map snd = [5]
//
//let isFourOfAKind (hand: Card list) : option<Rank * Rank> =
//    match hand |> List.countBy getRank with //|> List.map snd |> List.sort = [1; 4]
//    | [ y, 1; x, 4 ]
//    | [ x, 4; y, 1 ] -> Some (x, y)
//    | _ -> None
//
//// active patternként
//let (|FourOfAKind|_|) (hand: Card list) : option<Rank * Rank> =
//    match hand |> List.countBy getRank with //|> List.map snd |> List.sort = [1; 4]
//    | [ y, 1; x, 4 ]
//    | [ x, 4; y, 1 ] -> Some (x, y)
//    | _ -> None

//#nowarn "25" // ha nem akarjuk a match warningot

// segéd érték, az összes lehetséges sort legyártjuk
let straightRanks =
    Ace :: ranks |> List.windowed 5

// az összes magasság szerinti megkülönböztetés egy active patternben
// az active pattern előnyei:
// * match kifejezésként használható függvény, egy match-en belül legfeljebb egyszer lesz kiértékelve
// * ha teljes az active pattern - részleges pl a (|FourOfAKind|_|) - akkor felhasználási helyén
//   figyelmeztetést kapunk, ha nem fedünk le minden esetet, az uniókhoz hasonlóan
let (|FourOfAKind|FullHouse|ThreeOfAKind|TwoPairs|OnePair|Straight|NonRankMatched|) (hand: Card list) =
//    printfn "testing ranks..." // ellenőrizni, hogy tényleg nem hívódik meg többször
    // magasság szerinti csoportok elemszámait csökkenő sorrendbe rendezzük
    // ez jól alkalamas sokféle kéz érték felismerésére
    match hand |> List.countBy getRank |> List.sortByDescending snd with
    | [ x, 4; _ ] -> FourOfAKind x
    | [ x, 3; _ ] -> FullHouse x
    | [ x, 3; _; _ ] -> ThreeOfAKind x
    | [ x, 2; y, 2; z ] -> 
        // fontos, hogy milyen sorrendben adjuk vissza a megkülönböztető magasságokat
        if x > y then
            TwoPairs (x, y, z)
        else 
            TwoPairs (y, x, z)
    | [ x, 2; y, _; z, _; u, _ ] -> 
        let [y; z; u] = List.sortDescending [y; z; u]
        OnePair (x, y, z, u)
    | _ ->
        // megnézzük, hogy sor-e: a magasságokat növekvő sorba rakva szerepel-e
        // az előre legyártott összes lehetságes sor közt
        let sortedRanks = hand |> List.map getRank |> List.sort
        if straightRanks |> List.contains sortedRanks then
            Straight (List.last sortedRanks)
        else  
            let [a; b; c; d; e] = List.rev sortedRanks 
            NonRankMatched (a, b, c, d, e)

// flös akkor van, ha színek szerint különbözőeket keresve csak egyet találunk 
// ez egy részleges active pattern
let (|Flush|_|) (hand: Card list)  =
    if hand |> List.distinctBy getSuit |> List.length = 1 then
        Some ()
    else None

// teljes kiértékelés, string végeredménnyel
let evaluate hand =
    match hand with
        | Straight r1 & Flush -> sprintf "straight flush, rank: %A" r1
        | FourOfAKind r1 -> sprintf "four of a kind, rank: %A" r1
        | FullHouse r1 -> sprintf "fullhouse, rank: %A" r1
        | Flush & NonRankMatched (r1, r2, r3, r4, r5) -> sprintf "flush, ranks: %A %A %A %A %A" r1 r2 r3 r4 r5
        | Straight r1 -> sprintf "straight, highest rank: %A" r1
        | ThreeOfAKind r1 -> sprintf "three of a kind, rank: %A" r1
        | TwoPairs (r1, r2, r3) -> sprintf "two pairs, ranks: %A %A %A" r1 r2 r3
        | OnePair (r1, r2, r3, r4) -> sprintf "one pair, ranks: %A %A %A %A" r1 r2 r3 r4
        | NonRankMatched (r1, r2, r3, r4, r5) -> sprintf "high card, ranks: %A %A %A %A %A" r1 r2 r3 r4 r5

// fő függvény, az [<EntryPoint>] attribútum jelöli a konzol alkalmazás belépési pontját
[<EntryPoint>]
let main argv = 
    let hand = drawNewHand()
    printfn "Hand: %A" (hand |> List.map printCard)
    printfn "Value: %s" (evaluate hand)

//    System.Console.ReadKey() |> ignore // ha meg akarsz várni egy billentyűnyomást. vagy indítsd Ctrl+F5-tel
    0 // return an integer exit code        
