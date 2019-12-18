module Kv
open Commands

module Scripts =
    let Create = """ 
    CREATE TABLE IF NOT EXISTS [Kv] ([Key] TEXT NOT NULL UNIQUE, [Value] TEXT Not Null )
    """
    let Drop = """
    DROP TABLE IF EXISTS [Kv]
    """
    let All = "Select [Key], [Value] From [Kv]"

    let ByKey = "Select [Key], [Value] From [Kv] Where [Key] = @key"

    let Insert = """
        INSERT INTO [Kv] ([Key], [Value]) VALUES (@key, @value)
    """

    let Update = """
        UPDATE [Kv] 
        SET [Value] = @value
        Where [Key] = @key 
    """
    let Delete = """
        DELETE from [Kv] Where [Key] = @key
    """

type Kv =
    { Key: string
      Value: string }

let drop: Exec = Exec Scripts.Drop None

let create: Exec = Exec Scripts.Create None

let insert: Kv -> Exec =
    (fun { Key = key; Value = value } ->
    [ ("key", key :> obj)
      ("value", value :> obj) ])
    >> Some
    >> Exec Scripts.Insert

let update: Kv -> Exec =
    (fun { Key = key; Value = value } ->
    [ ("key", key :> obj)
      ("value", value :> obj) ])
    >> Some
    >> Exec Scripts.Update

let delete: string -> Exec =
    (fun key -> [ ("key", key :> obj) ])
    >> Some
    >> Exec Scripts.Delete

let all: Query = Query Scripts.All None

let byKey: string -> Query =
    (fun key -> [ ("key", key :> obj) ])
    >> Some
    >> Query Scripts.ByKey