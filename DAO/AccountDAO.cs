using Microsoft.EntityFrameworkCore;
using Project212.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project212.DAO
{
    class AccountDAO
    {
        private readonly Prn212AssignmentContext context;

        public AccountDAO()
        {
            context = new Prn212AssignmentContext();
        }

        public List<Account> GetAllAccountsWithCitizens()
        {
            return context.Accounts.Include(a => a.Citizens).ToList();
        }

        public void UpdateAccount(Account account)
        {
            context.Accounts.Update(account);
            context.SaveChanges();
        }

        public void AddAccount(Account account)
        {
            context.Accounts.Add(account);
            context.SaveChanges();
        }
    }
}
