namespace BookkeeperAPI.Controllers
{

    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
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
        public ActionResult<List<User>> GetUser()
        {
            return Ok(_context.Users.ToList());
        }
    }
}
