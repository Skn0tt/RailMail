FROM fsharp

ADD src/RailMail src/RailMail

RUN dotnet build src/RailMail

ENTRYPOINT ["mono", "/root/src/Program.exe"] 