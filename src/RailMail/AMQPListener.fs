module RailMail.AMQPListener

open RailMail.AMQPHelper

let start () =
  let unsubscribe = AMQPHelper.subscribe RailMailIngest (fun s ->
      let envelope = Envelope.parse s
      match envelope with
      | Choice1Of2 error -> printf "Error: %s" error
      | Choice2Of2 envelope -> Dispatcher.dispatch envelope
    )
  ()  
