#load "Domain.fs"
#load "Operations.fs"
#load "FileRepository.fs"

open Capstone4.FileRepository
open System

let transactions = tryFindTransactionsOnDisk "nick"

let loadAccountOptional owner = Option.map (Capstone4.Operations.loadAccount owner)

let loadAccount =
        Console.Write "What is your name: "
        let owner = Console.ReadLine()
        owner
        |> tryFindTransactionsOnDisk
        |> (loadAccountOptional owner)