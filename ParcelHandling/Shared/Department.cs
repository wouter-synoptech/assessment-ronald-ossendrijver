using Microsoft.VisualBasic;

namespace ParcelHandling.Shared
{
    public class Department : IDispatchTarget
    {
        public string? Name { get; set; }

        public List<HandlingAction> Actions { get; set; } = new();
    }
}