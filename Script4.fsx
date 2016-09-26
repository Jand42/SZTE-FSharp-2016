// számláló OO-módra
type Counter() =
    let mutable x = 0
    
    member this.Next() = 
        x <- x + 1
        x        

// példa leszármazásra
type Counter2() =
    inherit Counter()

    member this.Next2() = 
        this.Next(), this.Next()    

let c = Counter()
c.Next()

let c2 = Counter2()
c2.Next()
c2.Next2()

// számláló FP-módra
let getCounter() =
    let mutable x = 0
    
    fun () -> 
        x <- x + 1
        x        

let c3 = getCounter()
c3()
