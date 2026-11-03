using Microsoft.AspNetCore.Mvc;

using BankApi.Dtos;
using BankApi.Services;

namespace BankApi.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountResponse>> GetById(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);

        if (account is null)
        {
            return NotFound(new { error = "account not found." });
        }

        return Ok(account.ToResponse());
    }
}
