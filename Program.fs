// Learn more about F# at http://fsharp.org

module Capstone3.Program

open System
open Auditing
open Operations
open Operations.Commands
open FileRepository
open Capstone3.Domain


[<EntryPoint>]
let main _ =
    
    let withdrawWithAudit = auditAs 'w' composedLogger withdraw
    let depositWithAudit = auditAs 'd' composedLogger deposit

    let getAmmount command =
        Console.Write "Enter amount: $"
        let response = Console.ReadLine()
        Console.WriteLine()
        command, Decimal.Parse response
    
    let processCommand account (command, ammount) =
        match command with
        | Withdraw -> withdrawWithAudit ammount account
        | Deposit -> depositWithAudit ammount account
        | Exit -> account

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
                        |> Seq.takeWhile ((<>) Exit)
                        |> Seq.map getAmmount
                        |> Seq.fold processCommand initialAccount
    printfn "%A" closingAccount
    0 // return an integer exit code
