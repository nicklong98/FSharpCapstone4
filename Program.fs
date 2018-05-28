// Learn more about F# at http://fsharp.org

module Capstone4.Program

open System
open Auditing
open Operations
open FileRepository
open Capstone4.Domain.Commands


[<EntryPoint>]
let main _ =
    
    let withdrawWithAudit = auditAs 'w' composedLogger withdraw
    let depositWithAudit = auditAs 'd' composedLogger deposit

    let tryGetAmmount command =
        Console.WriteLine()
        Console.Write "Enter amount: $"
        let amount = Console.ReadLine() |> Decimal.TryParse
        match amount with
        | true, amount -> Some(command, amount)
        | false, _ -> None
    
    let processCommand account (command, ammount) =
        match command with
        | Withdraw -> withdrawWithAudit ammount account
        | Deposit -> depositWithAudit ammount account

    let getCommands =
        seq {
            while true do
                Console.Write "(w)ithdraw (d)eposit e(x)it: "
                let command = Char.ToLower((Console.ReadKey()).KeyChar)
                Console.WriteLine()
                yield command
        }
    
    let initialAccount = 
        Console.Write "What is your name: "
        let owner = Console.ReadLine()
        owner
        |> findTransactionsOnDisk
        |> loadAccount owner

    let closingAccount = getCommands
                        |> Seq.choose tryParseCommand
                        |> Seq.takeWhile (fun x -> x <> Exit)
                        |> Seq.choose tryGetBankOperation
                        |> Seq.choose tryGetAmmount
                        |> Seq.fold processCommand initialAccount
    printfn "%A" closingAccount
    0 // return an integer exit code
