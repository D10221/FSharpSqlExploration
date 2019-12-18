module Main

open System.Data.SQLite
open Connections
open Kv
open System.Data.Common
open CommandsAsync

[<Literal>]
let ConnectionString = "Data Source=sqlite.db"

let connect() = new SQLiteConnection(ConnectionString) :> DbConnection

let result x y = x, y

let next con x r = con,x @ [r]

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
            (tran, [])
            |> (fun (con, x) ->
                drop con :> obj
                |> result "drop"
                |> next con x)
            |> (fun (con, x) ->
                create con :> obj
                |> result "create"
                |> next con x)
            |> (fun (con, x) ->
                insert
                    { Key = "hello"
                      Value = "world" } con :> obj
                |> result "insert"
                |> next con x)
            |> (fun (con, x) ->
                update
                    { Key = "hello"
                      Value = "world!" } con :> obj
                |> result "update"
                |> next con x)
            |> (fun (con, x) ->
                all con :> obj
                |> result "all"
                |> next con x)
            |> (fun (con, x) ->
                byKey "hello" con :> obj
                |> result "byKey"
                |> next con x)
            |> (fun (con, x) ->
                delete "hello" con :> obj
                |> result "delete"
                |> next con x)
            |> (fun (con, x) ->
                QueryAsync (sprintf "select '%s' as Key, '%s' as Value" "hello" "async world") None tran
                |> Async.RunSynchronously
                |> (fun x -> x :> obj)
                |> result "async"
                |> next con x)
            |> snd
            |> printf "Commit!: %A\n"
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
