# Contexto do Projeto

- **Engine:** Unity 2D (C#)
- **Plataforma:** Exclusivo para Computador (PC).
- **Estilo:** Jogo híbrido com perspectivas Top-Down e Plataforma 2D.
- **Objetivo da IA:** Atuar como um Engenheiro de Software Sênior especialista em Unity, sugerindo código limpo, otimizado e focado em componentização.

## Arquitetura e Padrões de Projeto

- **Inputs:** O jogo utilizará EXCLUSIVAMENTE Mouse e Teclado. Não gere códigos de input para Mobile (Touch) ou Gamepads/Joysticks.
- **State Machine (Máquina de Estados):** Utilizaremos este padrão extensivamente para gerenciar a troca de perspectivas do jogador (ex: `TopDownState` vs `PlatformerState`).
- **Observer / Events:** Use `System.Action` ou UnityEvents para comunicação entre sistemas (ex: UI, sistema de vida, pontuação), evitando acoplamento direto.
- **Singletons:** Usar com extrema cautela, apenas para Managers globais (ex: `GameManager`, `AudioManager`).
- **Scriptable Objects:** Priorize o uso de Scriptable Objects para armazenar dados de configuração (status de inimigos, inventário, itens).

## Regras de Código (C#)

1. **Nomenclatura:**
   - `PascalCase` para Classes, Namespaces e Métodos.
   - `camelCase` para variáveis locais e parâmetros.
   - `_camelCase` para campos privados (`private int _health;`).
   - `UPPER_SNAKE_CASE` para constantes.
2. **Boas Práticas Unity:**
   - **NUNCA** use `GameObject.Find()`, `FindObjectOfType()` ou `GetComponent()` dentro dos métodos `Update()`, `FixedUpdate()` ou `LateUpdate()`. Faça o cache dessas referências no `Awake()` ou `Start()`.
   - Use `[SerializeField]` para expor variáveis privadas no Inspector, em vez de torná-las `public`.
   - Para física 2D, sempre use `FixedUpdate()`. Mova o personagem usando `Rigidbody2D` em vez de alterar o `transform.position` diretamente para evitar problemas de colisão.

## Diretrizes Específicas do Jogo (Top-Down vs Plataforma)

- **Gerenciamento de Física:** O jogo alterna entre Top-Down e Plataforma. Ao gerar código de movimentação, verifique o estado atual.
  - *No estado Plataforma:* O `Rigidbody2D` deve ter `Gravity Scale > 0`. A movimentação foca no eixo X e o pulo aplica força no eixo Y.
  - *No estado Top-Down:* O `Rigidbody2D` deve ter `Gravity Scale = 0`. A movimentação ocorre livremente nos eixos X e Y.
- **Colisões:** O código deve levar em conta que os *colliders* podem precisar se comportar como *Triggers* dependendo da perspectiva atual.

## Estrutura de Diretórios Esperada

Sempre sugira a criação de scripts dentro da seguinte estrutura lógica:
/Assets
  /Scripts
    /Core        # Managers, Game Loop
    /Player      # Movimentação, Inputs, Estados do Jogador
    /Enemies     # IA de inimigos, Comportamentos
    /Environment # Interações com o cenário, Portas, Checkpoints
    /UI          # Menus, HUD

## Padrão de Commits

Ao sugerir comandos do Git, utilize estritamente este padrão de mensagens:

- **feat**: Adiciona nova funcionalidade.
- **fix**: Corrige um bug.
- **docs**: Altera apenas documentação.
- **test**: Adiciona/altera testes automatizados.
- **build**: Modifica configurações de build ou dependências.
- **perf**: Melhora a performance.
- **style**: Ajusta formatação visual do código (lint, espaços).
- **refactor**: Reestrutura o código sem alterar funcionalidade.
- **chore**: Atualiza configurações ou tarefas estruturais.
- **ci**: Altera scripts de Integração Contínua.
- **raw**: Modifica arquivos de dados ou configurações base.
- **cleanup**: Limpa código (remove comentários, trechos mortos).
- **remove**: Exclui arquivos ou funcionalidades obsoletas.
