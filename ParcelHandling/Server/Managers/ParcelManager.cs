using ParcelHandling.Shared;
using System.Text.Json;
using System.Xml.Serialization;

namespace ParcelHandling.Server.Managers
{
    public static class ParcelManager
    {
        /// <summary>
        /// Checks if there are any unhandled containers and if so extracts all parcels from those containers.
        /// </summary>
        public static void CheckAndHandleNewContainers(IConfiguration configuration)
        {
            var containerFolder = configuration?["ContainerFolder"];

            if (containerFolder == null) throw new ArgumentException("Container folder not configured");

            var parcelFolder = configuration?["ParcelFolder"];

            if (parcelFolder == null) throw new ArgumentException("Parcel folder not configured");

            Directory.CreateDirectory(containerFolder);
            Directory.CreateDirectory(parcelFolder);

            foreach (var containerFile in Directory.GetFiles(containerFolder))
            {
                ReadContainer(containerFile, parcelFolder);
            }
        }

        private static void ReadContainer(string containerFile, string parcelFolder)
        {
            using (StreamReader sr = new(containerFile))
            {
                var serializer = new XmlSerializer(typeof(Container));
                var container = (Container?)serializer.Deserialize(sr);

                if (container != null)
                {
                    var containerfile = $"{parcelFolder}/container_{container.Id}.json";

                    if (!File.Exists(containerfile))
                    {
                        File.WriteAllText(containerfile, JsonSerializer.Serialize(container));
                        ExtractParcelsFromContainer(container, parcelFolder);
                    }
                }
            }
        }

        private static void ExtractParcelsFromContainer(Container container, string parcelFolder)
        {
            int parcelId = 0;
            if (container.Parcels != null)
            {
                foreach (var parcel in container.Parcels)
                {
                    parcel.Id = container.Id + "-" + parcelId++;
                    parcel.State = ParcelState.NewAndUnauthorized;

                    var parcelfile = $"{parcelFolder}/parcel_{parcel.Id}.json";
                    File.WriteAllText(parcelfile, JsonSerializer.Serialize(parcel));
                }
            }
        }

        public static IEnumerable<Parcel> GetParcels(string departmentName, IConfiguration configuration)
        {
            var departmentConfig = configuration?["DepartmentConfig"];
            if (departmentConfig == null) throw new ArgumentException("Department definition file not configured");
            if (!File.Exists(departmentConfig)) throw new ArgumentException("Departments not defined");

            var parcelFolder = configuration?["ParcelFolder"];

            if (parcelFolder == null) throw new ArgumentException("Parcel folder not configured");

            var result = new List<Parcel>();

            using (StreamReader departmentFile = new(departmentConfig))
            {
                var dispatcher = DepartmentManager.Create(departmentFile);
                var department = dispatcher.Targets.FirstOrDefault(dept => dept.Name == departmentName);

                if (department != null)
                {
                    CheckAndHandleNewContainers(configuration);

                    foreach (var parcelfile in Directory.GetFiles(parcelFolder, "parcel*"))
                    {
                        var parcel = JsonSerializer.Deserialize<Parcel>(File.ReadAllText(parcelfile));

                        if (parcel != null && dispatcher.DetermineTarget(parcel) == department)
                        {
                            result.Add(parcel);
                        }
                    }
                }
            }

            return result;
        }

        public static void UpdateParcel(Parcel parcel, IConfiguration configuration)
        {
            var parcelFolder = configuration?["ParcelFolder"];
            if (parcelFolder == null) throw new ArgumentException("Parcel folder not configured");

            var parcelfile = $"{parcelFolder}/parcel_{parcel.Id}.json";
            File.WriteAllText(parcelfile, JsonSerializer.Serialize(parcel));
        }

        public static void DeleteAllParcels(IConfiguration configuration)
        {
            var parcelFolder = configuration?["ParcelFolder"];
            if (parcelFolder == null) throw new ArgumentException("Parcel folder not configured");

            foreach (var filename in Directory.GetFiles(parcelFolder))
            {
                File.Delete(filename);
            }
        }
    }
}
