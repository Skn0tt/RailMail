module RailMail.Routing

open Giraffe
open Microsoft.AspNetCore.Http

let mailHandler (next: HttpFunc) (ctx: HttpContext) =
  task {
    let! body = ctx.ReadBodyFromRequestAsync()
    
    return! match Envelope.parse body with
            | Choice1Of2 e ->
              RequestErrors.BAD_REQUEST e next ctx
            | Choice2Of2 v ->
              Dispatcher.dispatch v
              setStatusCode 200 next ctx
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