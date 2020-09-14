using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FamilyFinances.Data;
using FamilyFinances.Models;

namespace FamilyFinances.Helpers
{
    public static class Money
    {
        public static void Spend(FamilyFinancesContext context, Expense expense)
        {
            var paySource = context.PaySources.Find(expense.PaySourceID);
            SetNewBalance(context, paySource, paySource.Balance - expense.Sum);
        }

        public static void RemoveSpend(FamilyFinancesContext context, Expense expense)
        {
            var paySource = context.PaySources.Find(expense.PaySourceID);
            SetNewBalance(context, paySource, paySource.Balance + expense.Sum);
        }

        public static void Earn(FamilyFinancesContext context, Inpayment inpayment)
        {
            var paySource = context.PaySources.Find(inpayment.PaySourceID);
            SetNewBalance(context, paySource, paySource.Balance + inpayment.Sum);
        }

        public static void RemoveEarn(FamilyFinancesContext context, Inpayment inpayment)
        {
            var paySource = context.PaySources.Find(inpayment.PaySourceID);
            SetNewBalance(context, paySource, paySource.Balance - inpayment.Sum);
        }

        public static void CheckExpenseSumAndAjustPaySource(FamilyFinancesContext context, Expense expense)
        {
            var originalSum = (decimal)context.Entry(expense).GetDatabaseValues().GetValue<object>("Sum");
            var currentSum = context.Entry(expense).Property(x => x.Sum).CurrentValue;
            PaySource paySource = context.PaySources.Find(expense.PaySourceID);
            SetNewBalance(context, paySource, paySource.Balance + originalSum - currentSum);
        }

        public static void CheckInpaymentSumAndAjustPaySource(FamilyFinancesContext context, Inpayment inpayment)
        {
            var originalSum = (decimal)context.Entry(inpayment).GetDatabaseValues().GetValue<object>("Sum");
            var currentSum = context.Entry(inpayment).Property(x => x.Sum).CurrentValue;
            PaySource paySource = context.PaySources.Find(inpayment.PaySourceID);
            SetNewBalance(context, paySource, paySource.Balance - originalSum + currentSum);
        }

        private static void SetNewBalance(FamilyFinancesContext context, PaySource paySource, decimal newBalance)
        {
            int? group = paySource.Group;
            if (group != null)
            {
                foreach (var ps in context.PaySources.Where(p => p.Group == group))
                {
                    ps.Balance = newBalance;
                }
            }
            else
            {
                paySource.Balance = newBalance;
            }
        }
    }
}
