using BookkeeperAPI.Exceptions;
using BookkeeperAPI.Model;
using BookkeeperAPI.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookkeeperAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class IncomeController : Controller
    {
        private readonly IIncomeService _incomeService;

        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        [HttpGet("/api/me/income")]
        public async Task<ActionResult<IEnumerable<IncomeView>>> GetAllIncomes()
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            return Ok(await _incomeService.GetAllIncomesAsync(userId));
        }

        [HttpPost("/api/me/income")]
        public async Task<ActionResult<IncomeView>> AddIncome([Required][FromBody] AddIncomeRequest request)
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            IncomeView income = await _incomeService.AddIncomeAsync(userId, request);
            return CreatedAtAction(null, income);
        }

        [HttpPatch("/api/me/income")]
        public async Task<ActionResult<IncomeView>> UpdateIncome([Required][FromQuery] string name, [Required][FromBody] UpdateIncomeRequest request)
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            IncomeView income = await _incomeService.UpdateIncomeAsync(userId, name, request);
            return Ok(income);
        }

        [HttpDelete("/api/me/income")]
        public async Task<ActionResult> RemoveIncome([Required][FromQuery] string name)
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            await _incomeService.RemoveIncomeAsync(userId, name);
            return Ok(new ResponseModel()
            {
                Message = "Income removed successfully",
                StatusCode = StatusCodes.Status200OK
            });
        }
    }
}
