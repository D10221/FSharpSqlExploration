open System.Data
open System.Data.SQLite

let ConnectionString = "Data Source=sqlite.db"

let connect(): IDbConnection = new SQLiteConnection(ConnectionString):> IDbConnection
let SqlCommand ()= new SQLiteCommand() :> IDbCommand
let SqlParameter (name: string) (value: obj) = SQLite.SQLiteParameter(name, value) :> System.Data.IDbDataParameter

let Exec (q: string) (p: (string*obj) list option) (con: IDbConnection) =
    use cmd = SqlCommand()
    cmd.CommandText <- q
    if p <> None then 
        for (k, v) in p.Value do 
            cmd.Parameters.Add(SqlParameter k v)|> ignore
    if con.State <> ConnectionState.Open then con.Open() |> ignore
    cmd.Connection <- con
    cmd.ExecuteNonQuery()

let rec read (reader: IDataReader) =     
    if not (reader.Read()) then [] else 
    let row = [ for i = 0 to reader.FieldCount - 1 do 
                 (reader.GetName i, reader.GetValue i )]
    row:: read reader

let inline Query(q: string) (p: (string*obj) list option) (con: IDbConnection) = 
    if con.State <> ConnectionState.Open then con.Open() |> ignore
    use cmd = SqlCommand()
    cmd.CommandText <- q
    if p <> None then 
        for (k, v) in p.Value do 
            cmd.Parameters.Add(SqlParameter k v)|> ignore
    cmd.Connection <- con
    use reader = cmd.ExecuteReader()
    let r = read reader
    r
       
[<EntryPoint>] //
let main argv =
    use con = connect()
    con |> Exec "drop table if exists [Kv]"  None |> ignore    
    con |> Exec "create table if not exists [Kv] (Key text Not Null UNIQUE, Value Text Not Null)" None |> ignore 
    con |> Exec "insert into [Kv] (Key, Value) VALUES (@key, @value)" (Some ([("key", "hello" :> obj);("value", "world" :> obj)]))  |> ignore
    con |> Query "select * from [KV]" (Some [ ( "key", "hello" :> obj) ; ("value", "world":> obj) ])
    |> printf "All<Kv> %A\n"
    0 // return an integer exit code
