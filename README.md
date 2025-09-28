# GameTimeX

GameTimeX is a desktop application for precise tracking of actual gameplay time.  
Unlike platform counters that may continue running while a title is idle, GameTimeX records playtime only when the game is truly active.

**Version 2.1.0**  
Same reliable core, new refinements and December playtime statistics.

Download the latest installer:  
[GameTimeXSetup_Latest.msi](https://github.com/MaxPra/GameTimeX/releases/download/Version2.1.0/GameTimeXSetup_Latest.msi)

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
- 📊 **December Statistics** – during December, GameTimeX shows a **overview of which games you played/recorded and for how long**.
- 🎨 **UI Style Refinements** – subtle visual improvements for a cleaner and more consistent experience.

---

## What’s New in 2.1.0

- 📊 **December Stats** – a new feature that displays a **summary of your recorded playtime per game in December**, giving you a clear end-of-year overview.  
- 🎨 **UI Style Refinements** – minor improvements in visuals, spacing, and consistency across the app.

---

## Overview

GameTimeX is designed for reliability and clarity, with an emphasis on clean interaction and accurate tracking.

### Main Window

Initial application view:  
<img width="1218" height="814" alt="image" src="https://github.com/user-attachments/assets/fe5d1687-a3ec-428a-9501-d4cb20dca6b1" />

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
<img width="1220" height="820" alt="image" src="https://github.com/user-attachments/assets/fbb43f6a-3d25-46f3-906a-4e8979a26190" />

Profile names and images are editable at any time. Hover over the “i” icon next to *Game Time* to view the tracked minutes, as well as the first and last play sessions.  
➡️ The tooltip now also shows **how many hours you played today**.  
➡️ **New in 2.1.0:** during December, you also get a **statistical overview of your playtime by game**.

If a new playthrough begins after time has already been recorded, use **New Playthrough Startpoint**. The application saves the current total time and reports playthrough time as the difference from that point forward.  
- **Total time** remains cumulative.  
- **Playthrough time** is calculated relative to the saved start point.

---

## Properties & Steam Linking

The **Properties window** provides central configuration for each profile:  

<img width="573" height="434" alt="image" src="https://github.com/user-attachments/assets/d81642ce-315e-4cf5-a76b-9487e4aec0c4" />

- Define the **installation folder** for installed game detection  
- Assign profile image, rename profile, and manage monitored executables  
- **Steam Integration:**  
  - Shows whether the profile is already linked to a Steam game  
  - A dedicated **Steam button with link icon** allows importing and linking Steam games directly  
  - When linked, Steam games can be launched straight from GameTimeX  
  - Optional: enable **HDR activation before launch**  
  - **Steam Start Parameters:** specify custom Steam launch arguments per game profile

---

## Settings

The settings dialog provides granular control:  
<img width="529" height="730" alt="image" src="https://github.com/user-attachments/assets/f4e36339-03aa-4b14-a7a9-5d5bbab2a9c9" />
<img width="533" height="729" alt="image" src="https://github.com/user-attachments/assets/a99536a0-5d8a-4dd9-996d-a1e753bba252" />

**Options include:**

- **Session Time Tracking** – display the current session duration in the information panel while tracking.
- **Automatic Profile Switching** – automatically switches profiles upon game launch (requires the installation folder).  
  - Manage the set of **executables** to monitor and exclude irrelevant processes.
- **Monitor Key** – assign a global hotkey to start/stop monitoring without leaving the game.
- **Backup & Restore** – export or import profiles; the application restarts automatically after import/export. Optional automatic backup on startup.
- **OLED Care (Blackout Screen)** – fully darkens all screens to protect OLED panels (CTRL+B).
- **Side-Screen Dimming (optional)** – dims all secondary displays **only while tracking**; automatically restores them afterwards.
- **Steam Integration Options** – choose whether HDR should be toggled automatically when launching a Steam-linked game, and configure **per-game Steam launch parameters**.
- **New in 2.1.0:** **December Statistics** toggle and configuration – view and enable the monthly playtime overview.

*Tip:* For Steam titles, locate the installation folder via **Steam → Game → Properties → Installed Files → Browse**.

---

## Manage Executables

When Automatic Profile Switching is enabled, GameTimeX allows precise control over which executables are monitored.  
This prevents launchers, helpers, and engine processes from causing false detections.

<img width="554" height="494" alt="image" src="https://github.com/user-attachments/assets/920efc3b-2073-4004-bc3e-d52988125172" />

**Capabilities:**

- Enumerates executables in the installation directory  
- Checkboxes to mark executables as *active* or *inactive*  
- “Select All” for rapid bulk changes  
- Smart defaults: common non-game executables are deselected on initial profile creation  
- Changes can be saved at any time

---

## Profile Image Cropping

When adding a profile image, a cropping dialog enables quick framing:  
<img width="1220" height="813" alt="image" src="https://github.com/user-attachments/assets/5c0a23a5-8b2b-44ff-a4c9-8ba0debfd7d2" />

**Controls:**

- Resize the crop area with the mouse wheel  
- Reposition the crop area via drag (left mouse button)

The cropped image is saved and immediately shown in the profile view.

---

## Security

- All database operations use parameterized SQLite commands to mitigate SQL injection risks.  
- Initial executable pre-filtering reduces noise from non-game processes, improving detection accuracy.

---

*Icon assets by Icons8.*
