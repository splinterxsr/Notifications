# Projeto Tech Challenge FIAP - Notifications Function

Este projeto tem como objetivo receber eventos de forma assíncrona e simular o envio de e-mails para novos usuários cadastrados e pagamentos aprovados. Refatorado para uma arquitetura Serverless, o serviço não roda mais de forma contínua, sendo acionado sob demanda para otimizar recursos.

---

## Estrutura do Projeto

A aplicação foi migrada de um Worker tradicional para uma função Serverless orientada a eventos, executada dentro da Azure.

* **Notifications.Function:** Função (Azure Functions) autossuficiente invocada via eventos.
* **Notifications.Tests**: Teste unitários básicos para a function.
* **Gatilhos de Execução:** Consome as mensagens em pacotes (batches) dos tópicos `payments-topic` e `users-topic`.

---

## Tecnologias Utilizadas

* **.NET 10:** Framework principal para desenvolvimento backend.
* **Azure Functions (Worker v4)**: Framework de computação serverless para .NET (Isolated Worker) com suporte a ASP.NET Core integrado.
* **Azure Service Bus**: Extensão de mensageria para processamento reativo de filas/tópicos.
* **OpenTelemetry & Azure Monitor**: Stack completa de observabilidade, métricas e telemetria integrada à nuvem da Azure.
---
