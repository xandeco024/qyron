# 🎮 QYRON - Beat 'em Up Game

## Visão Geral do Projeto

**Tipo:** Beat 'em up 2.5D (movimento em X e Z, visual 2D)
**Engine:** Unity 2022+ com URP
**Linguagem:** C#
**Multiplayer:** Local até 4 jogadores
**Input:** New Unity Input System

---

## 📁 Estrutura de Pastas

```
Assets/
├── Scripts/
│   ├── characters/
│   │   ├── Character.cs              # Classe base para TODOS os personagens
│   │   ├── PlayableCharacters/
│   │   │   ├── PlayableCharacter.cs  # Jogador controlável (herda Character)
│   │   │   └── LobbyPlayer.cs        # Seleção de personagem no lobby
│   │   └── Enemies/
│   │       ├── Enemy.cs              # Inimigo base (herda Character)
│   │       ├── Pigeon.cs             # Inimigo específico
│   │       ├── Behaviours/           # State Machine dos inimigos (StateMachineBehaviour)
│   │       │   ├── EnemyIdleState.cs
│   │       │   ├── EnemyFreeWalkState.cs
│   │       │   ├── EnemyFollowingState.cs
│   │       │   ├── EnemyJoiningState.cs
│   │       │   ├── EnemyAttackState.cs
│   │       │   ├── EnemyWaitingCDState.cs
│   │       │   ├── EnemyDamageState.cs
│   │       │   ├── EnemyGrabbedState.cs
│   │       │   └── EnemyDeadState.cs
│   │       └── Pets/
│   │           └── Pet.cs
│   │
│   ├── Managers/
│   │   ├── GameManager.cs            # Lista de players, tempo do jogo
│   │   ├── LevelManager.cs           # Segmentos, progressão, spawn
│   │   ├── Segment.cs                # Arena individual com waves
│   │   ├── CameraManager.cs          # Câmera dinâmica multiplayer
│   │   ├── PauseManager.cs           # Pause/resume
│   │   ├── GameOverManager.cs        # Game over, vidas
│   │   ├── LobbyManager.cs           # Seleção de personagens
│   │   ├── MainInputManager.cs       # Input System centralizado
│   │   └── UI/
│   │       └── HUDManager.cs         # HUD para 4 players
│   │
│   ├── Enviroment/
│   │   ├── Parallax.cs               # Efeito parallax
│   │   ├── Fade.cs                   # Fade in/out
│   │   └── TransparencyTrigger.cs    # Objetos transparentes
│   │
│   └── Items/
│       ├── Item.cs                   # Item coletável
│       └── ItemData.cs               # ScriptableObject de item
│
├── Scenes/
│   ├── Menu.unity
│   └── Level 1.unity
│
├── Sprites/
│   ├── Characters/
│   │   ├── PlayableCharacters/       # Qyron, Meowcello, Gark, Nyx
│   │   └── Enemies/
│   │       ├── Pigeons/
│   │       └── Rats/
│   └── UI/
│
├── Prefab/                           # Prefabs do jogo
├── Audio/                            # SFX e música
├── InputSys/                         # Input Actions
└── ScriptableObjects/                # Dados de personagens, itens
```

---

## 🏗️ Hierarquia de Classes

### Characters (Personagens)

```
MonoBehaviour
└── Character (CLASSE BASE)
    ├── Stats: health, damage, speed, resistance
    ├── Combat: TakeDamage(), attack boxes
    ├── Stun system
    ├── Knockback
    ├── Grab system (grabbable)
    │
    ├── PlayableCharacter (JOGADOR)
    │   ├── Movement: WASD, jump, dash
    │   ├── Combat: Light (L), Heavy (H), Grab (G)
    │   ├── Combos: "LLL", "HHH", "LLH"
    │   ├── Progression: XP, level, coins
    │   ├── Downed/Revive system
    │   └── Input callbacks (OnMove, OnJump, etc.)
    │
    └── Enemy (INIMIGO)
        ├── AI: target acquisition, attack range
        ├── State Machine (Animator-based)
        ├── Death & loot distribution
        ├── Join system (spawn off-screen)
        │
        └── Pigeon, Rat, etc. (específicos)
```

### Managers (Gerenciadores)

```
GameManager          → Lista de players, tempo in-game
    ↓
LevelManager         → Segmentos, spawn, camera limits
    ↓
Segment              → Waves de inimigos, bounds, progressão
    ↓
CameraManager        → Follow multiplayer, FOV dinâmico, shake

HUDManager           → UI de 4 players (HP, XP, coins, combo)
PauseManager         → Time.timeScale, pause UI
GameOverManager      → Lives, restart segment
LobbyManager         → Character selection, ready state
```

### Data (ScriptableObjects)

```
CharacterData        → Stats base (HP, damage, speed)
PlayableCharacterData → Sprites, cores do dash
ItemData             → Drop info, efeitos
```

---

## ⚔️ Sistema de Combate

### Ataques
- **Light (L):** Rápido, baixo dano
- **Heavy (H):** Lento, alto dano, screen shake
- **Grab (G):** Agarra inimigo, permite throw

### Combos Válidos
```csharp
"LLL" → Light combo finisher (damage +50%)
"HHH" → Heavy combo finisher (damage +100%)
"LLH" → Mixed combo (knockback +100%)
```

### Fluxo de Dano
```
Ataque → OverlapBox → TakeDamage() → Knockback → Stun → Flash Red
                                   ↓
                           DamageText spawn
                                   ↓
                           Screen shake (heavy)
```

---

## 🎬 Sistema de Segmentos/Waves

### Estrutura
```
Level
└── Segment 1 (Arena)
    ├── Wave 1: [Pigeon x3]
    ├── Wave 2: [Pigeon x2, Rat x1] (unlock: 2 mortes)
    └── Wave 3: [Boss] (unlock: todas mortes)
    → COMPLETE → desbloqueia caminho
└── Segment 2
    └── ...
```

### EnemySpawn Config
```csharp
[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab;
    public Vector3 position;
    public int wave;              // Qual wave esse inimigo spawna
    public bool join;             // Se spawna fora e anda até joinPosition
    public Vector3 joinPosition;
}
```

---

## 🎥 Sistema de Câmera

### Single Player
- Câmera segue com offset
- Smooth follow via Cinemachine

### Multiplayer
- Rastreia ponto médio entre players
- FOV aumenta quando players se afastam
- Movement restrictions quando FOV máximo
- Camera limits por segmento

---

## 🔧 Padrões Atuais (A MELHORAR)

### ⚠️ FindObjectOfType (EVITAR)
```csharp
// ATUAL (ruim - lento)
gameManager = FindObjectOfType<GameManager>();

// FUTURO (bom - singleton)
gameManager = GameManager.Instance;
```

### ⚠️ Magic Strings (EVITAR)
```csharp
// ATUAL (ruim - typos)
combo.Add("L");
if (direction == "left")

// FUTURO (bom - enums)
combo.Add(AttackType.Light);
if (direction == Direction.Left)
```

### ⚠️ Classes Grandes
- `PlayableCharacter.cs` tem ~968 linhas
- Deveria ser dividido em componentes menores

---

## 📝 Instruções de Edição

### Ao criar novos scripts:

1. **Managers:** Usar Singleton pattern
```csharp
public class NewManager : MonoBehaviour
{
    public static NewManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
```

2. **Novos personagens:** Herdar de `Character`
```csharp
public class NewEnemy : Enemy
{
    // Comportamento específico
}
```

3. **Novos estados de AI:** Criar `StateMachineBehaviour`
```csharp
public class EnemyNewState : StateMachineBehaviour
{
    private Enemy enemy;

    public override void OnStateEnter(Animator animator, ...)
    {
        enemy = animator.GetComponent<Enemy>();
    }
}
```

4. **Dados configuráveis:** Usar ScriptableObject
```csharp
[CreateAssetMenu(fileName = "NewData", menuName = "Game/New Data")]
public class NewData : ScriptableObject
{
    public int value;
}
```

### Ao editar scripts existentes:

1. **Cachear FindObjectOfType** no Awake/Start, NUNCA no Update
2. **Usar constantes** em vez de magic numbers
3. **Adicionar null checks** em referências críticas
4. **Manter compatibilidade** com sistema existente
5. **Testar multiplayer** (até 4 players)

---

## 🎮 Personagens Jogáveis

| Nome | Tipo | Características |
|------|------|-----------------|
| Qyron | Main | Balanceado |
| Meowcello | Support | ? |
| Gark | Tank | ? |
| Nyx | ? | ? |

---

## 🐦 Inimigos

| Nome | Arquivo | Comportamento |
|------|---------|---------------|
| Pigeon | `Pigeon.cs` | Inimigo básico |
| Rat | ? | ? |
| Trantolona | ? | Rat especial? |

---

## 📊 Fluxo do Jogo

```
Menu
  ↓
Lobby (seleção de personagens)
  ↓ [Todos ready]
Level 1
  ↓
Segment 1 → Waves → Complete
  ↓
Segment 2 → Waves → Complete
  ↓
...
  ↓
Level Complete / Game Over
```

---

## 🔌 Input System

### Action Maps
- **Lobby:** Seleção de personagem
- **Player:** Gameplay

### Actions Principais
```
OnMove(Vector2)      → Movimento WASD
OnJump()             → Pulo/Double jump
OnDash()             → Dash com trail
OnLightAttack()      → Ataque leve
OnHeavyAttack()      → Ataque pesado
OnGrab()             → Agarrar
OnPause()            → Pause menu
```

---

## 🐛 Problemas Conhecidos

1. **Performance:** `FindObjectOfType` usado demais
2. **Coupling:** Managers muito acoplados
3. **God Class:** PlayableCharacter muito grande
4. **Magic Strings:** Combos, directions como strings
5. **No Pooling:** Instantiate/Destroy frequente
6. **Update loops:** HUD atualiza todo frame

---

## 📋 TODO Técnico (Prioridade)

### 🔴 Alta
- [ ] Implementar Singleton nos managers
- [ ] Criar enums para AttackType, Direction
- [ ] Object Pooling para DamageText
- [ ] Refatorar PlayableCharacter

### 🟡 Média
- [ ] Event System para desacoplamento
- [ ] Interfaces (IDamageable, etc.)
- [ ] Mover magic numbers para config

### 🟢 Baixa
- [ ] Save/Load system
- [ ] Audio Manager
- [ ] UI Toolkit migration

---

## 🛠️ Comandos Úteis

### Git
```bash
# Ver último commit
git log -1 --oneline

# Desfazer último commit (mantém mudanças)
git reset --soft HEAD~1

# Desfazer completamente
git reset --hard HEAD~1
```

### Unity
- **Play Mode:** Ctrl+P
- **Pause:** Ctrl+Shift+P
- **Build:** Ctrl+B

---

## 📚 Arquivos Importantes

| Arquivo | Descrição |
|---------|-----------|
| `Character.cs` | Base de TODOS os personagens |
| `PlayableCharacter.cs` | Player controller (968 linhas!) |
| `Enemy.cs` | Base dos inimigos |
| `LevelManager.cs` | Controle de segmentos |
| `Segment.cs` | Waves e spawn |
| `CameraManager.cs` | Câmera multiplayer |
| `HUDManager.cs` | UI dos players |
| `ANALISE_E_RECOMENDACOES.md` | Análise completa do código |

---

## 💬 Contexto para o Claude

Este é um projeto pessoal de beat 'em up desenvolvido há ~2 anos. O código é funcional mas precisa de refatoração para:
- Melhor performance
- Código mais manutenível
- Padrões modernos de Unity

O desenvolvedor tem conhecimento intermediário de Unity/C# e está aberto a sugestões de melhoria.

**Idioma preferido:** Português brasileiro
**Estilo de código:** Comentários podem ser em PT-BR
**Prioridade:** Funcionalidade > Perfeição

---

## 🎯 Próximos Passos Sugeridos

1. Migrar para UI Toolkit (HUD)
2. Implementar Singletons
3. Criar sistema de eventos
4. Object pooling
5. Refatorar classes grandes
