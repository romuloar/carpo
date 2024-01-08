using System.ComponentModel.DataAnnotations;

namespace Carpo.Core.Domain.Attributes
{
    public class StringEqualsAttribute : ValidationAttribute
    {
        private int _valueLength;
        public StringEqualsAttribute(int valueLength)
        {
            _valueLength = valueLength;
        }
        public override bool IsValid(object value)
        {
            var str = value as String;
            if (value != null)
            {
                return str.Length == _valueLength;
            }
            return false;
        }
    }
}
