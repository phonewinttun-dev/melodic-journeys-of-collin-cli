# 🏴‍☠️ Melodic Journeys Of Collin

A retro-styled, pixel-inspired music experience themed after **Monkey D. Luffy**. This application reimagines a modern music player through the lens of 90s aesthetics and vibrant colors, built on the power of **Blazor WebAssembly**.

![Project Logo](BlazorWasm.MelodicJourneysOfCollin/wwwroot/favicon.svg)

## 🌟 Key Features

### 🎮 Retro UI/UX Overhaul
- **Sidebar-Centric Layout**: A fixed left-hand navigation sidebar for an immersive, console-like feel.
- **Chunky Esthetics**: Sharp borders, heavy offset shadows, and pixelated typography inspired by classic "2D" interfaces.
- **Glassmorphism & Texture**: Subtle micro-textures and glass-like panels that blend retro vibes with modern depth.

### 👒 Luffy Theme
- **Custom Color Palette**: Specifically curated HSL variables representing the iconic Straw Hat captain—Vibrant Red (`--accent`), Sand/Straw (`--bg`), and Deep Shadow tones.
- **Dynamic Theming**: Support for both Light and Dark modes, each carefully adjusted to maintain the retro-pixel aesthetic.

### 💿 Immersive Music Experience
- **Retro Vinyl Player**: A dedicated home page component featuring a spinning record, animated tonearm, and pixelated progress bars.
- **DJ Deck Integration**: Custom-built DJ Deck icons and a dedicated "DJ Room" route for a full-screen mixing experience.
- **Global Audio Host**: Persistent playback across navigation, ensuring your journey never misses a beat.

## 🛠️ Technology Stack

- **Framework**: [Blazor WebAssembly](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) (.NET 8/9+)
- **Styling**: [Tailwind CSS v4](https://tailwindcss.com/) with native HSL variable integration.
- **Typography**: 
  - `VT323`: Pixel-art display font for headings and UI markers.
  - `DM Sans`: Modern sans-serif for high readability in content.
- **State Management**: Service-based architecture for `PlayerState`, `LibraryState`, and `ThemeService`.

## 🚀 Getting Started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (Version 8.0 or later)
- [Node.js](https://nodejs.org/) (For Tailwind CSS build processes)

### Running Locally
1. Clone the repository.
2. Navigate to the project directory:
   ```bash
   cd BlazorWasm.MelodicJourneysOfCollin
   ```
3. Restore dependencies and run:
   ```bash
   dotnet watch run
   ```

## 🎨 Visual Identity
The project uses a custom-designed **DJ Deck SVG** as its primary logo, symbolizing the fusion of classic music culture and modern web tech.

---
*Built with ❤️ and a bit of Pirate Spirit.*
