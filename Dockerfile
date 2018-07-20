FROM dcurylo/fsharp-mono-netcore:2.1.301 AS build-env
WORKDIR /app

# Add Paket
COPY .paket/paket.bootstrapper.exe .paket/
RUN mono .paket/paket.bootstrapper.exe

# Install Dependencies
COPY paket.lock paket.dependencies ./
COPY .paket/Paket.Restore.targets .paket/

RUN mono .paket/paket.exe install

# Build
COPY src/RailMail/** src/RailMail/
RUN dotnet publish -c Release -o out src/RailMail

# Copy to Runtime
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/src/RailMail/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "RailMail.dll"]
