# Projeto Tech Challenge FIAP - Notifications Worker

Este projeto tem como objetivo receber mensagens e disparar envio de e-mails quando novos usuários forem cadastrados e quando pagamento for aprovado.

## Estrutura do Projeto

- **Notifications.Worker**: Aplicação Console que consome mensagens das filas users-queue e payments-queue e faz o envio de e-mails.

## Tecnologias Utilizadas

- **.NET 10**: Framework principal
- **MassTransit.RabbitMQ 8.5.10**: Biblioteca para comunicação com RabbitMQ
