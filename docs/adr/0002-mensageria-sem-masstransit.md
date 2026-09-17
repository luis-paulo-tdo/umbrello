# ADR 0002 — RabbitMQ.Client direto, sem MassTransit e MediatR

- **Status:** Aceita
- **Data:** 17/09/2026
- **Decisores:** Luis Paulo

## Contexto

Em 2025 e 2026, três bibliotecas muito usadas no ecossistemas .NET passaram a ter licença comercial: MassTransit (a partir da v9), MediatR e AutoMapper. A v8 do MassTransit segue aberta, mas com suporte previsto até o fim de 2026. FluentAssertions fez um movimento parecido na v8.

O Umbrello depende de mensageria no núcleo e usa um pipeline de casos de uso com preocupações transversais (validação, log e transações).

## Decisão

1. **Transporte:** `RabbitMQ.Client` 7 diretamente, com uma camada fina própria em `Umbrello.Messaging.RabbitMq` (topologia, publisher com confirms, consumer base, retry escalonado, DLQ, outbox e inbox).
2. **Pipeline de Casos de Uso:** Interfaces próprias `ICommandHandler`/`IQueryHandler` com decorators registrados via Scrutor, no lugar de MediatR.
3. **Mapeamento:** Manual, sem AutoMapper.
4. **Asserções de Testes:** Shouldly, no lugar de FluentAssertions.

## Consequências

**Positivas**

- Zero risco de licença e custo zero.
- Exposição direta aos conceitos de AMQP: exchange, binding, ack, prefetch, confirm, TTL, dead-lettering. Em entrevista, isso é a diferença entre saber configurar uma biblioteca e entender o protocolo.
- O pipeline de decorators é uma aplicação real do padrão Decorator, escrita à mão.

**Negativas**

- Mais código de infraestrutura a manter e testar (a fundação de mensageria é uma fase inteira do projeto).
- Recursos prontos do MassTransit — sagas, agendamento de mensagens, request/response não existem. Se algum for necessário, terá de ser escrito ou o escopo reavaliado.
- Mapeamento manual é verboso em telas com muitos campos.

## Alternativas Consideradas

| Alternativa | Por que não |
| --- | --- |
| MassTransit v8 (ainda aberta) | Suporte previsto só até o fim de 2026; começar um projeto novo numa versão em fim de vida é divida no dia zero. |
| MassTransit v9 com Licença | Custo mensal incompatível com um projeto de portfólio. |
| Wolverine | Boa alternativa aberta, mas abstrai o broker — e abstrair é justamente o que este projeto não quer fazer. |
| MediatR v11 (Última Livre) | Mesma lógica: dependência congelada no início do projeto, por algo que são ~60 linhas de código próprio. |
