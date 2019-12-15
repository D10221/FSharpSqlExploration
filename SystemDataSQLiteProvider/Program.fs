open System
open System.Data
open System.Data.SQLite
open FSharp.Data
open FSharp.Data.Sql

[<Literal>]
let ConnectionString = 
    "Data Source=" + 
    __SOURCE_DIRECTORY__ + @"/sqlite.db;" + 
    "Version=3;foreign keys=true"

[<Literal>] // local build
let ResolutionPath = __SOURCE_DIRECTORY__  + "/../SQLite/netstandard2.1"

type Sql = SqlDataProvider<
                Common.DatabaseProviderTypes.SQLITE, 
                SQLiteLibrary = Common.SQLiteLibrary.SystemDataSQLite,
                ConnectionString = ConnectionString, 
                ResolutionPath = ResolutionPath,
                CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL>

FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O\n")

[<EntryPoint>]
let main argv =
    let ctx = Sql.GetDataContext()
    let kv = ctx.Main.Kv.Create()
    kv.Key <- "hello"
    kv.Value <- "world"
    kv.OnConflict <- FSharp.Data.Sql.Common.Update
    ctx.SubmitUpdates()
    let x = query {for kv in ctx.Main.Kv do head}
    printfn "%A\n" (x.Key, x.Value)
    0 // return an integer exit code
