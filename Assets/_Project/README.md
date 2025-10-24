# CVA - Vampire Survivors Style Monster Defense Game

## Project Overview

CVA is a **complete, playable Vampire Survivors-style 2D survival game** built with Unity 6 and Universal Render Pipeline (URP). Players must survive infinite waves of enemies while collecting XP, leveling up, and choosing powerful upgrades. The game features auto-aiming weapons, data-driven design with ScriptableObjects, and exponential difficulty scaling through an infinite wave loop system.

### Game Genre
- **Type**: Wave-based survival / Bullet heaven
- **Gameplay**: Infinite survival with roguelike progression
- **Inspiration**: Vampire Survivors, Magic Survival

## Technical Stack

- **Unity Version**: Unity 6000.2.8f1 (Unity 6)
- **Render Pipeline**: Universal Render Pipeline (URP) 17.2.0
- **Input System**: New Input System (1.14.2)
- **Target Platform**: PC (expandable to mobile)
- **Language**: C# (.NET)
- **Development Philosophy**: Data-driven design, component-based architecture, performance-first

## Implemented Features

### Combat System ⚔️
The game features a complete auto-aim weapon system with 6 distinct weapon types:

1. **ProjectileWeapon** - Auto-aiming bullet projectiles with spread patterns
2. **LaserWeapon** - Continuous beam weapon that damages over time
3. **ExplosionWeapon** - Area-of-effect explosion damage
4. **GroundAOEWeapon** - Ground-based area damage zones
5. **LightningWeapon** - Chain lightning that jumps between enemies
6. **OrbitWeapon / OrbitingWeapon** - Rotating weapons that orbit the player

**Key Features:**
- All weapons auto-target the nearest enemy
- Auto-fire system (no player input required)
- Pierce, bounce, and chain mechanics for projectiles
- Weapon unlocks through powerup system
- Data-driven configuration via WeaponData ScriptableObjects

### Enemy AI System 💀
Complete enemy system with chase AI and multiple enemy types:

**Enemy Types:**
- **Enemy_Basic** - Standard balanced enemy
- **Enemy_Weak** - Low health, fast spawning
- **Enemy_Tank** - High health, slow movement
- **Enemy_Elite_Fast** - Fast, dangerous enemy
- **Enemy_Elite_Bruiser** - Heavy damage dealer

**Enemy Features:**
- Chase AI that follows the player
- Dynamic health bars
- XP orb drops on death
- Configurable stats via EnemyData ScriptableObjects
- Contact damage to player

### Progression System ⭐
Complete XP and leveling system with Vampire Survivors-style powerup selection:

**XP System:**
- Collectible XP orbs with 3 rarity levels (Common, Uncommon, Rare)
- Magnetic collection radius (increased by Magnet powerup)
- Level-up XP curve with scaling requirements
- XP bar UI with smooth progress visualization

**Level-Up System:**
- Game pauses on level-up (Time.timeScale = 0)
- 3 random powerup choices presented
- Cannot get duplicate stat powerups in same selection
- Resume on selection

### Powerup System 🎮
15+ powerup types across multiple categories:

**Stat Boosts:**
- Damage Boost (+20%)
- Fire Rate Boost (+25%)
- Speed Boost (+20%)
- Health Boost (+20 max health)
- Magnet (increased XP collection radius)

**Projectile Modifiers:**
- Additional Projectiles (+1 projectile per shot)
- Pierce (+1 enemy pierce)
- Bounce (+1 bounce)
- Chain (+1 chain target for lightning)

**Weapon Unlocks:**
- Unlock Explosion Weapon
- Unlock Laser Weapon
- Unlock Lightning Weapon
- Unlock Ground AOE Weapon
- Unlock Orbiting Weapon

**PowerupManager Features:**
- Tracks all active powerups and stat multipliers
- Applies stacking bonuses
- Manages weapon unlocks
- Prevents duplicate powerup choices in selection UI

### Wave Management System 🌊
Infinite wave-based survival system with exponential difficulty scaling:

**Wave System:**
- 3 distinct wave configurations (Wave_01, Wave_02, Wave_03)
- Each wave has duration and enemy composition
- Automatic progression through waves
- Visual wave start notifications

**Loop System:**
- After completing Wave_03, loops back to Wave_01 with increased difficulty
- Each loop increases:
  - Enemy health multiplier
  - Enemy damage multiplier
  - Enemy speed multiplier
- Exponential difficulty scaling for infinite gameplay

**WaveManager Features:**
- Controls enemy spawning through EnemySpawner
- Tracks current wave and loop count
- Displays wave information to player
- Data-driven configuration via WaveData ScriptableObjects

### UI Systems 📊
Complete UI implementation with multiple screens:

**HUD (In-Game):**
- **HealthBar** - Player health visualization
- **XPBar** - Experience progress with smooth fill
- **PlayerStatsUI** - Real-time display of:
  - Current wave/loop
  - Kill count
  - Survival time
  - Level
- **DamageNumber** - Floating damage numbers with fade effect

**Menus:**
- **PowerupSelectionUI** - Level-up powerup choice screen (3 options)
- **GameOverUI** - Game over screen with final stats:
  - Total kills
  - Survival time
  - Waves survived
  - Level reached
  - Restart option

**Debug Tools:**
- **GameOverDebugHelper** - On-screen debug UI
- **ForceGameOverTest** - Test game over screen instantly

### Visual Effects System ✨
Complete VFX system managed by VFXManager:

**VFX Types:**
- Enemy death effects
- Player level-up effects
- Hit impact effects
- Damage flash on player hit
- Camera shake on damage

**Features:**
- Centralized VFX spawning
- Configurable effect prefabs
- Particle system integration

### Statistics Tracking 📈
GameStatsManager tracks all player stats:

**Tracked Stats:**
- Total kills
- Survival time (formatted display)
- Current wave number
- Current loop number
- Player level
- Events for stat updates

### Core Systems 🔧
**Camera System:**
- **CameraFollow** - Smooth camera following player
- **CameraShake** - Screen shake effects on damage

**Object Pooling:**
- **IPoolable** - Interface for poolable objects
- Used for projectiles, XP orbs, and effects

**Input System:**
- New Input System integration
- WASD movement controls
- Customizable input actions

## Game Mechanics

### Player Controls
- **Movement**: WASD keys
- **Weapons**: Auto-aim, auto-fire (no player input needed)
- **XP Collection**: Automatic when in range (improved by Magnet powerup)

### Core Loop
1. Player moves with WASD to avoid enemies
2. Weapons automatically target and attack nearest enemies
3. Defeated enemies drop XP orbs
4. Collect XP to fill experience bar
5. On level-up, choose from 3 random powerups
6. Survive waves of increasing difficulty
7. After Wave 03, loop restarts with harder enemies
8. Survive as long as possible

### Victory Condition
- Survive as long as possible (infinite survival)
- No win condition - test your endurance!

### Difficulty Scaling
- Waves have timed durations
- After completing all 3 waves, loop counter increases
- Each loop multiplies enemy stats:
  - Health increases exponentially
  - Damage increases exponentially
  - Speed increases exponentially

## Project Structure

This project follows Unity best practices with a clean, organized folder structure:

### `/Scripts/` - C# Code Organization

All scripts use the root namespace `CVA` with subnamespaces for each category.

#### `/Scripts/Core/` - Core Systems
**Namespace**: `CVA.Core`

**Implemented Scripts:**
- `GameStatsManager.cs` - Singleton tracking kills, survival time, waves, loops
- `WaveManager.cs` - Manages wave progression and infinite loop system
- `ExperienceManager.cs` - Handles XP collection, level-ups, and XP curve
- `VFXManager.cs` - Centralized visual effects spawning and management
- `CameraFollow.cs` - Smooth camera following with configurable speed
- `CameraShake.cs` - Screen shake effects for damage and impacts
- `XPOrb.cs` - XP pickup behavior with magnetic collection
- `IPoolable.cs` - Interface for object pooling implementation

#### `/Scripts/Player/` - Player Systems
**Namespace**: `CVA.Player`

**Implemented Scripts:**
- `PlayerController.cs` - WASD movement with powerup speed multipliers
- `PlayerHealth.cs` - Health system with damage flash, invincibility frames, and death
- `PowerupManager.cs` - Manages all powerups, stat multipliers, and weapon unlocks
- `WeaponManager.cs` - Manages multiple active weapons on player

#### `/Scripts/Enemies/` - Enemy Systems
**Namespace**: `CVA.Enemies`

**Implemented Scripts:**
- `Enemy.cs` - Complete enemy with chase AI, damage, health, and XP drops
- `EnemySpawner.cs` - Spawns enemies based on wave configuration
- `EnemyHealthBar.cs` - Dynamic health bar rendering above enemies

#### `/Scripts/Combat/` - Weapon Systems
**Namespace**: `CVA.Combat`

**Implemented Scripts:**
- `ProjectileWeapon.cs` - Auto-aiming projectile weapon with spread patterns
- `LaserWeapon.cs` - Continuous beam weapon with damage over time
- `ExplosionWeapon.cs` - Area-of-effect explosion damage
- `GroundAOEWeapon.cs` - Ground-based area damage zones
- `LightningWeapon.cs` - Chain lightning attack system
- `OrbitWeapon.cs` / `OrbitingWeapon.cs` - Rotating orbital weapons
- `Projectile.cs` - Base projectile with pierce, bounce, and chain mechanics

#### `/Scripts/UI/` - User Interface
**Namespace**: `CVA.UI`

**Implemented Scripts:**
- `HealthBar.cs` - Player health bar with smooth fill
- `XPBar.cs` - Experience progress bar with smooth fill
- `PlayerStatsUI.cs` - Real-time display of wave, kills, time, level
- `DamageNumber.cs` - Floating damage numbers with fade animation
- `PowerupSelectionUI.cs` - Level-up powerup choice screen (3 options)
- `PowerupChoiceButton.cs` - Individual powerup choice button
- `GameOverUI.cs` - Game over screen with final statistics
- `GameOverDebugHelper.cs` - On-screen debug UI overlay
- `ForceGameOverTest.cs` - Debug tool to test game over screen

#### `/Scripts/Data/` - ScriptableObjects
**Namespace**: `CVA.Data`

**Implemented Scripts:**
- `EnemyData.cs` - Enemy configuration (health, speed, damage, XP value)
- `WeaponData.cs` - Weapon properties (damage, fire rate, projectile count, etc.)
- `PowerupData.cs` - Powerup definitions (stat modifiers, weapon unlocks)
- `WaveData.cs` - Wave composition (enemy types, spawn rates, duration)
- `XPOrbData.cs` - XP orb configuration (XP value, rarity, visual)

#### `/Scripts/Editor/` - Editor Tools
**Namespace**: `CVA.Editor`

**Implemented Scripts:**
- `CompleteGameSetup.cs` - Editor tool for complete game setup
- `QuickSetup.cs` - Fast setup utility for new scenes
- `GameOverSetupChecker.cs` - Validates game over UI configuration

### `/Prefabs/` - Reusable Game Objects

#### `/Prefabs/Player/`
- **Player.prefab** - Complete player with:
  - PlayerController (movement)
  - PlayerHealth (health system)
  - PowerupManager (powerup management)
  - WeaponManager (weapon management)
  - Sprite renderer and collider

#### `/Prefabs/Enemies/`
- **Enemy_Basic.prefab** - Standard enemy with:
  - Enemy component (AI, health, damage)
  - EnemyHealthBar (dynamic health display)
  - Sprite renderer and collider

#### `/Prefabs/Weapons/`
- **Bullet.prefab** - Projectile weapon projectile
- **Weapon_Explosion.prefab** - Explosion weapon prefab
- **Weapon_Orbit.prefab** - Orbiting weapon prefab

#### `/Prefabs/xp/`
- **XPOrb.prefab** - Experience pickup with magnetic collection

### `/Data/` - ScriptableObject Assets

#### Enemy Data (5 Types)
- **Enemy_Basic.asset** - Standard balanced enemy
- **Enemy_Weak.asset** - Low health, fast spawning
- **Enemy_Tank.asset** - High health, slow movement
- **Enemy_Elite_Fast.asset** - Fast, dangerous enemy
- **Enemy_Elite_Bruiser.asset** - Heavy damage dealer

#### Weapon Data (3 Weapons)
- **Weapon_Basic.asset** - Starting projectile weapon
- **Weapon_Explosion.asset** - Explosion weapon configuration
- **Weapon_Orbit.asset** - Orbiting weapon configuration

#### Powerup Data (15+ Powerups)
**Stat Boosts:**
- Powerup_Damage.asset
- Powerup_FireRate.asset
- Powerup_Speed.asset
- Powerup_Health.asset
- Powerup_Magnet.asset

**Projectile Modifiers:**
- Powerup_AdditionalProjectile.asset
- Powerup_Pierce.asset
- Powerup_Bounce.asset
- Powerup_Chain.asset

**Weapon Unlocks:**
- Powerup_UnlockExplosion.asset
- Powerup_UnlockLaser.asset
- Powerup_UnlockLightning.asset
- Powerup_UnlockGroundAOE.asset
- Powerup_UnlockOrbiting.asset

#### Wave Data (3 Waves)
- **Wave_01.asset** - First wave configuration
- **Wave_02.asset** - Second wave configuration
- **Wave_03.asset** - Third wave configuration

#### XP Orb Data (3 Rarities)
- **XPOrb_Common.asset** - Standard XP value
- **XPOrb_Uncommon.asset** - Medium XP value
- **XPOrb_Rare.asset** - High XP value

## Coding Standards

### Namespace Convention
All scripts MUST use proper namespaces following this pattern:

```csharp
namespace CVA.CategoryName
{
    public class ExampleScript : MonoBehaviour
    {
        // Implementation
    }
}
```

### Naming Conventions
- **Classes/Structs**: PascalCase (e.g., `PlayerController`, `EnemyHealth`)
- **Methods**: PascalCase (e.g., `TakeDamage()`, `SpawnEnemy()`)
- **Public Fields/Properties**: PascalCase (e.g., `MaxHealth`, `MoveSpeed`)
- **Private Fields**: camelCase with underscore prefix (e.g., `_currentHealth`, `_rigidbody`)
- **Constants**: UPPER_SNAKE_CASE (e.g., `MAX_ENEMIES`, `SPAWN_RATE`)
- **Interfaces**: PascalCase with 'I' prefix (e.g., `IDamageable`, `IPoolable`)

### Script Template Example

```csharp
using UnityEngine;

namespace CVA.Player
{
    /// <summary>
    /// Controls player movement and input handling.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _acceleration = 10f;
        #endregion

        #region Private Fields
        private Rigidbody2D _rigidbody;
        private Vector2 _moveInput;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Update logic
        }

        private void FixedUpdate()
        {
            // Physics logic
        }
        #endregion

        #region Public Methods
        public void Move(Vector2 direction)
        {
            // Implementation
        }
        #endregion

        #region Private Methods
        private void HandleInput()
        {
            // Implementation
        }
        #endregion
    }
}
```

## Architecture Patterns Used

- **Data-Driven Design**: Heavy use of ScriptableObjects for game configuration and balancing
- **Component-Based Architecture**: Small, focused MonoBehaviour components
- **Singleton Managers**: Core systems accessible globally (GameStatsManager, ExperienceManager, PowerupManager, VFXManager)
- **Event System**: UnityEvents for XP collection, level-ups, wave changes, and stat updates
- **Interface-Based Pooling**: IPoolable interface for reusable object pools
- **Auto-Aim System**: All weapons automatically target nearest enemy
- **Pause System**: Time.timeScale manipulation for level-up pause

## Performance Best Practices

1. **Object Pooling**: Use IPoolable interface for frequently spawned objects (projectiles, XP orbs, VFX)
2. **Cache References**: Component references cached in Awake/Start, never in Update
3. **Avoid FindObject**: Direct references through managers and serialized fields
4. **Layer-Based Collision**: Collision layers optimize physics checks between player, enemies, projectiles
5. **Auto-Aim Optimization**: Target finding cached and updated periodically, not every frame
6. **Sprite Atlases**: Sprites organized in atlases to reduce draw calls
7. **Data-Driven Balance**: ScriptableObjects allow balancing without code changes

## Input System Setup

The project uses Unity's New Input System. Input actions are defined in:
- `/Assets/InputSystem_Actions.inputactions`

### Key Bindings (Default)
- **Movement**: WASD / Arrow Keys
- **Pause**: ESC (if pause system implemented)
- **Weapons**: Auto-fire (no input needed)

## Getting Started

### Quick Start
1. Open the project in **Unity 6000.2.8f1**
2. Navigate to `/Assets/Scenes/` and open **CVA.unity**
3. Press **Play** in the Unity Editor
4. Use **WASD** to move your character
5. Weapons automatically fire at nearest enemies
6. Collect **XP orbs** to level up
7. Choose powerups on level-up to become stronger
8. Survive waves and loops as long as possible!

### Scene Structure
**CVA.unity** - Single scene containing:
- Player
- Camera with CameraFollow and CameraShake
- Canvas with all UI elements
- Managers (GameStatsManager, ExperienceManager, WaveManager, VFXManager)
- EnemySpawner

### Development Workflow
1. **Modify ScriptableObjects**: Adjust game balance in `/Assets/_Project/Data/`
2. **Test Changes**: Use Play Mode to test immediately
3. **Create New Powerups**: Duplicate existing PowerupData assets
4. **Add New Enemies**: Create EnemyData assets and configure stats
5. **Adjust Waves**: Modify WaveData assets for different wave compositions
6. **Profile Performance**: Use Unity Profiler to identify bottlenecks

### Editor Tools
- **Window > CVA > Complete Game Setup** - Full game setup wizard
- **Window > CVA > Quick Setup** - Fast scene setup
- **Window > CVA > Game Over Setup Checker** - Validate UI configuration

## Git Workflow

### Important Files to Commit
- All scripts (.cs files)
- Prefabs (.prefab files)
- Scenes (.unity files)
- ScriptableObjects (.asset files)
- Input Action assets (.inputactions files)
- Project settings

### Files to Ignore
- Library/
- Temp/
- Obj/
- Build/
- Logs/
- UserSettings/
- *.csproj
- *.sln

A proper .gitignore file should already be in the project root.

## Resources

### Unity Official Documentation
- [Unity 6 Documentation](https://docs.unity3d.com/)
- [URP Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
- [New Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)
- [ScriptableObjects Guide](https://docs.unity3d.com/Manual/class-ScriptableObject.html)

### Game Design References
- Vampire Survivors - Core gameplay inspiration
- Magic Survival - Mobile roguelike survival
- 20 Minutes Till Dawn - Auto-aim survival shooter

## Team Notes

- Always test changes before committing
- Follow the coding standards outlined above
- Document complex systems with XML comments
- Use TODO comments for incomplete features
- Balance changes via ScriptableObjects, not code
- Test on target hardware regularly

---

## Project Status

**Last Updated**: 2025-10-23
**Project Phase**: Playable Prototype - Core Systems Complete
**Unity Version**: Unity 6000.2.8f1
**URP Version**: 17.2.0
**Primary Developer**: CVA Team

### Completion Status
✅ **Complete Systems:**
- Player movement and controls
- 6 weapon types with auto-aim
- 5 enemy types with chase AI
- XP and leveling system
- 15+ powerup system
- Infinite wave system with loops
- UI (HUD, level-up, game over)
- VFX and camera effects
- Statistics tracking
- Data-driven configuration

### Future Enhancements
- Additional weapon types
- More enemy variants
- Boss enemies
- Audio system (music and SFX)
- Particle effects polish
- Main menu
- Pause menu
- Settings menu
- Save/load system
- Achievements
- Mobile platform support

---

**Ready to play! Press Play in CVA.unity and survive as long as you can!** 🎮
