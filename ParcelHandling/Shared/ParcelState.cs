namespace ParcelHandling.Shared
{
    public enum ParcelState : int
    {
        None = 0,
        NewAndUnauthorized = 1000,
        Authorized = 2000,
        Handled = 3000
    }
}
