using Microsoft.AspNetCore.Mvc;
using ParcelHandling.Server.Managers;

namespace ParcelHandling.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly IConfiguration _configuration;

        public AppController(IConfiguration configuration, ILogger<DepartmentController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpDelete]
        public void Delete()
        {
            ParcelManager.DeleteAllParcels(_configuration);
        }
    }
}