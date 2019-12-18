module MyDapper

open Dapper
open System.Data

type Query<'T> = IDbConnection -> Option<IDbTransaction> -> System.Collections.Generic.IEnumerable<'T>

type Exec = IDbConnection -> Option<IDbTransaction> -> int

let Exec (q: string) (p: obj option): Exec =
    fun con tran ->
        let transaction =
            if tran = None then null
            else tran.Value
        match p with
        | None -> SqlMapper.Execute(con, q, null, transaction)
        | p -> SqlMapper.Execute(con, q, p.Value, transaction)

let Query<'T> (q: string) (p: obj option): Query<'T> =
    fun con tran ->
        let transaction =
            if tran = None then null
            else tran.Value
        match p with
        | None -> SqlMapper.Query<'T>(con, q, null, transaction)
        | _ -> SqlMapper.Query<'T>(con, q, p.Value, transaction)
