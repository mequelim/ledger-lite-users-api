using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Create;
using Users.Application.Features.BankAccountFeatures.Delete;
using Users.Application.Features.BankAccountFeatures.Get.GetAll;
using Users.Application.Features.BankAccountFeatures.Get.GetByBankName;
using Users.Application.Features.BankAccountFeatures.Get.GetById;
using Users.Application.Features.BankAccountFeatures.Get.GetByUserId;
using Users.Application.Features.BankAccountFeatures.Get.GetByUserName;
using Users.Application.Features.BankAccountFeatures.Update;

namespace Users.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController(IMediator mediator) : ControllerBase
    {
        // GET:
        [HttpGet("GetAllBankAccounts")]
        public async Task<IActionResult> GetAllBankAccounts(CancellationToken cancellationToken)
        {
            Result<GetAllBankAccountsResponse> response = await mediator.Send(new GetAllBankAccountsQuery(), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetBankAccountById/{bankAccountId:guid}")]
        public async Task<IActionResult> GetBankAccountById(Guid bankAccountId, CancellationToken cancellationToken)
        {
            Result<GetBankAccountByIdResponse> response = await mediator.Send(new GetBankAccountByIdQuery(bankAccountId), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetBankAccountByUserId/{userId:guid}")]
        public async Task<IActionResult> GetBankAccountByUserId(Guid userId, CancellationToken cancellationToken)
        {
            Result<GetBankAccountByUserIdResponse> response = await mediator.Send(new GetBankAccountByUserIdQuery(userId), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetBankAccountByBankName/{bankName}")]
        public async Task<IActionResult> GetBankAccountByBankName(string bankName, CancellationToken cancellationToken)
        {
            Result<GetBankAccountByBankNameResponse> response = await mediator.Send(new GetBankAccountByBankNameQuery(bankName), cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetBankAccountByUserName/{userName}")]
        public async Task<IActionResult> GetBankAccountByUserName(string userName, CancellationToken cancellationToken)
        {
            Result<GetBankAccountByUserNameResponse> response = await mediator.Send(new GetBankAccountByUserNameQuery(userName), cancellationToken);

            return Ok(response);
        }

        // CREATE:
        [HttpPost("CreateBankAccount")]
        public async Task<IActionResult> CreateBankAccount([FromBody] CreateBankAccountCommand command, CancellationToken cancellationToken)
        {
            Result<CreateBankAccountResponse> response = await mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        // UPDATE:
        [HttpPatch("UpdateBankAccount/{bankAccountId:guid}")]
        public async Task<IActionResult> UpdateBankAccount(Guid bankAccountId, [FromBody] UpdateBankAccountCommand command, CancellationToken cancellationToken)
        {
            UpdateBankAccountCommand request = command with
            {
                Id = bankAccountId
            };

            Result<UpdateBankAccountResponse> response = await mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        // DELETE:
        [HttpDelete("DeleteBankAccount/{bankAccountId:guid}")]
        public async Task<IActionResult> DeleteBankAccount(Guid bankAccountId, CancellationToken cancellationToken)
        {
            DeleteBankAccountCommand request = new(bankAccountId);

            Result<DeleteBankAccountResponse> response = await mediator.Send(request, cancellationToken);

            return Ok(response);
        }
    }
}