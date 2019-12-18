module Kvs

open MyDapper

type Kv =
    { Key: string
      Value: string }

let Drop: Exec = Exec "drop table if exists [Kv]" None

let Create: Exec = Exec "create table if not exists [Kv] (Key text Not Null UNIQUE, Value Text Not Null)" None

let Insert: Kv -> Exec =
    (fun x -> x :> obj)
    >> Some
    >> Exec "insert into [Kv] (Key, Value) VALUES (@key, @value)"

let SelectAll: Query<Kv> = Query<Kv> "select * from [KV]" None

type Key =
    { Key: string }

let SelectByKey: string -> Query<Kv> =
    (fun key -> { Key = key })
    >> (fun x -> x :> obj)
    >> Some
    >> Query<Kv> "select * from [Kv] where [Key] = @Key"   
