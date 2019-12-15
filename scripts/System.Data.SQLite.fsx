module SystemDataSQLite

#I "../SQLite/netstandard2.0" // SQLite local build
#r "System.Data.SQLite.dll" 

open System.Data
open System.Data.SQLite

let connect (connectionString: string) = new SQLiteConnection(connectionString)
let command() = new SQLiteCommand()

let ExecSql (q: string) (p: List<string * obj> option) (con: IDbConnection) =
    if con.State <> ConnectionState.Open then con.Open()
    use cmd = command()
    cmd.CommandText <- q
    if p <> None then p.Value |> List.iter (fun (k, v) -> cmd.Parameters.Add(SQLiteParameter(k, v)) |> ignore)
    let mutable r = 0
    cmd.Connection <- con :?> SQLiteConnection
    r <- cmd.ExecuteNonQuery()
    r

let inline QuerySql (q: string) (p: List<string * obj> option) (con: IDbConnection) =
    if con.State <> ConnectionState.Open then con.Open()
    use cmd = command()
    cmd.CommandText <- q
    cmd.Connection <- con :?> SQLite.SQLiteConnection
    if p <> None then
        p.Value |> List.iter (fun (k, v) -> cmd.Parameters.Add(SQLite.SQLiteParameter(k, v)) |> ignore)
    let reader = cmd.ExecuteReader()
    [ while reader.Read() do
        yield [ for i = 0 to reader.FieldCount - 1 do
                    yield (reader.GetName(i), reader.GetFieldValue(i)) ] ]