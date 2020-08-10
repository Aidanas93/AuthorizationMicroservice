using System.Threading.Tasks;
using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.UsersService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationMicroservice.API.Controllers
{
    [Route("[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;

        public RegistrationController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Unit>> Register([FromBody] Register.Command command)
        {
            return await unitOfWork.Mediator.Send(command);
        }

    }
}
