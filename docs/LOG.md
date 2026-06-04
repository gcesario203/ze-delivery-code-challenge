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

Criei um projeto para os testes e criei o domínio rico de parceiro, para atingir 100% de cobertura com testes ricos e válidos, foram criados 18 testes unitários de dominio, coisa simples mas que deu um certo trabalho manual (sdds IA).

# Dia 2

Estruturei tranquilamente o formato inicial dos use cases de busca de Parceiro por id respeitando a estrutura do padrão CQRS, modelar o dominio para as abstrações interfaces dos repositorios foi extremamente trivial, porém, como quis diferenciar por motivos de curiosidade, não utilizei o mediatr como biblioteca para o padrão mediador no projeto, mas sim, utilizei o WolwerineFX e já me deparei com uma diferença positiva: Os handlers de query e commands são injetados diretamente no DI, sem necessidade da existencia de outras abstrações. Porém, para diferenciar mais ainda, fiz algo que já tinha ideia que projetos robustos fazem por conta de performance de bibliotecas de manupulação/conexão de bancos de dados: utilizar o EF para os commands e Dapper para as queries por simples e puro overenginering e curiosidade. Criei as abstrações dos repositorios de parceiro no dominio e implementei de um modo """seco""" os repositorios com seus respectivos contextos de banco de dados, onde o repo de command utiliza um AppDbContext basico do EF e o repo de query utiliza a abstração do .net IDBConnection. Por mais """interessante""" e simples que tenha ficado, por conta do escopo e de como o projeto foi pensado, seria impossivel manter os dados do parceiro de forma congruente sem transações de banco de dados, com isso, foi implementada uma UnitOfWork, confesso que já tinha utilizado em projetos maiores em ambiente corporativo e muito mais em projetos menores, entretanto, estava confuso de como utilizar a mesma transação de banco de dados numa escrita e numa leitura e, outra surpresa positiva(sim, sou burro), acabei utilizando as abstrações que o EF utiliza do .net em comum com o dapper, logo, queries não utilizão diretamente IDbConnection, mas sim, IUnitOfWork, achei mágico e tendencia, mas me senti burro e atrasado ao mesmo tempo. Chegou a hora dos testes, me desejem sorte.