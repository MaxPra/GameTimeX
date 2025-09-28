using GameTimeX.Function.AppEnvironment;
using GameTimeX.XApplication.SubDisplays;

namespace GameTimeX.Function.WorldEvents
{
    public class WorldEventHandler
    {
        public static void HandleWorldEvents()
        {
            // Jahresstatisik anzeigen
            if (FuncYearStats.IsShowYearStats())
            {
                YearGameStats yearGameStats = new YearGameStats
                {
                    Owner = SysProps.mainWindow
                };
                yearGameStats.ShowDialog();
            }

            // Happy New Year
            if (FuncNewYear.IsShowNewYearEvent())
            {
                HappyNewYear happyNewYear = new HappyNewYear();
                happyNewYear.Owner = SysProps.mainWindow;
                happyNewYear.ShowDialog();
            }

            // Halloween
            if (FuncHalloweenEvent.IsShowHalloweenEvent())
            {
                HappyHalloween happyHalloween = new HappyHalloween();
                happyHalloween.Owner = SysProps.mainWindow;
                happyHalloween.ShowDialog();
            }
        }


    }
}
