using Microsoft.AspNetCore.Mvc;
using ParcelHandling.Shared;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace ParcelHandling.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParcelController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;

        public ParcelController(ILogger<DepartmentController> logger)
        {
            _logger = logger;
        }

        [HttpPost("{parcelId}")]
        public async Task Post(int parcelId)
        {

        }

        [HttpGet("{departmentName}")]
        public IEnumerable<Parcel> Get(string departmentName)
        {
            try
            {
                var result = new List<Parcel>();
                int parcelId = 0;

                using (StreamReader departmentFile = new("departmentconfig.txt"))
                {
                    var dispatcher = SimpleDepartmentDispatcherFactory.Create(departmentFile);
                    
                    var department = dispatcher.Targets.FirstOrDefault(dept => dept.Name == departmentName);

                    if (department != null) {

                        var serializer = new XmlSerializer(typeof(Container));

                        foreach (var containerFile in Directory.GetFiles("./ParcelContainers"))
                        {
                            using (StreamReader sr = new(containerFile))
                            {
                                var container = (Container?)serializer.Deserialize(sr);

                                if (container != null && container.Parcels != null)
                                {
                                    foreach (var parcel in container.Parcels)
                                    {
                                        parcel.Id = parcelId++;
                                        if (dispatcher.DetermineTarget(parcel) == department)
                                        {
                                            result.Add(parcel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.Print("Parcels found: " + result.Count);
                _logger.Log(LogLevel.Critical, "Parcels found: " + result.Count);

                return result;
            }
            catch (Exception)
            {
                return Array.Empty<Parcel>();
            }
        }
    }
}