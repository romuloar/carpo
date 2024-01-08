namespace Carpo.Core.Interface.Domain
{
    public interface IValidationDomain
    {
        public bool IsValidDomain { get; }
        public object ListValidationResult { get; }
    }
}
