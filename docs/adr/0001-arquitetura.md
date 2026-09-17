# ADR 0001 — Clean Architecture em monólito modular distribuído em processos

- **Status:** Aceita
- **Data:** 15/09/2026
- **Decisores:** Luis Paulo

## Contexto

O Umbrello tem um domínio pequeno (assinantes, cidades favoritas e envios diários), mas um fluxo assíncrono relevante: agendamento por fuso horário, busca em API externa com limite de chamadas e três efeitos independentes sobre o mesmo evento (e-mail, notificação in-app e histórico).

O projeto é de portfólio: precisa demonstrar de arquitetura sem virar um sistema caro de operar ou impossível de rodar na máquina de quem for avaliá-lo.

## Decisão

Clean Architecture com quatro camadas (`Domain`, `Application`, adaptadores de `Infrastructure` e `Messaging.RabbitMq`, e os hosts), distribuída em **cinco processos** que compartilham **um único banco SQL Server**, organizado por schemas: `identity`, `app`, `messaging` e `quartz`.

As mensagens seguem *Event-Carried State Transfer*: o evento `DailyForecastReady` carrega tudo que os consumidores precisam, sem consulta de volta à origem.

## Consequências

**Positivas**

- Uma única transação cobre a escrita de negócio e a outbox, o que torna a entrega confiável sem transação distribuída.
- Um `docker compose up` sobe o sistema inteiro; o custo de infraestrutura é zero.
- As camadas são verificadas por teste automatizado (`Umbrello.Architecture.Tests`), não por convenção.

**Negativas**

- Os processos são acoplados pelo schema do banco: uma migration pode afetar mais de um.
- Não há isolamento de falha no nível do banco. Se o SQL Server cair, tudo para.
- Escalar um worker não escala o banco junto, então o banco é o gargalo natural.

**Migitação:** Como os eventos já carregam estado próprio, separar os bancos por serviço no futuro exigiria mudar a persistência, não os contratos de mensagem.

## Alternativas Consideradas

| Alternativa | Porque não |
| --- | --- |
| Monólito de Processo Único | Não demonstraria competing consumers, escala horizontal e nem isolamento de falhas por fila — que é o ponto do projeto. |
| Microsserviços com Banco por Serviço | Custo de coordenação (saga, catálogo de eventos, duplicação de dados de assinante) desproporcional a um domínio deste tamanho. |
| Serverless (Functions + Fila Gerenciada) | Amarraria o projeto a um provedor e esconderia justamente a mecânica de AMQP que se quer mostrar. |
