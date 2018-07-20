namespace RailMail

open Microsoft.AspNetCore.Http
module RESTInterface =

  open Giraffe
  open System
  open Microsoft.AspNetCore.Builder
  open Microsoft.AspNetCore.Hosting
  open Microsoft.Extensions.Logging
  open Microsoft.Extensions.DependencyInjection

  open ProcessMessage

  // ---------------------------------
  // Error handler
  // ---------------------------------
  let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

  // ---------------------------------
  // Routing
  // ---------------------------------
  let mailHandler (next: HttpFunc) (ctx: HttpContext) =
    task {
      let! body = ctx.ReadBodyFromRequestAsync()

      let res = processMsg body
      
      return! match res with
              | Choice1Of2 e -> RequestErrors.BAD_REQUEST e next ctx
              | Choice2Of2 _ -> setStatusCode 200 next ctx
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
      RequestErrors.NOT_FOUND "Not Found"    
    ]

  // ---------------------------------
  // Config and Main
  // ---------------------------------
  let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    
    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
      .UseStaticFiles()
      .UseGiraffe(webApp)

  let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore

  let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = true
    builder
      .AddFilter(filter)
      .AddConsole()
      .AddDebug()
    |> ignore
    
  let start () =
    WebHostBuilder()
      .UseKestrel()
      .UseIISIntegration()
      .Configure(Action<IApplicationBuilder> configureApp)
      .ConfigureServices(configureServices)
      .ConfigureLogging(configureLogging)
      .Build()
      .Run()
    ()    