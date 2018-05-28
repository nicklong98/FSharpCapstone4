namespace Capstone3.Domain

open System

type Command =
    | Withdraw
    | Deposit
    | Exit

type Customer = {Name: string}
type Account = {AccountId: Guid; Balance: decimal; Owner:Customer}

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