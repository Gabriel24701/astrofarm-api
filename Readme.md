# 🛰️ AstroFarm API

> Backend da solução **AstroFarm** — monitoramento agrícola inteligente com integração de telemetria via satélite e IoT, desenvolvido para a **Global Solution FIAP**.

---

## 🎬 Links do Projeto

| Recurso | Link |
|---------|------|
| 📹 **Vídeo Pitch** (máx. 3 min) | [▶ Assistir no YouTube](#) *(placeholder)* |
| 🎥 **Vídeo Demonstração** (máx. 8 min) | [▶ Assistir no YouTube](#) *(placeholder)* |
| 📦 **Repositório** | [github.com/Gabriel24701/astrofarm-api](https://github.com/Gabriel24701/astrofarm-api.git) |

---

## 🏷️ Badges

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-ASP.NET_Core_8-239120?style=flat-square&logo=csharp&logoColor=white)
![Entity Framework](https://img.shields.io/badge/ORM-Entity_Framework_Core-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Oracle](https://img.shields.io/badge/Database-Oracle_DB-F80000?style=flat-square&logo=oracle&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker&logoColor=white)
![Azure](https://img.shields.io/badge/Cloud-Azure_VM-0078D4?style=flat-square&logo=microsoftazure&logoColor=white)
![Swagger](https://img.shields.io/badge/Docs-Swagger_UI-85EA2D?style=flat-square&logo=swagger&logoColor=black)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

---

## 📑 Índice

1. [Visão Geral](#-visão-geral)
2. [Tecnologias e Stack](#-tecnologias-e-stack)
3. [Arquitetura do Projeto](#-arquitetura-do-projeto)
4. [Diagrama de Arquitetura](#-diagrama-de-arquitetura)
5. [Desenvolvimento](#-desenvolvimento)
6. [Entidades e Endpoints](#-entidades-e-endpoints)
7. [Execução Local com Docker](#-execução-local-com-docker)
8. [Testes e Payloads](#-testes-e-payloads)
9. [Deploy em Produção — Azure](#-deploy-em-produção--azure)
10. [Equipe](#-equipe)

---

## 🌱 Visão Geral

O **AstroFarm** é uma solução de monitoramento agrícola que conecta produtores rurais a dados precisos sobre suas propriedades. O backend expõe uma API RESTful consumida pelo aplicativo **Mobile (React Native)**, fornecendo autenticação de produtores e gestão completa de propriedades agrícolas.

A plataforma foi projetada com foco em:

- **Escalabilidade** — infraestrutura 100% conteinerizada com Docker
- **Rastreabilidade** — versionamento de banco de dados via EF Core Migrations
- **Operabilidade** — deploy automatizado em nuvem Azure via IaC (Infrastructure as Code)
- **Developer Experience** — documentação interativa via Swagger UI

> **Nota sobre a IoT:** A placa **ESP32** opera de forma autônoma e isolada, sem comunicação direta com esta API. Os dados de telemetria do dispositivo são processados localmente na borda (edge computing). Esta API gerencia exclusivamente o domínio de **Produtores** e **Propriedades**.

---

## 🚀 Tecnologias e Stack

| Categoria | Tecnologia | Versão / Detalhe |
|-----------|-----------|-----------------|
| **Linguagem** | C# | 12.0 |
| **Framework** | ASP.NET Core | 8.0 (MVC com Controllers) |
| **ORM** | Entity Framework Core | Code-First Migrations |
| **Banco de Dados** | Oracle Database | `gvenzl/oracle-free:slim` |
| **Conteinerização** | Docker + Docker Compose | Multi-container |
| **Nuvem** | Microsoft Azure | Virtual Machine (Ubuntu) |
| **Documentação** | Swagger / OpenAPI | Via `Swashbuckle.AspNetCore` |
| **Autenticação** | CPF-based Login | Validação customizada |

---

## 🏗️ Arquitetura do Projeto

O backend adota o padrão **MVC (Model-View-Controller)** com separação clara de responsabilidades:

```
AstroFarm.Api/
├── Controllers/          # Camada de entrada HTTP (roteamento e validação de request)
│   ├── ProdutoresController.cs
│   └── PropriedadesController.cs
├── Models/               # Entidades de domínio (mapeadas pelo EF Core)
│   ├── Produtor.cs
│   └── Propriedade.cs
├── Data/                 # Contexto do EF Core e configurações de banco
│   └── AppDbContext.cs
├── Migrations/           # Histórico de migrações Code-First
├── DTOs/                 # Data Transfer Objects (request/response shapes)
├── Services/             # Lógica de negócio desacoplada dos Controllers
├── Program.cs            # Composition root: DI, middlewares, Swagger
├── Dockerfile
└── docker-compose.yml
```

---

## 📐 Diagrama de Arquitetura

O diagrama abaixo ilustra o fluxo de dados entre o App Mobile, a API .NET e o Oracle DB, todos orquestrados via Docker Compose e hospedados em uma VM Azure.

![diagrama-dotnet.png](diagrama-dotnet.png)

---

## 🔬 Desenvolvimento

### Padrão MVC

A API segue o padrão **Model-View-Controller** adaptado para APIs RESTful (sem camada de View, substituída por serialização JSON):

- **Controllers** recebem as requisições HTTP, delegam para Services e retornam `IActionResult` padronizados
- **Models** representam as entidades do domínio e são mapeados diretamente para tabelas Oracle pelo EF Core
- **Services** encapsulam a lógica de negócio, mantendo os Controllers enxutos e testáveis

### Injeção de Dependência (DI)

Todos os serviços e o `DbContext` são registrados no contêiner de DI nativo do ASP.NET Core em `Program.cs`:

```csharp
// Program.cs (trecho)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

builder.Services.AddScoped<IProdutorService, ProdutorService>();
builder.Services.AddScoped<IPropriedadeService, PropriedadeService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

Os Controllers recebem as dependências via **constructor injection**, sem acoplamento direto às implementações concretas — o que facilita testes unitários com mocks.

### Entity Framework Core — Code-First Migrations

O banco de dados é gerenciado inteiramente pelo **EF Core** no modelo Code-First:

1. As entidades C# (ex: `Produtor`, `Propriedade`) definem o schema
2. O EF Core gera as Migrations com `dotnet ef migrations add <NomeMigration>`
3. As Migrations são aplicadas automaticamente na inicialização da API via `DbContext.Database.Migrate()`

```csharp
// AppDbContext.cs (trecho)
public class AppDbContext : DbContext
{
    public DbSet<Produtor> Produtores { get; set; }
    public DbSet<Propriedade> Propriedades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produtor>()
            .HasIndex(p => p.Cpf)
            .IsUnique();

        modelBuilder.Entity<Propriedade>()
            .HasOne(p => p.Produtor)
            .WithMany(pr => pr.Propriedades)
            .HasForeignKey(p => p.ProdutorId);
    }
}
```

Isso garante que o schema do Oracle esteja sempre sincronizado com o código, sem SQL manual.

---

## 📡 Entidades e Endpoints

### Entidade: `Produtores`

Gerencia o ciclo de vida de autenticação dos produtores rurais. O identificador único de negócio é o **CPF**.

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| `POST` | `/api/Produtores/Cadastro` | Registra um novo produtor | ❌ Público |
| `POST` | `/api/Produtores/Login` | Autentica um produtor via CPF e senha | ❌ Público |

---

### Entidade: `Propriedades`

Gerencia as propriedades agrícolas vinculadas a um produtor. Suporta **CRUD completo**.

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| `GET` | `/api/Propriedades` | Lista todas as propriedades cadastradas | ✅ |
| `GET` | `/api/Propriedades/{id}` | Retorna uma propriedade pelo ID | ✅ |
| `POST` | `/api/Propriedades` | Cadastra uma nova propriedade | ✅ |
| `PUT` | `/api/Propriedades/{id}` | Atualiza dados de uma propriedade | ✅ |
| `DELETE` | `/api/Propriedades/{id}` | Remove uma propriedade | ✅ |

---

## 🛠️ Execução Local com Docker

### Pré-requisitos

| Ferramenta | Versão Mínima | Link |
|-----------|--------------|------|
| Docker Desktop | 24.x | [docker.com](https://www.docker.com/products/docker-desktop/) |
| Git | 2.x | [git-scm.com](https://git-scm.com/) |

---

### Passo 1 — Clonar o repositório

```bash
git clone https://github.com/Gabriel24701/astrofarm-api.git
cd astrofarm-api/AstroFarm.Api
```

---

### Passo 2 — Configurar variáveis de ambiente

Crie o arquivo `.env` na raiz do projeto (mesma pasta do `docker-compose.yml`):

```bash
touch .env
```

Adicione o seguinte conteúdo ao `.env`:

```env
# Credenciais do Oracle DB
ORACLE_ROOT_PASSWORD=SuaSenhaForte123
ORACLE_APP_USER=GS_USER
ORACLE_APP_PASSWORD=GS_PASSWORD
```

> ⚠️ **Atenção:** O arquivo `.env` está listado no `.gitignore`. Nunca versione credenciais.

---

### Passo 3 — Subir os containers

```bash
docker compose up -d --build
```

O comando irá:
- Realizar o **build** da imagem da API .NET a partir do `Dockerfile`
- Fazer o pull da imagem `gvenzl/oracle-free:slim`
- Criar e inicializar os dois containers em rede interna Docker
- Executar automaticamente as Migrations do EF Core na primeira inicialização

Para acompanhar os logs em tempo real:

```bash
docker compose logs -f astrofarm-api
```

Para verificar o status dos containers:

```bash
docker compose ps
```

---

### Passo 4 — Acessar a API

Após os containers estarem `healthy`, acesse:

| Recurso | URL |
|---------|-----|
| **Swagger UI** | http://localhost:8080/swagger |
| **API Base URL** | http://localhost:8080/api |

Para parar e remover os containers:

```bash
docker compose down
```

Para parar e remover containers + volumes (reset total do banco):

```bash
docker compose down -v
```

---

## 🧪 Testes e Payloads

Todos os endpoints podem ser testados diretamente pelo **Swagger UI** em `http://localhost:8080/swagger`. Abaixo estão os payloads de referência para as principais operações.

---

### POST `/api/Produtores/Cadastro`

Registra um novo produtor na plataforma.

**Request Body:**
```json
{
  "nome": "João da Silva",
  "cpf": "12345678900",
  "email": "joao.silva@astrofarm.com",
  "telefone": "11999999999",
  "estado": "SP",
  "cidade": "Ribeirão Preto"
}
```

**Response `201 Created`:**
```json
{
  "id": 1,
  "nome": "João da Silva",
  "cpf": "123.456.789-00",
  "email": "joao.silva@astrofarm.com",
  "dataCadastro": "2025-06-07T14:30:00Z"
}
```

---

### POST `/api/Produtores/Login`

Autentica um produtor utilizando CPF.

**Request Body:**
```json
{
  "cpf": "123.456.789-00"
}
```

**Response `200 OK`:**
```json
{
  "id": 1,
  "nome": "João da Silva",
  "email": "joao.silva@astrofarm.com",
  "autenticado": true
}
```

**Response `401 Unauthorized`:**
```json
{
  "mensagem": "CPF ou senha inválidos."
}
```

---

### POST `/api/Propriedades`

Cadastra uma nova propriedade agrícola vinculada a um produtor.

**Request Body:**
```json
{
  "nome": "Fazenda Esperança",
  "localizacao": "Ribeirão Preto, SP",
  "areaHectares": 450.75,
  "tipoCultura": "Soja",
  "produtorId": 1
}
```

**Response `201 Created`:**
```json
{
  "id": 10,
  "nome": "Fazenda Esperança",
  "localizacao": "Ribeirão Preto, SP",
  "areaHectares": 450.75,
  "tipoCultura": "Soja",
  "produtorId": 1,
  "dataCadastro": "2025-06-07T14:35:00Z"
}
```

---

### GET `/api/Propriedades`

**Response `200 OK`:**
```json
[
  {
    "id": 10,
    "nome": "Fazenda Esperança",
    "localizacao": "Ribeirão Preto, SP",
    "areaHectares": 450.75,
    "tipoCultura": "Soja",
    "produtorId": 1
  }
]
```

---

### PUT `/api/Propriedades/{id}`

Atualiza os dados de uma propriedade existente.

**Request Body:**
```json
{
  "nome": "Fazenda Esperança - Safra 2025",
  "localizacao": "Ribeirão Preto, SP",
  "areaHectares": 510.00,
  "tipoCultura": "Milho",
  "produtorId": 1
}
```

**Response `204 No Content`** *(sem body)*

---

### DELETE `/api/Propriedades/{id}`

Remove uma propriedade pelo ID.

**Response `204 No Content`** *(sem body)*

**Response `404 Not Found`:**
```json
{
  "mensagem": "Propriedade com ID 99 não encontrada."
}
```

---

## ☁️ Deploy em Produção — Azure

O provisionamento da infraestrutura em nuvem é totalmente automatizado via **IaC (Infrastructure as Code)** com um script Bash localizado em `/scripts/deploy-astrofarm.sh`.

O script executa automaticamente as seguintes etapas:

1. Criação do **Resource Group** na Azure
2. Provisionamento da **VM Linux (Ubuntu)** com tamanho adequado
3. Configuração das regras de **Firewall / Network Security Group** (liberação das portas 22, 80 e 8080)
4. Instalação e configuração do **Docker** na VM
5. Clone do repositório e execução do `docker compose up -d --build` remoto

### Como executar o script de deploy

```bash
# 1. Acessar o diretório de scripts
cd scripts/

# 2. Conceder permissão de execução ao script
chmod +x deploy-astrofarm.sh

# 3. Executar o deploy (requer Azure CLI autenticado: az login)
./deploy-astrofarm.sh
```

> **Pré-requisito:** Ter o [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) instalado e autenticado (`az login`) antes de executar o script.

Após o deploy, a API estará disponível publicamente em:

```
http://<IP_PUBLICO_DA_VM>:8080/swagger
```

---

## 👥 Equipe

| Nome | RM | GitHub                                             |
|------|----|----------------------------------------------------|
| **Gabriel Bebé Silva** | `562012` | [@Gabriel24701](https://github.com/Gabriel24701)   |
| **Emanuel Italo** | `561337` | [@Emanuel-italo](https://github.com/Emanuel-italo) |
| **Paulo Estalise** | `563811` | [@phestalise](https://github.com/phestalise)       ||

---

<p align="center">
  Desenvolvido com ☕ e C# para a <strong>Global Solution — FIAP 2025</strong>
</p>