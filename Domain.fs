namespace Capstone4.Domain

open System

type Customer = {Name: string}
type Account = {AccountId: Guid; Balance: decimal; Owner:Customer}

module Commands =
    type BankOperation = Deposit | Withdraw
    type Command = AccountCommand of BankOperation | Exit

    let tryParseCommand c =
        match c with
        | 'x' -> Some Exit
        | 'w' -> Some (AccountCommand Withdraw)
        | 'd' -> Some (AccountCommand Deposit)
        | _ -> None

    let tryGetBankOperation command =
        match command with
        | Exit -> None
        | AccountCommand op -> Some op

module Transactions =
    type Transaction = {Amount : decimal; Action: char; Successful: bool; Timestamp: DateTime}
    let serialized transaction =
        sprintf "%O***%c***%M***%b"
            transaction.Timestamp
            transaction.Action
            transaction.Amount
            transaction.Successful
    let deserialize (strTransaction:string) =
        let parts = strTransaction.Split([|'*'|],StringSplitOptions.RemoveEmptyEntries)
        {Timestamp = DateTime.Parse(parts.[0]);Action = (parts.[1].ToCharArray().[0]); Amount = (Decimal.Parse parts.[2]); Successful = (bool.Parse parts.[3])}