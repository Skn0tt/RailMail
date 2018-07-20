namespace RailMail

module Dispatcher =

  open System.Net.Mail
  open System.Net

  open RailMail.Helpers
  open RailMail.Envelope
  open System.Net.Mime

  let private SMTP_HOST = getEnvironmentVariable "SMTP_HOST"
  let private SMTP_PORT = getEnvironmentVariableWithDefault "SMTP_PORT" "587" |> int
  let private SMTP_USERNAME = getEnvironmentVariable "SMTP_USERNAME"
  let private SMTP_PASSWORD = getEnvironmentVariable "SMTP_PASSWORD"
  let private SMTP_SENDER = getEnvironmentVariable "SMTP_SENDER"

  let private client = new SmtpClient(SMTP_HOST, SMTP_PORT)
  client.EnableSsl <- true
  client.Credentials <- NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD)
  client.DeliveryMethod <- SmtpDeliveryMethod.Network

  let private mimeType = ContentType("text/html")
  let private constructAlternateView (e : Envelope) =
    AlternateView.CreateAlternateViewFromString(e.body.html, mimeType)

  let private constructMessage (e : Envelope) =
    let msg =
      new MailMessage(
        SMTP_SENDER,
        e.recipient,
        e.subject,
        e.body.text
      )
    
    if not(isNull e.body.html) then
      let alternateView = constructAlternateView e  
      msg.AlternateViews.Add(alternateView)
    
    msg  

  let dispatch (e : Envelope) =
    use msg = constructMessage e
    client.Send msg
