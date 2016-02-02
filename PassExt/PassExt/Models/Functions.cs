using System;

namespace PassExt.Models
{
    public static class Functions
    {
        public static string TranslateDFlag(string dflag)
        {
            switch (dflag)
            {
                case "a": return "Активна";
                case "d": return "Деактивирована";
                default: return null;
            }
        }
        public static string TranslateDFlagBack(string dflag)
        {
            switch (dflag)
            {
                case "Активна": return "a";
                case "Деактивирована": return "d";
                default: return null;
            }
        }
        public static string TranslateGoBack(string GoBack)
        {
            switch (GoBack)
            {
                case "0": return "Нет";
                case "1": return "Да";
                default: return null;
            }
        }

        public static int OrderDayOfWeek(DayOfWeek dw)
        {
            switch (dw)
            {
                case DayOfWeek.Friday: return 4;
                case DayOfWeek.Monday: return 0;
                case DayOfWeek.Saturday: return 5;
                case DayOfWeek.Sunday: return 6;
                case DayOfWeek.Thursday: return 3;
                case DayOfWeek.Tuesday: return 1;
                case DayOfWeek.Wednesday: return 2;
                default: return -1;
            }
        }
    }
}