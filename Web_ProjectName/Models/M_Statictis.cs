using Web_ProjectName.Models.Common;
using System.Text.Json.Serialization;

namespace Web_ProjectName.Models
{
    public class M_SurveyFarmBusinessFarm
    {
        public string Period { get; set; } // nhịp độ cạo
        public int? TappingAge { get; set; } // tuổi cạo
        public float? Area { get; set; }
        public float? AreaOld { get; set; }
        public int? TreeQuantity { get; set; }//Tổng số cây kiểm kê

        //public int? TreeQuantityShaving { get; set; }//Tổng số cây cạo
        public int? EffectiveTreeShavingQuantity { get; set; }//Số cây cạo hữu hiệu (KD)
        public int? EffectiveTreeNotshavingQuantity { get; set; }//Số cây chưa cạo hữu hiệu (KD)
        public int? ShavingTreeDensity { get; set; }//Mật độ cây cạo (cây/ha) (KD)
        public int? EffectiveTreeDensity { get; set; }//Mật độ cây hữu hiệu ( KTCB )
        public float? ProductivityByArea { get; set; }//Năng suất hằng năm (kg/ha) (KD)
        public float? ProductivityByTree { get; set; }//Năng suất hằng năm (kg/cây) (KD)
        public int? TotalShavingSlice { get; set; }//Tong so lat cao
        public string Remark { get; set; }
    }

    public class M_SurveyFarmBusinessFarmByFarmGroup
    {
        public string FarmName { get; set; } // đơn vị
        public float? Area { get; set; }//Diện tích năm  hiện tại
        public float? AreaOld { get; set; }//Diện tích năm  ngoái
        public int? TreeQuantity { get; set; }//Tổng số cây kiểm kê
        public int? EffectiveTreeShavingQuantity { get; set; }//Số cây cạo hữu hiệu (KD)
        public int? EffectiveTreeNotshavingQuantity { get; set; }//Số cây chưa cạo hữu hiệu (KD)
        public int? ShavingTreeDensity { get; set; }//Mật độ cây cạo (cây/ha) (KD)
        public int? EffectiveTreeDensity { get; set; }//Mật độ cây hữu hiệu ( KTCB )
        public int? TotalShavingSlice { get; set; } // Tổng số lát cạo
        public float? ProductivityByArea { get; set; }//Năng suất hằng năm (kg/ha) (KD)
        public float? ProductivityByTree { get; set; }//Năng suất hằng năm (kg/cây) (KD)
        public int? ActualLaborQuantity { get; set; } // Tổng số lao động bố trí thực tế
        public float? LaborProductivity { get; set; } // Năng suất lao động (tấn/công nhân)
    }
    public class M_SurveyFarmKTCB
    {
        public string IdPrivate { get; set; }// 1 mã lô 
        public string FarmCode { get; set; }// 2 mã nông trường 
        public string FarmGroupCode { get; set; }// 3 đội tổ 
        public string PlotName { get; set; }//4 Tên lô 
        public string PlotOldName { get; set; }//5 Tên lô 
        public string YearOfPlanting { get; set; }//6 năm trồng
        public string LandLevelCode { get; set; }//7 Hạng đất
        public decimal? AltitudeAverage { get; set; }//8 cao trình TB
        public string PlantingMethodCode { get; set; }//9 PP trồng 
        public string PlantingDistanceCode { get; set; }//10 k.c trồng 
        public string TypeOfTreeCode { get; set; }//11 Tên giống 
        public float? Area { get; set; }//12 diện tích 
        public int? TreeQuantity { get; set; }// 13 tổng cây kk
        public int? EffectiveTreeQuantity { get; set; }// 14 tổng cây hh 
        public float? Vanh_50 { get; set; }//15 vanh 
        public float? Vanh_45_49 { get; set; }//16
        public float? Vanh_40_44 { get; set; }//17
        public float? Vanh_35_39 { get; set; }//18
        public float? Vanh_34 { get; set; }//19
        public float? BarkThickness { get; set; } //20 độ dày vỏ
        public decimal? RatioTreeObtain_50 { get; set; }//21 % vanh cây 
        public decimal? RatioTreeObtain_45_49 { get; set; }//22 % vanh cây 
        public decimal? TotalVanh { get; set; }//23 % tổng vanh
        public int? EffectiveTreeDensity { get; set; }//24 mật độ cây
        public float? ProductivityByArea { get; set; }//25 dkien NS
    }

    public class M_SurveyFarmTypeOfTree
    {
        public int? TappingAge { get; set; }
        public List<M_ListTypeOfTree> TypeOfTreeObjs { get; set; }
    }

    public class M_ListTypeOfTree
    {
        public string TypeOfTreeName { get; set; }
        public float? Company { get; set; }
        public float? Area { get; set; }
        public float? ProductivityByArea { get; set; }
    }

    public class M_SurveyFarmKDLandLevel
    {
        public int? TappingAge { get; set; }
        public M_ListLandLevel LandLevelObjs { get; set; }
    }

    public class M_ListLandLevel
    {
        public double Rank1 { get; set; }
        public double Rank2 { get; set; }
        public double Rank3 { get; set; }
        public double Company { get; set; }
        public double ProductivityRank1 { get; set; }
        public double ProductivityRank2 { get; set; }
        public double ProductivityRank3 { get; set; }
        public double ProductivityCompany { get; set; }
    }

    public class M_SurveyFarmKDGardenRating
    {
        public int? TappingAge { get; set; }
        public double TotalHecta { get; set; }
        public M_Rating RatingObjs { get; set; }
    }

    public class M_Rating
    {
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }
        public double Q4 { get; set; }
        public double PercentQ1 { get; set; }
        public double PercentQ2 { get; set; }
        public double PercentQ3 { get; set; }
        public double PercentQ4 { get; set; }
    }

    public class M_SurveyFarmKTCBGardenRating
    {
        public string FarmName { get; set; }
        public string YearOfPlanting { get; set; }
        public double TotalArea { get; set; }
        public M_RatingKTCB RatingObjs { get; set; }
    }

    public class M_RatingKTCB
    {
        public float? TC1 { get; set; }
        public float? TC2 { get; set; }
        public float? TC3 { get; set; }
        public float? K1 { get; set; }
        public float? K2 { get; set; }
        public float? K3 { get; set; }
        public float? K4 { get; set; }
        public float? K5 { get; set; }
        public double? PercentTC1 { get; set; }
        public double? PercentTC2 { get; set; }
        public double? PercentTC3 { get; set; }
        public double? PercentK1 { get; set; }
        public double? PercentK2 { get; set; }
        public double? PercentK3 { get; set; }
        public double? PercentK4 { get; set; }
        public double? PercentK5 { get; set; }
    }

    public class M_SurveyFarmStatistic// nhieu form tổng hợp
    {
        public List<M_SurveyFarmBusinessFarm> ListPlaceMarkKDByYearObjs { get; set; }//2aKD
        public List<M_SurveyFarmBusinessFarmByFarmGroup> ListPlaceMarkKDByFarmGroupObjs { get; set; }//2bKD
        public List<M_SurveyFarmKTCB> ListPlaceMarkKTCBObjs { get; set; }//3KTCB
        public List<M_SurveyFarmTypeOfTree> ListPlaceMarkKDByTypeOfTreeObjs { get; set; }//3aKD
        public List<M_SurveyFarmKDLandLevel> ListPlaceMarkKDByLandLevelObjs { get; set; }//3bKD
        public List<M_SurveyFarmKDGardenRating> ListPlaceMarkKDByGardenRatingObjs { get; set; }//4KD
        public List<M_SurveyFarmKTCBGardenRating> GetListPlaceMarkKTCBByGardenRatingYearObjs { get; set; }//2aKTCB
        public List<M_SurveyFarmKTCBGardenRating> GetListPlaceMarkKTCBByGardenRatingFarmGroupObjs { get; set; }//2bKTCB
    }
    // public class M_SurveyFarmKTCBGardenRating
    // {
    //     public string FarmName { get; set; }
    //     public string YearOfPlanting { get; set; }
    //     public double TotalArea { get; set; }
    //     public M_RatingKTCB RatingObjs { get; set; }
    // }

    // public class M_RatingKTCB
    // {
    //     public float? TC1 { get; set; }
    //     public float? TC2 { get; set; }
    //     public float? TC3 { get; set; }
    //     public float? K1 { get; set; }
    //     public float? K2 { get; set; }
    //     public float? K3 { get; set; }
    //     public float? K4 { get; set; }
    //     public float? K5 { get; set; }
    //     public double? PercentTC1 { get; set; }
    //     public double? PercentTC2 { get; set; }
    //     public double? PercentTC3 { get; set; }
    //     public double? PercentK1 { get; set; }
    //     public double? PercentK2 { get; set; }
    //     public double? PercentK3 { get; set; }
    //     public double? PercentK4 { get; set; }
    //     public double? PercentK5 { get; set; }
    // }
}