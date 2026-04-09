# Encourage:The Power of HYPE
**Genre:** Typing / Rhythm Arcade


## 1. Game Overview

**EncourageMan** is a fast-paced arcade typing game.The goal is to survive while keeping hype high enough to push the crushers back.

## 2. Core Design Loop

```
Type a letter correctly  →  Hype goes UP slightly
Complete a full word     →  Hype goes UP more, crushers push back briefly
Type a wrong letter      →  Hype goes DOWN, streak breaks
Do nothing               →  Hype decays passively over time
```

## 4. Architecture Overview

The project follows a **hub-and-spoke** architecture with `StageManager` at the center.

```
                        ┌─────────────────┐
                        │  StageManager   │  ← Master state machine
                        │  (Orchestrator) │     Rules → Countdown → Active → Win/Lose
                        └────────┬────────┘
                                 │ wires events in WireEvents()
          ┌──────────────────────┼──────────────────────┐
          │                      │                       │
   ┌──────▼──────┐      ┌────────▼──────┐      ┌────────▼────────┐
   │   Typing    │      │  HypeMeter    │      │   Crusher       │
   │  Controller │─────▶│  Controller  │      │  Controller     │
   └──────┬──────┘      └────────┬──────┘      │  (Left/Right)   │
          │                      │              └────────┬────────┘
    events│              events  │                       │
    fire  │              fire    │                       │
          ▼                      ▼                       ▼
   WordDisplayUI          HypeMeterUI            EncourageMan
   StreakUI                                       Controller
   KeyboardDisplayUI
   GameAudioController
```

## 10. UI Systems

### HypeMeterUI

Driven entirely by events. Updates a `Slider` value on `onNormalizedHypeChanged` and updates the fill color + state label on `onStateChanged.

### WordDisplayUI

Renders the current word with per-letter color coding (green = typed, white = pending). Wrong key flashes the entire word red for `flashDuration` seconds via a countdown timer in `Update()`.

### KeyboardDisplayUI

Generates a full QWERTY keyboard UI at runtime. Monitors `Input.inputString` in `Update()` and flashes the matching key.

### CountdownUI

Uses a Coroutine (`PlayCountdown`) with nested `ShowStep` Coroutines to display 3, 2, 1, HYPE! with a scale animation.

### StreakUI

Subscribes to `onStreakChanged`. Converts raw streak count to `(streak - 1)x` display. Hides itself entirely when streak is 0 or 1.

## 11. Data Flow Diagram

```
┌──────────────────────────────────────────────────────────────────────┐
│                        USER KEYBOARD INPUT                           │
└─────────────────────────────┬────────────────────────────────────────┘
                              │
                    ┌─────────▼──────────┐
                    │  TypingController  │
                    │                    │
              
      │  - Letter matching │
                    │  - Word selection  │
                    │  - Streak tracking │
                    └──┬──────┬──────┬───┘
                       │      │      │
         onCorrectLetter│      │      │onWrongKey
         onWordCompleted│      │      │onStreakChanged
                        │      │      │
          ┌─────────────▼──┐   │  ┌───▼──────────────┐
          │ HypeMeter      │   │  │ GameAudio         │
          │ Controller     │   │  │ Controller        │
          │                │   │  │                   │
          │ + gain/penalty │   │  │ - correct sounds  │
          │ - passive decay│   │  │ - wrong sounds    │
          └──────┬─────────┘   │  │ - streak sounds   │
                 │             │  │ - heckler (timed) │
        onStateChanged│        │  └───────────────────┘
        onNormalizedHype│      │
        onHypeDepleted  │      │onWordCompleted
                 │      │      │
    ┌────────────▼─┐  ┌─▼──────▼──────────┐   ┌──────────────┐
    │ HypeMeterUI  │  │ CrusherController  │   │ EncourageMan │
    │              │  │ (Left & Right)     │   │ Controller   │
    │ - Slider     │  │                    │   │              │
    │ - Color      │  │ - Move per hype    │   │ - Animator   │
    │ - State text │  │ - Word burst       │   │ - Ragdoll    │
    └──────────────┘  │ - Death detection  │   └──────────────┘
                      └────────┬───────────┘
                               │onDeathZoneReached
                               │
                    ┌──────────▼──────────┐
                    │    StageManager     │
                    │    (State Machine)  │
                    │                    │
                    │  Rules             │
                    │    → Countdown     │
                    │      → Active      │
                    │        → Win/Lose  │
                    └─────────────────────┘
```

## 13. TODO

### Gameplay
- [ ] Add a score system (words typed, streak multiplier, time survived)
- [ ] Add multiple stages or a stage select screen
- [ ] Add a difficulty selector (Easy / Normal / Hard) that swaps `HypeConfig` presets

### EncourageMan
- [ ] Add a camera shake or screen effect on death

### Audio
- [ ] Add a background music track that changes intensity based on hype state
- [ ] Consider a countdown voice-over (3, 2, 1, HYPE!)

### UI / UX
- [ ] Add a combo-break visual feedback (screen flash or shake on wrong key at high streak)
- [ ] Show total words typed and accuracy % on the result panel
- [ ] Add a high score / personal best display on the result panel
- [ ] Mobile: replace keyboard input with on-screen tap targets

### Polish & Juice
- [ ] Add particle effects on word completion burst
- [ ] Add crusher impact effects when they slam shut on lose

### Technical
- [ ] Implement save/load for high scores using `PlayerPrefs` or a JSON save file
