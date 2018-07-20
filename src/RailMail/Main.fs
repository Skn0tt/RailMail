namespace RailMail

module Main =

  open dotenv.net
  
  [<EntryPoint>]
  let main _ =
    DotEnv.Config(false, "../../.env")
    AMQPInterface.start()
    RESTInterface.start()
    
    0