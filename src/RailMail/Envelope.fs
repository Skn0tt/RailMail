module RailMail.Envelope

[<CLIMutable>]
type EnvelopeBody =
  {
    text : string
    html : string
  }

[<CLIMutable>]
type Envelope =
  {
    recipient : string
    subject : string
    body : EnvelopeBody
  }
