module Capstone3.Operations

open Capstone3.Domain
open Domain.Transactions
open System

module Commands = 
    let tryParseCommand c =
        match c with
        | 'w' -> (Some Withdraw)
        | 'd' -> (Some Deposit)
        | 'x' -> (Some Exit)
        | _ -> None

let openAccount name balance = 
    {
        AccountId = Guid.NewGuid()
        Balance = balance
        Owner = {
                    Name = name
                }
    }

let withdraw ammount account =
    if ammount <= account.Balance then
        {account with Balance = account.Balance - ammount}
    else
        account

let deposit ammount account =
    if(ammount >= 0m) then
        {account with Balance = account.Balance + ammount}
    else
        account

let auditAs operationName audit operation amount account =
    let audit = audit account.AccountId account.Owner.Name
    let updatedAccount = operation amount account

    let transactionSuccessful = updatedAccount <> account

    let transaction = {Successful = transactionSuccessful; Amount = amount; Action = operationName; Timestamp = DateTime.UtcNow}
    audit transaction

    updatedAccount

let loadAccount owner (accountId, transactions) =
    let processTransaction account transaction =
        if transaction.Action = 'w' then {account with Balance = account.Balance - transaction.Amount}
        elif transaction.Action = 'd' then {account with Balance = account.Balance + transaction.Amount}
        else account
    let account = {AccountId = accountId; Balance = 0m; Owner = {Name = owner}}
    transactions 
    |> Seq.filter (fun t-> t.Successful)
    |> Seq.sortBy (fun t -> t.Timestamp)
    |> Seq.fold processTransaction account