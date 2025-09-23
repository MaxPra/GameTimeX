using System;
using System.Diagnostics;
using System.IO;

namespace GameTimeX.Function.Windows
{
    internal class FuncWindowsProcess
    {
        public static bool IsProcessRunning(string exeName)
        {
            // Hole alle laufenden Prozesse
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    // Überprüfe, ob der Prozess den Namen der EXE-Datei enthält
                    if (process.ProcessName.Equals(System.IO.Path.GetFileNameWithoutExtension(exeName), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public static bool IsProcessRunningWithPathPart(string exeName, string partialPath)
        {
            var nameNoExt = Path.GetFileNameWithoutExtension(exeName);

            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    if (!p.ProcessName.Equals(nameNoExt, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string procPath = p.MainModule?.FileName;
                    if (string.IsNullOrEmpty(procPath))
                        continue;

                    if (procPath.IndexOf(partialPath, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
                catch
                {
                    return IsProcessRunning(exeName);
                }
                finally
                {
                    try { p.Dispose(); } catch { }
                }
            }
            return false;
        }
    }
}
