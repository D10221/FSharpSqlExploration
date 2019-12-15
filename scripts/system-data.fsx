#!/usr/bin/env fsi

#load "./runtime.fsx"
Runtime.GetRuntime() |> printf "%A\n"

#load "./System.Data.SQLite.fsx"
open SystemDataSQLite

let ConnectionString = "Data Source=sqlite.db"

let Run() =
    use con = connect(ConnectionString)
    ExecSql "drop table if  exists Kv" None con |> ignore
    ExecSql "create table if not exists Kv (Key text Not Null UNIQUE, Value Text Not Null)" None con |> ignore
    ExecSql "insert into KV (Key, Value) VALUES (@key, @value)"
        (Some
            [ ("@key", "hello" :> obj)
              ("@value", "world" :> obj) ]) con |> ignore
    QuerySql "select * from KV" None con

Run()
|> printf "All<Kv> %A\n"
