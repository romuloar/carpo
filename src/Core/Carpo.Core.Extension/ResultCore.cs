using Carpo.Core.ResultState;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Carpo.Core.Extension
{
    /// <summary>
    /// Result extension
    /// </summary>
    public static class ResultCore
    {
        #region Success

        /// <summary>
        /// GetResultStateSuccess
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultStateSuccess<TEntity>(this TEntity result, string? message, string? date)
        {
            return GetResultState(result, StateTypeCore.Success, message, date);
        }

        /// <summary>
        /// GetResultStateSuccess
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateSuccessAsync<TEntity>(this TEntity result, string? message, string? date)
        {
            return await GetResultStateAsync(result, StateTypeCore.Success, message, date);
        }

        /// <summary>
        /// GetResultStateSuccess
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultStateSuccess<TEntity>(this TEntity result, string message)
        {
            return GetResultState(result, StateTypeCore.Success, message, null);
        }

        /// <summary>
        /// GetResultStateSuccessAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateSuccessAsync<TEntity>(this TEntity result, string message)
        {
            return await GetResultStateAsync(result, StateTypeCore.Success, message, null);
        }

        /// <summary>
        /// GetResultStateSuccess
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultStateSuccess<TEntity>(this TEntity result)
        {
            return GetResultStateSuccess(result, null, null);
        }

        /// <summary>
        /// GetResultStateSuccessAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateSuccessAsync<TEntity>(this TEntity result)
        {
            return await GetResultStateSuccessAsync(result, null, null);
        }

        #endregion

        #region Info

        /// <summary>
        /// GetResultStateInfo
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultStateInfo<TEntity>(this TEntity result, string message)
        {
            return GetResultState(result, StateTypeCore.Info, message, null);
        }

        /// <summary>
        /// GetResultStateInfo
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateInfoAsync<TEntity>(this TEntity result, string message)
        {
            return await GetResultStateAsync(result, StateTypeCore.Info, message, null);
        }

        /// <summary>
        /// GetResultStateInfo
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultStateInfo<TEntity>(this TEntity result, string message, string date)
        {
            return GetResultState(result, StateTypeCore.Info, message, date);
        }

        /// <summary>
        /// GetResultStateInfoAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateInfoAsync<TEntity>(this TEntity result, string message, string date)
        {
            return await GetResultStateAsync(result, StateTypeCore.Info, message, date);
        }

        #endregion

        #region Error

        /// <summary>
        /// GetResultStateError
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultStateCore GetResultStateError(string? message)
        {
            return new ResultStateCore
            {
                StateType = StateTypeCore.Error,
                Message = message
            };
        }

        /// <summary>
        /// GetResultStateErrorAsync
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore> GetResultStateErrorAsync(string? message)
        {
            return await Task.Run(() => new ResultStateCore
            {
                StateType = StateTypeCore.Error,
                Message = message
            });
        }

        /// <summary>
        /// GetResultStateError
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultStateError<TEntity>(this TEntity result, string message)
        {
            return GetResultState(result, StateTypeCore.Error, message, null);
        }

        /// <summary>
        /// GetResultStateErrorAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateErrorAsync<TEntity>(this TEntity result, string message)
        {
            return await GetResultStateAsync(result, StateTypeCore.Error, message, null);
        }

        /// <summary>
        /// GetResultStateError
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultStateError<TEntity>(this TEntity result, string message, string date)
        {
            return GetResultState(result, StateTypeCore.Error, message, date);
        }

        /// <summary>
        /// GetResultStateErrorAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateErrorAsync<TEntity>(this TEntity result, string message, string date)
        {
            return await GetResultStateAsync(result, StateTypeCore.Error, message, date);
        }

        #endregion

        /// <summary>
        /// GetResultState
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="stateType"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultState<TEntity>(this TEntity result, StateTypeCore stateType)
        {
            return GetResultState(result, stateType, null, null);
        }

        /// <summary>
        /// GetResultStateAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="stateType"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateAsync<TEntity>(this TEntity result, StateTypeCore stateType, string? message, string? date)
        {
            return await Task.Run(() => GetResultState(result, stateType, message, date));
        }

        /// <summary>
        /// GetResultState
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="result"></param>
        /// <param name="stateType"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultState<TEntity>(this TEntity result, StateTypeCore stateType, string? message, string? date)
        {
            return new ResultStateCore<TEntity>
            {
                ResultData = result,
                StateType = stateType,
                Message = message,
                Date = date ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };
        }

        /// <summary>
        /// GetResultStateException
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ResultStateCore GetResultStateException(this Exception exception)
        {
            return new ResultStateCore
            {
                StateType = StateTypeCore.Error,
                Message = GetExceptionMessage(exception)
            };
        }

        /// <summary>
        /// GetResultStateExceptionAsync
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore> GetResultStateExceptionAsync(this Exception exception)
        {
            return await Task.Run(() => GetResultStateException(exception));
        }

        /// <summary>
        /// GetResultStateException
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ResultStateCore<TEntity> GetResultStateException<TEntity>(this Exception exception)
        {
            return new ResultStateCore<TEntity>
            {
                StateType = StateTypeCore.Error,
                Message = GetExceptionMessage(exception)
            };
        }

        /// <summary>
        /// GetResultStateExceptionAsync
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task<ResultStateCore<TEntity>> GetResultStateExceptionAsync<TEntity>(this Exception exception)
        {
            return await Task.Run(() => GetResultStateException<TEntity>(exception));
        }

        /// <summary>
        /// GetExceptionMessage
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        internal static string GetExceptionMessage(Exception exception)
        {
            StringBuilder erroBuilder = new StringBuilder();
            erroBuilder.AppendLine(exception.Message);

            if (exception.InnerException != null)
            {
                erroBuilder.AppendLine("\n ");
                erroBuilder.AppendLine(exception.InnerException.Message);
            }

            if (exception.InnerException != null && exception.InnerException.InnerException != null)
            {
                erroBuilder.AppendLine("\n ");
                erroBuilder.AppendLine(exception.InnerException.InnerException.Message);
            }

            return erroBuilder.ToString();
        }
    }
}
