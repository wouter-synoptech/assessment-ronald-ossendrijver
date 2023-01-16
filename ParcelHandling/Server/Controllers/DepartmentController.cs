using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ParcelHandling.Server.Managers;
using ParcelHandling.Shared;

namespace ParcelHandling.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration, ILogger<DepartmentController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<Department> Get()
        {
            return DepartmentManager.GetDepartments(_configuration);
        }
    }
}