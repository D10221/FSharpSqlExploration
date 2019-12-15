open Dapper

let ConnectionString = "Data Source=sqlite.db"

let connect() = new Mono.Data.Sqlite.SqliteConnection(ConnectionString)

type Kv =
    { Key: string
      Value: string }

let hello = "hello"
let world = "world"

let Exec (q: string) con =  SqlMapper.Execute(con, q)
let ExecP (q: string) (p: obj) con = SqlMapper.Execute(con, q, p)
let inline Query<'T> (q: string) (p: obj) con = SqlMapper.Query<'T>(con, q, p)

let Run() =
    use con = connect()
    con |> Exec "drop table if exists [Kv]"  |> ignore    
    con |> Exec "create table if not exists [Kv] (Key text Not Null UNIQUE, Value Text Not Null)" |> ignore 
    con |> ExecP "insert into [Kv] (Key, Value) VALUES (@key, @value)" { Key = hello;Value = world }  |> ignore
    con |> Query<Kv> "select * from [KV]" { Key = hello; Value = world }

[<EntryPoint>] //
let main argv =
    Run() |> printf "All<Kv> %A\n"
    0 // return an integer exit code
