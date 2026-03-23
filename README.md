# 🌿 Cosmos - Connected Greenhouse Simulator

> A 3D serious game where you manage a smart greenhouse: grow plants, automate systems, and learn about connected agriculture.

![Godot 4.3](https://img.shields.io/badge/Godot-4.3-blue?logo=godotengine&logoColor=white)
![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)

---

## 📖 About

**Cosmos** is a 3D serious game built with **Godot 4.3 (C# / Mono)**. The player explores and manages a connected greenhouse in a first-person perspective. The goal is to buy seeds, plant them in pots, and configure the greenhouse's automated systems - watering, temperature, and lighting - to help the plants grow.

The game was designed as an educational tool to introduce players to the concepts behind **connected greenhouses** and **IoT-driven agriculture**.

## ✨ Features

- 🌱 **Plant Growth System** - Buy and plant seeds, then watch them grow through several stages depending on environmental conditions.
- 🛒 **Shop & Inventory** - Purchase seeds and materials from the in-game shop, manage your inventory, and sell your harvest.
- 💻 **Programmable Automation** - Use an in-game computer interface to schedule watering, adjust thermostat settings, and control shutters and lamps.
- 🔧 **Greenhouse Upgrades** - Improve your greenhouse equipment to unlock better growing conditions.
- 🌦️ **Dynamic Weather** - A weather and calendar system that influences plant growth day by day.
- 📚 **In-Game Encyclopedia & Guides** - Browse a built-in encyclopedia to learn about plants, materials, and how to play.
- 💾 **Save System** - Full save and load support so you can pick up right where you left off.
- ⚙️ **Settings** - Customizable key bindings and screen resolution, persisted across sessions.
- 🎓 **Tutorial** - A guided introduction to help new players get started.

## 🖼️ Tech Stack

| Component     | Technology                        |
|---------------|-----------------------------------|
| Engine        | Godot 4.3 (Forward+)             |
| Language      | C# (.NET / Mono)                  |
| 3D Rendering  | Godot Forward+ renderer           |
| Platform      | Windows (x86_64)                  |

## 🚀 Getting Started

### Prerequisites

- [Godot 4.3 Mono](https://godotengine.org/download/) (the C# version)
- [.NET SDK](https://dotnet.microsoft.com/download) (required for building C# projects in Godot)

### Running from Source

1. **Clone** the repository:
   ```bash
   git clone https://github.com/RaphaelBRENA/Connected-Greenhouse.git
   ```
2. Open the project in Godot by navigating to the `serre-connectee/` folder and opening `project.godot`.
3. Press **F5** (or click ▶️) to run the game.

### Building an Executable

The project includes an export preset for **Windows (x86_64)**. To export:

1. In Godot, go to **Project → Export…**
2. Select the **CosmosWindows** preset.
3. Click **Export Project** and choose your output directory.

## 🗂️ Project Structure

```
Connected-Greenhouse/
├── serre-connectee/           # Main Godot project
│   ├── Assets/                # Images, 3D models, fonts, animations
│   ├── Scenes/                # Godot scenes (.tscn)
│   │   ├── Menu/              # Start menu, save selection, settings
│   │   ├── Interface/         # 2D UI scenes (shop, inventory, upgrades…)
│   │   └── World.tscn         # Main 3D greenhouse world
│   ├── Scripts/               # C# game scripts
│   │   ├── Global.cs          # Autoloaded global state
│   │   ├── World.cs           # World logic & initialization
│   │   ├── Boutique/          # Shop system
│   │   ├── Inventaire/        # Inventory management
│   │   ├── Programmations/    # Automation scheduling
│   │   ├── Meteo/             # Weather system
│   │   └── …                  # Camera, interactions, menus, etc.
│   ├── Themes/                # UI themes
│   └── Save/                  # Save data
├── ConnectedGreenhouseReport.pdf   # Project report
└── LICENSE                    # MIT License
```

## 🎮 Controls

| Action           | Default Key |
|------------------|-------------|
| Move Forward     | W           |
| Move Backward    | S           |
| Move Left        | A           |
| Move Right       | D           |
| Interact         | F           |
| Open Inventory   | E           |
| Click / Select   | Left Mouse  |

> Controls can be remapped in the in-game settings menu.

## 🛣️ Possible Improvements

- Fix remaining bugs in the greenhouse upgrade interface
- Add more growth factors (nitrogen levels, oxygenation…)
- Add animated 3D models for shutters and lamps that reflect their programmed state

## 👥 Team

| Name              | Role        |
|-------------------|-------------|
| Raphaël BRENA     | Developer   |
| Marie CAVALLA     | Developer   |
| William RIBEIRO   | Developer   |
| Mathis RODIER     | Developer   |

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.