open Kvs

let ConnectionString = "Data Source=sqlite.db"
let connect() = new System.Data.SQLite.SQLiteConnection(ConnectionString)

[<EntryPoint>] //
let main argv =
    use con = connect()
    (con, None) ||> Drop |> printf "drop: %A\n"    
    (con, None) ||> Create |> printf "create: %A\n"       
    (con, None) ||> Insert { Key = "hello";Value = "world" } |> printf "insert: %A\n"      
    (con, None) ||> SelectAll |> printf "All<Kv> %A\n"
    (con, None) ||> SelectByKey "hello" |> printf "ByKey<Kv> %A\n"
    0 // return an integer exit code
