# GameTimeX

![Status](https://img.shields.io/badge/status-deprecated-red)

> ⚠️ **Project Status**  
> GameTimeX is no longer actively developed and is considered **deprecated**.  
>  
> It has been succeeded by **GameTimeNext**, a modern reimplementation with improved architecture, new features, and ongoing development.  
> 👉 https://github.com/MaxPra/GameTimeNext  
>
> 🔄 **Migration Recommended**  
> If you are using GameTimeX, it is strongly recommended to switch to GameTimeNext.  
> The new application includes a **built-in migration system** that allows you to import your existing data on first launch.

---

GameTimeX is a desktop application for precise tracking of actual gameplay time.  
Unlike platform counters that may continue running while a title is idle, GameTimeX records playtime only when the game is truly active.

**Version 2.1.0**  
Same reliable core, new refinements and yearly playtime statistics in December.

Download the latest installer:  
[GameTimeXSetup_Latest.msi](https://github.com/MaxPra/GameTimeX/releases/download/Version3.0.0/GameTimeXSetup_Latest.msi)

---

## Key Features

- 🎮 **Accurate Time Tracking** – records only while the game process is genuinely running.  
- 🎛️ **Installed Games Filter** *(controller + play icon)* – optional toolbar filter that shows only profiles for games that are currently installed (installation folder exists).  
  *Prerequisite:* the installation folder must be specified in each profile’s **Properties**.
- 🏷️ **Installed Icon Overlay** – each profile tile shows an indicator icon in the **top-right corner** of the profile image if the game is installed  
  *(installation folder set + valid executable detected)*.
- 🔄 **Automatic Profile Switching** – switches to the correct profile when a game launches.  
  - Per-executable selection to prevent conflicts where different games share executables.  
  - Smart initial pre-filtering automatically deselects common non-game executables (launchers, crash reporters, helpers, etc.).
- 🖼️ **Customizable Profiles** – create, rename, and assign cropped images for each game.  
  - **New:** You can now paste profile pictures directly from the clipboard using **CTRL+V** in both the **Create** and **Properties** windows.
- 🌙 **OLED Care (Blackout Screen)** – toggle with **CTRL+B** to black out all displays while AFK.
- 🖥️ **Side-Screen Dimming** – optionally dims secondary displays automatically while tracking is active.
- ⌨️ **Hotkey Monitoring** – start/stop tracking via a configurable hotkey.
- 💾 **Backup & Restore** – export or import all profiles; optional automatic backup at startup.
- 🔒 **Hardened Database Access** – parameterized SQLite queries for protection against SQL injection.
- 🟦 **Steam Integration** – import Steam games and link them with GameTimeX profiles.  
  - Launch Steam games directly from GameTimeX  
  - Optionally enable **HDR automatically before game launch** (only if linked with a Steam profile)  
  - **Steam Start Parameters** – specify custom Steam launch arguments per game directly inside GameTimeX
- 🗓️ **Today’s Playtime in Tooltip** – in the details panel, the **total time tooltip** now also shows **how many hours you’ve played today**.
- 📊 **December Yearly Statistics** – every December, GameTimeX shows a **summary of how long you played each game during the current year**.  
  This feature is always active and requires no configuration.
- 🎨 **UI Style Refinements** – subtle visual improvements for a cleaner and more consistent experience.

---

## What’s New in 2.1.0

- 📊 **December Yearly Stats** – in December, GameTimeX now displays a **clear overview of your total playtime per game for the entire current year**.  
- 🎨 **UI Style Refinements** – minor improvements in visuals, spacing, and consistency across the app.  
- 🖼️ **Profile picture pasting** – profile images can now be pasted directly from the clipboard using **CTRL+V** in both the **Create** and **Properties** windows.

---

## Overview

GameTimeX is designed for reliability and clarity, with an emphasis on clean interaction and accurate tracking.

### Main Window

Initial application view:  
<img width="1191" height="786" alt="image" src="https://github.com/user-attachments/assets/79ce0126-9752-4b1f-a071-052e46dfc2bf" />

**Key areas:**

1. **Profile Search** – quickly locate profiles in larger libraries.  
2. **Toolbar Controls** – next to the search field:  
   - **Installed Games Filter** *(controller + play icon)* – shows only profiles where the game is currently installed (installation folder exists).  
     *Prerequisite:* define the installation folder in **Properties**.  
   - Create New Profile  
   - Delete Profile  
   - Properties  
   - Settings  
   (Properties and Delete are also available via the profile context menu.)
3. **Profile Tiles with Install Indicator** – each game profile shows its image; if the game is installed, a **small install icon appears in the top-right corner**.  
   Example of a profile tile with the install icon:  
   <img width="191" height="219" alt="image" src="https://github.com/user-attachments/assets/0bdc29b4-df16-4bd4-b04e-bce4052f83dc" />
4. **Information Panel** – details for the selected profile, including thumbnail.  
5. **Start/Stop Monitoring** – status indicated by button color (blue = idle, red = active).  
6. **Rename Profile** – click the pencil icon next to the profile name.

Example with profiles:  
<img width="1188" height="787" alt="image" src="https://github.com/user-attachments/assets/ff8e56e5-81e5-4298-bbd0-1bfe558742dd" />

Profile names and images are editable at any time. Hover over the “i” icon next to *Game Time* to view the tracked minutes, as well as the first and last play sessions.  
➡️ The tooltip now also shows **how many hours you played today**.  
➡️ **New in 2.1.0:** in December, an **automatic yearly summary** shows how much time you spent in each game throughout the current year.  
➡️ **New:** profile pictures can now be pasted directly from the clipboard using **CTRL+V** in the **Create** and **Properties** windows.

If a new playthrough begins after time has already been recorded, use **New Playthrough Startpoint**. The application saves the current total time and reports playthrough time as the difference from that point forward.  
- **Total time** remains cumulative.  
- **Playthrough time** is calculated relative to the saved start point.

---

## Properties & Steam Linking

The **Properties window** provides central configuration for each profile:  

<img width="544" height="398" alt="image" src="https://github.com/user-attachments/assets/bd1e4c1b-1184-4611-b7f5-ab5ad2e170c8" />

- Define the **installation folder** for installed game detection  
- Assign profile image, rename profile, and manage monitored executables  
- **Paste from Clipboard:** profile images can now be inserted directly from the clipboard via **CTRL+V**.  
- **Steam Integration:**  
  - Shows whether the profile is already linked to a Steam game  
  - A dedicated **Steam button with link icon** allows importing and linking Steam games directly  
  - When linked, Steam games can be launched straight from GameTimeX  
  - Optional: enable **HDR activation before launch**  
  - **Steam Start Parameters:** specify custom Steam launch arguments per game profile  
  - **SteamGridDB Shortcut:**  
    - Via the **SteamGridDB button** (available during profile creation or in the Properties window), SteamGridDB is opened directly in your browser with the **correct game already selected**.  
    - Simply copy the desired artwork from SteamGridDB, paste it into the image textbox, and you’re done.

<img width="998" height="518" alt="image" src="https://github.com/user-attachments/assets/5199bb9d-323a-49f8-a6ea-ef795690ac08" />

---

## Settings

The settings dialog provides granular control:  
<img width="524" height="722" alt="image" src="https://github.com/user-attachments/assets/79e8355a-8bc2-476d-b8be-bc6981896653" />
<img width="524" height="720" alt="image" src="https://github.com/user-attachments/assets/3b4331bd-781e-4a6e-9db2-34b4e7597b49" />

**Options include:**

- **Session Time Tracking** – display the current session duration in the information panel while tracking.
- **Automatic Profile Switching** – automatically switches profiles upon game launch (requires the installation folder).  
  - Manage the set of **executables** to monitor and exclude irrelevant processes.
- **Monitor Key** – assign a global hotkey to start/stop monitoring without leaving the game.
- **Backup & Restore** – export or import profiles; the application restarts automatically after import/export. Optional automatic backup on startup.
- **OLED Care (Blackout Screen)** – fully darkens all screens to protect OLED panels (CTRL+B).
- **Side-Screen Dimming (optional)** – dims all secondary displays **only while tracking**; automatically restores them afterwards.
- **Steam Integration Options** – choose whether HDR should be toggled automatically when launching a Steam-linked game, and configure **per-game Steam launch parameters**.

---

## Manage Executables

When Automatic Profile Switching is enabled, GameTimeX allows precise control over which executables are monitored.  
This prevents launchers, helpers, and engine processes from causing false detections.

<img width="549" height="492" alt="image" src="https://github.com/user-attachments/assets/df729e02-8336-47e9-ae6b-cfe9cc786643" />

**Capabilities:**

- Enumerates executables in the installation directory  
- Checkboxes to mark executables as *active* or *inactive*  
- “Select All” for rapid bulk changes  
- Smart defaults: common non-game executables are deselected on initial profile creation  
- Changes can be saved at any time

---

## Profile Image Cropping

When adding a profile image, a cropping dialog enables quick framing:  
<img width="788" height="491" alt="image" src="https://github.com/user-attachments/assets/b1b1ac7e-77e5-421c-b940-37a9cfab78a5" />

**Controls:**

- Resize the crop area with the mouse wheel  
- Reposition the crop area via drag (left mouse button)
- **New:** profile images can also be pasted directly from the clipboard using **CTRL+V** in the **Create** and **Properties** windows.

The cropped image is saved and immediately shown in the profile view.

---

## Security

- All database operations use parameterized SQLite commands to mitigate SQL injection risks.  
- Initial executable pre-filtering reduces noise from non-game processes, improving detection accuracy.

---

*Icon assets by Icons8.*
