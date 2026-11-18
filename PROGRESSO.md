# PROGRESSO — ControleFinanceiro Backend

## ✅ Fase 1 — Estrutura do backend .NET (concluída)
- Solution Clean Architecture: Domain → Application → Infrastructure → Api
- Entidades: Usuario, Gasto, Ganho (nomes PT, campos mapeados do frontend)
- Repositories + Services + DTOs com interfaces na Application
- Controllers: AuthController (login/registro), GastosController (CRUD), GanhosController (CR+D)
- JWT auth (BCrypt hash), CORS, Swagger UI, EF Core + Npgsql
- Migration inicial gerada. docker-compose.yml com PostgreSQL 17
- .env.example com placeholders; .gitignore exclui .env, bin, obj
- **Build OK — 0 erros, 0 avisos**

## ⬜ Fase 2 — Adaptar frontend (app.js) para consumir a API .NET
## ⬜ Fase 3 — Google Auth (login social)
## ⬜ Fase 4 — Testes (xUnit)

