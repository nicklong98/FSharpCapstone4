open System

let getAmmount command =
    Console.WriteLine()
    Console.Write "Enter amount: $"
    let amount = Console.ReadLine() |> Decimal.TryParse
    match amount with
    | true, amount -> Some(command, amount)
    | false, _ -> None