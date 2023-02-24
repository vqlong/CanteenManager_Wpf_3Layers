using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Models
{
    /// <summary>
    /// Bảng Account của database.
    /// </summary>
    public class Account
    {
        public Account()
        {

        }
        public Account(string username, string displayname, AccountType type, string password = null)
        {
            Username = username;
            DisplayName = displayname;
            Type = type;
            Password = password;
        }

        public Account(DataRow row)
        {
            Username = row["Username"].ToString();
            DisplayName = row["DisplayName"].ToString();
            Type = (AccountType)row["Type"];
            //this.PassWord = row["Password"].ToString();
        }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public AccountType Type { get; set; }
    }

    [TypeConverter(typeof(AccountTypeConverter))]
    public enum AccountType
    {
        [Description("Quản lý")]
        Admin = 1,
        [Description("Nhân viên")]
        Staff = 0
    }

    public class AccountTypeConverter : EnumConverter
    {
        public AccountTypeConverter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] Type type) : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                FieldInfo info = typeof(AccountType).GetField(value.ToString());
                DescriptionAttribute description = info?.GetCustomAttribute<DescriptionAttribute>();

                return description?.Description;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
