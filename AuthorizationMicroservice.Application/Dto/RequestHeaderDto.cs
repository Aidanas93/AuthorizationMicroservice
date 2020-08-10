using Microsoft.AspNetCore.Mvc;

namespace AuthorizationMicroservice.Application.Dto
{
    public class RequestHeaderDto
    {
        [FromHeader]
        public string Authorization { get; set; }
    }

}
