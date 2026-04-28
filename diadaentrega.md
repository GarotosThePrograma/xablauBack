Dá tempo sim. Falta pouca coisa para ter um carrinho funcional “entregável”.

O que você já tem/quase tem:

Cadastro
Login
Criação de carrinho junto com usuário
Buscar carrinho por usuário
Estrutura de entidades pronta: Usuario, Carrinho, ItemCarrinho, Produto
O que falta para ficar redondinho:

Testar o que você digitou
Ver se compila e se GET /api/carrinho/{usuarioId} funciona.

Criar produtos ou ter produtos no banco
O carrinho só consegue adicionar algo se existir produto cadastrado. Pode ser via banco manualmente ou um endpoint simples de produtos.

Adicionar item ao carrinho
Criar uma rota tipo:

POST /api/carrinho/{usuarioId}/itens
Enviando:

{
  "produtoId": 1,
  "quantidade": 2
}
Alterar quantidade
Rota tipo:

PUT /api/carrinho/{usuarioId}/itens/{produtoId}
Remover item
Rota tipo:

DELETE /api/carrinho/{usuarioId}/itens/{produtoId}
Ligar com o frontend
Depois do login, salvar o usuarioId no frontend e usar ele nas chamadas do carrinho.

Para amanhã ao meio-dia, eu faria o mínimo viável assim:

Hoje parar sem culpa.
Amanhã primeiro testar build/login/carrinho.
Depois fazer só:
buscar carrinho
adicionar item
remover item
Se sobrar tempo, faz alterar quantidade.
Isso já parece carrinho real para uma entrega de clone ecommerce.

Estimativa realista:

Se o projeto compilar fácil: 1h30 a 2h30
Se aparecer erro de migration/banco/namespace: 3h a 4h
Integrando frontend junto: pode virar 4h a 6h, dependendo do estado do front.
A parte mais importante para não travar amanhã: quando voltar, começa rodando:

dotnet build
e me manda o erro inteiro se aparecer. A gente resolve por ordem, sem tentar consertar tudo no escuro.