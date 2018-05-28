module Capstone3.Auditing

open Domain.Transactions
    
let printTransaction accountId owner transaction =
    let getFormOfWas =
        if transaction.Successful then "was"
        else "wasn't"
    printfn "Account %O: %O %c for $%M on %s's account %s successful" accountId transaction.Timestamp transaction.Action transaction.Amount owner getFormOfWas

// Logs to both console and file system
let composedLogger = 
    let loggers =
        [ FileRepository.writeTransaction
          printTransaction ]
    fun accountId owner message ->
        loggers
        |> List.iter(fun logger -> logger accountId owner message)