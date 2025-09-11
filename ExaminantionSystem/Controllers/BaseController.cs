using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminantionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly IMapper _mapper;
        public BaseController(IMapper mapper) 
        {
            mapper = _mapper;

        }
    }
}
