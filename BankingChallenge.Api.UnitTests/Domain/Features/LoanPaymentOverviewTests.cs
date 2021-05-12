using BankingChallenge.Api.Domain;
using BankingChallenge.Api.Domain.Features.LoanPaymentOverview;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingChallenge.Api.Domain.Common.Exceptions;
using Xunit;

namespace BankingChallenge.Api.UnitTests.Domain.Features
{
    public class LoanPaymentOverviewTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            // Banking Challenge example
            yield return new object[]
            {
                new DomainConfiguration
                {
                    MaxAdministrationFee = 10000,
                    AdministrationFeePercentage = 1,
                    AnnualInterestRatePercentage = 5
                },

                new LoanPaymentOverview.Query
                {
                    LoanAmount = 500000,
                    DurationOfLoanInYears = 10
                },

                new LoanPaymentOverview.Result
                {
                    MonthlyPayment =  5303.28m,
                    TotalInterest =  136393.09m,
                    AdministrationFee = 5000m
                }
            };

            // example from: https://www.thebalance.com/loan-payment-calculations-315564
            yield return new object[]
            {
                new DomainConfiguration
                {
                    AnnualInterestRatePercentage = 6
                },

                new LoanPaymentOverview.Query
                {
                    LoanAmount = 100000,
                    DurationOfLoanInYears = 30
                },

                new LoanPaymentOverview.Result
                {
                    MonthlyPayment =  599.55m,
                    TotalInterest =  115838.19m
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task LoanPaymentOverviewFeature_Should_Calculate_Values_Properly(DomainConfiguration domainConfiguration, LoanPaymentOverview.Query query, LoanPaymentOverview.Result expectedResult)
        {
            // Arrange
            var domainConfigurationOptionsMock = new Mock<IOptions<DomainConfiguration>>();
            domainConfigurationOptionsMock.Setup(m => m.Value).Returns(domainConfiguration);

            var handler = new LoanPaymentOverview.QueryHandler(domainConfigurationOptionsMock.Object);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.Equal(expectedResult.MonthlyPayment, result.MonthlyPayment);
            Assert.Equal(expectedResult.TotalInterest, result.TotalInterest);
            Assert.Equal(expectedResult.AdministrationFee, result.AdministrationFee);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async Task LoanPaymentOverviewFeature_Should_Throw_ValidationException_For_Invalid_DurationOfLoanInYears(int durationOfLoanInYears)
        {
            // Arrange
            var domainConfiguration = new DomainConfiguration
            {
                MaxAdministrationFee = 10000,
                AdministrationFeePercentage = 1,
                AnnualInterestRatePercentage = 5
            };

            var domainConfigurationOptionsMock = new Mock<IOptions<DomainConfiguration>>();
            domainConfigurationOptionsMock.Setup(m => m.Value).Returns(domainConfiguration);

            var handler = new LoanPaymentOverview.QueryHandler(domainConfigurationOptionsMock.Object);

            var query = new LoanPaymentOverview.Query
            {
                LoanAmount = 500000,
                DurationOfLoanInYears = durationOfLoanInYears
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(query, default));
        }
    }
}
