namespace Zoppas.Model
{
    using System.Collections.Generic;

    public static class Helper
    {
        public static bool Difference(this List<double> list, double max, double min, out double? high, out double? low, out double? value)
        {
            if (null == list || list.Count == 0)
            {
                high = null;
                low = null;
                value = null;

                return false;
            }

            high = list[0];
            low = list[0];
            foreach (var item in list)
            {
                if (item > high) { high = item; }
                if (item < low) { low = item; }
            }
            value = high - low;
            return (value <= max) && (value >= min);
        }

        public static bool MultiMin(this List<double> list, double max, double min, out double? value)
        {
            value = max;
            foreach (var item in list)
            {
                if (item < value)
                {
                    value = item;
                }
            }

            return (value <= max) && (value >= min);
        }

        public static bool Default(this List<double> list, double max, double min, out double? value)
        {
            value = 999;
            return false;
        }

        public static bool AllBetween(this List<double> list, double max, double min, out double? value)
        {
            if (null == list || list.Count == 0)
            {
                value = -1;
                return false;
            }

            foreach (var item in list)
            {
                if (item >= max || item <= min)
                {
                    value = item;
                    return false;
                }
            }
            value = list[0];
            return true;
        }
    }
}
