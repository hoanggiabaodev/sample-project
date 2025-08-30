using Web_ProjectName.Lib;
using Web_ProjectName.Models;

namespace Web_ProjectName.Services
{
    public interface IS_SurveyFarm
    {
        Task<ResponseData<List<M_SurveyFarm>>> GetListSurveyFarmFullData(string accessToken, int? surveyBatchId, int? activeStatusId);
    }

    public class S_SurveyFarm : IS_SurveyFarm
    {
        private readonly ICallBaseApi _callApi;

        public S_SurveyFarm(ICallBaseApi callApi)
        {
            _callApi = callApi;
        }

        public async Task<ResponseData<List<M_SurveyFarm>>> GetListSurveyFarmFullData(string accessToken, int? surveyBatchId, int? activeStatusId)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>();

            if (surveyBatchId.HasValue)
                dictPars.Add("surveyBatchId", surveyBatchId.Value);

            if (activeStatusId.HasValue)
                dictPars.Add("activeStatusId", activeStatusId.Value);

            return await _callApi.GetResponseDataAsync<List<M_SurveyFarm>>(
                "SurveyFarm/GetListSurveyFarmFullData",
                dictPars,
                accessToken
            );
        }
    }
}
