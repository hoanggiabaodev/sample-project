using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Principal;
using Web_ProjectName.Models.Common;

namespace Web_ProjectName.Models
{
    public class M_SurveyFarm: M_BaseModel.BaseCustom
    {
        public int Id { get; set; }
        public int PlaceMarkId { get; set; }
        public int SurveyBatchId { get; set; }
        public int? LandLevelId { get; set; }
        public int? OwnerId { get; set; }
        public string Code { get; set; }
        public int? CultivatorId { get; set; }
        public string PlotId { get; set; }
        public string IdPrivate { get; set; }
        public string RiskLevel { get; set; }
        public string PlotOldName { get; set; }
        public string PlotNewName { get; set; }
        public string PlotName { get; set; }
        public string LandType { get; set; }
        public float? AreaCultivated { get; set; }
        public float? Area { get; set; }
        public float? Area1 { get; set; }
        public float? Area2 { get; set; }
        public float? Area3 { get; set; }
        public float? AltitudeLowest { get; set; }
        public float? AltitudeHighest { get; set; }
        public decimal? AltitudeAverage { get; set; }
        public float? AreaManagementChange { get; set; }
        public int? ClassifyId { get; set; }
        public decimal? AverageHeight { get; set; }
        public int? ActiveStatusId { get; set; }
        public int? TypeOfTreeId { get; set; }
        public int? PlantingMethodId { get; set; }
        public int? PlantingDistanceId { get; set; }
        public decimal? PlantingDesignDensity { get; set; }
        public double? Hecta { get; set; }
        public int? HoleQuantity { get; set; }
        public int? GraftedTreeCorrectQuantity { get; set; }
        public int? GraftedTreeMixedQuantity { get; set; }
        public int? EmptyHoleQuantity { get; set; }
        public int? DensityOfGraftedTree { get; set; }
        public decimal? AverageNumberLeafLayer { get; set; }
        public string PlantingEndDate { get; set; }
        public int? TreeQuantity { get; set; }
        public int? TreeQuantityShaving { get; set; }
        public int? EffectiveTreeNotshavingQuantity { get; set; }
        public int? EffectiveTreeShavingQuantity { get; set; }
        public int? IneffectiveTreeNotgrowQuantity { get; set; }
        public int? IneffectiveTreeDryQuantity { get; set; }
        public int? ShavingTreeDensity { get; set; }
        public int? ShavingModeId { get; set; }
        public DateOnly? StartExploitationDate { get; set; }
        public DateOnly? EndExploitationDate { get; set; }
        public int? TotalShavingSlice { get; set; }
        public int? ShavingFaceConditionId { get; set; }
        public int? TappingAge { get; set; }
        public float? ProductivityByArea { get; set; }
        public float? ProductivityByTree { get; set; }
        public int? IneffectiveTreeQuantity { get; set; }
        public int? EffectiveTreeCorrectQuantity { get; set; }
        public int? EffectiveTreeMixedQuantity { get; set; }
        public int? EffectiveTreeDensity { get; set; }
        public decimal? StandardDeviation { get; set; }
        public decimal? RatioTreeObtain { get; set; }
        public bool? PlannedExtendedGarden { get; set; }
        public DateTime? ExpectedExploitationDate { get; set; }
        public int? GardenRatingId { get; set; }
        public float? VanhAverage { get; set; }
        public int? RootTreeMixedQuantity { get; set; }
        public int? RootTreeCorrectQuantity { get; set; }
        public float? TotalOutput { get; set; }
        public string YearOfPlanting { get; set; }
        public string YearOfShaving { get; set; }
        public short? IntercropType { get; set; }
        public string IntercropName { get; set; }
        public string IntercroppingYear { get; set; }
        public decimal? IntercroppingArea { get; set; }
        public decimal? CareContract { get; set; }
        public decimal? ProductContract { get; set; }
        public decimal? FinancialIncome { get; set; }
        public decimal? IntercroppingOther { get; set; }
        public decimal? IntercroppingCompany { get; set; }
        public decimal? NoContribEcon { get; set; }
        public decimal? NoContribPers { get; set; }
        public decimal? PartContribEcon { get; set; }
        public decimal? PartContribPers { get; set; }
        public string Remark { get; set; }

        public M_PlaceMark PlaceMarkObj { get; set; }
        public M_Farmer OwnerObj { get; set; }
        public M_Farmer CultivatorObj { get; set; }
        public M_Account CreatedObj { get; set; }
        public CustomIdName ActiveStatusObj { get; set; }
        public CustomIdName ClassifyObj { get; set; }
        public CustomIdName SurveyBatchObj { get; set; }
        public CustomIdName LandLevelObj { get; set; }
        public CustomIdName TypeOfTreeObj { get; set; }
        public CustomIdName PlantingMethodObj { get; set; }
        public CustomIdName PlantingDistanceObj { get; set; }
        public CustomIdName ShavingModeObj { get; set; }
        public CustomIdName ShavingFaceConditionObj { get; set; }
        public CustomIdName GardenRatingObj { get; set; }
    }

    public class M_PlaceMark : M_BaseModel.BaseCustom
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int? FarmerChildId { get; set; }
        public int WardAreaId { get; set; }
        public bool IsLock { get; set; }
        public short Type { get; set; }
        public int FarmId { get; set; }
        public int LandCertificateId { get; set; }
        public int TypeOfTreeId { get; set; }
        public int ActiveStatusId { get; set; }
        public string YearOfPlanting { get; set; }
        public string Tt { get; set; }
        public string CertOfRightToUse { get; set; }
        public string MapSheetNumber { get; set; }
        public string PlotNumber { get; set; }
        public string NameCertOfRightToUse { get; set; }
        public DateOnly? ExpiredDate { get; set; }
        public string PlotId { get; set; }
        public string IdPrivate { get; set; }
        public string RiskLevel { get; set; }
        public string PlotOldName { get; set; }
        public string PlotNewName { get; set; }
        public string PlotName { get; set; }
        public string LandType { get; set; }
        public float? AreaCultivated { get; set; }
        public float? Area { get; set; }
        public float? Area1 { get; set; }
        public float? Area2 { get; set; }
        public float? Area3 { get; set; }
        public string Owner { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentitycardNumber { get; set; }
        public string AddressText { get; set; }
        public string Commune { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public double? Vn2000Lat { get; set; }
        public double? Vn2000Lng { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string LatLng { get; set; }
        public string Polygon { get; set; }
        public string Remark { get; set; }
        public int? LockedBy { get; set; }
        public DateTime? LockedAt { get; set; }

        public M_WardArea WardAreaObj { get; set; }
        public M_Farm FarmObj { get; set; }
        public M_FarmerChild_Min FarmerChildObj { get; set; }
        public M_LandCertificate LandCertificateObj { get; set; }
        public CustomIdName TypeOfTreeObj { get; set; }
        public CustomIdName ActiveStatusObj { get; set; }
        public M_Account_Info CreatedObj { get; set; }
    }

    public class CustomIdName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    #region Placeholder related models (minimal) - adjust/replace with real implementations if they exist elsewhere.
    public class M_Farmer : M_BaseModel.BaseCustom { public int Id { get; set; } public string Name { get; set; } }
    public class M_Account_Info : M_BaseModel.BaseCustom { public int Id { get; set; } public string FullName { get; set; } }
    public class M_WardArea : M_BaseModel.BaseCustom { public int Id { get; set; } public string Name { get; set; } }
    public class M_Farm : M_BaseModel.BaseCustom { public int Id { get; set; } public string Name { get; set; } }
    public class M_FarmerChild_Min : M_BaseModel.BaseCustom { public int Id { get; set; } public string Name { get; set; } }
    public class M_LandCertificate : M_BaseModel.BaseCustom { public int Id { get; set; } public string Code { get; set; } }
    #endregion
}
