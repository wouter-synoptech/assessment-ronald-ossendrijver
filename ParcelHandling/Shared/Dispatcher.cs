namespace ParcelHandling.Shared
{
    /// <summary>
    /// A Generic Dispatcher that can determine the Target of Dispatchable items based on Rules applied to the Characteristics of those Dispatchable items.
    /// </summary>
    /// <typeparam name="TargetType">The Type of targets this dispatcher will dispatch items to, e.g. Departments.</typeparam>
    public class Dispatcher<TargetType> where TargetType : notnull
    {
        private Dictionary<TargetType, IExpression> Rules { get; set; } = new();

        public void AddDispatchRule(TargetType target, IExpression dispatchRule)
        {
            Rules.Remove(target);
            Rules.Add(target, dispatchRule);
        }

        public TargetType DetermineTarget(IDispatchable dispatchable)
        {
            return Rules.FirstOrDefault(r => r.Value.Evaluate(dispatchable.GetCharacteristics())).Key;
        }

        public IEnumerable<TargetType> Targets => Rules.Keys;

        public IExpression GetRule(TargetType target) => Rules[target];
    }
}
