namespace ParcelHandling.Shared
{
    public class SimpleAndExpression : IExpression
    {
        public List<IExpression> Terms { get; set; } = new();

        public void AddTerm(IExpression term)
        {
            Terms.Add(term);
        }

        public bool Evaluate(IDictionary<string, object> values) => Terms.All(term => term.Evaluate(values));
    }

    public class SimpleOrExpression : IExpression
    {
        public List<IExpression> Terms { get; set; } = new();

        public void AddTerm(IExpression term)
        {
            Terms.Add(term);
        }

        public bool Evaluate(IDictionary<string, object> values) => Terms.Any(term => term.Evaluate(values));
    }

    public class SimpleIntervalCondition : IExpression
    {
        public string Variable { get; set; }

        public Interval Interval { get; set; }

        public SimpleIntervalCondition(string variable, IComparable? lowerBound, bool includeLowerBound, IComparable? upperBound, bool includeUpperBound)
            : this(variable, new Interval(lowerBound, includeLowerBound, upperBound, includeUpperBound))
        {
        }

        public SimpleIntervalCondition(string variable, Interval interval)
        {
            Variable = variable;
            Interval = interval;
        }

        public bool Evaluate(IDictionary<string, object> values)
        {
            return values.TryGetValue(Variable, out object? value) && Interval.Contains(value);
        }

        public static SimpleIntervalCondition Parse(string text)
        {
            var parts = text.Split("in");

            if (parts.Length == 2)
            {
                return new SimpleIntervalCondition(parts[0].Trim(), Interval.Parse(parts[1].Trim()));
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

    public class SimpleEqualityCondition : IExpression
    {
        public string Variable { get; set; }

        public object Value { get; set; }

        public SimpleEqualityCondition(string variable, object value)
        {
            Variable = variable;
            Value = value;
        }

        public bool Evaluate(IDictionary<string, object> values)
        {
            return values.TryGetValue(Variable, out object? value) && Value.Equals(value);
        }

        public static SimpleEqualityCondition Parse(string text)
        {
            var parts = text.Split("=");

            if (parts.Length == 2)
            {
                return new SimpleEqualityCondition(parts[0].Trim(), Converter.Convert(parts[1].Trim()));
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
