namespace RailMail


module AMQPInterface =
  
  open AMQP
  open ProcessMessage
  
  let start () =
    let unsubscribe = AMQP.subscribe RailMailIngest (fun s ->
      processMsg s |> ignore
      ()
    )
    
    ()
