namespace BankingChallenge.Api.Domain.Features.LoanPaymentOverview
{
    public partial class LoanPaymentOverview
    {
        public class Result
        {
            public decimal MonthlyPayment { get; set; }

            public decimal TotalInterest { get; set; }

            public decimal AdministrationFee { get; set; }
        }
    }
}