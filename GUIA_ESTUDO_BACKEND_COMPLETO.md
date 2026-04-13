# Guia Completo de Estudo do Backend xablau

## 1. Objetivo deste documento

Este documento foi escrito para voce estudar, revisar e conseguir explicar o backend inteiro do projeto `xablau` com seguranca.

A ideia aqui nao e so decorar rotas. A ideia e entender:

- qual foi o objetivo do projeto
- como a API foi estruturada
- por que cada arquivo existe
- como os dados se relacionam
- como autenticacao e autorizacao funcionam
- como carrinho, favoritos e checkout funcionam
- como o estoque e controlado
- quais regras de negocio foram aplicadas
- como defender tecnicamente as decisoes tomadas

Se voce dominar este guia, voce consegue explicar o backend com propriedade.

---

## 2. Visao geral do projeto

O projeto `xablau` e um backend em C# com ASP.NET Core Web API para um clone da KaBuM.

Ele atende os seguintes requisitos principais:

- autenticacao com login
- distincao entre `Admin` e `User`
- CRUD de usuarios por admin
- CRUD de produtos por admin
- catalogo publico de produtos disponiveis
- favoritos por usuario
- carrinho por usuario
- checkout com criacao de pedido
- reducao de estoque ao finalizar compra
- bloqueio de compra sem estoque suficiente

Em termos de arquitetura, o projeto segue um estilo simples e direto:

- controllers para receber e responder HTTP
- entities para modelar o banco
- DTOs para definir o formato de entrada e saida da API
- `AppDbContext` para mapear as tabelas no Entity Framework Core
- migrations para versionar a estrutura do banco

Nao foi usada uma arquitetura exageradamente complexa com services, repositories e camadas extras, porque o desafio pedia organizacao e consistencia de regras de negocio, nao sofisticacao arquitetural.

---

## 3. Stack utilizada

### 3.1 ASP.NET Core Web API

Foi usado para construir a API HTTP.

Responsabilidades:

- mapear rotas
- receber requests
- serializar JSON
- aplicar autenticacao e autorizacao
- integrar com Swagger

### 3.2 Entity Framework Core

Foi usado como ORM.

Responsabilidades:

- mapear classes C# para tabelas do banco
- consultar, inserir, atualizar e remover dados
- gerar migrations
- controlar relacionamentos e indices

### 3.3 PostgreSQL

Banco relacional principal do projeto.

Foi escolhido porque funciona muito bem com:

- usuarios
- produtos
- carrinho
- favoritos
- pedidos
- integridade relacional

### 3.4 JWT Bearer Authentication

Usado para autenticar usuarios sem sessao de servidor.

Fluxo:

1. usuario envia email e senha
2. backend valida as credenciais
3. backend gera um token JWT
4. frontend envia esse token no header `Authorization: Bearer <token>`
5. backend identifica o usuario pelo token

### 3.5 BCrypt

Usado para hash de senha.

Importancia:

- senha nunca vai em texto puro para o banco
- mesmo que o banco vaze, a senha original nao esta armazenada

### 3.6 Swagger / OpenAPI

Usado para documentar e testar os endpoints.

Importancia:

- facilita a validacao manual
- ajuda no desenvolvimento do frontend
- permite demonstrar o projeto com rapidez

---

## 4. Estrutura do projeto

### 4.1 `Program.cs`

Arquivo de bootstrap da aplicacao.

Ele:

- cria o `builder`
- registra controllers
- registra Swagger
- registra CORS
- registra `AppDbContext`
- configura JWT
- configura autorizacao
- cria seed do admin
- monta o pipeline HTTP

### 4.2 `Data/AppDbContext.cs`

Classe principal do Entity Framework.

Ela:

- expone os `DbSet`s
- define indices
- define conversoes
- define relacionamentos

### 4.3 `Entities`

Modelam o dominio e o banco.

Arquivos:

- `User.cs`
- `Product.cs`
- `Favorite.cs`
- `Cart.cs`
- `CartItem.cs`
- `Order.cs`
- `OrderItem.cs`

### 4.4 `controllers`

Camada HTTP da API.

Arquivos:

- `HealthController.cs`
- `AuthController.cs`
- `UsersController.cs`
- `ProductsController.cs`
- `FavoritesController.cs`
- `CartController.cs`
- `OrdersController.cs`

### 4.5 `DTOs`

DTO significa Data Transfer Object.

Eles existem para:

- controlar o formato do JSON de entrada
- controlar o formato do JSON de saida
- evitar expor a entidade inteira diretamente
- aplicar validacoes com annotations

### 4.6 `Migrations`

Pasta com o historico da estrutura do banco.

Cada migration representa uma mudanca de schema.

---

## 5. Boot da aplicacao em `Program.cs`

Esse arquivo e o coracao da inicializacao.

### 5.1 Registro de controllers

`builder.Services.AddControllers();`

Isso diz ao ASP.NET Core para procurar controllers e expor rotas HTTP.

### 5.2 Swagger

O projeto registra:

- documentacao `v1`
- esquema `Bearer`
- requisito global de seguranca

Isso permite clicar em `Authorize` no Swagger e testar endpoints protegidos com token.

### 5.3 CORS

Foi configurado CORS para o frontend local.

Origens permitidas:

- `http://localhost:5173`
- `http://127.0.0.1:5173`
- `http://localhost:3000`
- `http://127.0.0.1:3000`

Importancia:

- sem CORS, um frontend React em outra origem falha na requisicao
- com CORS, o navegador aceita a comunicacao entre frontend e backend

### 5.4 `AddDbContext`

Registra o `AppDbContext` com PostgreSQL.

O `DbContext` passa a ser injetado automaticamente nos controllers.

### 5.5 JWT

O backend le:

- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpiresInHours`

Depois configura o `JwtBearer`.

Validacoes feitas:

- issuer
- audience
- lifetime
- assinatura

### 5.6 Autorizacao

`builder.Services.AddAuthorization();`

Isso ativa o uso de atributos como:

- `[Authorize]`
- `[Authorize(Roles = "Admin")]`

### 5.7 Seed do admin

Na inicializacao, o backend:

- cria um escopo
- pega o `AppDbContext`
- procura se existe usuario com email `admin@xablau.com`
- se nao existir, cria o admin inicial

Credenciais seed:

- email: `admin@xablau.com`
- senha: `123456`

### 5.8 Pipeline HTTP

Ordem relevante:

1. `UseHttpsRedirection`
2. `UseCors`
3. `UseAuthentication`
4. `UseAuthorization`
5. `MapControllers`

Essa ordem importa.

Especialmente:

- autenticacao precisa acontecer antes da autorizacao
- CORS precisa estar no pipeline antes das chamadas finais de endpoint

---

## 6. Configuracao em `appsettings.json`

### 6.1 Connection string

Aponta para o PostgreSQL local.

Exemplo:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=xablau_db;Username=otaviodasilvamachado"
```

### 6.2 Jwt

Define:

- chave de assinatura
- emissor
- audiencia
- tempo de expiracao

### 6.3 Cors

Lista as origens permitidas do frontend local.

---

## 7. Entidades do dominio

## 7.1 User

Arquivo: `Entities/User.cs`

Campos:

- `Id`: chave primaria
- `Name`: nome do usuario
- `Email`: email unico
- `PasswordHash`: hash da senha
- `Role`: `Admin` ou `User`
- `CreatedAt`: data de criacao
- `UpdatedAt`: data de atualizacao

Pontos importantes:

- senha nunca e salva em texto puro
- `Role` e convertido para string no banco
- email e unico

## 7.2 Product

Arquivo: `Entities/Product.cs`

Campos:

- `Id`
- `Name`
- `Description`
- `Price`
- `ImageUrl`
- `Stock`
- `CreatedAt`

Regras:

- `Price` deve ser maior que zero
- `Stock` deve ser maior ou igual a zero
- produto com `Stock = 0` nao aparece no catalogo publico

## 7.3 Favorite

Arquivo: `Entities/Favorite.cs`

Representa a relacao entre usuario e produto favorito.

Campos:

- `Id`
- `UserId`
- `ProductId`
- `CreatedAt`

Relacoes:

- favorito pertence a um usuario
- favorito aponta para um produto

Regra importante:

- um usuario nao pode favoritar o mesmo produto duas vezes

## 7.4 Cart

Arquivo: `Entities/Cart.cs`

Representa o carrinho do usuario.

Campos:

- `Id`
- `UserId`
- `CreatedAt`
- `UpdatedAt`
- `Items`

Regra importante:

- existe um unico carrinho por usuario

## 7.5 CartItem

Arquivo: `Entities/CartItem.cs`

Representa um item dentro do carrinho.

Campos:

- `Id`
- `CartId`
- `ProductId`
- `Quantity`
- `CreatedAt`
- `UpdatedAt`

Regra importante:

- no mesmo carrinho, um produto aparece apenas uma vez
- adicionar o mesmo produto de novo soma quantidade

## 7.6 Order

Arquivo: `Entities/Order.cs`

Representa o pedido finalizado.

Campos:

- `Id`
- `UserId`
- `TotalAmount`
- `CreatedAt`
- `Items`

## 7.7 OrderItem

Arquivo: `Entities/OrderItem.cs`

Representa um item do pedido.

Campos:

- `Id`
- `OrderId`
- `ProductId`
- `ProductNameSnapshot`
- `UnitPrice`
- `Quantity`
- `Subtotal`

Ponto importante:

`ProductNameSnapshot` existe para congelar o estado da compra.

Se o produto mudar de nome depois, o pedido continua fiel ao momento da compra.

O mesmo vale para `UnitPrice`.

---

## 8. `AppDbContext` e modelagem relacional

O `AppDbContext` e a ponte entre o codigo e o banco.

### 8.1 DbSets

Ele declara:

- `Users`
- `Products`
- `Favorites`
- `Carts`
- `CartItems`
- `Orders`
- `OrderItems`

### 8.2 Regras de modelagem

#### User

- indice unico em `Email`
- `Role` salvo como string

#### Favorite

- indice unico composto em `UserId + ProductId`
- relacionamento com `User`
- relacionamento com `Product`

#### Cart

- indice unico em `UserId`
- isso garante um carrinho por usuario

#### CartItem

- indice unico composto em `CartId + ProductId`
- relacionamento com `Cart`
- relacionamento com `Product`

#### Order

- relacionamento com `User`

#### OrderItem

- relacionamento com `Order`

### 8.3 Cascade delete

Foi usado `OnDelete(DeleteBehavior.Cascade)` em varias relacoes.

Significa:

- ao remover um pai, os filhos relacionados sao removidos automaticamente

Exemplos:

- remover um `Cart` remove seus `CartItems`
- remover um `Order` remove seus `OrderItems`

---

## 9. DTOs e por que eles existem

Sem DTO, voce exporia entidades diretamente.

Problemas disso:

- vazamento de campos desnecessarios
- resposta acoplada ao banco
- menos controle de validacao

Com DTO:

- request fica controlado
- response fica controlada
- regras ficam mais claras

### 9.1 Auth DTOs

- `LoginRequestDto`
- `LoginResponseDto`

### 9.2 Product DTOs

- `CreateProductDto`
- `ProductResponseDto`

### 9.3 User DTOs

- `CreateUserDto`
- `UpdateUserDto`
- `UserResponseDto`

### 9.4 Favorite DTOs

- `FavoriteResponseDto`

### 9.5 Cart DTOs

- `AddCartItemDto`
- `UpdateCartItemDto`
- `CartItemResponseDto`
- `CartResponseDto`

### 9.6 Order DTOs

- `OrderItemResponseDto`
- `OrderResponseDto`

---

## 10. Autenticacao e autorizacao

## 10.1 Diferenca

### Autenticacao

Responde a pergunta:

"quem e voce?"

No projeto, isso acontece com email, senha e JWT.

### Autorizacao

Responde a pergunta:

"o que voce pode fazer?"

No projeto, isso acontece com:

- `[Authorize]`
- `[Authorize(Roles = "Admin")]`

## 10.2 Login

No `AuthController`, o metodo `Login`:

1. recebe email e senha
2. busca o usuario por email no banco
3. usa `BCrypt.Verify` para validar a senha
4. se estiver tudo certo, gera o token
5. devolve token + dados basicos do usuario

## 10.3 Claims do token

O token carrega:

- `NameIdentifier`
- `Name`
- `Email`
- `Role`

Essas claims permitem:

- identificar o usuario logado
- verificar o perfil do usuario

## 10.4 `/api/auth/me`

Esse endpoint pega o `NameIdentifier` do token, busca o usuario no banco e devolve os dados basicos.

Uso:

- restaurar sessao no frontend
- saber quem esta autenticado

---

## 11. Controllers explicados

## 11.1 HealthController

Objetivo:

- provar que a API esta online

Rota:

- `GET /api/health`

Resposta:

- mensagem simples em JSON

## 11.2 AuthController

Rotas:

- `POST /api/auth/login`
- `GET /api/auth/me`

Conceitos importantes:

- injecao de dependencia de `AppDbContext`
- injecao de `IConfiguration`
- uso de claims
- geracao de JWT

## 11.3 UsersController

Classe protegida com:

`[Authorize(Roles = "Admin")]`

Ou seja:

- so admin pode acessar qualquer rota daqui

Rotas:

- `GET /api/users`
- `GET /api/users/{id}`
- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

Regras importantes:

- email duplicado retorna `409 Conflict`
- role invalida retorna `400 Bad Request`
- admin nao pode remover a si mesmo
- nao pode remover o ultimo admin
- senha nova no update e opcional

## 11.4 ProductsController

Rotas:

- `GET /api/products`
- `GET /api/products/admin`
- `GET /api/products/{id}`
- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`

Regras importantes:

- `GET /api/products` e publico
- listagem publica mostra apenas `Stock > 0`
- `GET /api/products/admin` mostra tudo, mas so para admin
- `POST`, `PUT` e `DELETE` sao rotas administrativas

## 11.5 FavoritesController

Rotas:

- `GET /api/favorites`
- `POST /api/favorites/{productId}`
- `DELETE /api/favorites/{productId}`

Regras:

- so usuario autenticado acessa
- produto precisa existir
- favorito nao pode duplicar
- cada usuario so enxerga os proprios favoritos

## 11.6 CartController

Rotas:

- `GET /api/cart`
- `POST /api/cart/items`
- `PUT /api/cart/items/{itemId}`
- `DELETE /api/cart/items/{itemId}`

Regras:

- carrinho e baseado no usuario do token
- se o carrinho nao existir, ele e criado sob demanda
- nao adiciona produto inexistente
- nao permite quantidade maior que o estoque
- adicionar o mesmo produto soma a quantidade

## 11.7 OrdersController

Rotas:

- `POST /api/orders/checkout`
- `GET /api/orders`
- `GET /api/orders/{id}`

Esse controller concentra a logica mais critica do sistema.

---

## 12. Fluxo completo do checkout

Esse e o fluxo mais importante do backend.

Quando o usuario chama `POST /api/orders/checkout`, o backend faz:

1. pega o `userId` pelo token
2. carrega o carrinho do usuario com os itens
3. carrega os produtos relacionados
4. valida se o carrinho existe e se nao esta vazio
5. valida se todos os produtos ainda existem
6. valida se todos os itens possuem estoque suficiente
7. abre uma transacao com `BeginTransactionAsync`
8. cria o `Order`
9. cria um `OrderItem` para cada item do carrinho
10. reduz o estoque de cada produto
11. calcula o total do pedido
12. remove os itens do carrinho
13. salva tudo com `SaveChangesAsync`
14. confirma a transacao com `CommitAsync`
15. retorna o pedido criado

### Por que a transacao e importante

Imagine este problema:

- o pedido foi criado
- mas a atualizacao de estoque falhou no meio

Sem transacao, o sistema poderia ficar inconsistente.

Com transacao:

- ou tudo acontece
- ou nada acontece

Essa e a ideia de atomicidade.

---

## 13. Conceitos de LINQ e EF Core usados no projeto

### `FindAsync`

Usado para buscar por chave primaria.

Exemplo mental:

- buscar `User` por `Id`
- buscar `Product` por `Id`

### `FirstOrDefaultAsync`

Usado quando a busca depende de condicoes customizadas.

Exemplo:

- buscar carrinho pelo `UserId`
- buscar item do carrinho pelo `itemId` + `UserId`

### `AnyAsync`

Usado para verificar existencia.

Exemplo:

- email ja existe?
- favorito ja existe?

### `Include`

Carrega entidades relacionadas.

Exemplo:

- carregar carrinho com itens
- carregar favoritos com produto

### `ThenInclude`

Continua o carregamento em niveis mais profundos.

Exemplo:

- `Cart -> Items -> Product`

### `Select`

Mapeia entidades para DTOs.

Isso evita expor a entidade inteira.

### `OrderBy`

Usado para ordenar respostas.

---

## 14. Regras de negocio mais importantes

Estas sao as regras que mais importam para explicar o projeto:

1. email de usuario e unico
2. apenas admin acessa rotas administrativas
3. um usuario comum nao acessa CRUD de usuarios nem CRUD de produtos
4. favorito e unico por usuario e produto
5. cada usuario possui um unico carrinho
6. um produto aparece uma vez por carrinho
7. quantidade do carrinho nao pode ultrapassar estoque
8. checkout nao pode acontecer com carrinho vazio
9. checkout nao pode acontecer se algum item estiver sem estoque suficiente
10. o estoque e reduzido quando a compra e concluida
11. produto com estoque zero sai do catalogo publico
12. o carrinho e limpo apos o checkout
13. o pedido guarda snapshot de nome e preco

---

## 15. Codigos HTTP usados no projeto

### `200 OK`

Operacao bem-sucedida com resposta no corpo.

Exemplos:

- login
- listar produtos
- listar favoritos
- ver carrinho

### `201 Created`

Criacao bem-sucedida.

Exemplos:

- criar usuario
- criar produto
- adicionar favorito
- checkout

### `204 No Content`

Sucesso sem corpo de resposta.

Exemplos:

- delete
- remover item do carrinho

### `400 Bad Request`

Dados invalidos ou regra de negocio violada.

Exemplos:

- carrinho vazio
- estoque insuficiente
- role invalida

### `401 Unauthorized`

Faltou token ou token invalido.

### `403 Forbidden`

Usuario autenticado, mas sem permissao suficiente.

Exemplo:

- `User` tentando acessar rota de `Admin`

### `404 Not Found`

Recurso nao encontrado.

Exemplos:

- usuario nao existe
- produto nao existe
- pedido nao existe

### `409 Conflict`

Conflito de unicidade.

Exemplos:

- email duplicado
- favorito duplicado

---

## 16. Historico de migrations

### `InitialCreate`

Criou as tabelas iniciais.

### `PrepareUsersForAuth`

Migration antiga que ficou vazia no historico, sem impacto funcional atual.

### `AddUserRoleAndEmailUniqueConstraint`

Consolidou o `Role` e o indice unico de email.

### `ImplementFavoritesModule`

Criou a tabela de favoritos corretamente.

### `AddCartModule`

Criou:

- `Carts`
- `CartItems`

### `AddOrdersModule`

Criou:

- `Orders`
- `OrderItems`

---

## 17. Como explicar cada modulo em uma fala curta

### Auth

"Eu implementei autenticacao com JWT. O usuario faz login com email e senha, a senha e validada com BCrypt, e o backend gera um token com claims de identificacao e role."

### Users

"O CRUD de usuarios e administrativo. O backend impede email duplicado, nao deixa remover o proprio admin autenticado e tambem nao deixa eliminar o ultimo administrador do sistema."

### Products

"Produtos podem ser criados, editados e removidos apenas por admin. A listagem publica mostra somente produtos disponiveis, ou seja, com estoque maior que zero."

### Favorites

"Favoritos sao vinculados ao usuario autenticado. Um usuario nao consegue favoritar o mesmo produto duas vezes porque existe regra de negocio e tambem indice unico no banco."

### Cart

"Cada usuario possui um unico carrinho. O carrinho soma quantidade quando o mesmo produto e adicionado de novo, mas nunca permite ultrapassar o estoque disponivel."

### Orders

"No checkout, o backend carrega o carrinho, valida estoque, cria o pedido, gera os itens do pedido, reduz o estoque dos produtos e limpa o carrinho dentro de uma transacao para garantir consistencia."

---

## 18. Perguntas provaveis e respostas

### "Por que voce usou JWT?"

Porque e uma forma simples e estateless de autenticacao para API. O servidor nao precisa manter sessao em memoria, e o frontend consegue reenviar o token a cada requisicao protegida.

### "Por que voce usou BCrypt?"

Porque senha nunca deve ser armazenada em texto puro. O BCrypt gera hash seguro e permite verificar a senha sem recuperar a senha original.

### "Por que voce criou DTOs?"

Para controlar o contrato da API, validar entrada e evitar expor diretamente as entidades do banco.

### "Por que voce usou Entity Framework Core?"

Porque ele acelera o mapeamento entre classes e banco, facilita queries, tracking de entidades e migrations.

### "Por que checkout usa transacao?"

Porque o checkout mexe em varias tabelas e regras ao mesmo tempo. Sem transacao, uma parte poderia salvar e outra falhar, deixando o banco inconsistente.

### "Por que o pedido salva nome e preco do produto?"

Porque o pedido precisa refletir a compra real do momento. Se o produto mudar depois, o pedido historico nao pode mudar junto.

### "Por que existe rota publica e rota admin de produtos?"

Porque o usuario comum deve ver apenas itens disponiveis para compra, enquanto o admin precisa ver todos para gerenciar estoque e cadastro.

---

## 19. Como demonstrar o projeto ao vivo

Sequencia ideal de demonstracao:

1. abrir Swagger
2. mostrar `GET /api/health`
3. fazer login de admin
4. mostrar CRUD de usuarios
5. mostrar CRUD de produtos
6. mostrar que `GET /api/products` publico lista apenas itens com estoque
7. criar usuario comum
8. logar com usuario comum
9. favoritar produto
10. adicionar produto ao carrinho
11. atualizar quantidade no carrinho
12. fazer checkout
13. mostrar pedido criado
14. mostrar carrinho vazio
15. mostrar que o estoque do produto diminuiu

Essa sequencia prova praticamente todo o backend.

---

## 20. Pontos fortes tecnicos para voce citar

- uso de JWT com role-based authorization
- hash de senha com BCrypt
- modelagem relacional consistente
- indices unicos para proteger regras de negocio
- carrinho por usuario
- checkout atomico com transacao
- snapshot do pedido
- Swagger configurado com Bearer
- CORS pronto para frontend local
- separacao entre entidade, DTO e controller

---

## 21. Pontos que voce pode admitir como escolhas conscientes

Se alguem perguntar por que nao houve arquitetura com service layer ou repository, uma resposta boa e:

"Eu priorizei uma arquitetura simples e legivel, adequada ao escopo do desafio. Como a base de dominio e pequena, mantive a logica nos controllers com EF Core para acelerar a entrega e preservar clareza. Se o sistema crescesse, eu extrairia services e camadas de aplicacao."

Se perguntarem sobre testes automatizados:

"Eu validei o backend com testes end-to-end reais via API, incluindo cenarios de sucesso, autorizacao, conflito, estoque insuficiente e checkout. Em uma evolucao seguinte, eu adicionaria testes automatizados de integracao."

---

## 22. Resumo final para memorizar

Se voce precisar de uma resposta bem curta, memorize esta:

"Eu construI uma Web API em ASP.NET Core com PostgreSQL e Entity Framework Core. Implementei autenticacao com JWT, hash de senha com BCrypt, autorizacao por role, CRUD administrativo de usuarios e produtos, catalogo publico com filtro por estoque, favoritos por usuario, carrinho com controle de quantidade e checkout com criacao de pedido, reducao de estoque e limpeza do carrinho dentro de transacao. Tambem configurei Swagger com Bearer, seed de admin e CORS para integracao com frontend React."

---

## 23. Checklist de dominio real

Voce deve conseguir explicar sem hesitar:

- o que e controller
- o que e entity
- o que e DTO
- o que e DbContext
- o que e migration
- o que e JWT
- o que e claim
- o que e BCrypt
- o que e CORS
- o que e transacao
- o que e indice unico
- o que e foreign key
- por que usar `Include`
- por que usar `CreatedAt` e `UpdatedAt`
- por que o checkout precisa ser atomico
- por que o produto com estoque zero sai do catalogo

Se voce domina esses pontos, voce domina a logica do backend.
