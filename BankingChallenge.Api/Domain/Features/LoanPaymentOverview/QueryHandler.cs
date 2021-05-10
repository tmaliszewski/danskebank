using System;
using System.Threading;
using System.Threading.Tasks;
using BankingChallenge.Api.Domain.Common.Exceptions;
using MediatR;

namespace BankingChallenge.Api.Domain.Features.LoanPaymentOverview
{
    public partial class LoanPaymentOverview
    {
        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private const int NumberOfMonthsInYear = 12;

            //TODO: move to appsettings.json
            private readonly DomainConfiguration _domainConfiguration = new DomainConfiguration
            {
                MaxAdministrationFee = 10000,
                AdministrationFeePercentage = 1,
                AnnualInterestRatePercentage = 5
            };

            public Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                Validate(query);

                var annualInterestRate = _domainConfiguration.AnnualInterestRatePercentage / 100m;

                var monthlyInterestRate = annualInterestRate / NumberOfMonthsInYear;

                var durationOfLoanInMonths = query.DurationOfLoanInYears * NumberOfMonthsInYear;

                var monthlyPayment = CalculateMonthlyPayment((double) query.LoanAmount, (double) monthlyInterestRate, durationOfLoanInMonths);

                var result = new Result
                {
                    MonthlyPayment = monthlyPayment.RoundToCurrencyAmount(),
                    TotalInterest = CalculateTotalInterest(query.LoanAmount, monthlyInterestRate, durationOfLoanInMonths, monthlyPayment).RoundToCurrencyAmount(),
                    // already rounded, no need to round it again
                    AdministrationFee = CalculateAdministrationFee(query.LoanAmount)
                };
                return Task.FromResult(result);
            }

            private void Validate(Query query)
            {
                if (query.DurationOfLoanInYears <= 0)
                {
                    throw new ValidationException($"{nameof(Query.DurationOfLoanInYears)} must be greater than zero.");
                }
            }

            private decimal CalculateTotalInterest(decimal loanAmount, decimal monthlyInterestRate, int durationOfLoanInMonths, decimal monthlyPayment)
            {
                var totalInterest = 0m;

                var capitalLeft = loanAmount;

                for (var paymentMonth = 1; paymentMonth <= durationOfLoanInMonths; paymentMonth++)
                {
                    var monthlyInterest = capitalLeft * monthlyInterestRate;

                    totalInterest += monthlyInterest;

                    var currentCapital = monthlyPayment - monthlyInterest;

                    capitalLeft -= currentCapital;
                }

                return totalInterest;
            }

            private decimal CalculateAdministrationFee(decimal loanAmount)
            {
                var administrationFee = loanAmount * (_domainConfiguration.AdministrationFeePercentage / 100.0m);
                return Math.Min(_domainConfiguration.MaxAdministrationFee, administrationFee.RoundToCurrencyAmount());
            }

            // since there is no Math.Pow(...) implementation for decimal we are using double instead
            private decimal CalculateMonthlyPayment(double loanAmount, double monthlyInterestRate, int durationOfLoanInMonths)
            {
                // formula taken from: https://www.thebalance.com/loan-payment-calculations-315564 (Amortized Loan Payment Formula)
                var monthlyPayment = loanAmount / ((Math.Pow(1.0 + monthlyInterestRate, durationOfLoanInMonths) - 1.0) / (monthlyInterestRate * (Math.Pow(1.0 + monthlyInterestRate, durationOfLoanInMonths))));
                return (decimal) monthlyPayment;
            }
        }
    }
}