namespace RailMail

module SMTP =
  
  open System.Net
  open System.Net.Mail
  open System.Net.Mime
  
  open Envelope

  let private config = SMTPConfig.config
  let private fromAddress = new MailAddress(config.sender)

  let private client = new SmtpClient(config.host, config.port)
  client.EnableSsl <- true
  client.Credentials <- NetworkCredential(config.username, config.password)
  client.DeliveryMethod <- SmtpDeliveryMethod.Network

  let private mimeType = ContentType("text/html")
  let private constructAlternateView (e : Envelope) =
    AlternateView.CreateAlternateViewFromString(e.body.html, mimeType)

  let private constructMessage (e : Envelope) =
    let msg = new MailMessage()
    msg.From <- fromAddress
    msg.Body <- e.body.text
    msg.Subject <- e.subject
    
    for r in e.recipients do
      msg.To.Add(r)
    
    if not(isNull e.body.html) then
      let alternateView = constructAlternateView e  
      msg.AlternateViews.Add(alternateView)
    
    msg  

  let dispatch (e : Envelope) =
    use msg = constructMessage e
    client.Send msg
