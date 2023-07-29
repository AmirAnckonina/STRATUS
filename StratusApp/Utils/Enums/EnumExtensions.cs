using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Enums
{
    public static class EnumExtensions
    {
        public static DisplayAttribute GetDisplayAttributesFrom(this Enum enumValue, Type enumType)
        {
            return enumType.GetMember(enumValue.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>();
        }

        public static eCurrencyType GetCurrencyType(string currencySymbol)
        {
            // Get all enum members of the CurrencyType enum
            foreach (eCurrencyType currencyType in Enum.GetValues(typeof(eCurrencyType)))
            {
                // Use reflection to get the Display attribute value for each enum member
                MemberInfo memberInfo = typeof(eCurrencyType).GetMember(currencyType.ToString())[0];
                DisplayAttribute displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null && displayAttribute.Name == currencySymbol)
                {
                    return currencyType;
                }
            }

            throw new ArgumentException("Invalid currency symbol");
        }
    }
}
