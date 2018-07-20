module RailMail.AMQPHelper

open System.Text

open RabbitMQ.Client
open RabbitMQ.Client.Events

open RailMail.Helpers


let private AMQP_QUEUE = getEnvironmentVariableWithDefault "AMQP_QUEUE" "RAILMAIL_INGEST"
let private AMQP_USERNAME = getEnvironmentVariable "AMQP_USERNAME"
let private AMQP_PASSWORD = getEnvironmentVariable "AMQP_PASSWORD"
let private AMQP_HOST = getEnvironmentVariable "AMQP_HOST"
let private AMQP_PORT = getEnvironmentVariableWithDefault "AMQP_PORT" "5672" |> int


type Queue =
  | RailMailIngest

let private resolve queue =
  match queue with
  | RailMailIngest -> AMQP_QUEUE

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