module Capstone4.FileRepository

open Capstone4.Domain.Transactions
open System
open System.IO

let private accountsPath =
    let path = @"accounts"
    Directory.CreateDirectory path |> ignore
    path

let private tryFindAccountFolder owner =
    let folder = Directory.EnumerateDirectories(accountsPath, sprintf "%s_*" owner)
                    |> Seq.tryHead
    match folder with
    | Some folder -> Some (DirectoryInfo(folder).Name)
    | None -> None
    

let private buildPath(owner, accountId:Guid) = sprintf @"%s\%s_%O" accountsPath owner accountId

let writeTransaction accountId owner transaction =
    let path = buildPath(owner, accountId)
    path |> Directory.CreateDirectory |> ignore
    let filePath = sprintf "%s/%d.txt" path (DateTime.UtcNow.ToFileTimeUtc())
    File.WriteAllText(filePath, serialized transaction)

let tryFindTransactionsOnDisk owner =
    let accountFolder = tryFindAccountFolder owner
    match accountFolder with
    | Some folder ->
        let accountPath = Path.Combine(accountsPath, folder)
        Some ((Guid.Parse(accountPath.Split('_').[1])), accountPath
                                                    |> Directory.EnumerateFiles
                                                    |> Seq.map (deserialize << File.ReadAllText))
    | None -> None