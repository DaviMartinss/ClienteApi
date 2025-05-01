# Microsserviço de Cadastro de Clientes

## Objetivo

Este projeto consiste na implementação de uma **API REST** e um **Microsserviço Worker** em C# para o cadastro e processamento de clientes, utilizando boas práticas de desenvolvimento, mensageria com RabbitMQ e persistência no banco de dados PostgreSQL.

## Tecnologias Utilizadas

- C# (.NET 8)
- PostgreSQL
- RabbitMQ
- Docker Desktop
- API ViaCEP para consulta de endereços

## Arquitetura

A implementação é composta por:

1. **API REST**: Responsável pelo cadastro, consulta e alteração de clientes.
2. **Microsserviço Worker**: Consumidor de mensagens do RabbitMQ, que processa eventos de novo cliente e simula o envio de email.

## Funcionalidades

### API REST - Cadastro de Cliente
- Endpoints para criar, consultar e alterar clientes.
- Campos obrigatórios: **Nome, Email (único no banco) e CEP**.
- Validação e consulta do CEP via [ViaCEP](https://viacep.com.br/ws/{cep}/json/).
- Persistência no banco dos seguintes dados:
  - Nome
  - Email
  - CEP
  - Logradouro
  - Bairro
  - Cidade
  - Estado
- Tratamento de erros:
  - Email duplicado → **HTTP 400 - Bad Request**.
  - CEP inválido → **HTTP 400 - Bad Request**.
  - Erros inesperados → **HTTP 500**, sem exposição de dados sensíveis.

### Microsserviço Worker - Processamento de Eventos
- Consome mensagens publicadas no **RabbitMQ** após o cadastro de um novo cliente.
- Processa o evento e simula o envio de um email para um endereço pré-configurado (exemplo: **notificacao@empresa.com**).
