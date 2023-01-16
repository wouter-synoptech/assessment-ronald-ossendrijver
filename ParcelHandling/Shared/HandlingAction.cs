namespace ParcelHandling.Shared
{
    /// <summary>
    /// Definition of a Handling action that can be performed on a parcel, e.g. Authorizing a parcel.
    /// </summary>
    public class ParcelAction
    {
        public string? Action { get; set; }

        public ParcelState Result { get; set; }
    }
}
