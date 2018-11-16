using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models.PagSeguro
{
    public static class ListPaymentMethodGroups
    {
        /// <summary>
        /// PagSeguro Balance
        /// </summary>
        public const string Balance = "BALANCE";

        /// <summary>
        /// Boleto
        /// </summary>
        public const string Boleto = "BOLETO";

        /// <summary>
        /// Credit card
        /// </summary>
        public const string CreditCard = "CREDIT_CARD";

        /// <summary>
        /// Deposit
        /// </summary>
        public const string Deposit = "DEPOSIT";

        /// <summary>
        /// Online debit
        /// </summary>
        public const string ETF = "EFT";
    }
}