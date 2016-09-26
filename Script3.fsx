// aszinkron érték
// várakozik 1 másodpercet, és aztán végzi el a számítást
// példaként ezzel szimulálunk egy hosszabb ideig tartó műveletet
let a =
    async {
        do! Async.Sleep 1000
        return 1 + 1
    }

// aktuális szálon bevárja az eredményt
a |> Async.RunSynchronously

// timeout paraméterrel: legfeljebb adott ideig vár
// jelen esetben biztos a hiba ágra fut
let a_res =
    try
        Async.RunSynchronously(a, 500) |> Some
    with :? System.TimeoutException -> None

// bind művelet:
// aszinkron érték bevárása másik async blokkon belül
let b =
    async {
        let! x = a
        return x + 1
    }

// egy nem látunk semmit a végeredményből
b |> Async.Ignore |> Async.Start

// kiíratjuk a végeredményt, így már lesz látható hatása az Async.Start-nak 
let c =
    async {
        let! x = b        
        printfn "b has finished: %d" x
    }

c |> Async.Start

// háttérben dolgozó agent (MailboxProcessor) létrehozása
// az async végtelen ciklus valójában leginkább csak várakozik,
// az m.Receive() addig félreteszi a végrehajtást, amíg nincs újabb üzenet
let mbp =
    new MailboxProcessor<int>(fun m ->
        async {
            while true do
                let! input = m.Receive()
                printfn "MailboxProcessor received: %d" input
                do! Async.Sleep 1000
        }
    )

// teszteljük
mbp.Start()
mbp.Post(1)
mbp.Post(2)
mbp.Post(3)

// ez folyamatosan teleszemeteli nekünk a konzolt
async {
    while true do
        printfn "Hello"
        do! Async.Sleep 1000
} |> Async.Start

// minden explicit módon megadott CancellationToken nélküli futó async számítást leállít 
// pl az előzőt
Async.CancelDefaultToken()

// válaszolni is képes agent (MailboxProcessor)
let mbp2 =
    new MailboxProcessor<int * AsyncReplyChannel<string>>(fun m ->
//        async {
//            while true do
//                let! input, chan = m.Receive()
//                printfn "MailboxProcessor received: %d" input
//                do! Async.Sleep 1000
//                chan.Reply(string input) 
//        }
        // rekurzív módon ciklus helyett
        let rec loop() =
            async {
                let! input, chan = m.Receive()
                printfn "MailboxProcessor received: %d" input
                do! Async.Sleep 1000
                chan.Reply(string input)
                do! loop()
            }
        loop()
    )

mbp2.Start()

// teszt
let sendThenPrintReply x =
    async {
        let! reply = mbp2.PostAndAsyncReply(fun chan -> x, chan)
        printfn "MailboxProcessor replied: %A" reply
    }
    |> Async.Start
    
sendThenPrintReply 1
sendThenPrintReply 2  
sendThenPrintReply 3   