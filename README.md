# xablauBack

Backend em ASP.NET Core Web API para um clone da KaBuM.

## Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT Bearer Authentication
- BCrypt para hash de senha
- Swagger / OpenAPI

## Funcionalidades implementadas

- Login com JWT
- Identificacao do usuario autenticado
- Seed automatico de admin
- CRUD de usuarios para admin
- CRUD de produtos para admin
- Catalogo publico de produtos com `Stock > 0`
- Favoritos por usuario autenticado
- Carrinho por usuario autenticado
- Checkout com criacao de pedido
- Atualizacao de estoque no checkout
- Bloqueio de checkout com carrinho vazio
- Bloqueio de checkout com estoque insuficiente

## Requisitos

- .NET SDK 10
- PostgreSQL rodando localmente
- Banco `xablau_db`

## Configuracao

Arquivo [appsettings.json](/Users/otaviodasilvamachado/Desktop/xablau/appsettings.json):

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpiresInHours`
- `Cors:AllowedOrigins`

## Como rodar

1. Restaurar pacotes:

```bash
dotnet restore
```

2. Aplicar migrations:

```bash
dotnet ef database update
```

3. Rodar a API:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

4. Abrir Swagger:

```text
http://localhost:5000/swagger
```

## Credenciais do admin seed

- Email: `admin@xablau.com`
- Senha: `123456`

## Principais rotas

### Auth

- `POST /api/auth/login`
- `GET /api/auth/me`

### Users

- `GET /api/users`
- `GET /api/users/{id}`
- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

### Products

- `GET /api/products`
- `GET /api/products/admin`
- `GET /api/products/{id}`
- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`

### Favorites

- `GET /api/favorites`
- `POST /api/favorites/{productId}`
- `DELETE /api/favorites/{productId}`

### Cart

- `GET /api/cart`
- `POST /api/cart/items`
- `PUT /api/cart/items/{itemId}`
- `DELETE /api/cart/items/{itemId}`

### Orders

- `POST /api/orders/checkout`
- `GET /api/orders`
- `GET /api/orders/{id}`

## Regras de negocio importantes

- Apenas `Admin` acessa rotas administrativas
- Email de usuario nao pode duplicar
- Favorito nao pode duplicar para o mesmo usuario e produto
- Cada usuario possui um unico carrinho
- Quantidade do carrinho nao pode ultrapassar o estoque
- Produto com `Stock = 0` sai da listagem publica
- Checkout cria pedido, reduz estoque e limpa o carrinho

## Status

Backend pronto para integracao com frontend local em `localhost:5173` ou `localhost:3000`.
