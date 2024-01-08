namespace Carpo.Core.ResultState
{
    public enum StateTypeCore
    {
        Success,
        Error,
        Info
    }
    public class ResultStateCore<TEntity> : ResultStateCore
    {
        public TEntity? ResultData { get; set; }
    }

    public class ResultStateCore
    {
        public StateTypeCore StateType { get; set; }
        public string StateTypeName { get { return StateType.ToString(); } }
        public bool IsSuccess { get { return StateType == StateTypeCore.Success || StateType == StateTypeCore.Info; } }
        public string? Message { get; set; }
        public string Date { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
