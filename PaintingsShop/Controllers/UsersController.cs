using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
//using Entities;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaintingsShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        UserService userService = new UserService();

        // GET: api/<UsersController>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            List<User> users = userService.GetUser();
            if (users.Count > 0)
                return Ok(users);
            return NoContent();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            User user = userService.getUserByID(id);
            if (user == null)
                return NoContent();
            return Ok(user);
        }

        // POST api/<UsersController>
        [HttpPost]
        public IActionResult SignUp([FromBody] User user)
        {
            User newUser = userService.SignUp(user);
            if (user == null)
                return BadRequest();
            return Ok(user);
        }

        [Route("login")]
        [HttpPost]
        public ActionResult<User> Login(User user)
        {
            User newUser = userService.Login(user);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        //PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public ActionResult<User> Put(int id, [FromBody] User user)
        {
            User newUser = userService.update(id, user);
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
