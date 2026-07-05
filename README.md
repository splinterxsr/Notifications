# Projeto Tech Challenge FIAP - Notifications Worker

Este projeto tem como objetivo receber mensagens e disparar envio de e-mails quando novos usuários forem cadastrados e quando pagamento for aprovado.

## Estrutura do Projeto

- **Notifications.Worker**: Aplicação Console que consome mensagens das filas users-queue e payments-queue e faz o envio de e-mails.
- **docker-compose.yml**: Configuração do RabbitMQ.

## Pré-requisitos

- .NET 10 SDK
- Docker e Docker Compose

## Como executar

### 1. Iniciar o serviço RabbitMQ
```bash
# Docker-compose
docker-compose up -d
```

### 2. Executar o Notifications.Worker (Terminal)
```bash
cd Notifications.Worker
dotnet run
```

## Tecnologias Utilizadas

- **.NET 10**: Framework principal
- **MassTransit.RabbitMQ 8.5.10**: Biblioteca para comunicação com RabbitMQ
- **RabbitMQ 3.11**: Message broker
