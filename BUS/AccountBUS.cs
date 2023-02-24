using Interfaces;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace BUS
{
    public class AccountBUS
    {
        private AccountBUS()
        {

        }

        private static readonly AccountBUS instance = new AccountBUS();
        public static AccountBUS Instance => instance;
        IAccountDAO Account => Config.Container.Resolve<IAccountDAO>();

        public bool DeleteAccount(string username) => Account.DeleteAccount(username);
        public List<Account> GetListAccount() => Account.GetListAccount();
        public Account InsertAccount(string username) => Account.InsertAccount(username);
        public Account Login(string username, string password) => Account.Login(username, password);
        /// <summary>
        /// Update Account.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="displayname"></param>
        /// <param name="password"></param>
        /// <returns>
        /// Item1 - Kết quả cập nhật DisplayName.
        /// <br>Item2 - Kết quả cập nhật Password.</br>
        /// <br>Item3 - Kết quả cập nhật Type.</br>
        /// </returns>
        public (bool, bool, bool) Update(string username, string displayname = null, string password = null, AccountType? type = null) => Account.Update(username, displayname, password, type);
    }
}
