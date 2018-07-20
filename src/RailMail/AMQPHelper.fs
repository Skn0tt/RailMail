module RailMail.AMQPHelper

open System
open System.Text

open RabbitMQ.Client
open RabbitMQ.Client.Events


let private INGEST_QUEUE = match Environment.GetEnvironmentVariable "AMQP_QUEUE" with
                           | null -> "RAILMAIL_INGEST"
                           | v -> v           

let private AMQP_USERNAME = Environment.GetEnvironmentVariable "AMQP_USERNAME"
let private AMQP_PASSWORD = Environment.GetEnvironmentVariable "AMQP_PASSWORD"
let private AMQP_HOST = Environment.GetEnvironmentVariable "AMQP_HOST"
let private AMQP_PORT = Environment.GetEnvironmentVariable "AMQP_PORT" |> int


type Queue =
  | RailMailIngest

let private resolve queue =
  match queue with
  | RailMailIngest -> INGEST_QUEUE

let private declare (channel : IModel) queueName =
  channel.QueueDeclare(queueName, true, false, false, null)

let private factory =
  ConnectionFactory(
    HostName = AMQP_HOST,
    Port = AMQP_PORT,
    UserName = AMQP_USERNAME,
    Password = AMQP_PASSWORD
  )

let subscribe queue callback =
  let connection = factory.CreateConnection()
  let model = connection.CreateModel()

  let queueName =
    queue
    |> resolve
    |> (fun qn -> declare model qn |> ignore ; qn)

  let consumer = EventingBasicConsumer(model)  
  consumer.Received.Add((fun message ->
    message.Body
    |> Encoding.UTF8.GetString
    |> callback
  ))

  model.BasicConsume(queueName, true, consumer) |> ignore

  (fun () ->
    model.Close()
    connection.Close()
  )