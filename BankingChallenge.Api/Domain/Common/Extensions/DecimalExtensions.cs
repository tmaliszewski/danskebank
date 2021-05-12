using System;

namespace BankingChallenge.Api.Domain.Features.LoanPaymentOverview
{
    public static class DecimalExtensions
    {
        public static decimal RoundToCurrencyAmount(this decimal amount)
        {
            // banker's rounding rounds to the nearest even integer
            // https://wiki.c2.com/?BankersRounding
            // http://www.xbeat.net/vbspeed/i_BankersRounding.htm

            return Math.Round(amount, 2, MidpointRounding.ToEven);
        }

        // since there is no Math.Pow(...) implementation for decimal we are casting parameters to double
        public static decimal Pow(this decimal x, decimal y)
        {
            return (decimal) Math.Pow((double) x, (double) y);
        }
    }
}