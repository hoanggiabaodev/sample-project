using Web_ProjectName.Models.Common;
using System.Text.Json.Serialization;

namespace Web_ProjectName.Models
{
    public class M_DiaryTreeByPlaceMark : M_BaseModel.BaseCustom
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("plotId")]
        public string PlotId { get; set; }

        [JsonPropertyName("area")]
        public float? Area { get; set; }

        [JsonPropertyName("idPrivate")]
        public string IdPrivate { get; set; }

        [JsonPropertyName("yearOfPlanting")]
        public string YearOfPlanting { get; set; }

        [JsonPropertyName("typeOfTreeName")]
        public string TypeOfTreeName { get; set; }

        [JsonPropertyName("farmName")]
        public string FarmName { get; set; }

        [JsonPropertyName("treeQuantityObj")]
        public M_SumItem TreeQuantityObj { get; set; }
    }
}