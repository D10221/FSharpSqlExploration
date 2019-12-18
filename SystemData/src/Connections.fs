module Connections

open System
open System.Data
open System.Data.Common

type Connectable =
    | Connection of DbConnection
    | Transaction of DbTransaction
// | Pair of (IDbConnection*IDbTransaction)
// | OptionalPair of (IDbConnection*IDbTransaction option)
let getConnection =
    function
    | Connection con -> con
    | Transaction t -> t.Connection 

type Connectable with
    member x.GetConnection () = getConnection x

let Disposable =
    function
    | Transaction x ->
        { new IDisposable with
            // spy
            member _.Dispose() =
                printf "Disposing: %A\n" x
                x.Dispose() }
    | Connection x ->
        { new IDisposable with
            // spy
            member _.Dispose() =
                printf "Disposing: %A\n" x
                x.Dispose() }

let rec dispose x =
    match x with
    | Transaction tran ->
        printf "disposing %A\n" tran
        try
            if not (isNull tran.Connection) && tran.Connection.State = ConnectionState.Open then
                printf "Warn: Dispose Will close Connection!\n"
            tran.Dispose()
            printf "%A: Disposed\n" tran
        with
        | :? ObjectDisposedException ->
            printf "Warn: Can't Dispose: Already Disposed\n"
            ()
        | ex ->
            printf "Err: Can't Dispose: %A\n" ex
            raise ex
    | Connection con ->
        printf "disposing %A\n" con
        try
            con.Dispose()
            printf "%A disposed\n" con
        with
        | :? ObjectDisposedException ->
            printf "Can't Dispose %A Already Disposed\n" con
            ()
        | ex ->
            printf "Can't dispose  %A, ex: %A\n" con ex

let beginTransaction (con: Connectable) =
    match con with
    | Connection con ->
        printf "Begin Transaction ..."
        con.BeginTransaction() |> Transaction
    | Transaction tran -> failwithf "Not Implemented beginTransaction of %A" tran

let commit (con: Connectable) =
    match con with
    | Transaction t -> t.Commit()
    | Connection c -> failwithf "Can't commit Connection %A" c

let rollback (con: Connectable) =
    match con with
    | Transaction t -> t.Rollback()
    | Connection c -> failwithf "Can't rollback Connection %A" c

let Open(connection: Connectable) =
    match connection with
    | Connection connection ->
        if connection.State <> ConnectionState.Open then connection.Open()
    | _ ->
        // ignore
        ()
    connection
