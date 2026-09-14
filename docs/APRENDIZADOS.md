# Aprendizados de carreira — Umbrello

> Registro dos "aprendizados do momento" pedidos pelo Luis ao longo do projeto.
> **Regra para o assistente:** antes de responder a um pedido de aprendizado, leia este arquivo inteiro. **Nunca repita um tema já registrado** (nem com outras palavras). Escolha um tema novo — de preferência ancorado no que acabou de acontecer no desenvolvimento — e registre-o aqui em seguida, com data, gatilho e evidência real do projeto.

---

## Índice

| # | Data | Tema | Em uma frase |
|---|---|---|---|
| A01 | 14/09/2026 | Sistemas que falham alto, e verificação como gargalo | Com IA escrevendo o código, o valor migrou de produzir para restringir e verificar. |

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

## Temas candidatos para os próximos pedidos

Lista de apoio para o assistente escolher um tema novo. Não é ordem obrigatória; o ideal é escolher o que conversa com a fase em andamento. **Ao usar um tema, mova-o para o índice acima como entrada completa.**

- Trade-off explícito e ADR: por que registrar o que foi descartado vale mais que registrar o que foi escolhido.
- Consistência eventual como postura de projeto (e não como detalhe do broker).
- Idempotência como contrato, não como truque.
- Observabilidade como pré-requisito de senioridade: não se opera o que não se enxerga.
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
