# CONTEXTO — Umbrello (pair programming com Claude)

> **Leia este arquivo inteiro antes de qualquer resposta.** Depois leia a seção da fase atual em [`docs/PLANEJAMENTO.md`](docs/PLANEJAMENTO.md).
> Última atualização: **17/09/2026** · fim da Sessão 1

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

1. **Papéis:** o Luis Paulo executa todo o trabalho braçal (criar arquivos, rodar comandos, commits). O assistente **planeja, instrui, entrega o código completo pelo chat, valida o que foi escrito e explica conceitos**. O assistente **não cria nem edita código-fonte na pasta do projeto**; os únicos arquivos que ele mantém são `CONTEXTO.md`, `CLAUDE.md`, `docs/PLANEJAMENTO.md` e `docs/APRENDIZADOS.md`.
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
| 0 | Ambiente e fundação do repositório | ✅ Concluída |
| 1 | Aspire, ServiceDefaults e infraestrutura local | ⏳ Em andamento (1.1 quase pronto) |
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

**Fase 1 → Passo 1.1 — terminar e validar.** O projeto `Umbrello.ServiceDefaults` já existe, com `Extensions.cs` (OpenTelemetry, health checks, service discovery, resiliência padrão) e os pacotes instalados. Falta confirmar:

1. Os cinco hosts (`Api`, `Scheduler`, `Worker.Forecast`, `Worker.Notifications`, `MigrationService`) referenciam `Umbrello.ServiceDefaults`.
2. Os `Program.cs` dos quatro workers chamam `builder.AddServiceDefaults();`.
3. O `Program.cs` da Api chama `builder.AddServiceDefaults();` e `app.MapDefaultEndpoints();`.
4. `dotnet build` limpo e `dotnet run --project src\backend\Umbrello.Api` respondendo em `/health` e `/alive`.
5. Commit em `feat/fase-01-aspire`: `feat: add service defaults with telemetry and health checks`.

**Depois, Passo 1.2 — AppHost do Aspire.** Comandos e APIs já pesquisados e confirmados na documentação da Aspire 13.5:

- `aspire init` na raiz: como existe `Umbrello.slnx`, cria um AppHost **baseado em projeto** (`AppHost.csproj`) mais o `aspire.config.json`. Não cria ServiceDefaults (por isso ele foi escrito à mão no 1.1).
- `aspire add communitytoolkit-mailpit` para o Mailpit (`CommunityToolkit.Aspire.Hosting.MailPit`), API `builder.AddMailPit("mailpit", httpPort: 8025, smtpPort: 1025)`; endpoints `smtp` e `http`; `.WithDataVolume("mailpit-data")` opcional.
- SQL Server (`Aspire.Hosting.SqlServer`): `builder.AddSqlServer("sql").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()` + `.AddDatabase("umbrellodb")`. **A imagem padrão é `2022-latest`** — para o SQL Server 2025 (tipo `json` nativo, D03) é preciso `.WithImageTag("2025-latest")`; validar com `docker pull mcr.microsoft.com/mssql/server:2025-latest` antes.
- RabbitMQ (`Aspire.Hosting.RabbitMQ`): `builder.AddRabbitMQ("rabbitmq").WithManagementPlugin().WithDataVolume()`; imagem padrão já é `rabbitmq:4.3`. **Atenção:** o RabbitMQ 4.3 rejeita declaração de fila não durável e não exclusiva com erro AMQP 541 — nossas filas são quorum, então tudo bem, mas vale lembrar na Fase 7.
- Chave da OWM como parâmetro secreto: `builder.AddParameter("owm-api-key", secret: true)` + `dotnet user-secrets` (passo 0.2, ainda pendente).
- Projetos entram no AppHost com `dotnet add <AppHost> reference <projeto>` e depois `builder.AddProject<Projects.Umbrello_Api>("api")`; ordem com `WaitFor` / `WaitForCompletion(migrator)`; réplicas com `WithReplicas(2)` (útil na Fase 9).

Depois: **1.3** API com health checks visíveis no dashboard e **1.4** registro dos demais hosts.

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
- `docs/adr/`: ADR 0001 (arquitetura) e ADR 0002 (mensageria sem MassTransit) escritos, com índice em `docs/adr/README.md`.
- Solução `Umbrello.slnx` com 10 projetos em `src/backend` e 5 em `tests`, referências entre camadas conforme a seção 6.4 do planejamento. Build limpo (sem warnings).
- `src/backend/Umbrello.ServiceDefaults`: criado, com `Extensions.cs` (AddServiceDefaults, ConfigureOpenTelemetry, AddDefaultHealthChecks, MapDefaultEndpoints) e `FrameworkReference` de `Microsoft.AspNetCore.App`.
- Pacotes no `Directory.Packages.props`: `Microsoft.AspNetCore.OpenApi` 10.0.11 · `Microsoft.Extensions.Hosting` 10.0.11 · `xunit.v3.mtp-v2` 4.0.1 · `NetArchTest.Rules` 1.3.2 · `Microsoft.Extensions.Http.Resilience` 10.10.0 · `Microsoft.Extensions.ServiceDiscovery` 10.10.0 · `OpenTelemetry.*` 1.18.0 (Exporter.OpenTelemetryProtocol, Extensions.Hosting, Instrumentation.AspNetCore/Http/Runtime).
- `tests/Umbrello.Architecture.Tests`: 7 regras de dependência entre camadas, todas verdes. Marcadores `AssemblyReference` existem em Domain, Contracts, Application, Infrastructure e Messaging.RabbitMq (servem também para varredura de assembly na DI).
- Testes: **xUnit v3 sobre Microsoft Testing Platform** (template `xunit3` do pacote `xunit.v3.templates`; cada projeto de teste é um executável, `OutputType=Exe`). Enquanto um projeto de teste não tiver nenhum teste, o `dotnet test` retorna código 8 ("zero testes") — normal até a Fase 2.
- `.editorconfig`: `CA1707` desligada só em `[tests/**/*.cs]` (nomes de teste no padrão `Metodo_Cenario_ResultadoEsperado`). Para exceções de propriedades MSBuild em testes, usar `tests/Directory.Build.props` **com** `<Import Project="$([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))" />`, senão os projetos perdem tudo que vem da raiz.
- Atenção a colisões de `global using`: `<Using Include="Xunit" />` + NetArchTest deixam `TestResult` ambíguo (CS0104). Nos testes de arquitetura, o helper recebe `IEnumerable<string>?` em vez de `TestResult`.
- Convenção de nomenclatura: campo de instância privado `_camelCase`; `private static readonly` e `const` privado em `PascalCase`. No `.editorconfig` as regras estão nesta ordem obrigatória: (1) `private_const_pascal`, (2) `private_static_readonly_pascal`, (3) `private_fields_underscore` — o Roslyn aplica a **primeira** que casar, então a genérica fica sempre por último.
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
| 1 | 11/09/2026 | Levantamento de versões atuais; decisões D01–D16; criação do `docs/PLANEJAMENTO.md` (arquitetura, topologia, modelo de dados, API, front, padrões, testes, roadmap de 15 fases) e deste arquivo. D01 revisada: **.NET 11 RC1 → .NET 10 LTS**. SDKs na máquina: 8.0.404, 10.0.302, 10.0.400. Passos 0.1, 0.3, 0.4 e 0.5 concluídos: ambiente conferido, fundação do repositório, scaffold da solução com build limpo e testes de arquitetura verdes. Criado `docs/APRENDIZADOS.md` (regra 10). Fase 0 fechada com os ADRs 0001 e 0002. Fase 1 iniciada: `Umbrello.ServiceDefaults` escrito à mão (sem template do Aspire). Aprendizados registrados: A01, A02, A03. |
