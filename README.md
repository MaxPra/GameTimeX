# GameTimeX

GameTimeX is an application that enables you to accurately monitor your gaming time.  
If you are frustrated with Steam's inaccurate tracking (e.g., when a game is left idle while you are away from your keyboard), this tool will provide the solution you need.

**Version 1.9.0 is now available!**

Download the latest installer [here](https://github.com/MaxPra/GameTimeX/releases/download/Version2.0.0/GameTimeXSetup_Latest.msi).

## About This Application

GameTimeX is a personal project I developed as a hobby during my free time.

### Main Window Overview

Upon launching the application for the first time, you will be greeted with a window that looks like this:  
![image](https://github.com/user-attachments/assets/75cbce6e-db9b-4ef9-b450-1bdf1d23bde4)

Below is an overview of the various sections within the main window:

1. **Profile Search Bar**: Located at the top of the window, this search bar allows you to easily find games if the list becomes too long and scrolling is inconvenient.
2. **Profile Overview Mode**: Here, the view mode can be changed (list or tile view).
3. **Profile Management Buttons**: Next to the search bar, you will find three buttons:
   - "Switch View"
   - "Create New Profile"  
   - "Delete Profile"
   - "Properties"  
   - "Settings"
  
   Note: you can also use the contextmenu to open the properties and delete a game profile.
5. **Information Section**: On the right side of the screen, this section displays the details of the currently selected profile, including a small profile image.
6. **Start/Stop Monitoring**: In the "Information Section", you can start or stop the game time tracking by simply pressing the button at the bottom.  
   - A **blue** button indicates no active monitoring.  
   - A **red** button signals that monitoring is in progress.
7. **Rename Profile**: You can rename the selected game profile by clicking the pencil icon next to the profile name, which is also displayed in the "Information Section."

After adding a game profile (using the "Create New Profile" button), it will look something like this:  
![image](https://github.com/user-attachments/assets/53bca098-912e-4b0e-9458-3293fa1313d0)

Or it will look something like this (depending on the mode you chose):
![image](https://github.com/user-attachments/assets/976e0e03-9cc9-4b76-b288-a7553736f7ab)




If you are dissatisfied with the profile name or the image selected, you can easily modify them by clicking the pencil icon next to the profile name or by selecting "Change Image."

Hovering over the small clock next to "Game Time" will show the tracked time in minutes (though game time is usually displayed in hours). Additionally, you can view the first and last time you played the game.

If playtime has already been recorded for a game and a new playthrough is started (i.e., the game is being played for the second time), the "New Playthrough Startpoint" context function can be used to specify the starting point for the new playtime recording. In the background, the current total playtime is saved, and the difference from the specified start point is calculated.

Note: The total playtime is always accumulated, while the playthrough-specific playtime is always calculated as the difference from the designated start point.

### Settings

Now that the main window has been explained, let's review the settings window:  
![image](https://github.com/user-attachments/assets/8aa9a7ac-b2a8-4bff-90d2-08b2f2574b9c)

In the settings window, you can enable or disable the tracking of the current session's game time. When enabled, the game time for the current session will appear under the start/stop monitoring button while the monitoring is active.

If the checkbox "Automatic switching between game profiles" is enabled, GameTimeX will automatically switch to the correct profile when launching a game. However, this only works if the game's folder has been specified.

**How to find the game's folder on Steam?**

Right-click on the respective game => Properties => Installed Files => Browse.

This path must then be entered in the corresponding profile – GameTimeX will handle the rest.

Additionally, a shortcut key can be used to start and stop the time tracking, eliminating the need to alt-tab out of the game to begin or end the session.

By clicking on "Monitor Key," you can assign your desired key for the action (almost all keys on the keyboard are supported, with the exception of mouse buttons).

Additionally, you can back up your profiles or import an existing backup. This feature is particularly useful if you need to reset your computer, transfer profiles to another device, or recover profiles that may have been accidentally deleted.  
As noted in the settings window, the application will automatically restart after importing or exporting a backup.

The auto-backup feature can also be activated via the checkbox to automatically create a backup upon each program startup (provided that a valid path is specified in the backup path field).
It is recommended to organize the backup folder at your discretion when auto-backup is enabled. This is not done automatically by GameTimeX as a security measure

### Profile Image Cropping

When creating a new game profile, you will need to select an image to be displayed in the "Information Display."  
Once you’ve selected your image via the file explorer, you will be prompted with this cropping window:  
![image](https://github.com/user-attachments/assets/396e4d83-66af-454f-bf59-b03509f3d5ff)

In this window, you can crop the image to suit your preferences (such as selecting the game title or any other section that fits best).

The controls for cropping are as follows:
- **Resize the cropping area**: Use the mouse wheel to adjust the size of the cropping area.
- **Move the cropping area**: Hold the left mouse button and drag to reposition the cropping area.

Once you are satisfied with the selection, click the "X" in the top-right corner to save the cropped image.

Icons by Icons8.
```
