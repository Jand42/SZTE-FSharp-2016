type Suit =  // magyarul szín
    | Hearts
    | Spades
    | Clubs
    | Diamonds

type Rank =  // magyarul magasság
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

// első sort ne módosítsd, csak a `failwith "TODO"` helyére írj

// 1. feladat
// állapítsátok meg, hogy a bemeneti listában van-e legalább két ász
let hasAtLeastTwoAces (cards: Card list) : bool =
    failwith "TODO"

// 2. feladat
// változtassátok meg a kapott kártyalapok színeit a kapott függvény szerint
// sorrenden ne változtass
let mapSuits (mapping: Suit -> Suit) (cards: Card list) : Card list =
    failwith "TODO"

// 3. feladat
// adjatok vissza egy olyan függvényt, ami egy lap magasságra megmondja,
// hogy a kapott kártya listában hány olyan magasságú volt.
// az összeszámolást végezze el előre, ne csak a válasz függvény meghívásakor
let cardRankOcurrences (people: Card list) : (Rank -> int) =
    failwith "TODO"

/// 4. feladat                                  
// adj vissza egy aszinkron magasság értéket, ami a két aszinkron kártyalap magassága közül a nagyobb
// az első értéket várd be először, ha az ász, ne is várd be a másodikat
let higherRankAsync (card1: Card Async) (card2: Card Async) : Rank Async =
    failwith "TODO"

// tesztelheted: ez mind hiba nélkül kell fusson:
let (--) r s =
    {
        Rank = r
        Suit = s
    }

let test name f x =
    try 
        let y = f()
        if x = y then
            printfn "%s: OK" name
        else
            printfn "%s: Hibás eredmény" name
            printfn "Kapott: %A" y
            printfn "Várt: %A" x
    with e ->
        printfn "%s: Futási hiba: %A" name e

test "Feladat1" (fun () -> hasAtLeastTwoAces []) false
test "Feladat1" (fun () -> hasAtLeastTwoAces [Ace -- Diamonds; Number 3 -- Clubs]) false
test "Feladat1" (fun () -> hasAtLeastTwoAces [Ace -- Diamonds; Number 3 -- Clubs; Ace -- Spades]) true

test "Feladat2" (fun () -> mapSuits (function Hearts -> Spades | Spades -> Hearts | s -> s) [Jack -- Hearts; Queen -- Spades; King -- Clubs; Ace -- Diamonds])
    [Jack -- Spades; Queen -- Hearts; King -- Clubs; Ace -- Diamonds]
 
test "Feladat3" (fun () -> cardRankOcurrences [Ace -- Diamonds; Number 3 -- Clubs; Ace -- Spades] King) 0
test "Feladat3" (fun () -> cardRankOcurrences [Ace -- Diamonds; Number 3 -- Clubs; Ace -- Spades] (Number 3)) 1
test "Feladat3" (fun () -> cardRankOcurrences [Ace -- Diamonds; Number 3 -- Clubs; Ace -- Spades] Ace) 2

test "Feladat4" (fun () -> higherRankAsync (async.Return (Number 3 -- Clubs)) (async.Return (King -- Spades)) |> Async.RunSynchronously) King
test "Feladat4" (fun () -> higherRankAsync (async.Return (Ace -- Clubs)) (async { return failwith "should not be called" }) |> Async.RunSynchronously) Ace