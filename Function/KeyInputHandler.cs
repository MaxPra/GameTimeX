using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.Objects;
using GameTimeX.Objects;
using static GameTimeX.Objects.KeyInput;
namespace GameTimeX.Function
{
    internal class KeyInputHandler
    {
        // Key auf den abgehört werden soll
        private VirtualKey keyToListenFor = VirtualKey.VK_NONE;
        // Thread in dem in gewissen Abständen auf den Key geprüft wird
        private Thread? th = null;

        //Übergebenes Window
        private Window wnd;

        private bool stopListening = false;

        private StartType startType;

        public static int KEY_DOWN_EVENT = 0x1;
        public static int KEY_UP_EVENT = 0x2;

        private bool key_was_pressed = false;
        private bool key_was_released = false;

        /// <summary>
        /// Erzeugt eine neue Instanz des KeyInputHandler
        /// </summary>
        /// <param name="keyToListenFor">Key auf den gehört werden soll</param>
        public KeyInputHandler(VirtualKey keyToListenFor, Window wnd)
        {
            this.keyToListenFor = keyToListenFor;
            this.wnd = wnd;
            this.startType = StartType.GAME_MONITORING;
            th = new Thread(HandleKeyInputEvent);
        }

        public KeyInputHandler(Window wnd)
        {
            this.wnd = wnd;
            this.startType = StartType.MONITORE_KEY;
            th = new Thread(HandleKeyInputEvent);
        }

        public KeyInputHandler(Window wnd, StartType startType)
        {
            this.wnd = wnd;
            this.startType = startType;
            th = new Thread(HandleKeyInputEvent);
        }

        /// <summary>
        /// Startet das Abhören auf den übergebenen Key
        /// </summary>
        public void StartListening()
        {
            if (th != null)
            {
                th.IsBackground = true;
                th.Start();
            }
        }

        /// <summary>
        /// Stoppt das Abhören auf den übergebenen Key
        /// </summary>
        public void StopListening()
        {
            stopListening = true;
            th = null;
        }

        /// <summary>
        /// Kümmert sich um das Abhören des Keys und das auslesen
        /// </summary>
        /// <param name="obj"></param>
        private void HandleKeyInputEvent()
        {

            while (true)
            {
                // Abbruch, wenn KeyInputHandler gestoppt wird
                if (stopListening)
                    break;

                if (startType == StartType.GAME_MONITORING)
                {
                    bool keyPressed = CheckForSpecificKeyOnKeyboard(keyToListenFor);
                    if (keyPressed)
                    {
                        this.wnd.Dispatcher.Invoke((Action)(() =>
                        {
                            if (SysProps.currentSelectedPID != 0)
                            {
                                // Profilnamen lt. aktuell selektiertem Spiel ermitteln
                                DBO_Profile dbo_Profile = DM_Profile.ReadPID(SysProps.currentSelectedPID);
                                string profileName = dbo_Profile.GameName;

                                string title = "GameTimeX | " + profileName;

                                if (!MonitorHandler.CurrentlyMonitoringGameTime())
                                {
                                    MonitorHandler.StartMonitoringGameTime((MainWindow)wnd, SysProps.currentSelectedPID);
                                    if (SysProps.startUpParms.ShowToastNotification)
                                        VisualHandler.ShowToastNotification(title, "Monitoring startet!", 3000);
                                    if (SysProps.startUpParms.BlackOutSideMonitorsWhileMonitoring)
                                        BlackoutHandler.ToggleSecondaryBlackout((MainWindow)wnd);
                                }

                                else
                                {
                                    MonitorHandler.EndMonitoringGameTime((MainWindow)wnd);
                                    if (SysProps.startUpParms.ShowToastNotification)
                                        VisualHandler.ShowToastNotification(title, "Monitoring stopped!", 3000);

                                    if (SysProps.startUpParms.BlackOutSideMonitorsWhileMonitoring)
                                        BlackoutHandler.ToggleSecondaryBlackout((MainWindow)wnd);
                                }


                                DisplayHandler.BuildInfoDisplay(SysProps.currentSelectedPID, (MainWindow)wnd);
                            }
                        }));
                    }
                }
                else if (startType == StartType.MONITORE_KEY)
                {
                    VirtualKey keyPressed = CheckAllKeysOnKeyboard();
                    if (keyPressed != VirtualKey.VK_NONE && keyPressed != VirtualKey.VK_NORESULT)
                    {
                        this.wnd.Dispatcher.Invoke((Action)(() =>
                        {
                            MonitorKey monitorKeyWnd = (MonitorKey)wnd;
                            monitorKeyWnd.key = keyPressed;
                            monitorKeyWnd.Close();
                        }));
                    }

                }
                else if (startType == StartType.BLACKOUT_SCREEN)
                {
                    if (CheckForKeyCombination(VirtualKey.VK_CONTROL, VirtualKey.VK_B))
                    {
                        this.wnd.Dispatcher.Invoke((Action)(() =>
                        {
                            // Schwarz auf allen Bildschirmen darstellen --> OLED-Freundlich
                            BlackoutHandler.ToggleBlackout((MainWindow)wnd);
                        }));
                    }
                }

                // 20 Millisekunden schlafen legen
                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Prüft alle Tasten auf der Tastatur und gibt die gedrückte zurück
        /// </summary>
        /// <returns></returns>
        private VirtualKey CheckAllKeysOnKeyboard()
        {
            foreach (int key in Enum.GetValues(typeof(KeyInput.VirtualKey)))
            {
                int keystate = SysWin32.GetAsyncKeyState(key);

                // Prüfen, ob Taste gedrückt
                if ((keystate & 0x8000) != 0)
                {
                    return ParseKeyEnum(key);
                }

            }

            return VirtualKey.VK_NORESULT;
        }

        /// <summary>
        /// Prüft nur eine spezifische Taste auf der Tastatur und gibt zurück, ob sie gedrückt wurde
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool CheckForSpecificKeyOnKeyboard(VirtualKey key)
        {
            int keystate = SysWin32.GetAsyncKeyState((int)key);

            // Prüfen, ob Taste gedrückt
            if ((keystate & 0x8000) != 0)
            {

                if (!key_was_pressed)
                {
                    key_was_pressed = true;
                    return true;
                }
            }
            else
            {
                if (key_was_pressed && !key_was_released)
                {
                    key_was_released = true;
                }
            }

            if ((keystate & 0x8000) == 0 && key_was_pressed)
            {
                key_was_pressed = false;
                key_was_released = false;
            }

            return false;
        }

        /// <summary>
        /// Prüft, ob eine Tastenkombination gedrückt wurde (z. B. STRG + B).
        /// </summary>
        private bool CheckForKeyCombination(params VirtualKey[] keys)
        {
            bool allPressed = true;

            foreach (var key in keys)
            {
                int keystate = SysWin32.GetAsyncKeyState((int)key);

                if ((keystate & 0x8000) == 0)
                {
                    allPressed = false;
                    break;
                }
            }

            if (allPressed && !key_was_pressed)
            {
                key_was_pressed = true;
                return true;
            }

            if (!allPressed && key_was_pressed)
            {
                key_was_pressed = false;
            }

            return false;
        }


        private VirtualKey ParseKeyEnum(int keyCode)
        {
            return (VirtualKey)Enum.ToObject(typeof(VirtualKey), keyCode);
        }

        public static VirtualKey GetVirtualKeyByValue(Dictionary<VirtualKey, string> list, string value)
        {
            return list.FirstOrDefault(x => x.Value == value).Key;
        }

        public enum StartType
        {
            MONITORE_KEY,
            GAME_MONITORING,
            BLACKOUT_SCREEN
        }
    }

    internal class SysWin32
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
    }
}
