using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public BaseController() 
        {
           

        }

        public int GetCurrentUserId()
        {
            if(User.Identity.IsAuthenticated)
              return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return 0;
        }
    }
}
