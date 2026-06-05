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

# Dia 2.1

Fiz a cobertura de testes, porém, foi mais chato/dificil que eu pensava! De modo obvio como descrito anteriormente, por mais que utilizem as mesmas abstrações, Dapper e EF são frameworks diferentes, particularidades como Guid não ser reconhecido no Dapper e o pior: EF trackeando os testes por que o contexto do banco de dados do SQLite em memória exige que a implementação seja uma singleton, fazendo com que o dispose seja apenas no final de todos os testes, tive que alterar:
- DbContext do EF não usa mais a entidade de dominio diretamente, mas sim, um DbModel que possui seu respectivo mapper dentro de infra
- PartnerCommandRepository precisou levar em consideração se as entidades estão trackeadas principalmente nas inserções e remoções
Gostei do resultado, mas custou meus neurônios, de 2 que funcionam, foi pra 1/2. Hora de criar os testes e as implementações dos useCases

# Dia 2.2

Com os repositorios completos, a criação dos testes e dos commands e das queries dos usecases foram extremamente triviais ao ponto de eu terminar em menos de 15 minutos, foi extremamente satisfatório! Porém, chegou a hora do meu momento mais emblemático: Criar os domínios de evento, isto foi uma dor de cabeça enorme pois:
- Nunca fiz domínio de eventos em aplicações reais, apenas vi como funcionam
Com isto em mente, já comecei erroneamente fazendo um eventbus """seco"""" dentro dos handlers, pesquisando e utilizando stackoverflow como minha fonte da verdade(sim, outros repositorios não possuem bons exemplos, principalmente públicos e sim, muitos repositorios do github que todo os usuário adicionam eles só listam uma quantidade N de técnologias e não possui NADA, absolutamente NADA de implementação) e descobri da pior maneira que o nome "Eventos de dominios" não são atoa, pois eles são removidos e inseridos pelas próprias entidades de dominio, tendo o encargo de ser disparado por um "despachador" de eventos na camada de aplicação, onde fiz os seguintes passos:
- criei a classe contendo as propriedades de um dominio de evento
- criei um handler para quando o evento x ocorrer
- criei um dispachador de eventos
O que me chamou mais a atenção das referencias que eu tive foi como é feito o fluxo:
- Dominio cria o evento
- Depachador de eventos da camada de aplicação é injetado na UOW da camada de infra
- Toda vez que a infra commita uma escrita, a UOW olha no contexto do banco de dados da operação que esta sendo realizada e procura os dominios de evento
- se possui eventos, commit chama os handlers cadastrados
Achei um rolê enorme, sinto que alguma coisa vai ficar com pontas soltas futuramente, principalmente como os eventos são criados hoje no dominio e como eles sãos disparados na UOW, mas segui em frente com testes de integração, não precisei fazer uma nova fixture para o teste de integração do usecase de criaçaõ de parceiro, porém, descobri que o Nsubstitute é um cara incrivel ao lidar com logs, pois utilizei o mock para verificar se o evento foi chamado e fiquei embasbacado, com isso, acredito que no momento, finalizado as camadas do serviço de manuseio de parceiros para a POC, com certeza terei que voltar aqui para fazer algumas coisas principalmente referente à criação de evento para adicionar as áreas de cobertura e o endereço que serão inseridos dentro do serviço de geolocalização.