using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Trạng thái sử dụng.
    /// </summary>
    [TypeConverter(typeof(UsingStateConverter))]
    public enum UsingState
    {
        /// <summary>
        /// Đang được phục vụ.
        /// </summary>
        [Description("Đang bán")]
        Serving = 1,

        /// <summary>
        /// Đã dừng phục vụ.
        /// </summary>
        [Description("Dừng bán")]
        StopServing = 0
    }

    public class UsingStateConverter : EnumConverter
    {
        public UsingStateConverter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] Type type) : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                FieldInfo info = typeof(UsingState).GetField(value.ToString());
                DescriptionAttribute description = info?.GetCustomAttribute<DescriptionAttribute>();

                return description?.Description;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

}
