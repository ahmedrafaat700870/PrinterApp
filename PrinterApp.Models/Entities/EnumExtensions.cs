using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PrinterApp.Models.Entities
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var displayAttribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? enumValue.ToString();
        }

        public static string GetDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());

            if (field == null)
                return enumValue.ToString();

            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Description ?? enumValue.ToString();
        }
    }
}