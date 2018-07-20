namespace RailMail

module ProcessMessage =

  open SMTP

  let processMsg msg =
    let envelope = Envelope.parse msg
    match envelope with
    | Choice1Of2 error -> Choice1Of2 error
    | Choice2Of2 e -> dispatch e; Choice2Of2 ()
                      