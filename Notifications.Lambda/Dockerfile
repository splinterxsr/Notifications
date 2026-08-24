FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Notifications.Lambda.csproj", "./"]
RUN dotnet restore "Notifications.Lambda.csproj"

COPY . .
RUN dotnet publish "Notifications.Lambda.csproj" -c Release -o /app/publish

FROM public.ecr.aws/lambda/dotnet:10
WORKDIR /var/task

# Copia o artefato compilado para o diretório de execução da Lambda
COPY --from=build /app/publish .
