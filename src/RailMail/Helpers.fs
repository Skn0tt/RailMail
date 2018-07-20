namespace RailMail

module Helpers =

  open System

  let getEnvironmentVariableOption name =
    let var: string = Environment.GetEnvironmentVariable name
    
    match var with
    | null -> None
    | n -> Some n
  
  let getEnvironmentVariable name =
    match getEnvironmentVariableOption name with
    | Some n -> n
    | None ->
      failwith (sprintf "Please provide %s environment variable." name)

  let getEnvironmentVariableWithDefault name defaultVal =
    match getEnvironmentVariableOption name with
    | Some n -> n
    | None -> defaultVal
