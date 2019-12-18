module CommandsAsync

open Connections
open System.Data
open System.Data.SQLite
open System.Data.Common

let private await = Async.AwaitTask

type private Parameters = (string * obj) list option

let private parameter key value =
    let p = SQLiteParameter()
    p.ParameterName <- key
    p.Value <- value
    p :> IDbDataParameter

let private command sql (parameters: Parameters) (con: Connectable) =
    let cmd = new SQLiteCommand() :> DbCommand
    cmd.CommandText <- sql
    cmd.Connection <- con.GetConnection()
    if parameters <> None then
        for (key, value) in parameters.Value do
            cmd.Parameters.Add(parameter key value) |> ignore
    cmd

type ExecAsync = Connectable -> Async<int>

let ExecAsync sql parameters (cnx: Connectable) =
    let cmd = command sql parameters cnx
    cmd.ExecuteNonQueryAsync()

let private readValue (reader: DbDataReader) i =
    async {
        let! value = await <| reader.GetFieldValueAsync(i)
        let name = reader.GetName(i)
        return (name, value)
    }

let rec private readReader (reader: DbDataReader) =
    async {
        let! canread = await <| reader.ReadAsync()
        if not (canread) then
            return []
        else
            let row =
                [ 0 .. (reader.FieldCount - 1) ]
                |> List.map (readValue reader)
                |> Async.Parallel
                |> Async.RunSynchronously
            let! prev = readReader reader
            return row :: prev
    }

type QueryAsync = Connectable -> Async<(string * obj) list list>

let QueryAsync sql parameters (con: Connectable) =
    let cmd = command sql parameters con
    async {
        use! reader = Async.AwaitTask(cmd.ExecuteReaderAsync())
        let! res = readReader reader
        return res }
