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

## Execução do Projeto

### 1. Configuração do Banco de Dados
Antes de executar a API, é necessário configurar a conexão com o banco. Para isso:

1. Acesse **WebApi → appsettings.json**.
2. Informe os dados de conexão:
   - **Host**: Endereço do banco de dados.
   - **Database**: Nome do banco de dados.
   - **Username**: Usuário do banco.
   - **Password**: Senha de acesso.

### 2. Executar Migrations
Após configurar o banco, é necessário executar as migrações para preparar o esquema do banco de dados:

```sh
dotnet ef migrations remove # Remove uma migration antiga, se necessário
dotnet ef migrations add NomeDaMigration # Cria uma nova migration
dotnet ef database update # Aplica as migrations e cria o banco
```

### 3. Inicializar o RabbitMQ
O RabbitMQ está configurado no **Docker**, portanto:

1. **Abra o Docker Desktop** e inicie o container do RabbitMQ.
2. Acesse a interface do RabbitMQ pelo navegador:
   - [http://localhost:15672/](http://localhost:15672/)
3. Faça login com as credenciais padrão:
   - **Username**: `guest`
   - **Password**: `guest`
4. Após logar, vá até a opção **Exchanges** para gerenciar a mensageria.
