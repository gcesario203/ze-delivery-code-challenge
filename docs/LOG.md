# Dia 1

Com base na documentação do próprio [repositório do desafio](https://github.com/ab-inbev-ze-company/ze-code-challenges/blob/master/backend_pt.md), consolidei os **requisitos funcionais** para facilitar o entendimento do problema e defini algumas **heurísticas** para guiar as decisões relacionadas aos **requisitos não funcionais**.

Pensando em um cenário de **teste técnico** (especialmente para uma vaga **júnior/pleno**), eu seguiria com uma solução direta — o “feijão com arroz”: **arquitetura em 3 camadas**, sem modelos ricos, com **controllers**, **services** e **repositories** por entidade, usando **MongoDB** como banco principal. Isso já entregaria uma **POC** satisfatória.

Apesar de eu já ter familiaridade com vários conceitos de **engenharia de software** (principalmente no nível de código), nunca tive a oportunidade de praticar de forma mais robusta:
- **TDD** como abordagem central de desenvolvimento;
- **DDD** para modelagem de domínio;
- **System Design** para definir o escopo e orientar a solução como um todo.

Isso explica o *overengineering* deste projeto: a ideia aqui é usar o desafio como laboratório para exercitar esses pontos com mais profundidade.  
Desde já: me desejem sorte.

# Dia 1.1

Criei um projeto para os testes e criei o domínio rico de parceiro, para atingir 100% de cobertura com testes ricos e válidos, foram criados 18 testes unitários