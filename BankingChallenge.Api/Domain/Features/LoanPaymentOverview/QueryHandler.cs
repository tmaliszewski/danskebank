using System;
using System.Threading;
using System.Threading.Tasks;
using BankingChallenge.Api.Domain.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace BankingChallenge.Api.Domain.Features.LoanPaymentOverview
{
    public partial class LoanPaymentOverview
    {
        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private const int NumberOfMonthsInYear = 12;

            private readonly DomainConfiguration _domainConfiguration;

            public QueryHandler(IOptions<DomainConfiguration> domainConfigurationOptions)
            {
                _domainConfiguration = domainConfigurationOptions.Value;
            }

            public Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                Validate(query);

                var annualInterestRate = _domainConfiguration.AnnualInterestRatePercentage / 100m;

                var monthlyInterestRate = annualInterestRate / NumberOfMonthsInYear;

                var durationOfLoanInMonths = query.DurationOfLoanInYears * NumberOfMonthsInYear;

                var monthlyPayment = CalculateMonthlyPayment(query.LoanAmount, monthlyInterestRate, durationOfLoanInMonths);

                var result = new Result
                {
                    MonthlyPayment = monthlyPayment.RoundToCurrencyAmount(),
                    TotalInterest = CalculateTotalInterest(query.LoanAmount, durationOfLoanInMonths, monthlyPayment).RoundToCurrencyAmount(),
                    AdministrationFee = CalculateAdministrationFee(query.LoanAmount).RoundToCurrencyAmount()
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

            private decimal CalculateTotalInterest(decimal loanAmount, int durationOfLoanInMonths, decimal monthlyPayment)
            {
                var totalPayments = durationOfLoanInMonths * monthlyPayment;
                return totalPayments - loanAmount;
            }

            private decimal CalculateAdministrationFee(decimal loanAmount)
            {
                var administrationFee = loanAmount * (_domainConfiguration.AdministrationFeePercentage / 100m);
                return Math.Min(_domainConfiguration.MaxAdministrationFee, administrationFee);
            }

            private decimal CalculateMonthlyPayment(decimal loanAmount, decimal monthlyInterestRate, int durationOfLoanInMonths)
            {
                // formula taken from: https://www.thebalance.com/loan-payment-calculations-315564 (Amortized Loan Payment Formula)
                var monthlyPayment = loanAmount / (((1m + monthlyInterestRate).Pow(durationOfLoanInMonths) - 1m) / (monthlyInterestRate * (1m + monthlyInterestRate).Pow(durationOfLoanInMonths)));
                return monthlyPayment;
            }
        }
    }
}