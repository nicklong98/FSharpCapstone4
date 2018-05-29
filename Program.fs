// Learn more about F# at http://fsharp.org

module Capstone4.Program

open System
open Auditing
open Operations
open FileRepository
open Capstone4.Domain.Commands
open Capstone4.Domain


[<EntryPoint>]
let main _ =
    
    let withdrawWithAudit = auditAs 'w' composedLogger withdrawSafe
    let depositWithAudit = auditAs 'd' composedLogger deposit

    let tryGetamount command =
        Console.WriteLine()
        Console.Write "Enter amount: $"
        let amount = Console.ReadLine() |> Decimal.TryParse
        match amount with
        | true, amount -> Some(command, amount)
        | false, _ -> None

    let tryLoadAccountFromDisk owner =
        let loadAccountOptional owner = Option.map (Capstone4.Operations.loadAccount owner)
        owner |> tryFindTransactionsOnDisk |> (loadAccountOptional owner)
    
    let processCommand (ratedAccount:RatedAccount) (command, amount) =
        let accountId = ratedAccount.GetField(fun x -> x.AccountId)
        let owner = ratedAccount.GetField(fun x-> x.Owner).Name
        match command with
        | Withdraw -> withdrawWithAudit amount ratedAccount accountId owner
        | Deposit -> depositWithAudit amount ratedAccount accountId owner

    let getCommands =
        seq {
            while true do
                Console.Write "(w)ithdraw (d)eposit e(x)it: "
                let command = Char.ToLower((Console.ReadKey()).KeyChar)
                Console.WriteLine()
                yield command
        }
    
    let initialAccount = 
        let loadAccountOptional owner = Option.map (Capstone4.Operations.loadAccount owner)
        Console.Write "What is your name: "
        let owner = Console.ReadLine()

        match (tryLoadAccountFromDisk owner) with
        | Some account -> account
        | None -> openAccount owner 0m |> classifyAccount

    printfn "Opening balance: $%M" (initialAccount.GetField(fun x-> x.Balance))

    let closingAccount = getCommands
                        |> Seq.choose tryParseCommand
                        |> Seq.takeWhile (fun x -> x <> Exit)
                        |> Seq.choose tryGetBankOperation
                        |> Seq.choose tryGetamount
                        |> Seq.fold processCommand initialAccount
    printfn "%A" closingAccount
    0 // return an integer exit code
