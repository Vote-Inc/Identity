# Identity API

ASP.NET Core 10 service responsible for authentication. Authenticates users via AWS Cognito, validates access tokens on behalf of nginx's `auth_request` module, and tracks users in DynamoDB.

## Project layout

```
Identity/
├── Identity.API/           # Controllers, Program.cs, Dockerfle
├── Identity.Application/   # Command/query handlers (CQRS)
├── Identity.Domain/        # Entities, value objects, service interfaces
└── Identity.Infrastructure/ # Cognito client, DynamoDB repository
```

## Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `POST` | `/api/auth/login` | None | Authenticate with email + password. Returns `{ token, expiresIn }`. |
| `POST` | `/api/auth/logout` | Bearer token | Revokes the Cognito access token globally. |
| `GET` | `/api/auth/validate` | Bearer token | Called by nginx `auth_request`. Returns `X-Voter-Id` and `X-Voter-Role` response headers on success. |
| `GET` | `/health` | None | Health check. |

### Login response

```json
{ "token": "<cognito-access-token>", "expiresIn": 3600 }
```

### Validate response headers

```
X-Voter-Id:   <cognito-sub>
X-Voter-Role: <cognito-group-name>   (omitted if user has no group)
```

## Configuration

| Key | Description |
|-----|-------------|
| `Cognito__UserPoolId` | Cognito user pool ID |
| `Cognito__ClientId` | App client ID (no secret) |
| `Cognito__Region` | AWS region |
| `DynamoDB__TableName` | Users table name (`identity-users`) |
| `DynamoDB__ServiceUrl` | Local DynamoDB URL — omit in production |
| `DynamoDB__Region` | DynamoDB region |
| `Frontend__Url` | Allowed CORS origin |

In production these are injected as ECS environment variables. Locally, set them in `appsettings.Development.json`.

## Local development

```bash
cd Identity
docker compose up -d        # DynamoDB Local on :8000
dotnet run --project Identity.API
```

`appsettings.Development.json` is pre-configured to point at `http://localhost:8000`. You still need a real Cognito user pool — set `Cognito__UserPoolId`, `Cognito__ClientId`, and `Cognito__Region` in `appsettings.Development.json` or as environment variables.

Swagger UI: `http://localhost:8081/swagger`

## Auth flow

1. Login returns a Cognito **access token** (not an ID token).
2. The token is stored as an `httpOnly` cookie by the frontend.
3. On each request nginx reads the cookie, calls `GET /api/auth/validate` with `Authorization: Bearer <token>`.
4. Identity calls Cognito `GetUser` to validate the token — no local JWT verification.
5. On success, identity returns `X-Voter-Id` and `X-Voter-Role`; nginx injects these as headers to the proxied service.

## DynamoDB table

**`identity-users`** — created by the identity Terraform stack.

| Attribute | Type | Notes |
|-----------|------|-------|
| `pk` | String (hash key) | User email |
| `lastLoginAt` | String | ISO-8601 timestamp |

## Docker

```bash
docker build -t identity .
docker run -p 8081:8081 \
  -e ASPNETCORE_URLS=http://+:8081 \
  -e Cognito__UserPoolId=... \
  -e Cognito__ClientId=... \
  -e Cognito__Region=us-east-1 \
  -e DynamoDB__TableName=identity-users \
  -e DynamoDB__Region=us-east-1 \
  identity
```
