namespace ParcelHandling.Shared
{
    /// <summary>
    /// Represents the expression: x1 && x2 && ..
    /// </summary>
    public class AndExpression : IExpression
    {
        public List<IExpression> Terms { get; set; } = new();

        public void AddTerm(IExpression term)
        {
            Terms.Add(term);
        }

        public bool Evaluate(IDictionary<string, object> values) => Terms.All(term => term.Evaluate(values));
    }

    /// <summary>
    /// Represents the expression: x1 || x2 || ..
    /// </summary>
    public class OrExpression : IExpression
    {
        public List<IExpression> Terms { get; set; } = new();

        public void AddTerm(IExpression term)
        {
            Terms.Add(term);
        }

        public bool Evaluate(IDictionary<string, object> values) => Terms.Any(term => term.Evaluate(values));
    }

    /// <summary>
    /// Represents the expression that the value of a Variable should be contained by an Interval
    /// </summary>
    public class IntervalCondition : IExpression
    {
        public string Variable { get; set; }

        public Interval Interval { get; set; }

        public IntervalCondition(string variable, IComparable? lowerBound, bool includeLowerBound, IComparable? upperBound, bool includeUpperBound)
            : this(variable, new Interval(lowerBound, includeLowerBound, upperBound, includeUpperBound))
        {
        }

        public IntervalCondition(string variable, Interval interval)
        {
            Variable = variable;
            Interval = interval;
        }

        public bool Evaluate(IDictionary<string, object> values)
        {
            return values.TryGetValue(Variable, out object? value) && Interval.Contains(value);
        }

        public static IntervalCondition Parse(string text)
        {
            var parts = text.Split("in");

            if (parts.Length == 2)
            {
                return new IntervalCondition(parts[0].Trim(), Interval.Parse(parts[1].Trim()));
            }
            else
            {
                throw new ArgumentException($"Illegal interval rule: {text}");
            }
        }

        public override string ToString()
        {
            return $"{Variable} in {Interval}";
        }
    }

    /// <summary>
    /// Represents the expression that the value of a Variable should equal to the specified Value
    /// </summary>
    public class EqualityCondition : IExpression
    {
        public string Variable { get; set; }

        public object Value { get; set; }

        public EqualityCondition(string variable, object value)
        {
            Variable = variable;
            Value = value;
        }

        public bool Evaluate(IDictionary<string, object> values)
        {
            return values.TryGetValue(Variable, out object? value) && Value.Equals(value);
        }

        public static EqualityCondition Parse(string text)
        {
            var parts = text.Split("=");

            if (parts.Length == 2)
            {
                return new EqualityCondition(parts[0].Trim(), Converter.Convert(parts[1].Trim()));
            }
            else
            {
                throw new ArgumentException($"Illegal equality expression: {text}");
            }
        }

        public override string ToString()
        {
            return $"{Variable} = {Value}";
        }
    }
}
