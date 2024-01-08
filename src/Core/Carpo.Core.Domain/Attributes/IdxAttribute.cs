using Carpo.Core.Interface.Idx;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Carpo.Core.Domain.Attributes
{
    public class IdxAttribute : ValidationAttribute
    {
        public List<IIdx> ListIdx { get { return _listIdx; } }

        private List<IIdx> _listIdx;
        public IdxAttribute(Type type)
        {
            if (type.GetInterface("IIdxField") == null)
            {
                throw new ArgumentException("A entidade necessita implementar IIdxField");
            }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            PropertyInfo property = type.GetProperty("ListIdx");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            var objTemp = Activator.CreateInstance(type);

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _listIdx = (List<IIdx>)property.GetValue(objTemp, null);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8601 // Possible null reference assignment.
        }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override bool IsValid(object value)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {
            return _listIdx.Any(b => value != null && b.IdxValue == value.ToString());
        }
    }
}
