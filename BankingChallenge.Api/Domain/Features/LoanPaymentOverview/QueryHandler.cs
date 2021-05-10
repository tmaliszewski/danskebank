using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BankingChallenge.Api.Domain.Features.LoanPaymentOverview
{
    public partial class LoanPaymentOverview
    {
        public class QueryHandler : IRequestHandler<Query, Result>
        {
            public Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                //TODO:
                return Task.FromResult(new Result());
            }
        }
    }
}