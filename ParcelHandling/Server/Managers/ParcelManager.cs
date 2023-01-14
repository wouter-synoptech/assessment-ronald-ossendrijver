using ParcelHandling.Shared;
using System.Text.Json;
using System.Xml.Serialization;

namespace ParcelHandling.Server.Managers
{
    public static class ParcelManager
    {
        public static void CheckForNewContainers()
        {
            int parcelId = 0;

            var serializer = new XmlSerializer(typeof(Container));

            foreach (var containerFile in Directory.GetFiles("./Containers"))
            {
                using (StreamReader sr = new(containerFile))
                {
                    var container = (Container?)serializer.Deserialize(sr);

                    if (container != null)
                    {
                        var containerfile = $"./Parcels/container_{container.Id}.json";

                        if (!File.Exists(containerfile))
                        {
                            File.WriteAllText(containerfile, JsonSerializer.Serialize(container));

                            if (container.Parcels != null)
                            {
                                foreach (var parcel in container.Parcels)
                                {
                                    parcel.Id = container.Id + "-" + parcelId++;
                                    parcel.State = ParcelState.NewAndUnauthorized;

                                    var parcelfile = $"./Parcels/parcel_{parcel.Id}.json";
                                    File.WriteAllText(parcelfile, JsonSerializer.Serialize(parcel));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<Parcel> GetParcels(string departmentName)
        {
            var result = new List<Parcel>();

            using (StreamReader departmentFile = new("departmentconfig.txt"))
            {
                var dispatcher = DepartmentManager.Create(departmentFile);
                var department = dispatcher.Targets.FirstOrDefault(dept => dept.Name == departmentName);

                if (department != null)
                {
                    CheckForNewContainers();

                    foreach (var parcelfile in Directory.GetFiles("./Parcels", "parcel*"))
                    {
                        var parcel = JsonSerializer.Deserialize<Parcel>(parcelfile);

                        if (parcel != null && dispatcher.DetermineTarget(parcel) == department)
                        {
                            result.Add(parcel);
                        }
                    }
                }
            }

            return result;
        }

        public static Parcel? GetParcel(int parcelId)
        {
            var parcelfile = $"./Parcels/parcel_{parcelId}.json";
            if (File.Exists(parcelfile))
            {
                return JsonSerializer.Deserialize<Parcel>(parcelfile);
            }

            return null;
        }

        public static void HandleParcel(int parcelId, ParcelState state)
        {
            var parcel = GetParcel(parcelId);
            if (parcel != null)
            {
                HandleParcel(parcel, state);
            }
        }

        public static void HandleParcel(Parcel parcel, ParcelState state)
        {
            parcel.State = state;
            var parcelfile = $"./Parcels/parcel_{parcel.Id}.json";
            File.WriteAllText(parcelfile, JsonSerializer.Serialize(parcel));
        }
    }
}
