namespace ParcelHandling.Shared
{
    public interface IExpression
    {
        public bool Evaluate(IDictionary<string, object> values);
    }
}
