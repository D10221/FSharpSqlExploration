module Commands 
open Connections
open System.Data
open System.Data.SQLite
open System.Data.Common

type Parameters = (string * obj) list option

let parameter key value =
    let p = SQLiteParameter()
    p.ParameterName <- key
    p.Value <- value
    p :> IDbDataParameter

let command sql (parameters: Parameters) (con: Connectable) =
    let cmd = new SQLiteCommand() :> DbCommand
    cmd.CommandText <- sql
    cmd.Connection <- con.GetConnection()
    if parameters <> None then
        for (key, value) in parameters.Value do
            cmd.Parameters.Add(parameter key value) |> ignore
    cmd

type Exec = Connectable -> int

let Exec sql parameters (cnx: Connectable) =
    let cmd = command sql parameters cnx
    cmd.ExecuteNonQuery()

let rec readReader (reader: IDataReader) =
    if not (reader.Read()) then
        []
    else
        let row =
            [ for i = 0 to reader.FieldCount - 1 do
                yield (reader.GetName(i), reader.GetValue(i)) ]
        row :: readReader reader

type Query = Connectable -> (string * obj) list list

let Query sql parameters (con: Connectable) =
    let cmd = command sql parameters con
    use reader = cmd.ExecuteReader()
    readReader reader
