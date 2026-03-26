# LedgerStock — Gestão de Estoque

Sistema de gestão de produtos e controle de estoque com autenticação, controle de acesso por perfil e rastreabilidade de movimentações.

---

# Objetivo

Simular um sistema corporativo de controle de estoque, com foco em:

* organização de dados
* segurança de acesso
* rastreabilidade de operações

---

# Regras de Negócio

## Produtos

* cadastro com SKU único
* estoque calculado automaticamente
* status baseado no estoque mínimo (Normal, Baixo, Crítico)

## Movimentações

* Entrada → aumenta estoque
* Saída → diminui estoque
* não permite estoque negativo
* registra usuário responsável e data

## Controle de Acesso

* **Master** → acesso total + gestão de usuários
* **Admin** → gerencia produtos e movimentações
* **Standard** → somente visualização

---

# Arquitetura

Aplicação estruturada com **Clean Architecture**:

```bash
backend/
 ├── Domain
 ├── Application
 ├── Infrastructure
 └── Api
```

---

# Tecnologias

## Back-end

* .NET 8
* ASP.NET Core
* Entity Framework Core
* SQL Server
* Identity + JWT

## Front-end

* Vue 3 + TypeScript
* Composition API
* Vue Router
* Axios

---

# Segurança

* autenticação com JWT
* controle de acesso por roles (RBAC)
* validações no back-end
* proteção de rotas no front

---

# Como rodar o projeto

## Backend

```bash
cd backend
dotnet restore
dotnet ef database update \
--project src/LedgerStock.Infrastructure \
--startup-project src/LedgerStock.Api
dotnet run --project src/LedgerStock.Api
```

Swagger:

```
https://localhost:5199/swagger
```

---

## Frontend

```bash
cd frontend/ledgerstock-web
npm install
npm run dev
```

```
http://localhost:5173
```

---

# Usuário inicial

```
Email: master@ledgerstock.com
Senha: Master123!
```

---

# 📌 Diferenciais

* controle de acesso por perfil (Master/Admin/Standard)
* arquitetura organizada
* rastreabilidade de movimentações
* integração completa front + back
* paginação e UX consistente

---

