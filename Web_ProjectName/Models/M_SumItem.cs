using Web_ProjectName.Models.Common;
using System.Text.Json.Serialization;

namespace Web_ProjectName.Models
{
    public class M_SumItem : M_BaseModel.BaseCustom
    {
        [JsonPropertyName("treeQuantity")]
        public int TreeQuantity { get; set; }
        [JsonPropertyName("treeQuantityActual")]
        public int? TreeQuantityActual { get; set; }
        [JsonPropertyName("treeQuantityRepair")]
        public int? TreeQuantityRepair { get; set; }
        [JsonPropertyName("treeQuantityActualRepair")]
        public int? TreeQuantityActualRepair { get; set; }
        [JsonPropertyName("treeQuantityExpectedIncrease")]
        public int? TreeQuantityExpectedIncrease { get; set; }
        [JsonPropertyName("treeQuantityActualIncrease")]
        public int? TreeQuantityActualIncrease { get; set; }
    }
}