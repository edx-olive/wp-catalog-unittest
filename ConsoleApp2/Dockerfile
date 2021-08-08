FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app

COPY . ./
RUN dotnet publish CampusHomePage.csproj -c Release -o out --self-contained -r linux-x64 /p:PublishTrimmed=true

FROM selenium/standalone-chrome
WORKDIR /app

COPY --from=build-env /app/out .
COPY entrypoint.sh .

ENTRYPOINT ["./entrypoint.sh"]