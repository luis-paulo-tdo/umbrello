# ADR 0001 — Clean Architecture em monólito modular distribuído em processos

- **Status:** aceita
- **Data:** 15/09/2026
- **Decisores:** Luis Paulo

## Contexto

O Umbrello tem um domínio pequeno (assinantes, cidades favoritas e envios diários), mas um fluxo assíncrono relevante: agendamento por fuso horário, busca em API externa com limite de chamadas e três efeitos independentes sobre o mesmo evento (e-mail, notificação in-app e histórico).

O projeto é de portfólio: precisa demonstrar de arquitetura sem virar um sistema caro de operar ou impossível de rodar na máquina de quem for avaliá-lo.