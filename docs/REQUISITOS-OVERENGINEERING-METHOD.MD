## Requisitos funcionais

### 1. Criar um parceiro
O parceiro deverá ter um endereço fixo e uma área de cobertura.

#### 1.1 Endereço
O endereço deve ser um GeoJSON do tipo `Point`.

#### 1.2 Coverage Area
O `coverageArea` deve ser um GeoJSON do tipo `MultiPolygon`, para desenhar o raio de alcance/cobertura.

#### 1.3 Documento único
O documento do parceiro deve ser único.

#### 1.4 ID único
O `id` do parceiro deve ser único.

### 2. Buscar parceiro por ID
Deve ser possível buscar um parceiro a partir do seu `id` único.

### 3. Buscar parceiro mais próximo
Com base na localização de quem está consumindo a API, deve ser retornado o parceiro mais próximo.

## Requisitos não-funcionais

Por conta de ser um teste técnico / criação de projeto para portfólio, vamos declarar algumas heurísticas específicas para o desenvolvimento do projeto.

- o projeto vai ser uma **POC** para comprovar e testar conhecimentos e todo o escopo de desenvolvimento conhecido pelos desenvolvedores do projeto;
- para o tamanho/escopo do projeto, sim, vai ter **overengineering**;
- o projeto será separado em serviços menores, onde teremos:
  - uma API para manuseio do parceiro e todas as entidades englobadas;
  - uma API para lidar com os objetos de geolocalização de cada parceiro;
- os serviços serão cercados por um **API Gateway** para controlar o fluxo;
- o gateway irá controlar o fluxo de solicitações do usuário, adicionando:
  - **rate-limiting** por rota;
  - **circuit breaker** em caso de indisponibilidade de algum dos serviços;
  - **CORS** para liberação controlada durante a POC;
  - **obs:** daria pra adicionar autenticação facilmente também;
- o serviço de manuseio do parceiro deve conter todas as definições de **CRUD** de parceiro e suas respectivas entidades;
- o serviço de geolocalização será responsável **apenas pela busca geográfica**;
- dados de entrada para escrita virão **somente** via serviço de manuseio de parceiros;
- ambos os serviços serão desenvolvidos em **.NET**;
- para armazenamento dos parceiros, será utilizado **PostgreSQL**;
- para armazenamento das geolocalizações, será utilizado **MongoDB**, por conta do seu suporte nativo a geolocalização;
- o serviço de geolocalização vai contar com estratégia de **cache aside** usando **Redis**;
- a comunicação entre serviços será feita via fila, utilizando **RabbitMQ** como barramento;
- por tratar-se de uma POC, o desenvolvimento será guiado inicialmente pelas funções funcionais descritas como **MVP**;
- para criação de contextos e escopos de negócio, o desenvolvimento será guiado por **DDD**;
- o desenvolvimento será criado a partir de fluxo **TDD**;
- deverão ser criados testes respeitando a pirâmide de testes:
  - `unit`
  - `integration`
  - `e2e`
- será utilizado **CQRS (Command Query Responsibility Segregation)** na camada de aplicação de ambos os serviços;
- cada requisição de entrada terá um **correlation-id** para identificação de fluxos;
- o escopo do projeto deverá ser **event driven**;
- será utilizado **Saga Pattern** para manter a consistência dos dados;
- será utilizado **Outbox Pattern** para lidar com casos de falha de integração na consistência/publicação dos dados;
- o **cache aside** deve ser invalidado quando um evento do parceiro ocorrer, seja uma deleção ou uma alteração;
- o **TTL do cache** deve ser de **20 minutos**.

## Resumo da ideia arquitetural

Pra resumir bem a brincadeira:

- escrita acontece no serviço de parceiros;
- leitura geográfica acontece no serviço de geolocalização;
- sincronização entre serviços acontece via eventos;
- consistência vai ser tratada com **Saga + Outbox**;
- performance de leitura geográfica vai ser ajudada por **Redis**;
- e sim, tem overengineering, mas de propósito, porque a ideia aqui é provar conhecimento e não só entregar o mínimo.

## Observações finais

Esse projeto não quer necessariamente ser a solução mais simples possível pro problema, e tudo bem.

A ideia aqui é montar uma POC com cara de arquitetura mais robusta, justamente pra demonstrar conhecimento em:

- microserviços;
- mensageria;
- consistência eventual;
- DDD;
- CQRS;
- TDD;
- cache;
- observabilidade básica com correlation-id;
- e modelagem geoespacial.