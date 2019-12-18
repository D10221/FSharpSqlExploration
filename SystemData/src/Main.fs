module Main

open System.Data.SQLite
open Connections
open Kv
open System.Data.Common
open CommandsAsync

[<Literal>]
let ConnectionString = "Data Source=sqlite.db"

let connect() = new SQLiteConnection(ConnectionString) :> DbConnection

let Run() =
    printf "%s\n" "Connecting."
    try // can't connect
        let con = connect() |> Connection
        use __ = con |> Disposable // \(0.o)/
        let tran =
            con
            |> Open
            |> beginTransaction

        use __ = tran |> Disposable // \(o.0)/
        printf "%s\n" "Connected!"
        try // handle
            tran
            |> drop
            |> printf "drop: %A\n"
            // ...
            tran
            |> create
            |> printf "create: %A\n"
            // ...
            tran
            |> insert
                { Key = "hello"
                  Value = "world" }
            |> printf "insert: %A\n"
            // ...
            tran
            |> update
                { Key = "hello"
                  Value = "world!" }
            |> printf "update: %A\n"
            // ...
            tran
            |> all
            |> printf "all: %A\n"
            // ...
            tran
            |> byKey "hello"
            |> printf "byKey: %A\n"
            // ...
            tran
            |> delete "hello"
            |> printf "delete: %A\n"
            // ...
            (QueryAsync (sprintf "select '%s' as Key, '%s' as Value" "hello" "async world") None tran) 
            |> Async.RunSynchronously
            |> printf "QueryAsync: %A\n"
            // ...
            printf "%s\n" "Commit"
            commit tran
            printf "%s\n" "Success"

        with ex ->
            printf "rollback: %s\n" ex.Message
            // rollback tran // no need ... use tran
            raise ex
    with ex ->
        eprintf "can't connect: %s\n" ex.Message
        raise ex
    printf "%s\n" "Done."