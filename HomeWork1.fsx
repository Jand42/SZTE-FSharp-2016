type Person =
    {
        Name : string
        Age : int option
    }

// első sort ne módosítsd, csak a `failwith "TODO"` helyére írj

// 1. feladat
// állapítsátok meg, hogy a bemeneti listában van-e ismert életkorú kiskorú (Age < 18)
let hasKnownMinor (people: Person list) : bool =
    people |> List.exists (function { Age = Some a } when a < 18 -> true | _ -> false) 
//        (fun p -> 
//            match p with
//            | { Age = Some a } when a < 18 -> true
//            | _ -> false
//        )
        //p.Age.IsSome && p.Age.Value < 18)

// 2. feladat
// ismert életkorú emberek korának átlaga
// tipp: "float" segítségével konvertált törtre az életkort, hogy tudj jól átlagolni
let averageKnownAge (people: Person list) : float =
    people 
    |> List.choose (function { Age = Some a } -> Some (float a) | _ -> None)
    |> List.average

// 3. feladat
// add vissza az ismeretlen korú emberek neveit listában
let withUnknownAge (people: Person list) : string list =
    people 
    |> List.choose (function { Name = n; Age = None } -> Some n | _ -> None)

/// 4. feladat                                  
// add vissza az ismert korú emberek neveit és életkorát párokban (tuple)
let withKnownAge (people: Person list) : (string * int) list =
    people 
    |> List.choose (function { Name = n; Age = Some a } -> Some (n, a) | _ -> None)

// tesztelheted: ez mind hiba nélkül kell fusson:
let testPeople = [
    { Name = "András"; Age = Some 31 }
    { Name = "Béla"; Age = Some 19 }
    { Name = "Csilla"; Age = None }
    { Name = "Dénes"; Age = Some 7 }
    { Name = "Eszter"; Age = Some 21 }
]

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

test "Feladat1" (fun () -> hasKnownMinor []) false
test "Feladat1" (fun () -> hasKnownMinor testPeople) true
test "Feladat2" (fun () -> averageKnownAge testPeople) 19.5
test "Feladat3" (fun () -> withUnknownAge testPeople) [ "Csilla" ]
test "Feladat4" (fun () -> withKnownAge testPeople) [ "András", 31; "Béla", 19; "Dénes", 7; "Eszter", 21 ]
