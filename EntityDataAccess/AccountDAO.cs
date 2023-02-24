using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDataAccess
{
    public class AccountDAO : IAccountDAO
    {
        private AccountDAO() { }

        public bool DeleteAccount(string username)
        {
            using var context = new CanteenContext();

            context.Accounts.Remove(new Account { Username = username });

            if (context.SaveChanges() == 1) return true;
            return false;
        }

        public List<Account> GetListAccount()
        {
            using var context = new CanteenContext();

            return context.Accounts.ToList();
        }

        public Account InsertAccount(string username)
        {
            using var context = new CanteenContext();
            if (context.Find<Account>(username) != null) return null;
            var account = new Account { Username = username };
            context.Accounts.Add(account);
            if (context.SaveChanges() == 1) return account;
            return null;
        }

        public Account Login(string username, string password)
        {
            using var context = new CanteenContext();

            return context.Accounts.FirstOrDefault(a => a.Username == username && a.Password == password);
        }

        public (bool, bool, bool) Update(string username, string displayname = null, string password = null, AccountType? type = null)
        {
            using var context = new CanteenContext();

            var account = context.Accounts.FirstOrDefault(a => a.Username == username);

            if (account == null) return (false, false, false);

            var result1 = false;
            if (!string.IsNullOrEmpty(displayname))
            {
                account.DisplayName = displayname;

                result1 = true;
            }

            var result2 = false;
            if (!string.IsNullOrEmpty(password))
            {
                account.Password = password;

                result2 = true;
            }

            var result3 = false;
            if (type != null)
            {
                account.Type = (AccountType)type;

                result3 = true;
            }

            context.SaveChanges();

            return (result1, result2, result3);
        }
    }
}
