module Capstone4.Operations

open Capstone4.Domain
open Domain.Transactions
open System

let openAccount name balance = 
    {
        AccountId = Guid.NewGuid()
        Balance = balance
        Owner = {
                    Name = name
                }
    }

let classifyAccount account = 
    if account.Balance >= 0M then (InCredit(CreditAccount account))
    else Overdrawn account

let private withdraw amount (CreditAccount account) =
    {account with Balance = account.Balance - amount} |> classifyAccount

let withdrawSafe amount ratedAccount =
    match ratedAccount with
    | InCredit account -> account |> withdraw amount
    | Overdrawn _ -> 
        printfn "Your account is overdrawn - withdrawl rejected!"
        ratedAccount

let requestBalance (ratedAccount:RatedAccount) =
    ratedAccount.GetField(fun x-> x.Balance)

let deposit amount account =
    let account =
        match account with
        | InCredit (CreditAccount account) -> account
        | Overdrawn account -> account
    {account with Balance = account.Balance + amount }
    |> classifyAccount

let auditAs operationName audit operation amount account accountId owner =
    let audit = audit accountId owner
    let updatedAccount = operation amount account

    let transactionSuccessful = updatedAccount <> account

    let transaction = {Amount = amount; Action = operationName; Timestamp = DateTime.UtcNow; Successful = transactionSuccessful}
    audit transaction

    updatedAccount

let loadAccount owner (accountId, transactions) =
    let processTransaction account transaction =
        if transaction.Action = 'w' then {account with Balance = account.Balance - transaction.Amount}
        elif transaction.Action = 'd' then {account with Balance = account.Balance + transaction.Amount}
        else account
    let account = {AccountId = accountId; Balance = 0m; Owner = {Name = owner}}
    transactions 
    |> Seq.filter (fun t -> t.Successful)
    |> Seq.sortBy (fun t -> t.Timestamp)
    |> Seq.fold processTransaction account
    |> classifyAccount