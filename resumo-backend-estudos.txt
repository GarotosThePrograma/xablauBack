RESUMO DE ESTUDOS - BACKEND XABLAUBACK

Data: 24/04/2026

Objetivo
Você precisa entregar um backend de um clone da Kabum e, ao mesmo tempo, quer aprender o máximo possível de C#/.NET até a data da entrega. A prioridade do projeto é:

1. Permitir cadastro de usuário.
2. Permitir login.
3. Fazer cada usuário ter seu próprio carrinho.
4. Persistir os dados no banco Neon (PostgreSQL).
5. Manter a solução organizada em camadas no estilo DDD.
6. Entender o que está acontecendo em cada parte, em vez de só copiar código.


ESTADO ATUAL DO PROJETO

A solução está dividida em quatro projetos:

1. xablauBack.API
   Camada de entrada da aplicação.
   Responsável por receber requisições HTTP do front, expor endpoints e devolver respostas.

2. xablauBack.Application
   Camada dos casos de uso.
   Responsável por orquestrar as ações do sistema, validar regras de negócio e coordenar domínio + infraestrutura.

3. xablauBack.Domain
   Camada central do negócio.
   Responsável por representar os conceitos do sistema, como Usuario, Carrinho, Produto e ItemCarrinho.

4. xablauBack.Infrastructure
   Camada técnica.
   Responsável por acesso ao banco, Entity Framework Core, persistência e integração com PostgreSQL/Neon.


ARQUIVOS IMPORTANTES PARA ESTUDAR

Ignorar:
- bin/
- obj/

Essas pastas são geradas automaticamente pelo .NET. Elas não são o lugar certo para aprender a estrutura real da aplicação.

Focar em:
- xablauBack.API/Program.cs
- xablauBack.Infrastructure/data/AppDbContext.cs
- xablauBack.Domain/Entities/Usuario.cs
- xablauBack.Domain/Entities/Carrinho.cs
- xablauBack.Domain/Entities/ItemCarrinho.cs
- xablauBack.Domain/Entities/Produto.cs


COMO PENSAR O FLUXO DE UMA REQUISICAO

Exemplo: cadastro de usuário.

1. O front envia uma requisição HTTP para a API.
2. Essa requisição normalmente leva um JSON no corpo.
3. O ASP.NET converte esse JSON automaticamente para um objeto C#.
4. O Controller recebe esse objeto.
5. O Controller chama um serviço da camada Application.
6. A Application valida as regras de negócio.
7. A Application usa repositórios/infraestrutura para buscar ou salvar dados.
8. A Infrastructure usa o Entity Framework Core.
9. O Entity Framework converte objetos C# em SQL.
10. O SQL é executado no PostgreSQL do Neon.
11. O banco responde.
12. A API monta a resposta HTTP.
13. O front recebe sucesso, erro ou dados processados.


EXEMPLO CONCEITUAL DO FLUXO

Front envia:

{
  "nome": "Gui",
  "email": "gui@email.com",
  "senha": "123456"
}

Para:
POST /api/auth/register

O controller recebe isso assim, conceitualmente:

public async Task<IActionResult> Register([FromBody] RegisterRequest request)

Significados:
- [FromBody]: os dados vêm do corpo da requisição.
- RegisterRequest: classe C# que representa o JSON recebido.
- IActionResult: tipo de resposta HTTP que o controller devolve.

Depois o controller chama a Application:

var result = await _authService.RegisterAsync(request);

A Application decide:
- nome é válido?
- email é válido?
- senha é válida?
- já existe usuário com esse email?
- precisa gerar hash da senha?
- precisa criar carrinho inicial?

Depois a Infrastructure salva no banco usando EF Core.


RESPONSABILIDADE DE CADA CAMADA

1. API
   Papel:
   - receber HTTP
   - converter JSON em objeto C#
   - chamar a camada Application
   - devolver resposta HTTP/JSON

   O que não deveria ficar aqui:
   - regra de negócio pesada
   - consulta complexa ao banco
   - lógica principal do sistema

2. Application
   Papel:
   - implementar casos de uso
   - validar regras de negócio
   - coordenar Domain + Infrastructure

   Exemplos de casos de uso:
   - cadastrar usuário
   - logar usuário
   - obter carrinho do usuário
   - adicionar item no carrinho
   - remover item do carrinho

3. Domain
   Papel:
   - representar o negócio
   - modelar entidades
   - expressar relacionamentos

   Exemplo:
   - um Usuario tem Id, Nome, Email, SenhaHash
   - um Carrinho pertence a um Usuario
   - um Carrinho tem vários ItemCarrinho
   - um ItemCarrinho aponta para um Produto

4. Infrastructure
   Papel:
   - acessar banco
   - implementar repositórios
   - configurar EF Core
   - salvar e buscar dados no PostgreSQL


O QUE E O ENTITY FRAMEWORK CORE

O EF Core é um ORM.

ORM, na prática, significa:
"Uma ferramenta que permite trabalhar com objetos C# como se fossem registros do banco."

Exemplo mental:

_context.Usuarios.Add(usuario);
await _context.SaveChangesAsync();

Isso vira algo parecido com:

INSERT INTO Usuarios (...)

Ou seja:
- você escreve em C#
- o EF traduz para SQL
- o PostgreSQL executa


O QUE E O NEON

O Neon é um banco PostgreSQL hospedado na nuvem.

No seu projeto, ele entra por meio da connection string em:
- xablauBack.API/appsettings.json

O AppDbContext usa essa connection string no Program.cs.


EXPLICACAO DO Program.cs

No Program.cs, a linha mais importante atualmente é a que registra o AppDbContext:

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

Isso quer dizer:
- "Quando alguém pedir AppDbContext por injeção de dependência, crie ele usando PostgreSQL"
- "Pegue a connection string chamada DefaultConnection do appsettings.json"

Depois disso:

builder.Services.AddControllers();

Quer dizer:
- "A aplicação vai usar controllers"

E:

app.MapControllers();

Quer dizer:
- "Mapeie as rotas dos controllers na pipeline HTTP"


EXPLICACAO DO AppDbContext

O AppDbContext é a classe que representa a sessão com o banco.

Hoje ele tem:

public DbSet<Usuario> Usuarios { get; set; }
public DbSet<Produto> Produtos { get; set; }
public DbSet<Carrinho> Carrinhos { get; set; }
public DbSet<ItemCarrinho> ItensCarrinho { get; set; }

Cada DbSet representa uma tabela/conjunto de dados.

Na prática:
- Usuario -> tabela de usuários
- Produto -> tabela de produtos
- Carrinho -> tabela de carrinhos
- ItemCarrinho -> tabela de itens de carrinho


RELACIONAMENTOS QUE O SISTEMA PRECISA TER

Modelo mental ideal:

1. Um usuário tem um carrinho.
2. Um carrinho pertence a um único usuário.
3. Um carrinho tem vários itens.
4. Cada item do carrinho aponta para um produto.

Ou seja:

Usuario 1 --- 1 Carrinho
Carrinho 1 --- N ItemCarrinho
Produto 1 --- N ItemCarrinho


O QUE SAO REGRAS DE NEGOCIO

Regra de negócio é uma decisão importante do sistema.

Exemplos para esse projeto:
- email não pode ser duplicado
- senha precisa ter tamanho mínimo
- usuário precisa existir para ter carrinho
- produto precisa existir para entrar no carrinho
- quantidade não pode ser menor ou igual a zero
- quantidade não pode ultrapassar o estoque

Essas regras ficam principalmente na Application.


O QUE O CONTROLLER DEVE FAZER

O Controller deve ser fino.

Ele deve:
- receber a requisição
- validar o formato básico, quando necessário
- chamar o serviço de Application
- devolver a resposta

Ele não deve concentrar a lógica principal do sistema.

Exemplo de controller ideal:

1. Receber RegisterRequest
2. Chamar AuthService.RegisterAsync
3. Devolver Ok(...) ou Created(...)


O QUE O SERVICE DE APPLICATION DEVE FAZER

Exemplo: AuthService.RegisterAsync

Ele pode:
- validar nome/email/senha
- verificar se o email já existe
- gerar hash da senha
- criar o usuário
- criar um carrinho para esse usuário
- mandar persistir
- devolver uma resposta mais segura para o front

Importante:
o front não deve receber a senha nem o hash da senha de volta.


JSON E SERIALIZACAO

Você perguntou se precisa transformar os dados em JSON.

Na prática:
- o front normalmente envia JSON
- o ASP.NET lê esse JSON e transforma em objeto C# automaticamente
- a API devolve um objeto C#
- o ASP.NET transforma esse objeto em JSON automaticamente

Então, na maior parte do tempo, você não monta JSON "na mão" no backend.


EXEMPLO DE FLUXO COMPLETO

1. Front:
   envia POST /api/auth/register

2. Corpo JSON:
   {
     "nome": "Gui",
     "email": "gui@email.com",
     "senha": "123456"
   }

3. Controller:
   recebe RegisterRequest

4. Application:
   valida regras

5. Infrastructure:
   salva Usuario

6. EF Core:
   gera SQL

7. Neon:
   grava no banco

8. API:
   devolve resposta JSON

Exemplo de resposta:

{
  "id": 1,
  "nome": "Gui",
  "email": "gui@email.com"
}


CODIGOS HTTP IMPORTANTES

- 200 OK
  Operação deu certo.

- 201 Created
  Um recurso foi criado com sucesso.

- 400 Bad Request
  A requisição veio errada ou quebrou regra de validação.

- 404 Not Found
  O recurso procurado não existe.

- 500 Internal Server Error
  O servidor falhou por erro interno.


ORDEM DE IMPLEMENTACAO RECOMENDADA

Para aprender e também entregar, a melhor ordem é:

1. Entender o que já existe.
2. Criar DTO de entrada para cadastro.
3. Criar DTO de resposta.
4. Criar controller de autenticação.
5. Criar serviço de cadastro.
6. Criar repositório de usuário.
7. Salvar usuário no Neon.
8. Criar login.
9. Criar carrinho por usuário.
10. Criar endpoints do carrinho.
11. Testar tudo com Swagger/Postman.
12. Integrar com o front.


FORMA DE ESTUDO COM O ASSISTENTE

Você pediu para não montar tudo por você, mas para ensinar e deixar você digitar.

A estratégia combinada é:

1. Receber o código em blocos pequenos.
2. Entender o objetivo de cada arquivo.
3. Entender linha por linha.
4. Você mesmo digitar.
5. Testar a cada etapa.
6. Só avançar para o próximo bloco depois de entender o anterior.


IMPORTANTE SOBRE O REPOSITORIO

Durante a conversa, houve uma tentativa de alteração automática no projeto, mas isso foi limpo depois.

Situação após a limpeza:
- xablauBack.Application/Class1.cs foi restaurado.
- xablauBack.Domain/Repositories ficou vazio novamente.
- xablauBack.Domain/Entities/Pedido.cs voltou ao estado anterior.

Ou seja: o projeto voltou ao estado básico para você estudar do jeito certo.


PROXIMO PASSO RECOMENDADO QUANDO RETOMAR

Quando você voltar no trabalho, o melhor ponto de retomada é:

1. Abrir estes arquivos:
   - xablauBack.API/Program.cs
   - xablauBack.Infrastructure/data/AppDbContext.cs
   - xablauBack.Domain/Entities/Usuario.cs
   - xablauBack.Domain/Entities/Carrinho.cs
   - xablauBack.Domain/Entities/ItemCarrinho.cs
   - xablauBack.Domain/Entities/Produto.cs

2. Relembrar esta ideia:
   - Controller recebe
   - Application decide
   - Infrastructure salva
   - EF Core traduz
   - Neon persiste
   - API responde

3. Começar pelo caso de uso "Cadastro de usuário".


PRIMEIRO CASO DE USO A IMPLEMENTAR

Cadastro de usuário.

Objetivo:
- receber nome, email e senha
- validar
- verificar duplicidade de email
- hash da senha
- salvar usuário
- criar carrinho vinculado ao usuário
- devolver resposta segura


PERGUNTAS GUIA PARA CADA FUNCIONALIDADE

Sempre pensar assim:

1. O que o usuário quer fazer?
2. Qual endpoint representa isso?
3. Qual JSON o front vai mandar?
4. Qual classe C# representa esse JSON?
5. Qual regra de negócio precisa validar?
6. Quais entidades participam?
7. O que precisa ir para o banco?
8. O que volta para o front?


META REALISTA

Você não precisa dominar C# inteiro até amanhã.

Você precisa sair sabendo:
- como uma API ASP.NET recebe dados
- como passar isso para services
- como salvar com EF Core
- como modelar Usuario e Carrinho
- como testar endpoint
- como explicar o fluxo em camadas

Isso já é uma vitória grande e suficiente para ganhar tração no projeto.


COMBINADO DE TRABALHO

Quando retomar:
- pedir explicação em passos pequenos
- sempre focar em um caso de uso por vez
- digitar você mesmo o código
- testar a cada etapa
- evitar pular para "arquitetura perfeita" antes do básico funcionar


FRASE-RESUMO FINAL

O front manda JSON para a API; o controller recebe e chama a Application; a Application valida a regra de negócio e usa a Infrastructure; a Infrastructure usa EF Core para conversar com o PostgreSQL no Neon; depois a API devolve uma resposta JSON para o front.
