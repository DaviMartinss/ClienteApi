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
Note que o banco de dados foi criado
![image](https://github.com/user-attachments/assets/8e283145-748a-4264-9d36-803e7acd3a3f)

### 3. Inicializar o RabbitMQ
O RabbitMQ está configurado no **Docker**, portanto:
1. **Abra o Docker Desktop** e inicie o container do RabbitMQ.
![image](https://github.com/user-attachments/assets/c73a584c-d01f-4c90-adbd-06bc2b32ccc9)
3. Acesse a interface do RabbitMQ pelo navegador:
   - [http://localhost:15672/](http://localhost:15672/)
4. Faça login com as credenciais padrão:
   - **Username**: `guest`
   - **Password**: `guest`
![image](https://github.com/user-attachments/assets/154c3649-cd9c-41c7-8e28-db7a28b24c9e)

5. Após logar, vá até a opção **Exchanges** para gerenciar a mensageria.

## Abertura do Swagger

Após executar a API, o **Swagger** será aberto automaticamente em seu navegador, permitindo a visualização e interação com os endpoints da aplicação.

![Swagger](https://github.com/user-attachments/assets/3a6ae5c2-8c11-45b5-b339-7eef152d3937)

## 1º Caso - Cadastro de Cliente

Para realizar o cadastro de um cliente, é necessário informar os seguintes dados obrigatórios:

- **Nome**
- **Email** (único no banco)
- **CEP**

![Cadastrar Cliente](https://github.com/user-attachments/assets/fdfa888b-3256-4b03-bc70-eaac94fa7453)

### Retorno da API

Após a requisição, a API retorna uma resposta de **sucesso**, indicando que o cliente foi cadastrado corretamente.

![Retorno da API](https://github.com/user-attachments/assets/0a8c8a4e-9cd9-4b41-86b4-8c03c996fbb9)

### Processamento dos Dados

- Os dados do cliente foram **persistidos** no banco de dados na tabela `clientes`.

![Cliente no Banco](https://github.com/user-attachments/assets/ab8155e2-00af-45ad-871d-43820280751b)

- Uma **mensagem foi publicada no RabbitMQ**, notificando o evento de cadastro.

![Total do MQ](https://github.com/user-attachments/assets/445dc0ac-2e1b-487e-82dd-19db4557d8da)

![Dado no RabbitMQ](https://github.com/user-attachments/assets/38831087-d409-4724-a485-b02d22bc1746)

## 2º Caso - Listar Cliente

Para buscar um cliente cadastrado no banco de dados, basta realizar uma requisição **GET**, informando o **Id** do cliente.

### Exemplo de Requisição

- **Método:** GET
- **Endpoint:** `/clientes/{id}`

![exemplo de get](https://github.com/user-attachments/assets/f1dfcc19-4cd0-4c3b-819c-4917bfe81a84)

### Retorno da API

Se o cliente existir, a API retornará os dados cadastrados.

![Retorno de Busca](https://github.com/user-attachments/assets/cea70c91-024d-46aa-aec3-f42e8be628b5)


Caso o **Id** informado não exista, será retornado um erro **404 - Not Found**.
![erro no get](https://github.com/user-attachments/assets/75601af2-b129-468c-94cf-06c73e4f8041)


## 3º Caso - Atualizar Cliente

Agora vamos realizar a **atualização** do nome e email do cliente.

### Informações que podem ser editadas:

- **Nome**
- **Email** (único no banco)
- **Dados relacionados ao CEP** *(para isso, basta inserir um novo CEP)*

### Exemplo de Requisição

- **Método:** PUT
- **Endpoint:** `/clientes/{id}`

### Retorno da API

Se a atualização for bem-sucedida, a API retornará os **dados atualizados do cliente**, confirmando que a alteração foi aplicada.

### Dados Atualizados no Banco
![retorno update](https://github.com/user-attachments/assets/cb4993ff-4236-42ca-a0ac-788b90625904)

Após a atualização, os **dados do cliente foram modificados no banco de dados**, garantindo que as novas informações foram persistidas corretamente.
![dado no banco update](https://github.com/user-attachments/assets/9e5a26ac-2e0b-4c7d-9840-ff13ac0f8d42)

## 4º Caso - Executar o Worker para Consumir Mensagens do RabbitMQ

Por fim, vamos executar o **worker `ClienteApi.SendClientRegisteredEmail`** para consumir os dados que estão armazenados no **RabbitMQ**.

### Passos para executar o Worker:

1. **Setar o projeto** que será executado, conforme a imagem abaixo:
![set](https://github.com/user-attachments/assets/fcb4b8d4-731f-4719-8edc-1aee78f1dd84) 

2. **Executar o Worker** para iniciar o consumo das mensagens.

3. **Verificar que os dados foram consumidos da fila**, confirmando que o processo foi realizado corretamente.

![Dados consumidos](https://github.com/user-attachments/assets/d981ffde-0649-4a21-a998-8e5a8e0dd9e0)

Simulando envio
![simulando envio](https://github.com/user-attachments/assets/00a3b4bf-ee64-491c-a51a-85b84e00f07f)

