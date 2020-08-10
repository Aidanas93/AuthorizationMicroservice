using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Dto;
using AuthorizationMicroservice.Application.UsersService;

namespace AuthorizationMicroservice.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;

        public UsersController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(typeof(List<UserCredentialDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserCredentialDto>>> List()
        {
            return await unitOfWork.Mediator.Send(new List.Query());
        }

        [HttpGet("details/{id}")]
        [ProducesResponseType(typeof(UserCredentialDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<UserCredentialDto>> Details([FromHeaderAttribute] RequestHeaderDto header, [FromRoute] Guid id)
        {
            return await unitOfWork.Mediator.Send(new Details.Query { Id = id, JwtToken = header.Authorization });
        }

        [HttpPost("image-upload")]
        [Authorize]
        public async Task<AccessTokenDto> ImageUpload(
            [FromBody] ImageUpload.Command command,
            [FromHeaderAttribute] RequestHeaderDto header)
        {
            command.JwtToken = header.Authorization;
            return await unitOfWork.Mediator.Send(command);
        }

        [HttpPut("details")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccessTokenDto>> Edit(
            [FromHeaderAttribute] RequestHeaderDto header,
            [FromBody] Edit.Command command)
        {
            command.JwtToken = header.Authorization;
            return await unitOfWork.Mediator.Send(command);
        }

        [HttpPut("changePassword")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccessTokenDto>> EditPassword(
            [FromHeaderAttribute] RequestHeaderDto header,
            [FromBody] EditPassword.Command command)
        {
            command.JwtToken = header.Authorization;
            return await unitOfWork.Mediator.Send(command);
        }

        [HttpPut("admin-edit")]
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Unit>> SuperEdit(
            [FromHeaderAttribute] RequestHeaderDto header,
            [FromBody] SuperAdmin.Command command)
        {
            command.JwtToken = header.Authorization;
            return await unitOfWork.Mediator.Send(command);
        }

        [HttpDelete()]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Unit>> Delete(
            [FromBody] Delete.Command command,
            [FromHeaderAttribute] RequestHeaderDto header)
        {
            command.JwtToken = header.Authorization;
            return await unitOfWork.Mediator.Send(command);
        }


    }
}