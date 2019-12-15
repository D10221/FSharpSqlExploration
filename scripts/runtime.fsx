module Runtime

open System
open System.Reflection
open System.Runtime
open System.Linq

let GetRuntime () = 
    let mono = Type.GetType("Mono.Runtime")
    (if isNull mono then "NOT Mono.Runtime" else "Mono.Runtime"), 
    (Assembly.GetEntryAssembly().GetCustomAttributesData()
             .FirstOrDefault((fun a -> 
                a.AttributeType = typedefof<Versioning.TargetFrameworkAttribute>)))
             .ConstructorArguments    
