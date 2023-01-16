namespace ParcelHandling.Shared
{
    public static class Converter
    {
        public static IComparable Convert(string value)
        {
            if (bool.TryParse(value, out bool tryBool))
            {
                return tryBool;
            }

            if (float.TryParse(value, out float tryFloat))
            {
                return tryFloat;
            }

            return value;
        }
    }
}
