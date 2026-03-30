# AGENTS.md — Repository Agent Guidelines

> This file instructs AI coding agents (GitHub Copilot, Claude, Cursor, etc.) on how to work effectively in this repository. Place it at the repo root. For monorepos, add scoped versions in subdirectories.

---

## Project Overview

<!-- Brief description so the agent understands the domain -->
- **Project**: [Project name]
- **Type**: [Web app / API / CLI / Library / Monorepo / etc.]
- **Primary Language**: [e.g., TypeScript, C#, Python, Go]
- **Runtime**: [e.g., Node.js 20, .NET 9, Python 3.12]
- **Package Manager**: [e.g., npm, pnpm, yarn, pip, NuGet]

## Repository Structure

<!-- Help the agent navigate the codebase — adjust to your layout -->
```
/
├── src/                    # Application source code
│   ├── api/                # API routes / controllers
│   ├── services/           # Business logic layer
│   ├── models/             # Data models / entities
│   ├── utils/              # Shared utilities
│   └── config/             # Configuration and environment
├── tests/                  # Test files (mirrors src/ structure)
│   ├── unit/
│   ├── integration/
│   └── e2e/
├── docs/                   # Documentation
├── scripts/                # Build, deploy, and utility scripts
├── infra/                  # Infrastructure-as-code (Terraform, Bicep, etc.)
├── .github/                # CI/CD workflows, issue templates
│   ├── workflows/
│   └── instructions/       # Agent instruction files
├── package.json            # Dependencies and scripts
├── tsconfig.json           # TypeScript configuration
└── AGENTS.md               # This file
```

---

## Build, Run, and Test Commands

<!-- Essential commands the agent needs to execute tasks -->

| Task | Command |
|------|---------|
| Install dependencies | `npm install` |
| Build | `npm run build` |
| Run locally | `npm run dev` |
| Run all tests | `npm test` |
| Run unit tests only | `npm run test:unit` |
| Run integration tests | `npm run test:integration` |
| Lint | `npm run lint` |
| Format | `npm run format` |
| Type check | `npm run typecheck` |
| Database migrations | `npm run db:migrate` |
| Generate API docs | `npm run docs` |

---

## Code Style and Conventions

### Language Standards

- **TypeScript strict mode** — all files must pass `strict: true`
- Prefer `const` over `let`; never use `var`
- Use named exports over default exports
- Use arrow functions for callbacks; regular functions for top-level declarations
- Max line length: 100 characters
- Indentation: 2 spaces (enforced by Prettier)

### Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Files (components) | PascalCase | `UserProfile.tsx` |
| Files (utilities) | camelCase | `formatDate.ts` |
| Files (tests) | `*.test.ts` / `*.spec.ts` | `userService.test.ts` |
| Classes | PascalCase | `PaymentProcessor` |
| Interfaces | PascalCase, no `I` prefix | `UserRepository` |
| Functions | camelCase | `calculateTotal()` |
| Constants | UPPER_SNAKE_CASE | `MAX_RETRY_COUNT` |
| Environment vars | UPPER_SNAKE_CASE | `DATABASE_URL` |
| Database tables | snake_case, plural | `user_accounts` |
| API endpoints | kebab-case, plural | `/api/v1/user-accounts` |

### Import Order

```typescript
// 1. Node/built-in modules
import path from 'path';

// 2. External packages
import express from 'express';

// 3. Internal aliases (@/)
import { UserService } from '@/services/UserService';

// 4. Relative imports
import { validate } from './helpers';

// 5. Type-only imports
import type { Request, Response } from 'express';
```

---

## Architecture Principles

### Layered Architecture

```
Controller / Route  →  Service  →  Repository  →  Database
     (HTTP)           (Logic)      (Data Access)   (Storage)
```

- **Controllers**: Handle HTTP request/response only. No business logic.
- **Services**: Contain all business logic. Receive and return plain objects/DTOs.
- **Repositories**: Abstract database access. One per aggregate root.
- **Models**: Data shapes. Separate DB models from API DTOs.

### Key Architectural Rules

1. **Dependency direction**: Dependencies flow inward (Controller → Service → Repository). Never reverse.
2. **No circular dependencies**: If A imports B, B must not import A (directly or indirectly).
3. **Single responsibility**: Each file/class/function does one thing.
4. **Interface segregation**: Depend on abstractions. Services depend on repository interfaces, not implementations.
5. **Configuration via environment**: All config from environment variables, never hardcoded.

### Error Handling Pattern

```typescript
// Use typed application errors, not generic Error
class AppError extends Error {
    constructor(
        public readonly code: string,
        public readonly statusCode: number,
        message: string
    ) {
        super(message);
    }
}

// Throw in services
throw new AppError('USER_NOT_FOUND', 404, 'User does not exist');

// Catch in error middleware — never swallow errors silently
```

---

## Testing Guidelines

### Test Structure

- Place tests adjacent to source OR in a mirrored `tests/` directory (pick one, be consistent)
- Name: `*.test.ts` for unit, `*.integration.test.ts` for integration, `*.e2e.test.ts` for e2e
- Use `describe` / `it` blocks with descriptive names

### Test Principles

1. **Arrange-Act-Assert** pattern in every test
2. **No test interdependence** — each test runs in isolation
3. **Mock external boundaries** (HTTP, DB, file system) — never mock internal modules
4. **Test behavior, not implementation** — assert on outputs and side effects
5. **Minimum 80% coverage** for services and utilities

### Example Test

```typescript
describe('UserService', () => {
    describe('createUser', () => {
        it('should create a user with hashed password', async () => {
            // Arrange
            const mockRepo = { save: jest.fn().mockResolvedValue({ id: '1' }) };
            const service = new UserService(mockRepo);

            // Act
            const result = await service.createUser({ email: 'a@b.com', password: 'pass' });

            // Assert
            expect(result.id).toBe('1');
            expect(mockRepo.save).toHaveBeenCalledWith(
                expect.objectContaining({ email: 'a@b.com' })
            );
        });
    });
});
```

---

## API Design

### REST Conventions

| Method | Path Pattern | Purpose |
|--------|-------------|---------|
| GET | `/api/v1/resources` | List (with pagination) |
| GET | `/api/v1/resources/:id` | Get single |
| POST | `/api/v1/resources` | Create |
| PUT | `/api/v1/resources/:id` | Full update |
| PATCH | `/api/v1/resources/:id` | Partial update |
| DELETE | `/api/v1/resources/:id` | Delete |

### Response Format

```json
{
    "data": { ... },
    "meta": { "page": 1, "limit": 20, "total": 100 },
    "errors": null
}
```

### Error Response Format

```json
{
    "data": null,
    "errors": [
        { "code": "VALIDATION_ERROR", "message": "Email is required", "field": "email" }
    ]
}
```

---

## Security Rules

<!-- Critical — agent MUST follow these -->

1. **Never hardcode secrets** — use environment variables or a secret manager
2. **Validate all user input** at the API boundary — use schema validation (Zod, Joi, etc.)
3. **Parameterize all SQL queries** — never concatenate user input into queries
4. **Sanitize output** — escape HTML to prevent XSS
5. **Use HTTPS only** in production
6. **Authentication**: JWT with short expiry + refresh tokens
7. **Authorization**: Check permissions at the service layer, not just the controller
8. **Rate limiting**: Apply to all public endpoints
9. **CORS**: Whitelist specific origins, never use `*` in production
10. **Dependencies**: Run `npm audit` regularly; no packages with known critical CVEs

---

## Database Conventions

- **Migrations**: All schema changes via migration files — never modify DB manually
- **Migration naming**: `YYYYMMDD_HHMMSS_description.sql` (e.g., `20260322_143000_add_users_table.sql`)
- **Seed data**: Separate from migrations; environment-aware (dev vs prod)
- **Indexes**: Add indexes for columns used in WHERE, JOIN, ORDER BY
- **Soft deletes**: Use `deleted_at` timestamp instead of hard deletes for user-facing data

---

## Git and PR Conventions

### Commit Messages

```
type(scope): short description

- Detail 1
- Detail 2
```

Types: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `ci`, `perf`

### Branch Naming

```
feat/short-description
fix/issue-123-description
refactor/module-name
```

### Pull Request Checklist

- [ ] Tests pass locally (`npm test`)
- [ ] Lint passes (`npm run lint`)
- [ ] No `console.log` or debug code left
- [ ] New endpoints documented
- [ ] Migration included if schema changed
- [ ] Environment variables documented in `.env.example`

---

## Agent-Specific Directives

<!-- Instructions that specifically guide AI agent behavior -->

### DO

- Read existing code before modifying — understand patterns in use
- Follow existing patterns in the file/module you're editing
- Run tests after making changes to verify correctness
- Use the project's existing utilities — don't reinvent helpers
- Keep changes minimal and focused — don't refactor unrelated code
- Add tests for new logic — match existing test style

### DON'T

- Don't add new dependencies without justification
- Don't create abstractions for one-time use
- Don't add comments for self-evident code
- Don't modify configuration files unless explicitly asked
- Don't change code formatting style (Prettier handles it)
- Don't bypass TypeScript strict checks with `any` or `@ts-ignore`
- Don't catch errors silently — always log or rethrow

### When Uncertain

1. Look at 2-3 similar files in the codebase for patterns
2. Check `docs/` for architecture decisions or ADRs
3. If still unclear, ask the developer rather than guessing

---

## Environment Setup

### Required Environment Variables

```env
# .env.example — copy to .env and fill in values
NODE_ENV=development
PORT=3000
DATABASE_URL=postgresql://user:pass@localhost:5432/dbname
REDIS_URL=redis://localhost:6379
JWT_SECRET=<generate-a-random-string>
API_KEY=<your-api-key>
```

### Prerequisites

- Node.js >= 20
- PostgreSQL >= 15 (or Docker)
- Redis >= 7 (or Docker)

### First-Time Setup

```bash
git clone <repo-url>
cd <repo-name>
cp .env.example .env        # Fill in values
npm install
npm run db:migrate
npm run dev                  # http://localhost:3000
```

---

## Monorepo Extensions

<!-- For monorepos: add scoped AGENTS.md files in subdirectories -->
<!-- Example: /frontend/AGENTS.md overrides root for frontend-specific rules -->

<!--
/frontend/AGENTS.md might contain:
- React component conventions
- State management patterns
- CSS/styling approach

/backend/AGENTS.md might contain:
- API versioning strategy
- Database migration process
- Service communication patterns

/shared/AGENTS.md might contain:
- Shared type definitions
- Utility function conventions
- Package publishing rules
-->

---

## Additional References

<!-- Link to detailed docs rather than duplicating content here -->
- Architecture decisions: `docs/adr/`
- API documentation: `docs/api/`
- Deployment guide: `docs/deployment.md`
- Contributing guide: `CONTRIBUTING.md`
