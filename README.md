# ⬡ Sistema de controle financeiro!

<div align="center">

![Finance Control System](https://img.shields.io/badge/Finance%20Control-System-00f5ff?style=for-the-badge&labelColor=0d0d0d)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&labelColor=0d0d0d)
![xUnit](https://img.shields.io/badge/testes-27%20passed-ffd700?style=for-the-badge&labelColor=0d0d0d)

**Sistema de controle de gastos e ganhos pessoais com API REST em .NET 10 (Clean Architecture), PostgreSQL em Docker e frontend web.**

[🌐 Acessar o Sistema](https://kyon-s2.github.io/ControleFinanceiro/)

</div>

---

## 👥 Membros do Grupo

| Nome | GitHub |
|------|--------|
| Felipe Gabriel do Nascimento Rodrigues| — |
| Daniel Carlos Delfino dos Santos| — |

---

## 📌 Sobre o Projeto

O **Sistema de controle financeiro** permite controlar gastos e ganhos pessoais com autenticação segura via JWT e Google, persistência em banco relacional PostgreSQL e isolamento completo de dados por usuário.

## ✨ Funcionalidades

- 🔐 **Autenticação JWT & Google** — cadastro e login com e-mail/senha (BCrypt) ou via **Google Identity Services**
- 🛡️ **Proteção Antiabuso** — normalização de e-mail (`EmailNormalizer`), bloqueio de domínios descartáveis e *Rate Limiting* por IP
- 🏷️ **Categorias Dinâmicas** — CRUD completo de categorias por usuário
- ➕ **CRUD de Gastos** — criação, listagem com filtros (`nome`, `categoriaId`, `mes`), edição e remoção
- 💰 **CRUD de Ganhos** — criação, listagem com filtros, **edição completa**, remoção e flag de ganho fixo mensal
- 📊 **Relatório** — tabela completa com filtro por categoria e total acumulado
- 📈 **Resumo & Dashboard** — total de gastos, quantidade, maior gasto, total de ganhos e saldo em tempo real (`/api/resumo`)
- 🎯 **Limite Mensal** — definição de teto de gastos no banco (`/api/limite`) com alerta visual em 80% (`Alerta80`) e 100% (`Estourado`)
- 📚 **Swagger UI & RFC 7807** — documentação interativa com suporte a Bearer Token (`/swagger`) e respostas de erro padronizadas (`ProblemDetails`)

---

## 🛠️ Stack Tecnológica

| Camada | Tecnologia |
|--------|-----------|
| Frontend | HTML5, CSS3, JavaScript (ES6+), Google Identity Services |
| Backend | C# / .NET 10, ASP.NET Core Web API (Clean Architecture) |
| Banco de dados | PostgreSQL 16 (Docker) + Entity Framework Core (Npgsql) |
| Autenticação | JWT Bearer, BCrypt.Net-Next, Google.Apis.Auth |
| Documentação API | Swagger UI (Swashbuckle) |
| Testes | xUnit (unitários + integração com `WebApplicationFactory` e SQLite em memória) |
| CI/CD | GitHub Actions |

---

## 🚀 Como Rodar Localmente

**Pré-requisitos:** [.NET 10 SDK](https://dotnet.microsoft.com/), [Docker / Docker Compose](https://www.docker.com/) e um servidor estático para o frontend (ex.: extensão Live Server no VS Code ou `python3 -m http.server`).

```bash
# 1. Clone o repositório
git clone https://github.com/Kyon-S2/ControleFinanceiro.git
cd ControleFinanceiro

# 2. Configure as variáveis de ambiente do backend
cp backend/.env.example backend/.env
# Preencha backend/.env com sua senha do Postgres, JWT_KEY e GOOGLE_CLIENT_ID
# ⚠️ Nunca envie arquivos .env ou config.js reais para o repositório.

# 3. Suba o banco de dados PostgreSQL 16
docker compose -f backend/docker-compose.yml up -d

# 4. Execute a API .NET (as migrations rodam automaticamente em Development)
dotnet run --project backend/ControleFinanceiro.Api
```

A API iniciará em `http://localhost:5065` e a documentação interativa estará disponível em `http://localhost:5065/swagger`.

### Configurando e abrindo o Frontend

```bash
# 5. Copie o modelo de configuração do frontend
cp config.example.js config.js
# Ajuste API_URL (padrão http://localhost:5065) e GOOGLE_CLIENT_ID se desejar testar login Google

# 6. Sirva os arquivos estáticos na porta 5500 (ou abra index.html com Live Server)
python3 -m http.server 5500
```

Acesse `http://localhost:5500` no navegador.

---

## 🔌 Principais Endpoints da API

| Recurso | Rotas | Descrição |
|---------|-------|-----------|
| Autenticação | `POST /api/auth/register`, `login`, `google` | Cadastro, login com senha e login Google |
| Categorias | `GET`, `POST`, `PUT /{id}`, `DELETE /{id}` em `/api/categorias` | Gestão de categorias do usuário |
| Gastos | `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` em `/api/gastos` | CRUD de gastos com filtros |
| Ganhos | `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` em `/api/ganhos` | CRUD de ganhos com filtros |
| Limite Mensal | `GET /api/limite`, `PUT /api/limite` | Consulta e definição do teto mensal |
| Resumo | `GET /api/resumo?mes=yyyy-MM` | Totais, saldo e status do limite |

---

## 🧪 Testes

O projeto possui **27 testes automatizados (xUnit)** cobrindo o código real da aplicação:

- Normalização de e-mail (`EmailNormalizer` para Gmail e outros domínios)
- Validações de regras de negócio de `Gasto` e `Ganho` (valor positivo, nome não numérico)
- Cálculo do resumo financeiro mensal e `statusLimite` (80% e 100%)
- Testes de integração ponta a ponta com `WebApplicationFactory` e SQLite em memória (fluxo registro → login → criação de gasto e bloqueio 409 de e-mail normalizado duplicado)

```bash
dotnet test backend/ControleFinanceiro.slnx
```

A pipeline de CI roda automaticamente via **GitHub Actions** ([.github/workflows/ci.yml](.github/workflows/ci.yml)).

---

## 📁 Estrutura do Projeto

```
ControleFinanceiro/
├── .github/
│   └── workflows/
│       └── ci.yml                              # Pipeline de testes .NET (GitHub Actions)
├── backend/
│   ├── ControleFinanceiro.slnx                 # Solution .NET 10
│   ├── ControleFinanceiro.Domain/              # Entidades, regras e EmailNormalizer
│   ├── ControleFinanceiro.Application/         # Services, DTOs e interfaces
│   ├── ControleFinanceiro.Infrastructure/      # EF Core, AppDbContext, migrations, Auth
│   ├── ControleFinanceiro.Api/                 # Controllers, Swagger e Middleware RFC 7807
│   ├── ControleFinanceiro.Tests/               # 27 testes unitários e de integração (xUnit)
│   ├── docker-compose.yml                      # Container PostgreSQL 16
│   └── .env.example                            # Modelo de variáveis de ambiente
├── .gitignore
├── api.js                                      # Client HTTP (fetch + Bearer JWT)
├── app.js                                      # Lógica do frontend integrada à API
├── config.example.js                           # Modelo de configuração do frontend
├── favicon.ico                                 # Ícone do site
├── filters.css                                 # Estilos do painel de filtros
├── filters.js                                  # Lógica de filtro por categoria no relatório
├── index.html                                  # Estrutura da aplicação
├── PROGRESSO.md                                # Acompanhamento das fases da migração
└── style.css                                   # Estilização principal (tema cyberpunk)
```

---

## 🔄 Origem do Projeto

Este projeto foi originalmente desenvolvido em **Python (CLI)** durante o BootCamp II, evoluído para uma aplicação web integrada ao Supabase e, posteriormente, migrado para uma **API REST própria em .NET 10 com Clean Architecture e PostgreSQL**, mantendo a mesma identidade visual e expandindo com:

- Backend próprio em .NET 10 com camadas desacopladas (Domain, Application, Infrastructure, Api)
- Autenticação JWT, BCrypt e Login com Google
- Edição de ganhos e categorias dinâmicas por usuário
- Tratamento global de exceções (RFC 7807), Rate Limiting e Swagger UI
- 27 testes automatizados em xUnit e pipeline de CI no GitHub Actions

Agora o projeto é utilizado como projeto final na disciplina de **Backend**.

---

<div align="center">

Desenvolvido durante o **BootCamp II — 2026** e aprimorado para **Backend - 2026**
Uso de Inteligência Artificial (Claude Code, Gemini, Antigravity)

</div>
