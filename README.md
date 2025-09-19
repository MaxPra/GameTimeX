# GameTimeX

GameTimeX is an application that enables you to accurately monitor your gaming time.  
If you are frustrated with Steam's inaccurate tracking (e.g., when a game is left idle while you are away from your keyboard), this tool will provide the solution you need.

**Version 2.0.11 / Retouched is now available!**  
Same core, brand-new look

Download the latest installer [here](https://github.com/MaxPra/GameTimeX/releases/download/Version2.0.11/GameTimeXSetup_Latest.msi).

---

## What’s new in 2.0.11

- Visual redesign of the UI
- Removed the list view
- Minor UI-related bug fixes
- **OLED Care (Blackout Screen)** – toggle at any time with **CTRL + B** to fully darken all screens while you’re AFK
- **Optional side-screen dimming while tracking** – when enabled in **Settings**, all non-primary displays are dimmed automatically while GameTimeX is actively tracking, and restored once tracking stops
- **Security: SQL-injection hardening** – database access now uses parameterized SQLite queries

---

## About This Application

GameTimeX is a personal project I developed as a hobby during my free time.

### Main Window Overview

When you launch the application for the first time, the main window will appear as shown below:  
<img width="1217" height="809" alt="image" src="https://github.com/user-attachments/assets/c3bba32f-0c31-4eb2-bdb2-af8b9b4f6ac0" />

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
<img width="1208" height="806" alt="image" src="https://github.com/user-attachments/assets/fea9d2e8-609a-48d5-b655-3eda2fdf0187" />

Profile names and images can be adjusted at any time. Simply click the pencil icon or select *Change Image*.  
Hovering over the small “i” next to *Game Time* will show the tracked time in minutes, alongside the recorded first and last play sessions.

If you restart a game after already having recorded playtime, the *New Playthrough Startpoint* function can be used. It saves the current total time and calculates the new playtime as the difference from that point onward.  
- **Total time** is always cumulative.  
- **Playthrough-specific time** is calculated as the difference from the designated start point.

---

### Settings

The settings window gives you fine-grained control over how GameTimeX behaves:  
<img width="533" height="732" alt="image" src="https://github.com/user-attachments/assets/6a25a393-b210-4b83-b2a4-164230bf5c47" />

**Available options:**

- **Session Time Tracking** – Display the duration of the current session directly in the information panel while monitoring is active.  
- **Automatic Profile Switching** – If enabled, GameTimeX will automatically switch to the correct profile when a game is launched (requires the game’s installation folder to be specified).  
- **Monitor Key** – Assign a hotkey to start and stop monitoring without leaving the game. Nearly all keyboard keys are supported.  
- **Backup & Restore** – Export or import your profiles at any time. The app restarts automatically after import/export. Optional auto-backup on every startup.  
- **OLED Care (Blackout Screen)** – Fully black out all screens while you’re AFK to protect OLED panels. Toggle with **CTRL + B**.  
- **Side-screen dimming while tracking (optional)** – When enabled, GameTimeX automatically dims all secondary displays **only while monitoring is active** and restores them afterwards.

*Tip: If you are looking for your game’s installation folder on Steam, right-click the game → Properties → Installed Files → Browse.*

---

### Profile Image Cropping

When creating a profile, you will be prompted to select an image.  
After choosing an image, the cropping window will open:  
<img width="1206" height="806" alt="image" src="https://github.com/user-attachments/assets/d0738e77-1188-4c4c-bc80-f12a60385153" />

**Cropping controls:**
- Resize the cropping area: use the mouse wheel.  
- Move the cropping area: drag with the left mouse button.  

Once confirmed, the cropped image is saved and displayed in the profile view.

---

### Security

- **SQL-injection protection:** all database operations use parameterized SQLite commands.

---

Icons by Icons8.
