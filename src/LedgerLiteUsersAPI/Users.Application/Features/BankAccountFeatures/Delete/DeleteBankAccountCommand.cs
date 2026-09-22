using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities.Enums;

namespace Users.Application.Features.BankAccountFeatures.Delete
{
    public sealed record DeleteBankAccountCommand(Guid Id) : IRequest<Result<DeleteBankAccountResponse>>;
}