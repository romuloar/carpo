using Carpo.Core.Interface.Idx;

namespace Carpo.Core.Domain.Idx
{
    public class BaseIdx : IIdx
    {
        public string IdxDescription { get; set; }

        public string IdxValue { get; set; }
    }
}
