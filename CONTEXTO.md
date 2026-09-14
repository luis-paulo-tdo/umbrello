# CONTEXTO — Umbrello (pair programming com Claude)

> **Leia este arquivo inteiro antes de qualquer resposta.** Depois leia a seção da fase atual em [`docs/PLANEJAMENTO.md`](docs/PLANEJAMENTO.md).
> Última atualização: **14/09/2026** · Sessão 1

---

## 1. Como iniciar uma nova sessão

Cole na nova sessão (com a pasta `C:\Estudos\umbrello` conectada):

```text
Estamos desenvolvendo o Umbrello em dupla. Leia CONTEXTO.md e a fase atual em
docs/PLANEJAMENTO.md. Siga as regras de colaboração e continue do "Próximo passo".
```

Se a pasta não estiver conectada, anexe `CONTEXTO.md` e `docs/PLANEJAMENTO.md` à conversa.

---

## 2. Regras de colaboração (obrigatórias para o assistente)

1. **Papéis:** o Luis Paulo executa todo o trabalho braçal (criar arquivos, rodar comandos, commits). O assistente **planeja, instrui, entrega o código completo pelo chat, valida o que foi escrito e explica conceitos**. O assistente **não cria nem edita código-fonte na pasta do projeto**; os únicos arquivos que ele mantém são `CONTEXTO.md`, `CLAUDE.md` e `docs/PLANEJAMENTO.md`.
2. **Um passo por vez:** siga a numeração das fases (ex.: 2.3). Para cada passo: objetivo → conceitos em 2 a 5 linhas → comandos → código completo de cada arquivo (com o caminho) → como testar → mensagem de commit sugerida.
3. **Código completo, não trechos**, salvo quando for uma alteração pequena e bem localizada (nesse caso, mostre o antes e o depois).
4. **Validação:** quando o Luis pedir, leia os arquivos na pasta (ou o que ele colar), compare com o planejamento e responda com: ✅ o que está certo, ⚠️ o que ajustar (com o motivo) e o próximo passo.
5. **Conceitos:** quando perguntado, explique de forma didática (o que é → por que existe → como aparece no Umbrello → armadilhas comuns). Use a seção 18 do planejamento como base.
6. **Versões:** confira a documentação oficial antes de entregar código que dependa de APIs novas (.NET 10, Aspire 13.x, Angular 22, RabbitMQ.Client 7). O projeto usa **.NET 10**: não entregue APIs que só existem no .NET 11. Não invente assinaturas de API.
7. **Atualize este arquivo ao fim de cada passo concluído:** status (seção 5), próximo passo (seção 6), decisões novas (seção 4), pendências (seção 7), estado do repositório (seção 8) e o log (seção 9). Mudanças de escopo ou arquitetura também vão para `docs/PLANEJAMENTO.md`.
8. **Idioma:** conversa e documentação em PT-BR; código, nomes de tipos, commits e branches em inglês.
9. Mudanças de rumo são permitidas, mas precisam ser **registradas como decisão** antes de seguir.
10. **Aprendizados de carreira:** quando o Luis pedir "o aprendizado mais importante até aqui" (ou variação), leia [`docs/APRENDIZADOS.md`](docs/APRENDIZADOS.md) **antes de responder**, escolha um tema que ainda não esteja registrado lá — de preferência ancorado no que acabou de acontecer no desenvolvimento — e registre a nova entrada no arquivo depois de responder. Nunca repita um tema já registrado, nem reformulado.

---

## 3. Resumo do projeto

**Umbrello** — o usuário cadastra cidades favoritas e recebe por e-mail, todo dia, no horário e fuso que escolher, a previsão das próximas 24 h (OpenWeatherMap).

Fluxo de mensageria (o coração do projeto):

```text
Quartz (a cada 5 min) ─┐
"Enviar agora" (API) ──┴─ outbox ─► [direct] umbrello.forecast.requests ─► forecast.fetch
                                                                              │ (competing consumers)
                                                                     Worker.Forecast (OWM + agregação)
                                                                              │ outbox
                                                                  [fanout] umbrello.forecast.ready
                                                    ┌─────────────────────────┼─────────────────────────┐
                                           notifications.email      notifications.inapp         forecast.history
                                          Worker.Notifications        Api + SignalR            Worker.Forecast
```

Retry: exchanges fanout `umbrello.retry.10s|1m|5m` com filas TTL → default exchange → fila de origem. Falha final → `umbrello.dlx` → `<fila>.dlq`.

---

## 4. Decisões tomadas

| # | Data | Decisão | Motivo |
|---|---|---|---|
| D01 | 11/09/2026 | **.NET 10 LTS** (SDK 10.0.4xx, C# 14, EF Core 10). *Revisada na sessão 1: a escolha inicial era .NET 11 RC1.* | Preferência do Luis por uma versão LTS atual (suporte até nov/2028) |
| D02 | 11/09/2026 | **Angular 22**, Node 24 LTS, TypeScript 6, Angular Material 3 | Versão estável mais recente (03/06/2026) |
| D03 | 11/09/2026 | **SQL Server 2025** e **RabbitMQ 4.3** (quorum queues) | Versões atuais; tipo `json` nativo; quorum é o padrão recomendado no 4.x |
| D04 | 11/09/2026 | **Aspire 13.5** em dev + **Docker Compose** para rodar "tipo produção" | Observabilidade pronta e demonstração de portfólio |
| D05 | 11/09/2026 | **ASP.NET Core Identity + JWT próprio** + refresh token rotativo em cookie HttpOnly | Mostra domínio de segurança sem infraestrutura extra |
| D06 | 11/09/2026 | **CI no GitHub Actions** (build, testes, imagens no GHCR); sem deploy em nuvem no MVP | Custo zero |
| D07 | 11/09/2026 | **RabbitMQ.Client 7 direto, sem MassTransit** | MassTransit v9 é comercial; aprender AMQP de verdade |
| D08 | 11/09/2026 | **Sem MediatR/AutoMapper/FluentAssertions**; handlers + decorators (Scrutor), mapeamento manual, Shouldly | Licenças comerciais; praticar o padrão Decorator |
| D09 | 11/09/2026 | Clean Architecture em **monólito modular distribuído em processos** (um banco com schemas `identity`, `app`, `messaging`, `quartz`) | Complexidade adequada ao domínio; trade-off documentado em ADR |
| D10 | 11/09/2026 | Dois estágios de mensageria: **work queue (direct) → fanout** | Busca uma vez por envio; efeitos independentes e extensíveis |
| D11 | 11/09/2026 | **Transactional Outbox + Inbox**, publisher confirms, retry escalonado com TTL e DLQ | Confiabilidade at-least-once com idempotência |
| D12 | 11/09/2026 | **Quartz.NET clustered** (JobStore SQL Server) + coluna `NextDispatchAtUtc` | SQL Server não entende fusos IANA; query simples e indexada; sem duplicidade em cluster |
| D13 | 11/09/2026 | OWM **5 day / 3 hour Forecast** (plano grátis) + Geocoding + Current Weather | One Call 3.0 exige assinatura; fica como Strategy no backlog |
| D14 | 11/09/2026 | Previsão = **próximas 24 h** em períodos (Madrugada/Manhã/Tarde/Noite) no fuso da cidade | Evita ambiguidade de "dia" quando usuário e cidade estão em fusos diferentes |
| D15 | 11/09/2026 | E-mail com **template Razor via `HtmlRenderer`** + MailKit; Mailpit em dev | Primeira parte (first-party), gratuito e tipado |
| D16 | 11/09/2026 | Código em inglês, docs em PT-BR, Conventional Commits, branch por fase `feat/fase-XX-nome` | Padrão de mercado |

---

## 5. Status das fases

| Fase | Nome | Status |
|---|---|---|
| 0 | Ambiente e fundação do repositório | ⏳ Em andamento (0.1 ✅ · 0.3 ✅ · 0.4 ✅) |
| 1 | Aspire, ServiceDefaults e infraestrutura local | ⬜ |
| 2 | Domínio e persistência | ⬜ |
| 3 | Autenticação | ⬜ |
| 4 | Integração OpenWeatherMap | ⬜ |
| 5 | Casos de uso: cidades, favoritos, preferências | ⬜ |
| 6 | Front-end I | ⬜ |
| 7 | Fundação de mensageria | ⬜ |
| 8 | Scheduler | ⬜ |
| 9 | Worker.Forecast | ⬜ |
| 10 | Consumidores da fanout | ⬜ |
| 11 | Front-end II | ⬜ |
| 12 | Hardening | ⬜ |
| 13 | Containerização, Compose e CI | ⬜ |
| 14 | Polimento de portfólio | ⬜ |

Legenda: ⬜ não iniciada · ⏳ próxima/em andamento · ✅ concluída

---

## 6. Próximo passo

**Fase 0 → Passo 0.5 (em andamento):** testes de arquitetura em `Umbrello.Architecture.Tests` com NetArchTest.Rules + marcadores `AssemblyReference` em cada camada.

Depois: **0.6** (ADRs 0001 e 0002) e então a **Fase 1** (Aspire).

Concluídos: 0.1 (ambiente), 0.3 (fundação do repositório), 0.4 (solução `Umbrello.slnx`, 10 projetos de src + 5 de teste, referências entre camadas, build limpo). **0.2 (API key da OpenWeatherMap) segue pendente** — confirmar se já ativou.

---

## 7. Pendências e pontos de atenção

- [ ] Confirmar a API exata do Aspire 13.5 para hospedar o Angular (`AddJavaScriptApp` x `AddViteApp`) na Fase 1/6.
- [ ] As versões dos pacotes NuGet entram no `Directory.Packages.props` via `dotnet add package` a cada fase (CPM), para não fixar versões inventadas.
- [ ] Confirmar os nomes de pacote do ArchUnitNET compatíveis com xUnit v3 na Fase 0.
- [ ] Criar a API key da OWM cedo (a ativação pode demorar).

---

## 8. Estado do repositório

- Remoto: `https://github.com/luis-paulo-tdo/umbrello` · branch `main`
- Conteúdo atual: `README.md`, `.gitignore`, `CONTEXTO.md`, `CLAUDE.md`, `docs/PLANEJAMENTO.md`, `docs/APRENDIZADOS.md`
- Solução `Umbrello.slnx` com 10 projetos em `src/backend` e 5 em `tests`, referências entre camadas conforme a seção 6.4 do planejamento. Build limpo (sem warnings).
- Pacotes já no `Directory.Packages.props`: `Microsoft.AspNetCore.OpenApi` 10.0.11, `Microsoft.Extensions.Hosting` 10.0.11, `xunit.v3.mtp-v2` 4.0.1.
- Testes: **xUnit v3 sobre Microsoft Testing Platform** (template `xunit3` do pacote `xunit.v3.templates`; cada projeto de teste é um executável, `OutputType=Exe`). Enquanto um projeto de teste não tiver nenhum teste, o `dotnet test` retorna código 8 ("zero testes") — normal até a Fase 2.
- Convenção de nomenclatura: campo de instância privado `_camelCase`; `private static readonly` e `const` em `PascalCase` (regra específica adicionada ao `.editorconfig` **antes** da regra genérica de campo privado, porque o Roslyn aplica a primeira que casar).
- `AnalysisLevel=latest-recommended` + `TreatWarningsAsErrors` valem desde o começo: CA1848 (usar `[LoggerMessage]` em vez de `LogInformation` direto), CA1727 (placeholders em PascalCase) e CA1852 (`sealed`) já apareceram e o código de exemplo dos templates foi removido.

**Ambiente verificado (passo 0.1 ✅, 11/09/2026):**

| Ferramenta | Versão |
|---|---|
| .NET SDK | 10.0.400 (também 8.0.404 e 10.0.302; recomendado atualizar para 10.0.401) |
| Node.js | v24.19.0 |
| npm | 11.17.0 |
| Angular CLI | 22.1.8 |
| Docker Desktop / Engine | 4.79.0 / 29.5.3 (WSL2) |
| Aspire CLI | 13.5.3 |

---

## 9. Log de sessões

| Sessão | Data | O que foi feito |
|---|---|---|
| 1 | 11/09/2026 | Levantamento de versões atuais; decisões D01–D16; criação do `docs/PLANEJAMENTO.md` (arquitetura, topologia, modelo de dados, API, front, padrões, testes, roadmap de 15 fases) e deste arquivo. D01 revisada: **.NET 11 RC1 → .NET 10 LTS**. SDKs na máquina: 8.0.404, 10.0.302, 10.0.400. Passos 0.1, 0.3 e 0.4 concluídos: ambiente conferido, fundação do repositório e scaffold da solução com build limpo. |
