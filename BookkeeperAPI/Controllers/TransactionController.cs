namespace BookkeeperAPI.Controllers
{
    #region usings
    using BookkeeperAPI.Data;
    using BookkeeperAPI.Entity;
    using BookkeeperAPI.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using BookkeeperAPI.Constants;
    using BookkeeperAPI.Utility;
    using System.ComponentModel;
    using System;
    using BookkeeperAPI.Service.Interface;
    using BookkeeperAPI.Service;
    using System.ComponentModel.DataAnnotations;
    using BookkeeperAPI.Exceptions;
    using Microsoft.AspNetCore.Authorization;
    #endregion

    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(BookkeeperContext context, ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("/api/me/transactions")]
        [ProducesDefaultResponseType(typeof(PaginatedResult<TransactionView>))]
        [ProducesErrorResponseType(typeof(ErrorResponseModel))]
        public async Task<ActionResult<PaginatedResult<TransactionView>>> GetTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25, [FromQuery] ExpenseCategory? category = null, [FromQuery] string? name = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null, [FromQuery] TransactionType? type = null)
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            string domain = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path;

            PaginatedResult<TransactionView> result = await _transactionService.GetPaginatedTransactionsAsync(userId, domain, pageNumber, pageSize, category, name, from, to, type);

            return result;
        }

        [HttpGet("/api/transactions/{transactionId}")]
        [ProducesDefaultResponseType(typeof(TransactionView))]
        [ProducesErrorResponseType(typeof(ErrorResponseModel))]
        public async Task<ActionResult<TransactionView>> GetTransactionById(Guid transactionId)
        {
            return Ok(await _transactionService.GetTransactionByIdAsync(transactionId));
        }

        [HttpPost("/api/transactions")]
        [ProducesDefaultResponseType(typeof(TransactionView))]
        [ProducesErrorResponseType(typeof(ErrorResponseModel))]
        public async Task<ActionResult<TransactionView>> CreateTransaction([FromBody][Required] CreateTransactionRequest transaction)
        {
            Guid userId;
            string userIdClaim = HttpContext.User.Claims.Where(x => x.Type == "user_id").First().Value.ToString();
            bool isValidUserId = Guid.TryParse(userIdClaim, out userId);

            if (!isValidUserId)
            {
                throw new HttpOperationException(401, "Unauthorized");
            }

            TransactionView result = await _transactionService.CreateTransactionAsync(userId, transaction);

            return Ok(result);
        }

        [HttpPatch("/api/transactions/{transactionId}")]
        [ProducesDefaultResponseType(typeof(TransactionView))]
        [ProducesErrorResponseType(typeof(NotFoundResult))]
        public async Task<ActionResult<TransactionView>> UpdateTransaction(Guid transactionId, UpdateTransactionRequest transaction)
        {
            TransactionView result = await _transactionService.UpdateTransactionAsync(transactionId, transaction);

            return Ok(result);
        }

        [HttpDelete("/api/transactions/{transactionId}")]
        [ProducesDefaultResponseType(typeof(ResponseModel))]
        [ProducesErrorResponseType(typeof(NotFoundResult))]
        public async Task<ActionResult<ResponseModel>> DeleteTransaction(Guid expenseId)
        {
            await _transactionService.DeleteTransactionAsync(expenseId);

            return Ok(new ResponseModel()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Transaction deleted successfully"
            });
        }

    }
}
