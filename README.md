# A 2D Platformer Prototype

> A challenging 2D platformer prototype built in Unity and C#, featuring a seamless world, dynamic traps, and a robust character controller. This project is a demonstration of core game development skills, from player physics to advanced scene management.

**This is a student project created for my portfolio. The game is a work in progress.**

**Gameplay :**

![Gameplay GIF](myGameGIf.gif)

---

## Table of Contents

- [About The Project](#about-the-project)
- [Key Features](#key-features)
- [Built With](#built-with)
- [Project Status](#project-status)
- [What I Learned](#what-i-learned)
- [Contact](#contact)

---

## About The Project

This project is a 2D platformer where the player must navigate a series of deadly levels connected seamlessly. The goal was to build a complete game system from the ground up, focusing on creating a robust and extendable foundation. The game currently features three fully playable levels that demonstrate the core mechanics.

The entire system is built around a persistent manager that streams levels in and out of the game world, eliminating loading screens and creating a continuous experience.

---

## Key Features

*   **Dynamic Character Controller:** A custom-built C# controller handles all player physics.
    *   Precise ground and platform detection.
    *   **Variable Jump Height:** Tap for a short hop, hold for a full jump.
    *   **Knockback System:** Player correctly reacts to damage sources like traps with a physics-based knockback.
*   **Seamless World:**
    *   An advanced **scene streaming system** loads the next level in the background and unloads the previous one.
    *   This results in zero loading screens between levels, allowing for a continuous, flowing world design.
*   **Interactive Traps & Obstacles:** All traps are scripted in C# for dynamic behavior.
    *   Swinging axes with customizable speed and arc.
    *   Grouped spike traps that activate in unison.
    *   Push walls that trigger based on multiple conditions (player proximity, object rotation, and player idle time).
*   **Robust Checkpoint System:**
    *   Saves the player's last triggered checkpoint, even across different scenes.
    *   On restart, the system intelligently reloads the specific scene containing the last checkpoint and places the player there.
*   **Cross-Platform Controls:**
    *   Supports PC controls via Keyboard.
    *   Supports Android controls via a custom on-screen UI with pointer events.

---

## Built With

*   [![Unity][Unity-Shield]][Unity-URL]
*   [![CSharp][CSharp-Shield]][CSharp-URL]

---

## Project Status

This project is a **playable prototype**. The core systems are in place and the first three levels are complete. Future plans include:

*   Designing more levels with increasing difficulty.
*   Adding new enemy types and trap mechanics.
*   Implementing a more complete UI and sound design.

---

## What I Learned

This project was a significant learning experience, allowing me to move beyond basic tutorials and solve complex, real-world development problems.

*   **Advanced Scene Management:** I learned how to implement a persistent manager scene to handle additive loading and unloading of game levels, creating a seamless world and managing game state effectively.
*   **Robust Input Handling:** I debugged complex, platform-specific input issues, learning the difference between Unity's old and new input systems and how to create reliable on-screen touch controls that work alongside keyboard input.
*   **Physics and Game Feel:** I spent a great deal of time fine-tuning the `Rigidbody2D` character controller to feel responsive. This included implementing variable jump height, debugging knockback forces that fought against player input, and handling interactions with moving platforms in a stable way.
*   **Coroutine-Based Events:** I learned to use Coroutines to create complex, sequential actions for traps and level events, which is far more efficient and manageable than using `Update()` with state flags and timers.

---

## Contact

EL ALLALI Achraf - [www.linkedin.com/in/achraf-el-allali] - [achrafelallali123@gmail.com]

Project Link: [https://github.com/H-raf0/Unity2DGamePlatformer](https://github.com/H-raf0/Unity2DGamePlatformer)


<!-- MARKDOWN LINKS & IMAGES -->
[Unity-Shield]: https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white
[Unity-URL]: https://unity.com/
[CSharp-Shield]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white
[CSharp-URL]: https://docs.microsoft.com/en-us/dotnet/csharp/