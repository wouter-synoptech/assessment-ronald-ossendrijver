using Microsoft.AspNetCore.Mvc;
using ParcelHandling.Server.Managers;
using ParcelHandling.Shared;

namespace ParcelHandling.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(ILogger<DepartmentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Department> Get()
        {
            IEnumerable<Department> departments = new List<Department>();
            
            try
            {
                using (StreamReader file = new("departmentconfig.txt"))
                {
                    departments = DepartmentManager.Create(file).Targets;
                }
            }
            catch (Exception)
            {
                departments = Array.Empty<Department>();
            }

            return departments;
        }
    }
}