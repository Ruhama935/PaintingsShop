using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Services;
using System.Threading.Tasks;
using DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860//

namespace PaintingsShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        
        //clean code - use meaningful names in all functions

        // GET: api/<UsersController>//
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            IEnumerable<UserDTO> users = await _userService.GetUsers();
            if (users.Count() > 0)
                return Ok(users);
            return NoContent();
            //users.Count() > 0 ? return Ok(users) : return NoContent();
        }

        // GET api/<UsersController>/5//
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            UserDTO user = await _userService.getUserByID(id);
            if (user == null)
                return NoContent();
            return Ok(user);
            //return user != null ? Ok(user) : NoContent(); //use in all functions
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] UserDTO user)
        {
            int strength = _userService.GetPasswordStrength(user.Password);
            if(strength <2)
                return BadRequest("ppassword is too weak");
            UserDTO newUser = await _userService.SignUp(user);
            if (user == null)
                return BadRequest();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, newUser);
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Login(UserDTO user)
        {
            UserDTO newUser = await _userService.Login(user);
            if (user == null)
                return Unauthorized();
            return Ok(newUser);
        }

        [Route("password")]
        [HttpPost]
        public ActionResult<UserDTO> CheckPasswordStrength([FromBody] string password)
        {
            int strength = _userService.GetPasswordStrength(password);
            return Ok(strength);
        }

        //PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> Put(int id, [FromBody] UserDTO user)
        {
            UserDTO newUser = await _userService.update(id, user);
            if (user == null)
                return NotFound();
            return Ok(user);

        }
    }
}
