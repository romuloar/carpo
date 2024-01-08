using System;
using System.ComponentModel;

namespace Carpo.Core.Extension
{
    public static class EnumCore
    {
        public static String GetDescrition(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var descriptionAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descriptionAttribute == null ? value.ToString() : descriptionAttribute.Description;
        }
    }
}
