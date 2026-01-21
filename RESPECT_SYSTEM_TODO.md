# 💀 Sistema de RESPECT (Intimidação)

## 📋 Status: NÃO IMPLEMENTADO
**Prioridade:** Média
**Requer:** Animação de fuga, novo state na State Machine

---

## 🎯 Conceito

Quando um player com **alto Respect** mata um inimigo, os inimigos próximos podem **fugir com medo** ao invés de continuar lutando, dando XP e coins automaticamente.

---

## 📊 Valores de Respect dos Personagens

| Personagem | Respect | Descrição |
|------------|---------|-----------|
| **Gark** | 6 | O INTIMIDADOR - Especialista em assustar |
| **Qyron** | 3 | Intimidação média |
| **Nyx** | 3 | Intimidação média |
| **Meowcello** | 2 | Tank mas não assusta muito |

---

## 🎮 Como Funciona

```
Player mata Enemy A (com Respect alto)
         ↓
Inimigos em X metros VÊM a morte
         ↓
Rolam check: Player Respect vs Enemy Courage
         ↓
    Sucesso?
    ├─ SIM → Inimigo FOGE (dropa coins/XP e some)
    └─ NÃO → Inimigo continua lutando (talvez assustado)
```

---

## 💻 Implementação Sugerida

### 1. Adicionar no Enemy.cs

```csharp
[Header("Fear System")]
[SerializeField] protected float fearRadius = 10f;
[SerializeField] protected float courage = 5f; // Resistência ao medo
[SerializeField] protected bool isFleeing = false;
public bool IsFleeing { get => isFleeing; }
```

### 2. No método Die() do Enemy

```csharp
public IEnumerator Die(int deathTime = 0)
{
    // ... código existente ...

    // Antes de destruir, checa fear nos inimigos próximos
    PlayableCharacter killer = GetPlayerWhoKilledMe();
    if (killer != null && killer.Respect >= 3)
    {
        CheckNearbyEnemiesForFear(killer);
    }

    // ... resto do código ...
}
```

### 3. Novo método: CheckNearbyEnemiesForFear

```csharp
void CheckNearbyEnemiesForFear(PlayableCharacter intimidator)
{
    Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, fearRadius);

    foreach (var col in nearbyEnemies)
    {
        Enemy enemy = col.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead && !enemy.IsFleeing)
        {
            // Roll de medo: Respect do player × 10%
            float fearChance = intimidator.Respect * 10f;

            // Resistência do inimigo reduz a chance
            fearChance -= enemy.courage * 5f;

            if (Random.Range(0f, 100f) < fearChance)
            {
                enemy.Flee(intimidator);
            }
        }
    }
}
```

### 4. Novo método: Flee

```csharp
public void Flee(PlayableCharacter intimidator)
{
    isFleeing = true;
    animator.SetBool("fleeing", true);

    // Para de atacar
    target = null;

    // Corre na direção oposta
    Vector3 fleeDirection = (transform.position - intimidator.transform.position).normalized;

    StartCoroutine(FleeCoroutine(fleeDirection));
}

IEnumerator FleeCoroutine(Vector3 direction)
{
    float fleeTimer = 0f;
    float fleeDuration = 2f;

    while (fleeTimer < fleeDuration)
    {
        rb.linearVelocity = new Vector3(
            direction.x * moveSpeed * 1.5f,
            rb.linearVelocity.y,
            direction.z * moveSpeed * 1.5f
        );

        fleeTimer += Time.deltaTime;
        yield return null;
    }

    // Dropa loot e some
    DropLoot();
    GiveCoins(coinAmount);
    GiveXP(xpAmount);

    Destroy(gameObject);
}
```

---

## 🎪 Balanceamento Sugerido

| Parâmetro | Fórmula | Exemplo |
|-----------|---------|---------|
| **Fear Chance Base** | Respect × 10% | Gark (6) = 60% |
| **Redução por Courage** | Courage × 5% | Courage 5 = -25% |
| **Fear Radius** | 10 unidades | Alcance do efeito |
| **Flee Speed** | moveSpeed × 1.5 | Corre mais rápido |
| **Flee Duration** | 2 segundos | Tempo até sumir |

### Exemplos de Cálculo

```
Gark (Respect 6) mata Pombo (Courage 5):
- Fear Base: 6 × 10% = 60%
- Courage: 5 × 5% = 25%
- Fear Final: 60% - 25% = 35% de chance

Meowcello (Respect 2) mata Pombo (Courage 5):
- Fear Base: 2 × 10% = 20%
- Courage: 5 × 5% = 25%
- Fear Final: 20% - 25% = 0% (negativo = sem medo)
```

---

## 🎯 Requisitos para Implementar

### ✅ O Que Já Existe
- [x] Variável `Respect` nos personagens
- [x] Sistema de tracking de damage (para saber quem matou)
- [x] Sistema de loot/XP distribution

### ❌ O Que Precisa Criar
- [ ] Variável `courage` nos inimigos
- [ ] Variável `isFleeing` no Enemy
- [ ] Método `GetPlayerWhoKilledMe()` no Enemy
- [ ] Método `CheckNearbyEnemiesForFear()` no Enemy
- [ ] Método `Flee()` no Enemy
- [ ] Coroutine `FleeCoroutine()` no Enemy
- [ ] Novo state `EnemyFleeingState` (StateMachineBehaviour)
- [ ] Animação de fuga (correndo assustado)
- [ ] Bool "fleeing" no Animator
- [ ] Visual feedback (partículas de suor, ícone "!", etc.)

---

## 🎨 Melhorias Futuras

### Variações de Medo (Opcional)

#### Níveis de Intensidade
1. **Leve (20-40%)** - Inimigo fica hesitante
   - Ataca com menos frequência
   - Mantém distância maior

2. **Médio (40-60%)** - Inimigo fica defensivo
   - Recua frequentemente
   - Prioriza esquiva sobre ataque

3. **Alto (60%+)** - Inimigo FOGE
   - Corre e desaparece
   - Dropa loot imediatamente

#### Modificadores de Fear

```csharp
// Tipo de kill aumenta intimidação
if (lastHitWasHeavyAttack) fearChance += 10f;
if (lastHitWasCritical) fearChance += 15f;
if (lastHitWasComboFinisher) fearChance += 20f;

// Kill streak aumenta fear
int killStreak = player.GetKillStreak();
fearChance += killStreak * 5f;

// Bosses são imunes ao medo
if (enemy.IsBoss) return;
```

### Visual Feedback

```csharp
// Quando inimigo vê morte brutal
[Header("Fear Visual")]
[SerializeField] GameObject fearIcon; // "!" sobre a cabeça
[SerializeField] ParticleSystem sweatParticles;
[SerializeField] Color scaredTint = new Color(0.7f, 0.7f, 1f); // Tom azulado

void ShowFearReaction()
{
    Instantiate(fearIcon, transform.position + Vector3.up * 2f, Quaternion.identity);
    sweatParticles.Play();
    StartCoroutine(FlashColor(scaredTint, 0.5f));
}
```

### Sound Effects

```csharp
[Header("Fear Audio")]
[SerializeField] AudioClip[] fearScreams;
[SerializeField] AudioClip fleeSound;

void Flee(PlayableCharacter intimidator)
{
    // Grito de medo
    AudioSource.PlayClipAtPoint(
        fearScreams[Random.Range(0, fearScreams.Length)],
        transform.position
    );

    // Som de corrida
    AudioSource.PlayClipAtPoint(fleeSound, transform.position);

    // ... resto do código ...
}
```

---

## 🎪 Impacto no Gameplay

### Gark (Respect 6) - "O Terror"
- Mata um inimigo → outros fogem com 60% base de chance
- Se fizer critical kill → aumenta ainda mais
- Excelente para limpar waves rapidamente
- **Identidade:** Intimidador nato

### Meowcello (Respect 2) - "O Tanque Silencioso"
- Mata inimigo → 20% de chance base (muito baixo)
- Precisa de modificadores (heavy, critical) para assustar
- **Trade-off:** Bate forte mas não assusta
- **Identidade:** Força bruta sem intimidação

### Qyron/Nyx (Respect 3) - "Equilibrados"
- 30% de chance base
- Médio termo entre intimidação e dano

---

## 📝 Notas de Implementação

### Ordem Sugerida

1. **Fase 1 - Core System (2-3h)**
   - Adicionar variável `courage` nos inimigos
   - Criar método `Flee()` básico
   - Testar fuga simples (sem animação)

2. **Fase 2 - Integration (1-2h)**
   - Adicionar `CheckNearbyEnemiesForFear()`
   - Integrar com `Die()`
   - Balancear percentuais

3. **Fase 3 - Visual/Audio (2-3h)**
   - Criar animação de fuga
   - Adicionar state `EnemyFleeingState`
   - Partículas e sons

4. **Fase 4 - Polish (1-2h)**
   - Modificadores (critical, heavy, etc.)
   - Visual feedback
   - Balanceamento final

**Total estimado:** 6-10 horas de trabalho

---

## 🔗 Arquivos que Precisam Modificar

```
Assets/Scripts/characters/
├── Character.cs (adicionar getter para Respect)
├── Enemies/
│   └── Behaviours/
│       ├── Enemy.cs (adicionar todo o sistema)
│       └── EnemyFleeingState.cs (CRIAR NOVO)
├── PlayableCharacters/
│   └── PlayableCharacter.cs (tracking de kill streak)

Assets/Sprites/
└── Enemies/
    └── [Enemy]/
        └── flee_animation.png (CRIAR)

Assets/Audio/
└── SFX/
    └── Enemies/
        ├── fear_scream.wav (CRIAR)
        └── flee.wav (CRIAR)
```

---

## ⚠️ Avisos

- **Não implementar em bosses** - Eles devem ser imunes ao medo
- **Testar com 4 players** - Gark em multiplayer pode limpar tudo fácil demais
- **Balancear XP** - Fugir dá XP normal ou reduzido? (Sugestão: 80% do XP)
- **Animação placeholder** - Pode usar animação de walk invertida no início

---

## 📚 Referências de Jogos

**Jogos que usam sistema similar:**
- **Shadow of Mordor/War** - Nemesis System com fear
- **Far Cry** - Inimigos fogem quando aliados morrem
- **DOOM Eternal** - Demons ficam stunned quando veem glory kill

---

**Criado em:** 2025-01-21
**Última atualização:** 2025-01-21
