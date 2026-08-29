# Projeto Tech Challenge FIAP - Notifications Lambda

Este projeto tem como objetivo receber eventos de forma assíncrona e simular o envio de e-mails para novos usuários cadastrados e pagamentos aprovados. Refatorado para uma arquitetura Serverless, o serviço não roda mais de forma contínua, sendo acionado sob demanda para otimizar recursos.

---

## Estrutura do Projeto

A aplicação foi migrada de um Worker tradicional para uma função Serverless orientada a eventos, executada dentro de containers via emulação da AWS.

* **Notifications.Lambda:** Função (AWS Lambda) autossuficiente invocada via eventos.
* **Gatilhos de Execução:** Consome as mensagens em pacotes (batches) das filas `users-queue` e `payments-2-queue`.
* **Infraestrutura:** Empacotamento em arquivo `.zip` e implantação via Serverless Framework.

---

## Tecnologias Utilizadas

* **.NET 10:** Framework principal para desenvolvimento backend.
* **Serverless Framework:** Ferramenta de Infraestrutura como Código (IaC) para implantação.
* **LocalStack:** Simulador local do ecossistema AWS (Lambda, SQS, S3, CloudWatch Logs).
* **MassTransit:** Abstração utilizada para ler o envelope de mensagens gerado pelos produtores.
* **Amazon SQS:** Serviço de filas utilizado no lugar do RabbitMQ.

---

## Como Executar Localmente

Antes de iniciar, certifique-se de que o **LocalStack** já esteja em execução através do `docker-compose` no repositório de orquestração. 

*Nota: Como o ambiente local roda inteiramente em memória, os passos de implantação abaixo devem ser repetidos sempre que os containers da orquestração forem recriados.*

### Passo a Passo da Implantação

**1. Instalar as dependências do Serverless:**
Abra o seu terminal na pasta raiz do projeto (`Notifications.Lambda`) e instale o plugin do LocalStack:
```bash
npm install -D serverless-localstack

```

**2. Instalar a ferramenta global da AWS Lambda:**
Execute o seguinte comando no terminal para instalar a ferramenta no seu sistema:
```bash
dotnet tool install -g Amazon.Lambda.Tools
```

**3. Compilar o projeto C# gerar Zip:**

```bash
dotnet lambda package -o package.zip
```

**4. Realizar o Deploy na Nuvem Local:**
Faça a implantação da Lambda e a criação automática das filas SQS no LocalStack:

```bash
npx serverless deploy --stage local --force

```

**5. Monitorar os Logs (Opcional):**
Para acompanhar as execuções da função e os "e-mails" sendo enviados em tempo real, deixe o comando abaixo rodando em uma janela de terminal:

```bash
npx serverless logs -f userNotification --stage local --tail
```
**Importante:**
O comando acima só vai funcionar após alguma mensagem ser publicada ao menos 1 vez.
