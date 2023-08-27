using DataAccess.Api.Domain;
using DatatAccess.Ef.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DataAccess.Api.Controllers
{
    [ApiController]
    [Route("api/v1/User")]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _user;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _user = _unitOfWork.GetRepository<User>();
        }


        [HttpPost]
        public IActionResult GetUsers()
        {
            var flowTypes = _user.GetAll(); // یک مثال از استفاده از _flowType

            if (flowTypes == null)
            {
                return BadRequest();
            }

            return Ok(flowTypes);
        }
    }
}
