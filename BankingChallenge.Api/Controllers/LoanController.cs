using System.Threading.Tasks;
using BankingChallenge.Api.Domain.Features.LoanPaymentOverview;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingChallenge.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class LoanController
    {
        private readonly IMediator _mediator;

        public LoanController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        public Task<LoanPaymentOverview.Result> GetLoanPaymentOverview(LoanPaymentOverview.Query query)
            => _mediator.Send(query);
    }
}
