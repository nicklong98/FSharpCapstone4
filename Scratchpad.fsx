#load "Domain.fs"
#load "Operations.fs"
#load "FileRepository.fs"

open Capstone4.FileRepository
open Capstone4.Domain
open Capstone4.Domain.Transactions
open Capstone4.Operations

let auditAs operationName audit operation amount account accountId owner =
    let audit = audit accountId owner
    let updatedAccount = operation amount account

    let transaction = {Amount = amount; Action = operationName; Timestamp = DateTime.UtcNow}
    audit transaction

    updatedAccount