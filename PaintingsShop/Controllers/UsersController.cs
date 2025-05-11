using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Entities;
using Services;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            IEnumerable<User> users = await _userService.GetUsers();
            if (users.Count() > 0)
                return Ok(users);
            return NoContent();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            User user = await _userService.getUserByID(id);
            if (user == null)
                return NoContent();
            return Ok(user);
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            int strength = _userService.GetPasswordStrength(user.Password);
            if(strength <2)
                return BadRequest("ppassword is too weak");
            User newUser = await _userService.SignUp(user);
            if (user == null)
                return BadRequest();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, newUser);
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<User>> Login(User user)
        {
            User newUser = await _userService.Login(user);
            if (user == null)
                return Unauthorized();
            return Ok(newUser);
        }

        [Route("password")]
        [HttpPost]
        public ActionResult<User> CheckPasswordStrength([FromBody] string password)
        {
            int strength = _userService.GetPasswordStrength(password);
            return Ok(strength);
        }

        //PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Put(int id, [FromBody] User user)
        {
            User newUser = await _userService.update(id, user);
            if (user == null)
                return NotFound();
            return Ok(user);

        }

        //DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
