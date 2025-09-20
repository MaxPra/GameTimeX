# GameTimeX

GameTimeX is an application that enables you to accurately monitor your gaming time.  
If you are frustrated with Steam's inaccurate tracking (e.g., when a game is left idle while you are away from your keyboard), this tool will provide the solution you need.

**Version 2.0.11 / Retouched is now available!**  
Same core, brand-new look

Download the latest installer [here](https://github.com/MaxPra/GameTimeX/releases/download/Version2.0.11/GameTimeXSetup_Latest.msi).

---

## Key Features

- 🎮 **Accurate Time Tracking** – records only when your game is truly running.  
- 🔄 **Automatic Profile Switching** – instantly switch to the correct game profile when launching a game.  
  - Includes **per-executable selection** to avoid conflicts if multiple games use the same executables.  
  - Smart **auto-pre-filtering** deselects common non-game executables (launchers, crash reporters, helpers, etc.) on first profile creation.  
- 🖼️ **Customizable Profiles** – create, rename, and assign cropped images for each game.  
- 🌙 **OLED Care (Blackout Screen)** – toggle with **CTRL + B** to black out all displays while AFK.  
- 🖥️ **Side-Screen Dimming** – optional automatic dimming of secondary displays while tracking is active.  
- ⌨️ **Hotkey Monitoring** – assign a hotkey to start/stop tracking without leaving your game.  
- 💾 **Backup & Restore** – export or import all profiles, with optional auto-backup at startup.  
- 🔒 **Secure Database Access** – parameterized SQLite queries protect against SQL injection.  

---

## What’s new in 2.0.11

- Visual redesign of the UI
- Removed the list view
- Minor UI-related bug fixes
- **OLED Care (Blackout Screen)** – toggle at any time with **CTRL + B** to fully darken all screens while you’re AFK
- **Optional side-screen dimming while tracking** – when enabled in **Settings**, all non-primary displays are dimmed automatically while GameTimeX is actively tracking, and restored once tracking stops
- **Security: SQL-injection hardening** – database access now uses parameterized SQLite queries
- **Executable selection for Automatic Game Switch** – you can now define which executables should be monitored in the background for profile switching. This prevents conflicts when different games share identical executables.
- **Smart executable pre-filtering** – when adding a game for the first time, GameTimeX automatically deselects common non-game executables (e.g., launchers, crash reporters, helpers, benchmark tools). Only relevant executables remain selected, ensuring more accurate and reliable automatic detection.

---

## About This Application

GameTimeX is a personal project I developed as a hobby during my free time.

### Main Window Overview

When you launch the application for the first time, the main window will appear as shown below:  
<img width="1212" height="813" alt="image" src="https://github.com/user-attachments/assets/6eb7e027-c952-408c-bd3a-179b4de85ab4" />

**Main window sections:**

1. **Profile Search Bar** – Quickly find profiles when the list grows too long.  
2. **Profile Management Buttons** – Located next to the search bar:  
   - Create New Profile  
   - Delete Profile  
   - Properties  
   - Settings  
   *Tip: you can also access properties and delete a profile via the context menu.*  
3. **Information Section** – Displays details about the selected profile, including a thumbnail image.  
4. **Start/Stop Monitoring** – Located in the information section:  
   - A **blue** button means no active monitoring.  
   - A **red** button means monitoring is active.  
5. **Rename Profile** – Click the pencil icon next to the profile name to rename it.

After adding a profile, the view may look like this:  
<img width="1207" height="809" alt="image" src="https://github.com/user-attachments/assets/1627bb5e-f9a4-4c3e-be67-d2cea5a2abf1" />

Profile names and images can be adjusted at any time. Simply click the pencil icon or select *Change Image*.  
Hovering over the small “i” next to *Game Time* will show the tracked time in minutes, alongside the recorded first and last play sessions.

If you restart a game after already having recorded playtime, the *New Playthrough Startpoint* function can be used. It saves the current total time and calculates the new playtime as the difference from that point onward.  
- **Total time** is always cumulative.  
- **Playthrough-specific time** is calculated as the difference from the designated start point.

---

### Settings

The settings window gives you fine-grained control over how GameTimeX behaves:  
<img width="529" height="730" alt="image" src="https://github.com/user-attachments/assets/f4e36339-03aa-4b14-a7a9-5d5bbab2a9c9" />
<img width="533" height="729" alt="image" src="https://github.com/user-attachments/assets/a99536a0-5d8a-4dd9-996d-a1e753bba252" />

**Available options:**

- **Session Time Tracking** – Display the duration of the current session directly in the information panel while monitoring is active.  
- **Automatic Profile Switching** – If enabled, GameTimeX will automatically switch to the correct profile when a game is launched (requires the game’s installation folder to be specified).  
  - With the new update, you can also manage which **executables** should be monitored for automatic switching, and exclude irrelevant ones.  
- **Monitor Key** – Assign a hotkey to start and stop monitoring without leaving the game. Nearly all keyboard keys are supported.  
- **Backup & Restore** – Export or import your profiles at any time. The app restarts automatically after import/export. Optional auto-backup on every startup.  
- **OLED Care (Blackout Screen)** – Fully black out all screens while you’re AFK to protect OLED panels. Toggle with **CTRL + B**.  
- **Side-screen dimming while tracking (optional)** – When enabled, GameTimeX automatically dims all secondary displays **only while monitoring is active** and restores them afterwards.

*Tip: If you are looking for your game’s installation folder on Steam, right-click the game → Properties → Installed Files → Browse.*

---

### Manage Executables

When using Automatic Profile Switching, GameTimeX now lets you control exactly which executables are tracked.  
This prevents helper processes, launchers, crash reporters, or engine-related executables from interfering with automatic switching.

<img width="554" height="494" alt="image" src="https://github.com/user-attachments/assets/920efc3b-2073-4004-bc3e-d52988125172" />

**Features:**
- Lists all executables found in the game’s installation directory  
- Checkbox list to mark executables as *active* or *inactive*  
- **Select All** toggle for quick bulk activation/deactivation  
- Smart default pre-filter: common non-game executables are automatically deselected when a profile is first created  
- Manual adjustments can be saved at any time via the **Save** button  

This gives you full control and prevents false detections when multiple games share the same background executables.

---

### Profile Image Cropping

When creating a profile, you will be prompted to select an image.  
After choosing an image, the cropping window will open:  
<img width="1211" height="814" alt="image" src="https://github.com/user-attachments/assets/a94e01b8-0482-459e-9d6a-a213b8a56f8d" />

**Cropping controls:**
- Resize the cropping area: use the mouse wheel.  
- Move the cropping area: drag with the left mouse button.  

Once confirmed, the cropped image is saved and displayed in the profile view.

---

### Security

- **SQL-injection protection:** all database operations use parameterized SQLite commands.  
- **Executable filtering:** automatic exclusion of non-game executables on first profile creation ensures more reliable detection while avoiding false positives.

---

Icons by Icons8.
