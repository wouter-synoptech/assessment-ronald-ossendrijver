using System.Globalization;

namespace ParcelHandling.Shared
{
    /// <summary>
    /// Specifies an Interval defined by a lower and upper bound. Leaving a bound empty means "no bound". The value of each bound is either included or excluded from the interval.
    /// </summary>
    public class Interval
    {
        public const char LOWERBOUND_INCLUDED_CHAR = '[';
        public const char LOWERBOUND_EXCLUDED_CHAR = '<';
        public const char UPPERBOUND_INCLUDED_CHAR = ']';
        public const char UPPERBOUND_EXCLUDED_CHAR = '>';
        public const char UNBOUNDED_CHAR = '*';
        public const char BOUND_SEPARATOR_CHAR = ',';

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

        public char IntervalOpenChar => MinValueIncluded ? LOWERBOUND_INCLUDED_CHAR : LOWERBOUND_EXCLUDED_CHAR;

        public char IntervalCloseChar => MaxValueIncluded ? UPPERBOUND_INCLUDED_CHAR : UPPERBOUND_EXCLUDED_CHAR;

        public bool Contains(object value)
        {
            if (value == null && (MinValue != null || MaxValue != null)) return false;

            var result = true;

            if (MinValue != null)
            {
                var valueToCompare = Convert.ChangeType(value, MinValue.GetType());

                if (MinValueIncluded)
                {
                    if (MinValue.CompareTo(valueToCompare) > 0)
                    {
                        result = false;
                    }
                }
                else
                {
                    if (MinValue.CompareTo(valueToCompare) >= 0)
                    {
                        result = false;
                    }
                }
            }

            if (MaxValue != null)
            {
                var valueToCompare = Convert.ChangeType(value, MaxValue.GetType());

                if (MaxValueIncluded)
                {
                    if (MaxValue.CompareTo(valueToCompare) < 0)
                    {
                        result = false;
                    }
                }
                else
                {
                    if (MaxValue.CompareTo(valueToCompare) <= 0)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Parses an interval from a string. Whitespaces are ignored. Examples: 
        /// [10,20] represents an interval with a lower bound of 10 and an upper bound of 20, where both bounds are included.
        /// <10,*> represents an interval with a lower bound of 10 (where 10 is not included) and no upper bound.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Interval Parse(string text)
        {
            var parts = text.Trim().Split(BOUND_SEPARATOR_CHAR);

            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalid interval: {text}");
            }

            var part1 = parts[0].Trim();
            var part2 = parts[1].Trim();

            if (part1[0] != LOWERBOUND_EXCLUDED_CHAR && part1[0] != LOWERBOUND_INCLUDED_CHAR || part2[^1] != UPPERBOUND_EXCLUDED_CHAR && part2[^1] != UPPERBOUND_INCLUDED_CHAR)
            {
                throw new ArgumentException($"Invalid interval: {text}");
            }

            return new Interval(
                part1[1] == UNBOUNDED_CHAR ? null : Converter.Convert(part1[1..]),
                part1[0] == LOWERBOUND_INCLUDED_CHAR,
                part2[^2] == UNBOUNDED_CHAR ? null : Converter.Convert(part2[..^1]),
                part2[^1] == UPPERBOUND_INCLUDED_CHAR);

        }

        public override string ToString()
        {
            var minValueToDisplay = MinValue == null ? "*" : Convert.ToString(MinValue, CultureInfo.InvariantCulture);
            var maxValueToDisplay = MaxValue == null ? "*" : Convert.ToString(MaxValue, CultureInfo.InvariantCulture);
            return $"{IntervalOpenChar}{minValueToDisplay},{maxValueToDisplay}{IntervalCloseChar}";
        }
    }

}
