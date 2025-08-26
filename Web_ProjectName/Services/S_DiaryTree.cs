using Web_ProjectName.Lib;
using Web_ProjectName.Models;

namespace Web_ProjectName.Services
{
    public interface IS_DiaryTree
    {
        Task<ResponseData<List<M_DiaryTreeByPlaceMark>>> GetListDataByPlaceMark(string accessToken);
    }

    public class S_DiaryTree : IS_DiaryTree
    {
        private readonly ICallBaseApi _callApi;

        public S_DiaryTree(ICallBaseApi callApi)
        {
            _callApi = callApi;
        }

        public async Task<ResponseData<List<M_DiaryTreeByPlaceMark>>> GetListDataByPlaceMark(string accessToken)
        {
            Dictionary<string, dynamic> dictPars = new Dictionary<string, dynamic>();
            return await _callApi.GetResponseDataAsync<List<M_DiaryTreeByPlaceMark>>("DiaryTree/GetListDataByPlaceMark", dictPars, accessToken);
        }
    }
}
