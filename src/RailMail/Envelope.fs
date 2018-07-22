namespace RailMail

module Envelope =
  
  open Newtonsoft.Json

  [<CLIMutable>]
  type EnvelopeBody =
    {
      text : string
      html : string
    }
    member this.HasErrors() =
      if isNull this.html && isNull this.text
        then Some "Either text or html body must be provided."
        else None

  [<CLIMutable>]
  type Envelope =
    {
      recipients : string list
      subject : string
      body : EnvelopeBody
    }
    member this.HasErrors() =
      let bodyErrors = this.body.HasErrors()
      if bodyErrors.IsSome
        then bodyErrors
        else None


  let parse s =
    let v = JsonConvert.DeserializeObject<Envelope> s
    
    match v.HasErrors() with
    | Some e -> Choice1Of2 e
    | None -> Choice2Of2 v