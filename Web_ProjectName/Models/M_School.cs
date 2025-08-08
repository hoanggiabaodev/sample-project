using Web_ProjectName.Models.Common;

namespace Web_ProjectName.Models
{
    public class M_School : M_BaseModel.BaseCustom
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string shortName { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string logoUrl { get; set; }
        public string faviconUrl { get; set; }
        public int? addressId { get; set; }
        public string hotline { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
        public string instagram { get; set; }
        public string zalo { get; set; }
        public string youtube { get; set; }
        public string tiktokUrl { get; set; }
        public AddressObj addressObj { get; set; }
        public int? status { get; set; }
        public DateTime? createdAt { get; set; }
        public string createdBy { get; set; }
        public DateTime? updatedAt { get; set; }
        public string updatedBy { get; set; }
    }

    public class AddressObj
    {
        public int id { get; set; }
        public string addressText { get; set; }
        public int? countryId { get; set; }
        public int? provinceId { get; set; }
        public int? districtId { get; set; }
        public int? wardId { get; set; }
        public int? townId { get; set; }
        public int? status { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public CountryObj countryObj { get; set; }
        public ProvinceObj provinceObj { get; set; }
        public DistrictObj districtObj { get; set; }
        public WardObj wardObj { get; set; }
    }

    public class CountryObj
    {
        public int id { get; set; }
        public string name { get; set; }
        public string countryCode { get; set; }
    }

    public class ProvinceObj
    {
        public int id { get; set; }
        public string name { get; set; }
        public string provinceCode { get; set; }
    }

    public class DistrictObj
    {
        public int id { get; set; }
        public string name { get; set; }
        public string districtCode { get; set; }
    }

    public class WardObj
    {
        public int id { get; set; }
        public string name { get; set; }
        public string wardCode { get; set; }
    }

}
