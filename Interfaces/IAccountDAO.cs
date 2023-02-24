using Models;

namespace Interfaces
{
    public interface IAccountDAO
    {
        bool DeleteAccount(string username);
        List<Account> GetListAccount();
        Account InsertAccount(string username);
        Account Login(string username, string password);

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
        (bool, bool, bool) Update(string username, string displayname = null, string password = null, AccountType? type = null);
    }
}