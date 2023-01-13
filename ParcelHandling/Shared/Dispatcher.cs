using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelHandling.Shared
{
    public class Dispatcher<TargetType>  where TargetType : IDispatchTarget 
    {
        private Dictionary<TargetType, IExpression> Rules { get; set; } = new();

        public void AddDispatchRule(TargetType target, IExpression rules)
        {
            Rules.Remove(target);
            Rules.Add(target, rules);
        }

        public TargetType DetermineTarget(IDispatchable dispatchable)
        {
            return Rules.FirstOrDefault(r => r.Value.Evaluate(dispatchable.GetCharacteristics())).Key;
        }

        public bool TryDetermineTarget(IDispatchable dispatchable, out TargetType target)
        {
            target = Rules.FirstOrDefault(r => r.Value.Evaluate(dispatchable.GetCharacteristics())).Key;

            return target != null;
        }

        public IEnumerable<TargetType> Targets => Rules.Keys;

        public IExpression GetRule(TargetType target) => Rules[target];
    }
}
