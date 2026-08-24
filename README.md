# \# Projeto Tech Challenge FIAP - Notifications Lambda

# 

# Este projeto tem como objetivo receber eventos de forma assíncrona e simular o envio de e-mails para novos usuários cadastrados e pagamentos aprovados. Refatorado para uma arquitetura Serverless, o serviço não roda mais de forma contínua, sendo acionado sob demanda para otimizar recursos.

# 

# \---

# 

# \## Estrutura do Projeto

# 

# A aplicação foi migrada de um Worker tradicional para uma função Serverless orientada a eventos, executada dentro de containers via emulação da AWS.

# 

# \*   \*\*Notifications.Lambda:\*\* Função (AWS Lambda) autossuficiente invocada via eventos.

# \*   \*\*Gatilhos de Execução:\*\* Consome as mensagens em pacotes (batches) das filas `users-queue` e `payments-2-queue`.

# \*   \*\*Infraestrutura:\*\* Empacotamento via imagem Docker e implantação via Serverless Framework.

# 

# \---

# 

# \## Tecnologias Utilizadas

# 

# \*   \*\*.NET 10:\*\* Framework principal para desenvolvimento backend.

# \*   \*\*Serverless Framework:\*\* Ferramenta de Infraestrutura como Código (IaC) para implantação.

# \*   \*\*LocalStack:\*\* Simulador local do ecossistema AWS (Lambda, SQS, ECR).

# \*   \*\*MassTransit:\*\* Abstração utilizada para ler o envelope de mensagens gerado pelos produtores.

# \*   \*\*Amazon SQS:\*\* Serviço de filas utilizado no lugar do RabbitMQ.

# 

# \---

# 

# \## Como Executar Localmente

# 

# Antes de iniciar, certifique-se de que o \*\*LocalStack\*\* já esteja em execução através do `docker-compose` no repositório de orquestração.

# 

# 1\. Abra o seu terminal e navegue até a pasta raiz do projeto (`Notifications.Lambda`).

# 2\. Instale as dependências de infraestrutura executando o comando `npm install -D serverless-localstack`.

# 3\. Realize a implantação da Lambda e a criação das filas SQS executando `serverless deploy --stage local`.

# 4\. Monitore a saúde do serviço verificando se os logs indicam sucesso na implantação.

