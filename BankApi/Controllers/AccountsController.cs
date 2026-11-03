using Microsoft.AspNetCore.Mvc;

using BankApi.Dtos;
using BankApi.Services;

namespace BankApi.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<AccountResponse>>> List(
        int page = 0,
        int pageSize = 10)
    {
        return Ok(await accountService.ListAsync(page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountResponse>> GetById(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);

        return account is null ? AccountNotFound() : Ok(account.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create(CreateAccountRequest request)
    {
        var account = await accountService.CreateAsync(request.Owner);

        return Created($"/accounts/{account.Id}", account.ToResponse());
    }

    [HttpPost("{id:guid}/deposits")]
    public async Task<ActionResult<AccountResponse>> Deposit(
        Guid id,
        AccountTransactionRequest request)
    {
        var account = await accountService.DepositAsync(id, request.Amount);

        return account is null ? AccountNotFound() : Ok(account.ToResponse());
    }

    [HttpPost("{id:guid}/withdrawals")]
    public async Task<ActionResult<AccountResponse>> Withdraw(
        Guid id,
        AccountTransactionRequest request)
    {
        var account = await accountService.WithdrawAsync(id, request.Amount);

        return account is null ? AccountNotFound() : Ok(account.ToResponse());
    }

    private NotFoundObjectResult AccountNotFound() =>
        NotFound(new { error = "account not found." });
}
