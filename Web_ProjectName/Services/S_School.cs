using Web_ProjectName.Lib;
using Web_ProjectName.Models;
using static System.String;

namespace Web_ProjectName.Services
{
    public interface IS_School
    {
        Task<ResponseData<M_School>> GetById(string? accessToken, int id);
    }

    public class S_School : IS_School
    {
        private readonly ICallBaseApi _callApi;

        public S_School(ICallBaseApi callApi)
        {
            _callApi = callApi;
        }

        public async Task<ResponseData<M_School>> GetById(string? accessToken, int id)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>
            {
                { "id", id }
            };

            return await _callApi.GetResponseDataAsync<M_School>(
                "School/GetById",
                dictPars,
                accessToken ?? Empty
            );
        }
    }
}
