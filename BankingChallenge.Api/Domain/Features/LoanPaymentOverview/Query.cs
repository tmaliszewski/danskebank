using MediatR;

namespace BankingChallenge.Api.Domain.Features.LoanPaymentOverview
{
    public partial class LoanPaymentOverview
    {
        public class Query : IRequest<Result>
        {
            public decimal LoanAmount { get; set; }

            public int DurationOfLoanInMonths { get; set; }
        }
    }
}