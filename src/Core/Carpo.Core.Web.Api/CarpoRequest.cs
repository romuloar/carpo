
using Carpo.Core.Extension;
using Carpo.Core.ResultState;
using Carpo.Core.Web.Api.DataTransfer;

namespace Carpo.Core.Web.Api
{
    public static class CarpoRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="carpoRequest"></param>
        /// <param name="listHeader"></param>
        /// <returns></returns>
        public static ResultStateCore<string> GetRequestPost(CarpoRequestDataTransfer carpoRequest, List<CarpoRequestParamDataTransfer> listHeader)
        {
            return GetRequestPost(carpoRequest, listHeader, new List<CarpoRequestParamDataTransfer>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carpoRequest"></param>
        /// <param name="listHeader"></param>
        /// <param name="listParam"></param>
        /// <returns></returns>
        public static ResultStateCore<string> GetRequestPost(CarpoRequestDataTransfer carpoRequest, List<CarpoRequestParamDataTransfer> listHeader, List<CarpoRequestParamDataTransfer> listParam)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(carpoRequest.BaseUrl);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");

                    foreach (var item in listHeader)
                    {
                        client.DefaultRequestHeaders.Add(item.Name, item.Value);
                    }

                    var parameters = new Dictionary<string, string> { { "grant_type", "password" } };
                    foreach (var item in listParam)
                    {
                        parameters.Add(item.Name, item.Value);
                    }

                    var encodedContent = new FormUrlEncodedContent(parameters);
                    var responseTask = client.PostAsync(carpoRequest.NameService, encodedContent).Result;
                    var readTask = responseTask.Content.ReadAsStringAsync().Result;

                    string ret = readTask.ToString();

                    return ret.GetResultStateSuccess();
                }
                catch (Exception exc)
                {
                    return exc.GetResultStateException<string>();
                }
            }
        }
    }
}
