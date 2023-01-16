using System.Globalization;

namespace ParcelHandling.Shared
{
    public class Interval
    {
        public IComparable? MinValue;
        public bool MinValueIncluded;
        public IComparable? MaxValue;
        public bool MaxValueIncluded;

        public Interval(IComparable? lowerBound, bool includeLowerBound, IComparable? upperBound, bool includeUpperBound)
        {
            MinValue = lowerBound;
            MinValueIncluded = includeLowerBound;
            MaxValue = upperBound;
            MaxValueIncluded = includeUpperBound;
        }

        public char IntervalOpenChar => MinValueIncluded ? '[' : '<';

        public char IntervalCloseChar => MaxValueIncluded ? ']' : '>';

        public bool Contains(object value)
        {
            var result = true;

            if (MinValue != null)
            {
                if (MinValueIncluded)
                {
                    if (MinValue.CompareTo(value) > 0)
                    {
                        result = false;
                    }
                }
                else
                {
                    if (MinValue.CompareTo(value) >= 0)
                    {
                        result = false;
                    }
                }
            }

            if (MaxValue != null)
            {
                if (MaxValueIncluded)
                {
                    if (MaxValue.CompareTo(value) < 0)
                    {
                        result = false;
                    }
                }
                else
                {
                    if (MaxValue.CompareTo(value) <= 0)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        public static Interval Parse(string text)
        {
            var parts = text.Trim().Split(',');

            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalid interval: {text}");
            }

            var part1 = parts[0].Trim();
            var part2 = parts[1].Trim();

            if (part1[0] != '<' && part1[0] != '[' || part2[^1] != '>' && part2[^1] != ']')
            {

                throw new ArgumentException($"Invalid interval: {text}");
            }

            return new Interval(
                part1[1] == '*' ? null : Converter.Convert(part1[1..]),
                part1[0] == '[',
                part2[^2] == '*' ? null : Converter.Convert(part2[..^1]),
                part2[^1] == ']');

        }

        public override string ToString()
        {
            var minValueToDisplay = MinValue == null ? "*" : Convert.ToString(MinValue, CultureInfo.InvariantCulture);
            var maxValueToDisplay = MaxValue == null ? "*" : Convert.ToString(MaxValue, CultureInfo.InvariantCulture);
            return $"{IntervalOpenChar}{minValueToDisplay},{maxValueToDisplay}{IntervalCloseChar}";
        }
    }

}
