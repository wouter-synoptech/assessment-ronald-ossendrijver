using Microsoft.AspNetCore.Mvc;
using ParcelHandling.Server.Managers;
using ParcelHandling.Shared;

namespace ParcelHandling.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParcelController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly IConfiguration _configuration;

        public ParcelController(IConfiguration configuration, ILogger<DepartmentController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPut]
        public void Put(Parcel parcel)
        {
            ParcelManager.UpdateParcel(parcel, _configuration);
        }

        [HttpGet("{departmentName}")]
        public IEnumerable<Parcel> Get(string departmentName)
        {
            return ParcelManager.GetParcels(departmentName, _configuration);
        }

        [HttpDelete]
        public void Delete()
        {
            ParcelManager.DeleteAllParcels(_configuration);
        }

    }
}