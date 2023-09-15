namespace BookkeeperAPI.Controllers
{

    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using Microsoft.AspNetCore.Mvc;
    #endregion

    [ApiController]
    [Route("/api/v1/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private BookkeeperContext _context;
        public UserController(BookkeeperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUser()
        {
            return Ok(_context.Users.ToList());
        }

        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] CreateUserRequest request)
        {
            User user = new User();
            user.Preference = request.UserPreference;
            var u = _context.Users.Add(user);
            user.Credential = new UserCredential()
            {
                User = u.Entity,
                UserId = u.Entity.Id,
                DisplayName = request.DisplayName,
                Password = request.Password,
                Email = request.Email,
                LastUpdated = DateTime.UtcNow,
                CreatedTime = DateTime.UtcNow,
            };
            _context.Credentials.Add(user.Credential);
            _context.SaveChanges();
            return StatusCode(201, u.Entity);
        }
    }
}
