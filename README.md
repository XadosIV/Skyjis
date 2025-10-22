# Skyjis (Prototype)

A personal 2D Unity prototype originally developed in 2022 to learn the fundamentals of game development and the Unity engine.  
The project started as an attempt to create a Metroidvania-style game but was later refocused into a self-contained prototype showcasing movement, combat, and interaction systems.

This release includes various bug fixes and small quality-of-life improvements to make the experience smoother and more polished.

## ⚙️ Requirements / Prérequis
- Unity **2021.3.15f1 (LTS)**  
- Unity **Universal Render Pipeline (URP)** package installed  
- Windows / macOS compatible (tested on AZERTY keyboard)  

> Without URP, the project may fail to load or display graphics incorrectly.

---

## 🎮 Controls
> ⚠️ Designed for an **AZERTY keyboard layout** — not tested on QWERTY.

### Movement
- **Arrow Keys** — Move  
- **Down Arrow** — Crouch  
- **Space** — Jump / Double Jump  
- **Shift** — Dash  
- **Ctrl** — Teleport  

### Combat
- **X** — Basic Attack  
- **F** — Parry  
- **F (Hold)** — Absorb  

### Spells
- **Q** — Red Spell  
- **S** — Green Spell  
- **D** — Blue Spell  
- **A (Hold)** — Open Spell Selection Menu  
  - **Arrow Keys** — Navigate Spell Grid  
  - **Q / S / D** — Assign Selected Spell  

### Interaction
- **Up Arrow** — Interact  

### Debug
- **Escape** — Reload Scene  

---

## 💫 Absorb Mechanic
Enemies can be dealt with in two ways:

1. **Defeat** — Reduce their health to zero.  
   - Rewards: Gold and mana.  
2. **Pacify** — Parry an enemy to stun them, then **hold F to Absorb** their dark essence.  
   - The enemy becomes pacified (friendly).  
   - Pacified enemies remain friendly even after reloading the scene.  
   - They continuously generate mana when the player is nearby.  

---

## 🧩 About the Project
This prototype was developed as a personal learning project to explore:
- Unity’s 2D tools and physics  
- Player movement and combat systems  
- Basic AI behavior  
- Scene management and data persistence  

It represents an early stage of my learning process and experimentation with game design and programming principles.

---

## 📦 Technical Info
- Built with **Unity 2021.3.15f1 (LTS)**  
- Coded entirely in C#  
- Standalone prototype — no further updates planned  

---

## ✨ Author
Developed by **Xad'**  
Feel free to reach out or check out my other projects!
