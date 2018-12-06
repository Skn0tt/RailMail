# RailMail

> Etymology: (*Reactive Mail* -> Rail) + Mail, just for the rhymes.

RailMail is a Reactive Mail dispatcher written in F#.
It takes Mail requests from AMQP/RabbitMQ and a REST API.
Dispatch happens over SMTP.

- [Blog Post](https://simonknott.de/articles/fsharp-railmail)

## API

Envelope information is transmitted in the following JSON format:

```json
{
  "recipients": "test@test.com",
  "subject": "Funky Mail Dispatcher!",
  "body": {
    "text": "Hey Flo, I found that great service called RailMail! Check it out!",
    "html": "Hey <b>Flo</b>, ... "
  }
}
```
This JSON is sent either to the specified AMQP Queue or POSTed to the REST Endpoint "/mail".

```sh
curl \
  -X POST \
  --data '{ "recipient": ... }' \
  http://railmail:5000/mail
```

## Configuration

| Env           | Default         | Needed |
| ------------- | --------------- | ------ |
| SMTP_HOST     |                 | x      |
| SMTP_PORT     | 587             |        |
| SMTP_USERNAME |                 | x      |
| SMTP_PASSWORD |                 | x      |
| SMTP_SENDER   |                 | x      |
| AMQP_HOST     |                 | x      |
| AMQP_PORT     | 5672            |        |
| AMQP_USERNAME |                 | x      |
| AMQP_PASSWORD |                 | x      |
| AMQP_QUEUE    | RAILMAIL_INGEST |        |

Railmail's standard port is `5000`.


## Build and test the application

### Windows

Run the `build.bat` script in order to restore, build and test (if you've selected to include tests) the application:

```
> ./build.bat
```

### Linux/macOS

Run the `build.sh` script in order to restore, build and test (if you've selected to include tests) the application:

```
$ ./build.sh
```

## Run the application

After a successful build you can start the web application by executing the following command in your terminal:

```
dotnet run src/RailMail
```

After the application has started visit [http://localhost:5000](http://localhost:5000) in your preferred browser.
