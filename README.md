# Escopo do Projeto: Zé Delivery Challenge

Projeto de portfólio baseado no [desafio técnico do Zé Delivery](https://github.com/ab-inbev-ze-company/ze-code-challenges/blob/master/backend_pt.md). O foco é aplicar conceitos de backend, geolocalização e boas práticas de arquitetura.

## Regras de Ouro

### 1. Proibido o uso de IA
IA é ferramenta de produtividade, não muleta. Todo o código, lógica e estrutura devem ser fruto de estudo e implementação manual. O objetivo é o aprendizado real e a capacidade de explicar cada linha escrita.

### 2. Documentação do Processo ("Ponta a Ponta")
Código limpo documenta o "como", mas não o "porquê". Este projeto exige o registro das decisões:
- Por que escolhi esta tecnologia/biblioteca?
- Qual foi o maior desafio técnico e como contornei?
- Quais alternativas foram descartadas?

---

## Objetivos Técnicos
- **Geolocalização:** Manipulação de GeoJSON (Point e MultiPolygon).
- **Persistência:** Busca eficiente por proximidade e contenção (localização dentro de área).
- **Arquitetura:** Separação clara entre rota, lógica de negócio e banco de dados.
- **Testes:** Garantir que o cálculo de distância e a busca por PDV funcionem conforme o contrato.

## Entregáveis Mentais e Práticos
- [ ] API funcional com os endpoints: `POST /pdv`, `GET /pdv/:id` e `GET /pdv/search`.
- [ ] Arquivo `LOG.md` ou similar registrando a evolução (ex: "Dia 1: Modelagem do banco", "Dia 2: Lógica de localização").
- [ ] README explicativo de como rodar o projeto rapidamente.

## Stack Sugerida (A definir pelo dev)
- **Linguagem:** [Sua escolha]
- **Banco:** [Sua escolha - Recomendado um com suporte a GIS]
- **Ferramenta de Testes:** [Sua escolha]

---