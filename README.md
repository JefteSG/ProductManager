# Product Manager

Aplicativo full-stack de gerenciamento de produtos construído com **ASP.NET Core 8** no backend e **React + Vite + TypeScript** no frontend.

---

## Stack Tecnológica

| Camada | Tecnologia |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Banco de dados | SQLite |
| Logging | Serilog (console + arquivo rotativo) |
| Validação | FluentValidation |
| Documentação da API | Swagger / OpenAPI |
| Frontend | React 19 + Vite + TypeScript |
| Cliente HTTP | Axios |

---

## Pré-requisitos para uso local

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Node.js 18+](https://nodejs.org/)
- CLI `dotnet-ef` — instale com: `dotnet tool install --global dotnet-ef`

---

## Executando com Docker (recomendado)

> Requer apenas o [Docker](https://docs.docker.com/get-docker/) instalado. Nenhum SDK ou Node é necessário.

```bash
# A partir da raiz do repositório
docker compose up --build
```

| Serviço | URL |
|---|---|
| Frontend (React) | http://localhost:3000 |
| Backend (API) | http://localhost:5241 |
| Swagger UI | http://localhost:5241/swagger |

O banco de dados SQLite é persistido em um volume nomeado (`sqlite_data`) — os dados sobrevivem a reinicializações e ao `docker compose down`.

Para parar e remover os contêineres:

```bash
docker compose down
```

Para remover também o volume (apaga todos os dados):

```bash
docker compose down -v
```

---

## Executando o Backend

```bash
# A partir da raiz do repositório
cd /caminho/para/ProductManager

dotnet restore
dotnet run
```

A API será iniciada em **http://localhost:5241**.  
O Swagger UI estará disponível em **http://localhost:5241/swagger**.

> O banco SQLite (`products.db`) é criado automaticamente na inicialização, junto com quaisquer migrações pendentes. Não é necessário rodar `dotnet ef database update` manualmente.

---

## Executando o Frontend

```bash
cd frontend
npm install
npm run dev
```

A aplicação React estará disponível em **http://localhost:5173**.  
Todas as requisições para `/api` são redirecionadas via proxy para o backend — nenhuma configuração de CORS é necessária durante o desenvolvimento.

---

## Endpoints da API

| Método | URL | Descrição |
|---|---|---|
| GET | `/api/products?page=1&pageSize=10` | Lista paginada de produtos |
| GET | `/api/products/{id}` | Busca produto por ID |
| POST | `/api/products` | Cria um novo produto |
| PUT | `/api/products/{id}` | Atualiza um produto existente |
| DELETE | `/api/products/{id}` | Remove um produto |

### Resposta de Listagem Paginada

```json
{
  "items": [],
  "page": 1,
  "pageSize": 10,
  "totalItems": 42,
  "totalPages": 5
}
```

### Resposta de Erro

```json
{
  "message": "Já existe um produto com o SKU 'SKU-001'.",
  "statusCode": 409
}
```

---

## Regras de Negócio

1. **Estoque não-negativo** — `stockQuantity` não pode ser menor que zero.
2. **Preço mínimo para Eletrônicos** — Produtos da categoria `Eletronicos` devem ter `price >= 50`.
3. **SKU único** — O SKU deve ser único entre todos os produtos, garantido tanto na camada de aplicação quanto no banco de dados.

---

## Estrutura do Projeto

```
ProductManager/
├── Api/
│   ├── Controllers/        # Controllers HTTP enxutos — sem lógica de negócio
│   └── Middleware/         # Handler global de exceções → erros JSON padronizados
│
├── Application/
│   ├── DTOs/               # Contratos de entrada/saída (entidades de domínio nunca expostas)
│   ├── Interfaces/         # Contratos para serviços e repositórios
│   ├── Mappings/           # Extensões manuais de mapeamento entidade → DTO
│   ├── Services/           # Orquestração de negócio (todas as regras ficam aqui)
│   └── Validators/         # Validadores com FluentValidation
│
├── Domain/
│   ├── Entities/           # Entidade de domínio pura — sem atributos de EF ou API
│   ├── Exceptions/         # Exceções tipadas (DuplicateSku, BusinessRule, NotFound)
│   └── Rules/              # Constantes de categoria — fonte única de verdade para strings mágicas
│
├── Infrastructure/
│   ├── Data/               # AppDbContext + migrações do EF
│   └── Repositories/       # Implementação EF Core de IProductRepository
│
├── frontend/               # SPA React + Vite + TypeScript
│   └── src/
│       ├── components/     # ProductTable, ProductForm, Pagination, ErrorMessage
│       ├── hooks/          # useProducts — encapsula estado da listagem e paginação
│       ├── pages/          # ProductsPage (raiz de composição)
│       ├── services/       # productService — chamadas via Axios
│       ├── types/          # Interfaces TypeScript espelhando os DTOs do backend
│       └── utils/          # Configuração da instância Axios
│
├── Program.cs              # Registro de DI + pipeline de middleware
├── appsettings.json        # Configuração do Serilog e connection string
└── ProductManager.csproj
```

---

## Decisões Arquiteturais

**Projeto .NET único com namespaces por pasta** — Impõe separação em camadas sem o overhead de múltiplos assemblies, adequado para um projeto de desafio técnico.

**Repository Pattern** — Desacopla a lógica de negócio do EF Core; serviços nunca acessam o `DbContext` diretamente.

**Mapeamento manual** — Sem dependência do AutoMapper; métodos de extensão `ToResponse()` explícitos são mais legíveis e fáceis de depurar.

**FluentValidation** — Preferido sobre DataAnnotations para regras entre campos (ex.: categoria Eletrônicos + preço mínimo).

**Exceções de domínio tipadas** — Cada tipo de erro mapeia para um status HTTP específico no middleware; controllers ficam livres de tratamento de exceções.

**Proxy Vite** — Evita problemas de CORS no desenvolvimento; o navegador enxerga apenas `http://localhost:5173` como origem.

**Auto-migrate na inicialização** — `db.Database.Migrate()` em `Program.cs` garante que a aplicação funcione do zero sem passos manuais.

---

## Logs

Os logs são gravados em dois destinos:

- **Console** — formato legível durante o desenvolvimento
- **`logs/productmanager-YYYYMMDD.log`** — arquivo diário rotativo com retenção de 7 dias

Eventos registrados: criação, atualização e remoção de produtos, falhas de validação e exceções não tratadas.

---

## TODO — Melhorias Planejadas

### Backend

- [ ] **Autenticação e autorização** — Implementar JWT com ASP.NET Identity; adicionar roles (Admin, Viewer) para proteger endpoints de escrita
- [ ] **Soft delete** — Substituir remoção física por campo `DeletedAt`; manter histórico de produtos removidos e filtrar nas queries
- [ ] **Testes de integração** — Cobrir controllers com `WebApplicationFactory<Program>` + banco SQLite in-memory; testar fluxos completos HTTP → banco
- [ ] **Testes de validação** — Adicionar testes unitários para `CreateProductRequestValidator` e `UpdateProductRequestValidator`
- [ ] **Tratamento da exceção de unicidade do banco** — Capturar `DbUpdateException` com violação de unique constraint no middleware e retornar 409 em vez de 500 (race condition)
- [ ] **Trocar SQLite por PostgreSQL** — Mais robusto para produção; ajustar `docker-compose.yml` para subir um container Postgres
- [ ] **Paginação com cursor** — Substituir paginação por offset por cursor-based para melhor performance em tabelas grandes
- [ ] **Cache de listagem** — Adicionar `IMemoryCache` ou Redis para cachear a listagem paginada com invalidação na escrita

### Frontend

- [ ] **Substituir inline styles por Tailwind CSS** — Remover todos os objetos de estilo inline; aplicar classes utilitárias para manutenção e theming consistentes
- [ ] **Trocar `window.confirm` por modal de confirmação** — Implementar componente `ConfirmDialog` reutilizável para ações destrutivas
- [ ] **Desabilitar botão de delete durante a requisição** — Evitar duplo clique; adicionar estado `deletingId` no hook `useProducts`
- [ ] **Substituir hook manual por React Query** — Cache automático, refetch em foco, retry configurável e states de loading/error mais robustos
- [ ] **React Hook Form** — Substituir estado controlado manual nos formulários; ganhar validação performática e reset automático
- [ ] **Feedback otimista** — Remover o item da lista imediatamente no delete, revertendo em caso de erro
- [ ] **Testes de componente** — Adicionar Vitest + React Testing Library para testar `ProductForm`, `ProductTable` e `useProducts`
- [ ] **Testes E2E** — Implementar fluxos críticos com Playwright (criar produto, editar, deletar, validação de formulário)

### DevOps / Infraestrutura

- [ ] **Variáveis de ambiente para secrets** — Usar `.env` + Docker secrets em vez de connection string hardcoded no `docker-compose.yml`
- [ ] **Pipeline CI/CD** — GitHub Actions para rodar build, testes e lint a cada push/PR
- [ ] **Healthcheck mais completo** — Adicionar endpoint `/health` com verificação de conectividade ao banco via `IHealthCheck`