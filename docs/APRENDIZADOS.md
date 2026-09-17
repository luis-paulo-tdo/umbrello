# Aprendizados de carreira — Umbrello

> Registro dos "aprendizados do momento" pedidos pelo Luis ao longo do projeto.
> **Regra para o assistente:** antes de responder a um pedido de aprendizado, leia este arquivo inteiro. **Nunca repita um tema já registrado** (nem com outras palavras). Escolha um tema novo — de preferência ancorado no que acabou de acontecer no desenvolvimento — e registre-o aqui em seguida, com data, gatilho e evidência real do projeto.

---

## Índice

| # | Data | Tema | Em uma frase |
|---|---|---|---|
| A01 | 14/09/2026 | Sistemas que falham alto, e verificação como gargalo | Com IA escrevendo o código, o valor migrou de produzir para restringir e verificar. |
| A02 | 17/09/2026 | Trade-off explícito: o valor está na alternativa rejeitada | Gerar opções ficou grátis; escolher com base em restrições de contexto, não. |
| A03 | 17/09/2026 | Observabilidade é pré-requisito, não ferramenta | Sistema assíncrono não é depurável por leitura de código; ou você enxerga, ou adivinha. |

---

## A01 — Sistemas que falham alto, e verificação como novo gargalo

**Data:** 14/09/2026
**Gatilho:** fim da Fase 0 (fundação do repositório e scaffold), depois de uma sequência de erros silenciosos de configuração.

### Tese

O que separa profissionalmente hoje não é a capacidade de escrever código, e sim duas coisas encadeadas:

1. **Desenhar sistemas em que o erro se manifesta cedo e alto** — restrição no lugar de convenção, verificação automática no lugar de documentação.
2. **Verificar a saída da IA com repertório suficiente para farejar o que cheira errado** — o gargalo deixou de ser produzir e passou a ser julgar.

### Evidência no projeto

Nos seis primeiros passos, nenhuma linha de regra de negócio foi escrita. O que foi escrito foram restrições — e elas já pegaram quatro erros que **não dariam exceção em tempo de execução**:

- `TreatWarningAsErrors` sem o "s": o MSBuild aceita qualquer nome de propriedade e ignora o que não conhece.
- XML malformado no `Directory.Packages.props`: o CPM desligou em silêncio (`NU1015` em todo projeto).
- Template do xUnit fixando `net8.0`, descoberto só ao referenciar um projeto `net10.0`.
- Analisadores (CA1848, CA1727, CA1852) barrando o código de exemplo dos templates.

A mesma ideia reaparece em camadas diferentes ao longo do planejamento: índice único no banco em vez de confiar na validação em C#; inbox em vez de torcer para a mensagem não duplicar; teste de arquitetura em vez de um documento dizendo "o domínio não conhece o EF"; publisher confirm em vez de supor que o broker recebeu.

Do lado da verificação, os três momentos em que o Luis cortou prejuízo:

- Perguntou **como** executar o script de referências antes de rodá-lo — o script tinha `$host`, variável reservada do PowerShell.
- Insistiu que o caminho de menu do Visual Studio 2026 que o assistente afirmou não existia — e não existia mesmo; a afirmação foi feita com confiança indevida.
- Não contornou o aviso de nomenclatura em `private static readonly Assembly Domain`: perguntou, e isso expôs um defeito real na regra do `.editorconfig` que atrapalharia dezenas de vezes.

### O que vale menos hoje

Decorar sintaxe, lembrar ordem de parâmetros, escrever CRUD de cabeça.

### Reflexo no portfólio

O que diferencia o Umbrello de mil tutoriais não é o código, é o `docs/adr/`, o registro de decisões com os motivos, o teste que prova que a arquitetura se sustenta, e a capacidade de explicar numa entrevista por que o retry republica com a routing key da própria fila em vez de passar de novo pela fanout.

---

## A02 — Trade-off explícito: o valor está na alternativa rejeitada

**Data:** 17/09/2026
**Gatilho:** passo 0.6, escrita dos ADRs 0001 e 0002 (fechamento da Fase 0).

### Tese

Sinal de senioridade não é saber a resposta certa, é conseguir mostrar **o espaço de decisão**: quais eram as opções, o que cada uma custava, e por que esta fazia sentido **naquele contexto**. Decisão sem alternativa registrada é indistinguível de cópia de tutorial.

Com IA, gerar a opção ficou grátis — três arquiteturas plausíveis e bem escritas saem em segundos. O que o modelo não faz é escolher com base em restrições que só o time conhece: orçamento, prazo, competência instalada, tempo de vida do sistema, quem opera às 3h da manhã. O texto virou commodity; o julgamento contextual não.

### Evidência no projeto

No ADR 0002, a decisão "RabbitMQ.Client direto" sozinha diz pouco. O que dá peso é o resto:

- **Restrição real:** MassTransit v9 é comercial; a v8 tem suporte até o fim de 2026.
- **Alternativa tentadora descartada:** Wolverine — aberta e madura — rejeitada porque **abstrai o broker**, o oposto do objetivo do projeto.
- **Preço aceito:** uma fase inteira (a 7) escrevendo infraestrutura que a biblioteca daria pronta, sem saga e sem request/response.

Decisão sem consequência negativa declarada é propaganda, não análise.

A D01 é o outro exemplo: recomendação de .NET 11 RC contra a preferência do Luis por LTS. Critérios diferentes ("mais atual" x "mais estável e com mais material"), nenhum errado em abstrato; o que torna a escolha defensável é o motivo registrado (projeto de estudo, uma pessoa só, evitar tempo gasto com bug de RC).

### Em entrevista

Três níveis de resposta para "por que você usou X?": (1) "é o padrão do mercado" — não diz nada; (2) "X faz A, B e C" — repete a documentação; (3) "considerei X, Y e Z, descartei Y por isso e Z por aquilo, X me custou tal coisa, que aceitei porque o contexto era esse". O nível 3 só existe se tiver sido registrado na hora.

### Armadilhas

- ADR escrito depois para justificar: vira teatro. O valor está em escrever enquanto ainda há dúvida — listar alternativas muda a escolha com frequência.
- ADR para tudo: se cada `if` virar ADR, ninguém lê nenhum. Critério: caro de reverter? afeta mais de um módulo? alguém vai perguntar "por que isso é assim?" depois?
- Alternativa de palha: se a opção rejeitada não for tentadora, não houve análise.
- Editar ADR aceito: decisão muda com ADR novo que substitui o antigo; o histórico é o produto.
- "Boa prática" como argumento: toda boa prática responde a um problema específico. Sem o problema, é cargo cult.

### Treino no dia a dia

Ao receber uma solução (de alguém ou da IA), perguntar **"o que isso me custa?"** antes de "isso funciona?". A segunda pergunta o código responde; a primeira exige a pessoa.

---

## A03 — Observabilidade é pré-requisito, não ferramenta

**Data:** 17/09/2026
**Gatilho:** passo 1.1, escrita do `Umbrello.ServiceDefaults` (OpenTelemetry, health checks) antes de existir qualquer regra de negócio.

### Tese

Sistema assíncrono **não é depurável por leitura de código**. Instrumentação não é enfeite de produção: é o que torna o sistema operável, e precisa existir antes do primeiro incidente.

### Evidência no projeto

Cenário que vai acontecer: o usuário clica em "Enviar agora" às 7h03 e o e-mail não chega. Ficou na outbox porque o relay morreu? Foi publicada e o broker recusou? O consumer pegou e a OWM devolveu 429? Foi para a DLQ na quarta tentativa? O e-mail saiu e o Mailpit estava fora? São cinco processos, duas exchanges, seis filas e uma API externa — nenhuma leitura de código responde; só o trace. E com retry no meio, o estado muda enquanto se olha.

Detalhes já embutidos no projeto que são exatamente isso:

- **`/health` x `/alive` separados**: readiness tira do balanceador, liveness reinicia o container. Confundir os dois gera reinício em massa de containers saudáveis quando uma dependência oscila.
- **Filtro que exclui `/health` dos traces**: um check a cada 5 s em cinco processos são ~86 mil spans/dia de ruído (e custo de ingestão). Saber o que **não** instrumentar é metade da habilidade.
- **`traceparent` salvo na outbox**: é o que faz o trace atravessar o broker. Sem ele, dois traces desconexos — HTTP de um lado, consumer do outro.

### Por que pesa mais agora

Mais código entrando em produção por semana (IA) somado a arquiteturas com mais partes móveis = mais comportamento que ninguém previu. A pergunta que separa perfis deixou de ser "você sabe escrever esse serviço?" e passou a ser "quando isso quebrar às 3h da manhã, quanto tempo você leva para saber onde?".

### Armadilhas

- Confundir volume com visibilidade: logar tudo em Information é a forma mais comum de não enxergar nada — e de gastar caro com isso.
- Métrica sem pergunta: antes de criar a métrica, saber que decisão ela informa.
- Explosão de cardinalidade: `UserId`/`MessageId` como *tag* de métrica derruba o backend. Identificador vai em log e trace; tag de métrica é dimensão de baixa cardinalidade.
- Alerta que ninguém age treina o time a ignorar alertas. Sem ação associada, é dashboard, não alerta.
- Dado sensível em atributo de span: telemetria costuma ter controle de acesso mais frouxo que o banco.

### Treino no dia a dia

Antes de abrir o PR de qualquer funcionalidade, responder: **"como eu vou saber que isso quebrou?"** e **"em quanto tempo?"**. Se a resposta for "o usuário reclama", falta a parte que ninguém vê — e que é justamente a que te procuram para resolver.

---

## Temas candidatos para os próximos pedidos

Lista de apoio para o assistente escolher um tema novo. Não é ordem obrigatória; o ideal é escolher o que conversa com a fase em andamento. **Ao usar um tema, mova-o para o índice acima como entrada completa.**

- Consistência eventual como postura de projeto (e não como detalhe do broker).
- Idempotência como contrato, não como truque.
- Custo e limites como requisito de arquitetura (rate limit, cota, cache).
- Segurança prática no dia a dia: ownership, segredos, superfície de ataque.
- Comunicação técnica escrita: PR, ADR, post-mortem.
- Estimativa e negociação de escopo; a arte de cortar.
- Testes como ferramenta de design, não de conferência.
- Ler código alheio rápido: a habilidade mais subestimada.
- Ciclo de feedback curto: por que build e teste lentos corroem a qualidade.
- Risco de dependência e licenciamento (o caso MassTransit/MediatR/AutoMapper).
- Operação: o que acontece depois do deploy (DLQ, watchdog, plantão).
- Como se portar numa entrevista técnica falando de um projeto próprio.
