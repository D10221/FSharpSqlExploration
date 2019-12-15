#!/usr/bin/env fsi
#r "../packages/Mono.Data.Sqlite/lib/net40/Mono.Data.Sqlite.dll"

#load "./runtime.fsx"
Runtime.GetRuntime() |> printf "%A\n"

open System.Data
open Mono.Data.Sqlite

let ConnectionString = "Data Source=sqlite.db"
let connect() = new SqliteConnection(ConnectionString)
let command () = new SqliteCommand()

let Exec (q: string) (p: List<(string * obj)> option) (con: IDbConnection) =
    if con.State <> ConnectionState.Open then con.Open()
    use cmd = command()
    cmd.CommandText <- q
    if p <> None then 
      p.Value |> List.iter (fun (k, v) ->
      cmd.Parameters.Add(SqliteParameter(k, v)) |> ignore )
    let mutable r = 0
    cmd.Connection <- con :?> SqliteConnection
    r <- cmd.ExecuteNonQuery()
    r

let inline Query (q: string) (p: List<(string * obj)> option) (con: IDbConnection) =
    if con.State <> ConnectionState.Open then con.Open()
    use cmd = command()
    cmd.CommandText <- q
    cmd.Connection <- con :?> SqliteConnection
    if p <> None then 
      p.Value |> List.iter (fun (k, v) ->
      cmd.Parameters.Add(SqliteParameter(k, v)) |> ignore )
    use reader = cmd.ExecuteReader()
    [ while reader.Read() do
        yield [ for i = 0 to reader.FieldCount - 1 do
                    yield (reader.GetName(i), reader.GetFieldValue(i)) ] ]

let Run() =
    use con = connect()
    Exec "drop table if  exists Kv" None con |> ignore
    Exec "create table if not exists Kv (Key text Not Null UNIQUE, Value Text Not Null)" None con |> ignore
    Exec "insert into KV (Key, Value) VALUES (@key, @value)"
        (Some [ ("@key", "hello" :> obj); ("@value",  "world" :> obj )]) con |> ignore
    Query "select * from KV" None con

Run()
|> printf "All<Kv> %A\n"