namespace RailMail

module Main =

  open dotenv.net
  
  [<EntryPoint>]
  let main _ =
    DotEnv.Config(false, "../../.env")

    match AMQPConfig.enabled with
    | true -> AMQPInterface.start()
    | false -> ()
    
    RESTInterface.start()
    
    0