using Microsoft.AspNetCore.Mvc;
using ParcelHandling.Server.Managers;
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

        [HttpPost("{parcelId}/{parcelState}")]
        public void Post(int parcelId, ParcelState parcelState)
        {
            ParcelManager.HandleParcel(parcelId, parcelState);
        }

        [HttpGet("{departmentName}")]
        public IEnumerable<Parcel> Get(string departmentName)
        {
            return ParcelManager.GetParcels(departmentName);
        }
    }
}