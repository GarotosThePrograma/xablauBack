# Relatorio de Requisitos do Frontend React para o Backend xablau

## Objetivo

Este documento descreve tudo o que o frontend em React precisa atender para consumir o backend atual do projeto `xablau`, com base na leitura do codigo e em validacoes reais da API local.

Escopo analisado:

- Autenticacao
- Produtos
- Favoritos
- Usuarios
- Healthcheck
- Regras de autorizacao, validacao e tratamento de erro

## Resumo executivo

O frontend precisa suportar quatro areas principais:

1. Area publica de catalogo de produtos.
2. Area autenticada para sessao do usuario e favoritos.
3. Area administrativa para gestao de produtos.
4. Area administrativa para gestao de usuarios.

Hoje existem alguns pontos importantes no backend que impactam diretamente o React:

- A autenticacao e feita com JWT via header `Authorization: Bearer <token>`.
- O endpoint `GET /api/auth/me` retorna o campo `token` vazio.
- O backend nao possui configuracao de CORS; um frontend rodando em outra origem falhara no preflight.
- O backend mistura erros em `problem+json`, texto puro e respostas vazias.
- Nao existe upload de arquivo; imagens sao enviadas apenas como URL.
- Nao existem refresh token, logout de servidor, paginacao, busca ou ordenacao.

## Base da API

- Base URL local usada na validacao: `http://127.0.0.1:5167`
- Prefixo padrao: `/api`
- Endpoints mapeados:
  - `GET /api/health`
  - `POST /api/auth/login`
  - `GET /api/auth/me`
  - `GET /api/products`
  - `GET /api/products/:id`
  - `POST /api/products`
  - `PUT /api/products/:id`
  - `DELETE /api/products/:id`
  - `GET /api/favorites`
  - `POST /api/favorites/:productId`
  - `DELETE /api/favorites/:productId`
  - `GET /api/users`
  - `GET /api/users/:id`
  - `POST /api/users`
  - `PUT /api/users/:id`
  - `DELETE /api/users/:id`

## Autenticacao e sessao

### Como funciona

- O login acontece em `POST /api/auth/login`.
- O backend devolve um JWT e os dados principais do usuario.
- O token deve ser persistido no frontend.
- Toda rota protegida exige o header:

```http
Authorization: Bearer <token>
```

### Regras de acesso por perfil

- `Admin`: pode gerenciar produtos e usuarios.
- `User`: pode autenticar e usar favoritos, mas nao pode usar endpoints administrativos.

### Fluxo recomendado no React

1. Usuario envia email e senha.
2. Front salva `token`, `id`, `name`, `email`, `role`.
3. Em recarga de pagina, se houver token salvo, chamar `GET /api/auth/me`.
4. O frontend nao deve substituir o token salvo pelo valor retornado em `/me`, porque esse endpoint devolve `token: ""`.
5. Ao receber `401`, limpar sessao e redirecionar para login.
6. Ao receber `403`, exibir mensagem de acesso negado.

### Seed util para desenvolvimento

Existe um admin criado automaticamente pelo backend:

- Email: `admin@xablau.com`
- Senha: `123456`

## Tipos sugeridos para o frontend

```ts
export type UserRole = 'Admin' | 'User';

export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  token: string;
  id: number;
  name: string;
  email: string;
  role: UserRole;
};

export type AuthMeResponse = {
  token: string;
  id: number;
  name: string;
  email: string;
  role: UserRole;
};

export type Product = {
  id: string;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  stock: number;
  createdAt: string;
};

export type ProductInput = {
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  stock: number;
};

export type Favorite = {
  id: number;
  productId: string;
  productName: string;
  description: string;
  price: number;
  imageUrl: string;
  stock: number;
  createdAt: string;
};

export type User = {
  id: number;
  name: string;
  email: string;
  role: UserRole;
  createdAt: string;
  updatedAt: string;
};

export type CreateUserInput = {
  name: string;
  email: string;
  password: string;
  role: UserRole;
};

export type UpdateUserInput = {
  name: string;
  email: string;
  password?: string;
  role: UserRole;
};

export type ValidationProblemDetails = {
  type: string;
  title: string;
  status: number;
  errors: Record<string, string[]>;
  traceId: string;
};
```

## Requisitos por modulo

## 1. Healthcheck

### Endpoint

- `GET /api/health`

### Uso no frontend

- Opcional.
- Pode ser usado em pagina interna de monitoramento, bootstrap de ambiente ou verificacao de conectividade.

### Resposta de sucesso

```json
{
  "message": "API rodando",
  "project": "xablau"
}
```

## 2. Autenticacao

### 2.1 Login

- Endpoint: `POST /api/auth/login`
- Autenticacao: publica

### Request

```json
{
  "email": "admin@xablau.com",
  "password": "123456"
}
```

### Response 200

```json
{
  "token": "jwt",
  "id": 3,
  "name": "Administrador",
  "email": "admin@xablau.com",
  "role": "Admin"
}
```

### Response 400

Formato `application/problem+json` quando faltam campos ou o email e invalido.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "The Email field is required.",
      "The Email field is not a valid e-mail address."
    ],
    "Password": [
      "The Password field is required."
    ]
  },
  "traceId": "..."
}
```

### Response 401

Texto puro:

```text
Email ou senha invalidos.
```

### Necessidades do frontend

- Tela de login.
- Validacao local de email obrigatorio e formato de email.
- Validacao local de senha obrigatoria.
- Exibicao de erro de validacao por campo.
- Exibicao de erro de autenticacao geral.
- Persistencia segura do token.

### 2.2 Sessao atual

- Endpoint: `GET /api/auth/me`
- Autenticacao: obrigatoria

### Response 200

```json
{
  "token": "",
  "id": 3,
  "name": "Administrador",
  "email": "admin@xablau.com",
  "role": "Admin"
}
```

### Response 401

- Corpo vazio.

### Response 404

Texto puro:

```text
Usuario nao encontrado.
```

### Necessidades do frontend

- Restaurar sessao com esse endpoint.
- Nao substituir o token salvo pelo `token` vazio vindo da resposta.
- Atualizar estado global do usuario.

## 3. Produtos

### 3.1 Listagem publica

- Endpoint: `GET /api/products`
- Autenticacao: publica

### Response 200

Retorna `Product[]`.

### Necessidades do frontend

- Pagina de listagem de produtos.
- Estado de carregamento, erro e lista vazia.
- Cards com imagem, nome, descricao, preco e estoque.
- Como nao existe paginacao, o frontend recebe toda a lista de uma vez.

### 3.2 Detalhe do produto

- Endpoint: `GET /api/products/:id`
- Autenticacao: publica

### Response 200

Retorna um `Product`.

### Response 404

```text
Produto nao encontrado.
```

### Necessidades do frontend

- Pagina de detalhe.
- Tratamento de ID invalido ou nao encontrado.

### 3.3 Criacao de produto

- Endpoint: `POST /api/products`
- Autenticacao: `Admin`

### Request

```json
{
  "name": "Produto Exemplo",
  "description": "Descricao do produto",
  "price": 199.9,
  "imageUrl": "https://exemplo.com/imagem.png",
  "stock": 10
}
```

### Response 201

Retorna o produto criado e header `Location`.

### Response 400

Formato `ValidationProblemDetails`.

Exemplo real:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "The Name field is required."
    ],
    "Price": [
      "The field Price must be between 0,01 and 1,7976931348623157E+308."
    ],
    "Stock": [
      "The field Stock must be between 0 and 2147483647."
    ],
    "ImageUrl": [
      "The ImageUrl field is required."
    ],
    "Description": [
      "The Description field is required."
    ]
  },
  "traceId": "..."
}
```

### Response 401

- Corpo vazio quando nao autenticado.

### Response 403

- Corpo vazio quando autenticado sem perfil `Admin`.

### Necessidades do frontend

- Formulario administrativo.
- Validacoes locais:
  - `name` obrigatorio
  - `description` obrigatorio
  - `imageUrl` obrigatorio
  - `price > 0`
  - `stock >= 0`
- Acao visivel apenas para admin.

### 3.4 Edicao de produto

- Endpoint: `PUT /api/products/:id`
- Autenticacao: `Admin`

### Observacao importante

O update e completo, nao parcial. O frontend deve enviar todos os campos do produto.

### Response 200

Retorna o produto atualizado.

### Response 404

```text
Produto nao encontrado.
```

### Necessidades do frontend

- Tela ou modal de edicao.
- Prefill do formulario com o produto atual.
- Envio completo do objeto.

### 3.5 Exclusao de produto

- Endpoint: `DELETE /api/products/:id`
- Autenticacao: `Admin`

### Response 204

- Sem corpo.

### Response 404

```text
Produto nao encontrado.
```

### Necessidades do frontend

- Confirmacao antes de excluir.
- Atualizacao otimista ou refetch da lista.

## 4. Favoritos

Todos os endpoints de favoritos exigem usuario autenticado.

### 4.1 Listar favoritos

- Endpoint: `GET /api/favorites`
- Autenticacao: qualquer usuario autenticado

### Response 200

Retorna `Favorite[]`.

Exemplo real de lista vazia:

```json
[]
```

### Necessidades do frontend

- Pagina ou drawer de favoritos.
- Badge de quantidade opcional.
- Estado vazio para usuario sem favoritos.

### 4.2 Adicionar favorito

- Endpoint: `POST /api/favorites/:productId`
- Autenticacao: qualquer usuario autenticado

### Response 201

```json
{
  "id": 2,
  "productId": "0f665ba4-a551-4af1-9e8d-9c4cf12cd45c",
  "productName": "Mouse Gamer",
  "description": "Mouse com RGB",
  "price": 199.9,
  "imageUrl": "https://meusite.com/mouse.png",
  "stock": 10,
  "createdAt": "2026-04-13T12:35:08.490102Z"
}
```

### Response 404

```text
Produto nao encontrado.
```

### Response 409

```text
Esse produto ja esta nos favoritos.
```

### Necessidades do frontend

- Botao de favoritar por produto.
- Bloqueio visual para evitar clique duplo.
- Tratamento especial para `409`, exibindo algo como "ja esta favoritado".

### 4.3 Remover favorito

- Endpoint: `DELETE /api/favorites/:productId`
- Autenticacao: qualquer usuario autenticado

### Response 204

- Sem corpo.

### Response 404

```text
Favorito nao encontrado.
```

### Necessidades do frontend

- Toggle de favorito.
- Refetch ou atualizacao local da lista.

## 5. Usuarios

Todos os endpoints de usuarios exigem `Admin`.

### 5.1 Listar usuarios

- Endpoint: `GET /api/users`
- Autenticacao: `Admin`

### Response 200

Retorna `User[]`.

### Necessidades do frontend

- Tela administrativa de usuarios.
- Tabela com nome, email, role, datas e acoes.

### 5.2 Buscar usuario por ID

- Endpoint: `GET /api/users/:id`
- Autenticacao: `Admin`

### Response 404

```text
Usuario nao encontrado.
```

### Necessidades do frontend

- Pode ser usado em detalhe ou na carga inicial da tela de edicao.

### 5.3 Criar usuario

- Endpoint: `POST /api/users`
- Autenticacao: `Admin`

### Request

```json
{
  "name": "Frontend Teste",
  "email": "frontend.teste@example.com",
  "password": "123456",
  "role": "User"
}
```

### Response 201

Retorna `User`.

### Response 400

Pode retornar `ValidationProblemDetails` para campos invalidos.

Exemplo real:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "The Name field is required."
    ],
    "Email": [
      "The Email field is not a valid e-mail address."
    ],
    "Password": [
      "The field Password must be a string or array type with a minimum length of '6'."
    ]
  },
  "traceId": "..."
}
```

### Response 409

```text
Ja existe um usuario com esse email.
```

### Response 400 adicional

```text
Role invalida. Use Admin ou User.
```

### Necessidades do frontend

- Formulario admin para criar usuario.
- Validacoes locais:
  - `name` obrigatorio
  - `email` obrigatorio e valido
  - `password` obrigatoria com minimo de 6 caracteres
  - `role` obrigatoria e limitada a `Admin` ou `User`

### 5.4 Atualizar usuario

- Endpoint: `PUT /api/users/:id`
- Autenticacao: `Admin`

### Request

```json
{
  "name": "Nome Atualizado",
  "email": "email@example.com",
  "password": "nova-senha-opcional",
  "role": "Admin"
}
```

### Regras importantes

- `password` e opcional.
- Se enviada vazia ou nula, a senha atual permanece.
- O backend impede remover o papel do ultimo admin.
- O backend impede duplicar email de outro usuario.

### Responses relevantes

- `200`: usuario atualizado.
- `400`: role invalida.
- `400`: nao e possivel remover o papel do ultimo administrador.
- `404`: usuario nao encontrado.
- `409`: ja existe outro usuario com esse email.

### Necessidades do frontend

- Formulario de edicao com senha opcional.
- Mensagem clara para a regra do ultimo admin.

### 5.5 Excluir usuario

- Endpoint: `DELETE /api/users/:id`
- Autenticacao: `Admin`

### Regras importantes

- Nao pode excluir o proprio usuario autenticado.
- Nao pode excluir o ultimo administrador.

### Responses relevantes

- `204`: excluido com sucesso.
- `400`: nao e possivel remover o proprio usuario autenticado.
- `400`: nao e possivel remover o ultimo administrador.
- `404`: usuario nao encontrado.

### Necessidades do frontend

- Desabilitar exclusao do proprio usuario, se o frontend souber quem esta logado.
- Mostrar mensagem especifica para a regra do ultimo admin.

## Validacoes que o frontend deve replicar

### Login

- `email`: obrigatorio, formato valido
- `password`: obrigatoria

### Produto

- `name`: obrigatorio
- `description`: obrigatorio
- `imageUrl`: obrigatorio
- `price`: maior que `0`
- `stock`: maior ou igual a `0`

### Usuario

- `name`: obrigatorio
- `email`: obrigatorio e valido
- `password` na criacao: obrigatoria, minimo `6`
- `password` na edicao: opcional
- `role`: apenas `Admin` ou `User`

## Formatos de erro que o React precisa tratar

O frontend precisa de um normalizador de erro, porque a API nao usa um unico padrao.

### 1. ValidationProblemDetails

- Content-Type: `application/problem+json`
- Usado em erros automativos de validacao de DTO

### 2. Texto puro

Exemplos:

- `Email ou senha invalidos.`
- `Produto nao encontrado.`
- `Favorito nao encontrado.`
- `Esse produto ja esta nos favoritos.`
- `Ja existe um usuario com esse email.`
- `Role invalida. Use Admin ou User.`

### 3. Corpo vazio

Exemplos:

- `401 Unauthorized`
- `403 Forbidden`
- `204 No Content`

### Estrategia recomendada

- Se houver `errors`, mostrar erros por campo.
- Se houver corpo texto, mostrar toast ou alerta geral.
- Se nao houver corpo:
  - `401`: expirar sessao
  - `403`: exibir acesso negado
  - `204`: sucesso silencioso

## Necessidades de interface no React

## Estrutura minima recomendada

- Pagina de login
- Rota protegida para area autenticada
- Guarda por role para area admin
- Pagina de listagem publica de produtos
- Pagina de detalhe de produto
- Tela admin de produtos
- Tela admin de usuarios
- Tela de favoritos do usuario

## Estado global recomendado

- `auth.user`
- `auth.token`
- `auth.role`
- `auth.isAuthenticated`
- `favorites.items`
- `favorites.loading`

## Servicos recomendados

- `authService`
- `productsService`
- `favoritesService`
- `usersService`
- `apiClient`

## Comportamentos recomendados

- Interceptor para enviar JWT automaticamente.
- Interceptor para capturar `401`.
- Formatacao de preco no frontend.
- Formatacao de datas ISO UTC para horario local.
- Controles visuais diferentes para `Admin` e `User`.

## Bloqueios e lacunas atuais do backend

### 1. CORS nao configurado

Este e o principal bloqueio para um frontend React separado.

Teste real de preflight:

- Requisicao `OPTIONS /api/products`
- Origem: `http://localhost:5173`
- Resultado: `405 Method Not Allowed`

Consequencia:

- O navegador bloqueara chamadas do frontend se ele rodar em outra origem.

O backend precisa adicionar algo equivalente a:

- `builder.Services.AddCors(...)`
- `app.UseCors(...)`

### 2. `GET /api/auth/me` devolve `token` vazio

Consequencia:

- O frontend precisa manter o token previamente salvo e ignorar o `token` vindo desse endpoint.

### 3. Nao existe endpoint de cadastro publico

Consequencia:

- Usuarios comuns nao conseguem se registrar sem um admin.

### 4. Nao existe refresh token

Consequencia:

- Ao expirar a sessao, o frontend precisa redirecionar para login.

### 5. Nao existe logout no backend

Consequencia:

- Logout sera apenas local, removendo token e estado da sessao.

### 6. Nao existe upload de imagem

Consequencia:

- O frontend precisa trabalhar com URL de imagem pronta.

### 7. Nao existem busca, filtros, paginacao e ordenacao

Consequencia:

- Qualquer filtro inicial sera client-side, ou o backend precisara evoluir.

## Checklist para o frontend

- Implementar `apiClient` com `baseURL` por ambiente.
- Persistir JWT.
- Restaurar sessao com `/api/auth/me`.
- Ignorar `token` vazio retornado por `/api/auth/me`.
- Proteger rotas autenticadas.
- Proteger rotas admin.
- Implementar modulo de produtos publicos.
- Implementar CRUD admin de produtos.
- Implementar modulo de favoritos.
- Implementar CRUD admin de usuarios.
- Normalizar tres formatos de erro.
- Validar formularios no cliente.
- Exibir estados de loading, vazio e erro.
- Preparar o frontend para `401`, `403`, `404`, `409` e `400`.
- Alinhar com o backend a liberacao de CORS antes da integracao em navegador.

## Conclusao

O backend ja oferece base suficiente para um frontend React com autenticacao, catalogo publico, favoritos e administracao. O principal impedimento tecnico imediato para integracao no navegador e a ausencia de CORS. Depois disso, o frontend precisa tratar com cuidado a sessao JWT, as diferencas de permissao por `role` e os formatos variados de erro.
