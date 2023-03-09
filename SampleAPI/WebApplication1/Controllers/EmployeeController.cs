using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Graph;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "SupplierClient")]
    [ApiController]
    [Route("[controller]")]
    //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class EmployeeController : ControllerBase
    {

        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/employees")]
        [Produces("application/json")]
        public async Task<IEnumerable<Employee>> Get()
        {
            var user = User;

            return Enumerable.Range(1, 5).Select(index => new Employee
            {
                EmployeeId = index,
                FirstName = "FirstName-" + index,
                LastName = "LastName-" + index
            })
            .ToArray();
        }
    }
}