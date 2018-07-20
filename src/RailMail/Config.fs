namespace RailMail

open Helpers

module SMTPConfig =
  
  type T =
    { host : string
      port : int
      username : string
      password : string
      sender : string }
  let config =
    
    let host =
      getEnvironmentVariable "SMTP_HOST"

    let port =
      getEnvironmentVariableWithDefault "SMTP_PORT" "587"
      |> int

    let username =
      getEnvironmentVariable "SMTP_USERNAME"

    let password =
      getEnvironmentVariable "SMTP_PASSWORD"    

    let sender =
      getEnvironmentVariable "SMTP_SENDER"    

    { host = host
      port = port
      username = username
      password = password
      sender = sender }

module AMQPConfig =

  type T =
    { host : string
      port : int
      username : string
      password : string
      queue : string }

  let private host =
    getEnvironmentVariableOption "AMQP_HOST"

  let private port =
    getEnvironmentVariableWithDefault "AMQP_PORT" "587"
    |> int

  let private username =
    getEnvironmentVariableOption "AMQP_USERNAME"

  let private password =
    getEnvironmentVariableOption "AMQP_PASSWORD"

  let private queue =
    getEnvironmentVariableWithDefault "AMQP_QUEUE" "RAILMAIL_INGEST"

  let enabled =
    [ host; username; password ]
    |> List.forall Option.isSome

  let config () =
    { host = host.Value
      port = port
      username = username.Value
      password = password.Value
      queue = queue }
      