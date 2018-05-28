
open System

type Command =
    | Withdraw
    | Deposit
    | Exit

let tryGetCommand c =
    match c with
    | 'w' -> (Some Withdraw)
    | 'd' -> (Some Deposit)
    | 'x' -> (Some Exit)
    | _ -> None

let inputLoop = 
    seq {
        while true do
            let key = (Console.ReadKey()).KeyChar
            Console.WriteLine()
            yield key
    }

inputLoop
|> Seq.choose tryGetCommand
|> Seq.takeWhile(fun x -> x <> Exit)
|> Seq.iter (fun x -> printfn "%A" x)