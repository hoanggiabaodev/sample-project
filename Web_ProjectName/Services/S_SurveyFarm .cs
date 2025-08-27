using Web_ProjectName.Lib;
using Web_ProjectName.Models;

namespace Web_ProjectName.Services
{
    public interface IS_SurveyFarm
    {
        Task<ResponseData<List<M_SurveyFarm>>> GetListSurveyFarmFullData(string accessToken, string? supplierId, string? year, string? season);
    }

    public class S_SurveyFarm : IS_SurveyFarm
    {
        private readonly ICallBaseApi _callApi;
        public S_SurveyFarm(ICallBaseApi callApi)
        {
            _callApi = callApi;
        }

        public async Task<ResponseData<List<M_SurveyFarm>>> GetListSurveyFarmFullData(string accessToken, string? supplierId, string? year, string? season)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                { "supplierId", supplierId },
                { "year", year },
                { "season", season }
            };

            return await _callApi.GetResponseDataAsync<List<M_SurveyFarm>>("SurveyFarm/GetListSurveyFarmFullData", dictPars, accessToken);
        }
    }
}
