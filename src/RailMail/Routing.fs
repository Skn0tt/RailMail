module RailMail.Routing

open Giraffe
open Microsoft.AspNetCore.Http

open RailMail.Envelope

let mailHandler (next: HttpFunc) (ctx: HttpContext) =
  task {
    let! envelope = ctx.BindJsonAsync<Envelope>()
    Dispatcher.dispatch envelope
    return! setStatusCode 200 next ctx
  }

let webApp: HttpHandler =
  choose [
    GET >=>
      choose [
        route "/status" >=> setStatusCode 200 >=> text "OK"
      ]
    POST >=>
      choose [
        route "/mail" >=> mailHandler
      ]
  ]