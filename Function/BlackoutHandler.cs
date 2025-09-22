using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace GameTimeX.Function
{
    internal static class BlackoutHandler
    {
        private static readonly List<Window> blackoutWindows = new();
        private static bool isActive = false;

        // Cursor-Management
        private static bool cursorClipped = false;
        private static bool mouseHidden = false;

        /// <summary>
        /// Aktiviert / deaktiviert den Blackout-Modus (alle Monitore).
        /// </summary>
        public static void ToggleBlackout(Window owner)
        {
            if (isActive)
                Disable(owner.Dispatcher);
            else
                Enable(owner);
        }

        /// <summary>
        /// Aktiviert Blackout auf allen Monitoren.
        /// </summary>
        public static void Enable(Window owner)
        {
            if (isActive) return;

            CreateWindowsForScreens(owner, s => true);
            MoveMouseToVirtualBottomRight();
            HideMouseCursorGlobally();
            ClipMouseToVirtualBottomRightPixel();

            isActive = true;
        }

        /// <summary>
        /// Aktiviert Blackout NUR auf Neben-Monitoren (alle außer Primary).
        /// Cursor-Handling ist optional (standardmäßig aus).
        /// Nutze Disable(...) zum Beenden.
        /// </summary>
        public static void EnableOnSecondaryMonitors(Window owner, bool manageCursor = false)
        {
            if (isActive) return;

            // nur Screens abdunkeln, die nicht Primary sind
            CreateWindowsForScreens(owner, s => !s.Primary);

            if (manageCursor)
            {
                MoveMouseToVirtualBottomRight();
                HideMouseCursorGlobally();
                ClipMouseToVirtualBottomRightPixel();
            }

            isActive = true;
        }

        /// <summary>
        /// Deaktiviert Blackout: Fenster schließen, Cursor wieder anzeigen, Clip aufheben.
        /// </summary>
        public static void Disable(Dispatcher dispatcher)
        {
            if (!isActive) return;

            if (dispatcher.CheckAccess())
                CloseAllWindows();
            else
                dispatcher.Invoke(CloseAllWindows);

            UnclipMouse();
            ShowMouseCursorGlobally();

            isActive = false;
        }

        /// <summary>
        /// Aktiviert/Deaktiviert Blackout nur für Neben-Monitore.
        /// </summary>
        public static void ToggleSecondaryBlackout(Window owner, bool manageCursor = false)
        {
            if (isActive)
                Disable(owner.Dispatcher);
            else
                EnableOnSecondaryMonitors(owner, manageCursor);
        }

        // -------------------- Fenster-Handling --------------------

        // NEU: generische Erzeugung basierend auf Prädikat (z. B. alle / nur Neben-Monitore)
        private static void CreateWindowsForScreens(Window owner, Func<Screen, bool> predicate)
        {
            foreach (var screen in Screen.AllScreens.Where(predicate))
            {
                var wnd = BuildBlackoutWindow(owner, screen);
                blackoutWindows.Add(wnd);
                wnd.Show();
            }
        }

        // (bestehend) – backward compatibility
        private static void CreateWindowsForAllScreens(Window owner)
        {
            CreateWindowsForScreens(owner, s => true);
        }

        private static void CloseAllWindows()
        {
            foreach (var w in blackoutWindows.ToList())
            {
                try { w.Close(); } catch { /* ignore */ }
            }
            blackoutWindows.Clear();
        }

        /// <summary>
        /// Baut ein WPF-Fenster, das exakt die Pixel-Bounds eines Screens abdeckt (inkl. Taskleiste).
        /// </summary>
        private static Window BuildBlackoutWindow(Window owner, Screen screen)
        {
            var (dx, dy) = GetDipScaleForScreen(owner);

            double leftDip = screen.Bounds.Left * dx;
            double topDip = screen.Bounds.Top * dy;
            double widthDip = screen.Bounds.Width * dx;
            double heightDip = screen.Bounds.Height * dy;

            var wnd = new Window
            {
                Owner = owner,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                Topmost = true,
                AllowsTransparency = false,
                Background = Brushes.Black,
                Left = leftDip,
                Top = topDip,
                Width = widthDip,
                Height = heightDip,
                WindowStartupLocation = WindowStartupLocation.Manual,
            };

            // Eingaben abfangen, damit „schwarz wie aus“ wirkt
            wnd.Cursor = System.Windows.Input.Cursors.None;
            wnd.Focusable = false;

            wnd.PreviewKeyDown += (s, e) => e.Handled = true;
            wnd.PreviewMouseDown += (s, e) => e.Handled = true;
            wnd.PreviewMouseUp += (s, e) => e.Handled = true;
            wnd.PreviewMouseMove += (s, e) => e.Handled = true;

            return wnd;
        }

        /// <summary>
        /// Liefert die Device→DIP-Skalierung des UI-Threads/Owners (für die meisten Setups ausreichend).
        /// </summary>
        private static (double dx, double dy) GetDipScaleForScreen(Window owner)
        {
            double dx = 1.0, dy = 1.0;
            var source = PresentationSource.FromVisual(owner);
            if (source?.CompositionTarget != null)
            {
                var m = source.CompositionTarget.TransformFromDevice;
                dx = m.M11;
                dy = m.M22;
            }
            return (dx, dy);
        }

        // -------------------- Maus-Handling --------------------

        private static void MoveMouseToVirtualBottomRight()
        {
            var vr = SystemInformation.VirtualScreen; // Device-Pixel
            int targetX = vr.Right - 1;
            int targetY = vr.Bottom - 1;
            SetCursorPos(targetX, targetY);
        }

        private static void ClipMouseToVirtualBottomRightPixel()
        {
            if (cursorClipped) return;

            var vr = SystemInformation.VirtualScreen;
            var rect = new RECT
            {
                left = vr.Right - 1,
                top = vr.Bottom - 1,
                right = vr.Right,
                bottom = vr.Bottom
            };

            ClipCursor(ref rect);
            cursorClipped = true;
        }

        private static void UnclipMouse()
        {
            if (!cursorClipped) return;
            ClipCursor(IntPtr.Zero);
            cursorClipped = false;
        }

        private static void HideMouseCursorGlobally()
        {
            if (mouseHidden) return;
            // ShowCursor hält einen globalen Sichtbarkeitszähler
            while (ShowCursor(false) >= 0) { }
            mouseHidden = true;
        }

        private static void ShowMouseCursorGlobally()
        {
            if (!mouseHidden) return;
            while (ShowCursor(true) < 0) { }
            mouseHidden = false;
        }

        // -------------------- Win32 Interop --------------------

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);

        [DllImport("user32.dll")]
        private static extern bool ClipCursor(ref RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClipCursor(IntPtr lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}
