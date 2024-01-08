using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Carpo.Core.Domain.Attributes.Sql
{
    public enum FieldValueType
    {
        String,
        Int,
        Function,
        DateTime,
        DateTimeNow,
        Getdate
    }

    public class FieldDefaultValueAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public FieldValueType FieldDefaultValueType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FieldDefaultValueAttribute(string value, string formatDateTime, string codeCulture)
        {
            FieldDefaultValueType = FieldValueType.DateTime;
            if (!(string.IsNullOrEmpty(value) && string.IsNullOrEmpty(formatDateTime)))
            {
                Value = DateTime.ParseExact(value, formatDateTime, CultureInfo.GetCultureInfo(codeCulture));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldDefaultValueAttribute(string value, FieldValueType valueType)
        {
            FieldDefaultValueType = valueType;

            if (valueType != FieldValueType.DateTime && !string.IsNullOrEmpty(value))
            {
                Value = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldDefaultValueAttribute(FieldValueType valueType)
        {
            if (valueType == FieldValueType.Getdate)
            {
                Value = "getdate()";
            }
            if (valueType == FieldValueType.DateTimeNow)
            {
                Value = DateTime.Now;
            }

            FieldDefaultValueType = valueType;
        }

        /// <summary>
        /// 
        /// </summary>
        public FieldDefaultValueAttribute(int value)
        {
            FieldDefaultValueType = FieldValueType.Int;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (FieldDefaultValueType != FieldValueType.Int && (Value == null || string.IsNullOrEmpty(Value.ToString())))
            {
                var val = new ValidationResult("FieldDefaultValueAttribute - String empty");
                return val;
            }

            PropertyInfo property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            // Set default value only if no value is already specified
            if (property.PropertyType.Name == "DateTime"
                && value != null
                && DateTime.Parse(value.ToString()) == DateTime.MinValue)
            {
                if (FieldValueType.Getdate == FieldDefaultValueType || FieldValueType.DateTimeNow == FieldDefaultValueType)
                {
                    DateTime defaultValue = DateTime.Now;
                    property.SetValue(validationContext.ObjectInstance, defaultValue);
                }

                if (FieldValueType.DateTime == FieldDefaultValueType)
                {
                    property.SetValue(validationContext.ObjectInstance, Value);
                }
            }

            //Set string default value
            if (FieldDefaultValueType == FieldValueType.String && (value == null || string.IsNullOrEmpty(value.ToString())))
            {
                property.SetValue(validationContext.ObjectInstance, Value);
            }

            return ValidationResult.Success;
        }

    }
}
