namespace ParcelHandling.Shared
{
    public class Department
    {
        public string? Name { get; set; }

        public List<ParcelAction> Actions { get; set; } = new();
    }
}