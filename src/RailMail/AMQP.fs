namespace RailMail

module AMQP =

  open System.Text

  open RabbitMQ.Client
  open RabbitMQ.Client.Events

  let private config = AMQPConfig.config()

  type Queue =
    | RailMailIngest

  let private resolve queue =
    match queue with
    | RailMailIngest -> config.queue

  let private declare (channel : IModel) queueName =
    channel.QueueDeclare(queueName, true, false, false, null)

  let private factory =
    ConnectionFactory(
      HostName = config.host,
      Port = config.port,
      UserName = config.username,
      Password = config.password
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