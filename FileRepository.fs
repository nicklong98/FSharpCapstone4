module Capstone3.FileRepository

open Domain.Transactions
open System
open System.IO

let private accountsPath =
    let path = @"accounts"
    Directory.CreateDirectory path |> ignore
    path

let private findAccountFolder owner =
    let folders = Directory.EnumerateDirectories(accountsPath, sprintf "%s_*" owner)
    if Seq.isEmpty folders then ""
    else
        let folder = Seq.head folders
        DirectoryInfo(folder).Name

let private buildPath(owner, accountId:Guid) = sprintf @"%s\%s_%O" accountsPath owner accountId

let writeTransaction accountId owner transaction =
    let path = buildPath(owner, accountId)
    path |> Directory.CreateDirectory |> ignore
    let filePath = sprintf "%s/%d.txt" path (DateTime.UtcNow.ToFileTimeUtc())
    File.WriteAllText(filePath, serialized transaction)

let findTransactionsOnDisk owner =
    let accountFolder = findAccountFolder owner
    let accountPath = Path.Combine (accountsPath, accountFolder)
    if String.IsNullOrEmpty accountFolder then (Guid.NewGuid(), Seq.empty)
    else
        (Guid.Parse(accountPath.Split('_').[1])), accountPath
                                                    |> Directory.EnumerateFiles
                                                    |> Seq.map (deserialize << File.ReadAllText)