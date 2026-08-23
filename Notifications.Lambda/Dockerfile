FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Fcg.Contracts/", "../Fcg.Contracts/"]
COPY ["Notifications.Lambda/Notifications.Lambda.csproj", "./"]

RUN dotnet restore "Notifications.Lambda.csproj"

COPY ["Notifications.Lambda/", "./"]
RUN dotnet build "Notifications.Lambda.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notifications.Lambda.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM public.ecr.aws/lambda/dotnet:10 AS final
WORKDIR /var/task
COPY --from=publish /app/publish .

CMD ["Notifications.Lambda::Notifications.Lambda.Handlers.UserConsumer::Consume"]