# 🛰️ AstroFarm API

> API desenvolvida em **.NET 8** para gerenciamento e monitoramento de propriedades agrícolas com integração de telemetria via satélite e IoT — projeto **Global Solution**.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker)
![Oracle DB](https://img.shields.io/badge/Oracle-DB-F80000?style=flat-square&logo=oracle)
![Azure](https://img.shields.io/badge/Azure-VM-0078D4?style=flat-square&logo=microsoftazure)
![Swagger](https://img.shields.io/badge/Docs-Swagger-85EA2D?style=flat-square&logo=swagger)

---

## 🚀 Tecnologias Utilizadas

| Camada | Tecnologia |
|--------|-----------|
| **Backend** | C# com ASP.NET Core 8 (Minimal APIs & Controllers) |
| **ORM** | Entity Framework Core |
| **Banco de Dados** | Oracle DB (`gvenzl/oracle-free:slim`) |
| **Conteinerização** | Docker + Docker Compose |
| **Nuvem** | Azure Virtual Machines (Ubuntu) |
| **Documentação** | Swagger / OpenAPI |

---

## 🏗️ Arquitetura do Projeto

O backend atua como o **núcleo central de dados** da solução AstroFarm, servindo:

- 📱 O aplicativo **Mobile (React Native)** pelas rotas de `Produtores` e `Propriedades`

> A infraestrutura é **100% conteinerizada**, garantindo paridade entre o ambiente de desenvolvimento e produção.

---

## ⚙️ Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução
- [Git](https://git-scm.com/) para clonar o repositório

---

## 🛠️ Como Executar (Ambiente Local com Docker)

### 1. Clone o repositório

```bash
git clone https://github.com/Gabriel24701/astrofarm-api.git
cd astrofarm-api/AstroFarm.Api
```

### 2. Configure as variáveis de ambiente

Crie um arquivo `.env` na mesma pasta do `docker-compose.yml` com as seguintes credenciais:

```env
ORACLE_ROOT_PASSWORD=SuaSenhaForte123
ORACLE_APP_USER=GS_USER
ORACLE_APP_PASSWORD=GS_PASSWORD
```

### 3. Suba os containers (API + Banco de Dados)

```bash
docker compose up -d --build
```

### 4. Acesse a documentação interativa

Com os containers em execução, abra no navegador:

```
http://localhost:8080/swagger
```

---

## 📡 Endpoints Principais

A documentação completa pode ser testada via Swagger. As rotas principais são:

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/api/Produtores/Cadastro` | Registra um novo produtor |
| `POST` | `/api/Produtores/Login` | Autenticação de acesso |
| `GET` | `/api/Propriedades` | Lista todas as propriedades |
| `POST` | `/api/Propriedades` | Cadastra uma nova propriedade |
| `PUT` | `/api/Propriedades/{id}` | Atualiza dados da propriedade |
| `DELETE`| `/api/Propriedades/{id}` | Remove uma propriedade |


---

## ☁️ Deploy em Produção (Azure)

O provisionamento da infraestrutura em nuvem foi **automatizado via script Bash**, localizado em:

```
/scripts/deploy-astrofarm.sh
```

O script executa automaticamente as seguintes etapas:

**Para executar o provisionamento:**
```bash
chmod +x scripts/deploy-astrofarm.sh
./scripts/deploy-astrofarm.sh

1. Cria o **Resource Group** na Azure
2. Provisiona a **VM Linux (Ubuntu)**
3. Configura as regras de **Firewall**
4. Prepara o ambiente para execução do **Docker**

---

## 👥 Equipe

| Nome               | RM     |
|--------------------|--------|
| Gabriel Bebé Silva | 562012 |
| Paulo Estalise     | 563811 |
| Emanuel Italo      | 561337 |