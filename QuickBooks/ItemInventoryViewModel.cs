using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using QBPOSFC3Lib;
using System;
using System.Linq;
using QuickBooks;
using System.Threading.Tasks;
using Core.Common.UI;
using InventoryDS.Business.Entities;
using TrackableEntities;
using TrackableEntities.EF6;

namespace QuickBooks
{
    public class ItemInventoryViewModel
    {

        public void BuildItemInventoryQueryRq(IMsgSetRequest ItemInventoryRequestMsgSet,int FromItemNumber, int ToItemNumber)
        {
            var ItemInventoryQueryRq = ItemInventoryRequestMsgSet.AppendItemInventoryQueryRq();
            //Set attributes
            //Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORTimeCreatedFilters.TimeCreatedFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncGreaterThanEqual);
            ////Set field value for TimeCreated
            //ItemInventoryQueryRq.ORTimeCreatedFilters.TimeCreatedFilter.TimeCreated.SetValue(DateTime.Parse("1/1/2000"), false);

         
                //Set field value for FromItemNumber
                ItemInventoryQueryRq.ORItemNumberFilters.ItemNumberRangeFilter.FromItemNumber.SetValue(FromItemNumber);
                //Set field value for ToItemNumber
                ItemInventoryQueryRq.ORItemNumberFilters.ItemNumberRangeFilter.ToItemNumber.SetValue(ToItemNumber);
            

            ItemInventoryQueryRq.OwnerIDList.Add("0");
            #region allshit
            //             IItemInventoryQuery ItemInventoryQueryRq= requestMsgSet.AppendItemInventoryQueryRq();
            ////Set attributes
            ////Set field value for metaData
            //ItemInventoryQueryRq.metaData.SetValue("IQBENmetaDataType");
            ////Set field value for iterator
            //ItemInventoryQueryRq.iterator.SetValue("IQBENiteratorType");
            ////Set field value for iteratorID
            //ItemInventoryQueryRq.iteratorID.SetValue("IQBUUIDType");
            ////Set field value for MaxReturned
            //ItemInventoryQueryRq.MaxReturned.SetValue(6);
            ////Set field value for OwnerIDList
            ////May create more than one of these if needed
            //ItemInventoryQueryRq.OwnerIDList.Add(Guid.NewGuid().ToString());
            ////Set field value for ListID
            //ItemInventoryQueryRq.ListID.SetValue("200000-1011023419");
            //string ORTimeCreatedFiltersElementType1 = "TimeCreatedFilter";
            //if (ORTimeCreatedFiltersElementType1 == "TimeCreatedFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORTimeCreatedFilters.TimeCreatedFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for TimeCreated
            //ItemInventoryQueryRq.ORTimeCreatedFilters.TimeCreatedFilter.TimeCreated.SetValue(DateTime.Parse("12/15/2007 12:15:12"),false);
            //}
            //if (ORTimeCreatedFiltersElementType1 == "TimeCreatedRangeFilter")
            //{
            ////Set field value for FromTimeCreated
            //ItemInventoryQueryRq.ORTimeCreatedFilters.TimeCreatedRangeFilter.FromTimeCreated.SetValue(DateTime.Parse("12/15/2007 12:15:12"),false);
            ////Set field value for ToTimeCreated
            //ItemInventoryQueryRq.ORTimeCreatedFilters.TimeCreatedRangeFilter.ToTimeCreated.SetValue(DateTime.Parse("12/15/2007 12:15:12"),false);
            //}
            //string ORTimeModifiedFiltersElementType2 = "TimeModifiedFilter";
            //if (ORTimeModifiedFiltersElementType2 == "TimeModifiedFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORTimeModifiedFilters.TimeModifiedFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for TimeModified
            //ItemInventoryQueryRq.ORTimeModifiedFilters.TimeModifiedFilter.TimeModified.SetValue(DateTime.Parse("12/15/2007 12:15:12"),false);
            //}
            //if (ORTimeModifiedFiltersElementType2 == "TimeModifiedRangeFilter")
            //{
            ////Set field value for FromTimeModified
            //ItemInventoryQueryRq.ORTimeModifiedFilters.TimeModifiedRangeFilter.FromTimeModified.SetValue(DateTime.Parse("12/15/2007 12:15:12"),false);
            ////Set field value for ToTimeModified
            //ItemInventoryQueryRq.ORTimeModifiedFilters.TimeModifiedRangeFilter.ToTimeModified.SetValue(DateTime.Parse("12/15/2007 12:15:12"),false);
            //}
            //string ORALUFiltersElementType3 = "ALUFilter";
            //if (ORALUFiltersElementType3 == "ALUFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORALUFilters.ALUFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for ALU
            //ItemInventoryQueryRq.ORALUFilters.ALUFilter.ALU.SetValue("ab");
            //}
            //if (ORALUFiltersElementType3 == "ALURangeFilter")
            //{
            ////Set field value for FromALU
            //ItemInventoryQueryRq.ORALUFilters.ALURangeFilter.FromALU.SetValue("ab");
            ////Set field value for ToALU
            //ItemInventoryQueryRq.ORALUFilters.ALURangeFilter.ToALU.SetValue("ab");
            //}
            //string ORAttributeFiltersElementType4 = "AttributeFilter";
            //if (ORAttributeFiltersElementType4 == "AttributeFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORAttributeFilters.AttributeFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for Attribute
            //ItemInventoryQueryRq.ORAttributeFilters.AttributeFilter.Attribute.SetValue("ab");
            //}
            //if (ORAttributeFiltersElementType4 == "AttributeRangeFilter")
            //{
            ////Set field value for FromAttribute
            //ItemInventoryQueryRq.ORAttributeFilters.AttributeRangeFilter.FromAttribute.SetValue("ab");
            ////Set field value for ToAttribute
            //ItemInventoryQueryRq.ORAttributeFilters.AttributeRangeFilter.ToAttribute.SetValue("ab");
            //}
            //string ORCOGSAccountFiltersElementType5 = "COGSAccountFilter";
            //if (ORCOGSAccountFiltersElementType5 == "COGSAccountFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORCOGSAccountFilters.COGSAccountFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for COGSAccount
            //ItemInventoryQueryRq.ORCOGSAccountFilters.COGSAccountFilter.COGSAccount.SetValue("ab");
            //}
            //if (ORCOGSAccountFiltersElementType5 == "COGSAccountRangeFilter")
            //{
            ////Set field value for FromCOGSAccount
            //ItemInventoryQueryRq.ORCOGSAccountFilters.COGSAccountRangeFilter.FromCOGSAccount.SetValue("ab");
            ////Set field value for ToCOGSAccount
            //ItemInventoryQueryRq.ORCOGSAccountFilters.COGSAccountRangeFilter.ToCOGSAccount.SetValue("ab");
            //}
            //string ORCostFiltersElementType6 = "CostFilter";
            //if (ORCostFiltersElementType6 == "CostFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORCostFilters.CostFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for Cost
            //ItemInventoryQueryRq.ORCostFilters.CostFilter.Cost.SetValue(10.01);
            //}
            //if (ORCostFiltersElementType6 == "CostRangeFilter")
            //{
            ////Set field value for FromCost
            //ItemInventoryQueryRq.ORCostFilters.CostRangeFilter.FromCost.SetValue(10.01);
            ////Set field value for ToCost
            //ItemInventoryQueryRq.ORCostFilters.CostRangeFilter.ToCost.SetValue(10.01);
            //}
            //string ORDepartmentCodeFiltersElementType7 = "DepartmentCodeFilter";
            //if (ORDepartmentCodeFiltersElementType7 == "DepartmentCodeFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORDepartmentCodeFilters.DepartmentCodeFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for DepartmentCode
            //ItemInventoryQueryRq.ORDepartmentCodeFilters.DepartmentCodeFilter.DepartmentCode.SetValue("ab");
            //}
            //if (ORDepartmentCodeFiltersElementType7 == "DepartmentCodeRangeFilter")
            //{
            ////Set field value for FromDepartmentCode
            //ItemInventoryQueryRq.ORDepartmentCodeFilters.DepartmentCodeRangeFilter.FromDepartmentCode.SetValue("ab");
            ////Set field value for ToDepartmentCode
            //ItemInventoryQueryRq.ORDepartmentCodeFilters.DepartmentCodeRangeFilter.ToDepartmentCode.SetValue("ab");
            //}
            ////Set field value for DepartmentListID
            //ItemInventoryQueryRq.DepartmentListID.SetValue("200000-1011023419");
            //string ORDesc1FiltersElementType8 = "Desc1Filter";
            //if (ORDesc1FiltersElementType8 == "Desc1Filter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORDesc1Filters.Desc1Filter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for Desc1
            //ItemInventoryQueryRq.ORDesc1Filters.Desc1Filter.Desc1.SetValue("ab");
            //}
            //if (ORDesc1FiltersElementType8 == "Desc1RangeFilter")
            //{
            ////Set field value for FromDesc1
            //ItemInventoryQueryRq.ORDesc1Filters.Desc1RangeFilter.FromDesc1.SetValue("ab");
            ////Set field value for ToDesc1
            //ItemInventoryQueryRq.ORDesc1Filters.Desc1RangeFilter.ToDesc1.SetValue("ab");
            //}
            //string ORDesc2FiltersElementType9 = "Desc2Filter";
            //if (ORDesc2FiltersElementType9 == "Desc2Filter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORDesc2Filters.Desc2Filter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for Desc2
            //ItemInventoryQueryRq.ORDesc2Filters.Desc2Filter.Desc2.SetValue("ab");
            //}
            //if (ORDesc2FiltersElementType9 == "Desc2RangeFilter")
            //{
            ////Set field value for FromDesc2
            //ItemInventoryQueryRq.ORDesc2Filters.Desc2RangeFilter.FromDesc2.SetValue("ab");
            ////Set field value for ToDesc2
            //ItemInventoryQueryRq.ORDesc2Filters.Desc2RangeFilter.ToDesc2.SetValue("ab");
            //}
            //string ORIncomeAccountFiltersElementType10 = "IncomeAccountFilter";
            //if (ORIncomeAccountFiltersElementType10 == "IncomeAccountFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORIncomeAccountFilters.IncomeAccountFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for IncomeAccount
            //ItemInventoryQueryRq.ORIncomeAccountFilters.IncomeAccountFilter.IncomeAccount.SetValue("ab");
            //}
            //if (ORIncomeAccountFiltersElementType10 == "IncomeAccountRangeFilter")
            //{
            ////Set field value for FromIncomeAccount
            //ItemInventoryQueryRq.ORIncomeAccountFilters.IncomeAccountRangeFilter.FromIncomeAccount.SetValue("ab");
            ////Set field value for ToIncomeAccount
            //ItemInventoryQueryRq.ORIncomeAccountFilters.IncomeAccountRangeFilter.ToIncomeAccount.SetValue("ab");
            //}
            ////Set field value for IsBelowReorder
            //ItemInventoryQueryRq.IsBelowReorder.SetValue(true);
            ////Set field value for IsEligibleForCommission
            //ItemInventoryQueryRq.IsEligibleForCommission.SetValue(true);
            ////Set field value for IsPrintingTags
            //ItemInventoryQueryRq.IsPrintingTags.SetValue(true);
            ////Set field value for IsUnorderable
            //ItemInventoryQueryRq.IsUnorderable.SetValue(true);
            ////Set field value for HasPictures
            //ItemInventoryQueryRq.HasPictures.SetValue(true);
            ////Set field value for IsEligibleForRewards
            //ItemInventoryQueryRq.IsEligibleForRewards.SetValue(true);
            ////Set field value for IsWebItem
            //ItemInventoryQueryRq.IsWebItem.SetValue(true);
            //string ORItemNumberFiltersElementType11 = "ItemNumberFilter";
            //if (ORItemNumberFiltersElementType11 == "ItemNumberFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORItemNumberFilters.ItemNumberFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ItemNumber
            //ItemInventoryQueryRq.ORItemNumberFilters.ItemNumberFilter.ItemNumber.SetValue(6);
            //}
            //if (ORItemNumberFiltersElementType11 == "ItemNumberRangeFilter")
            //{
            ////Set field value for FromItemNumber
            //ItemInventoryQueryRq.ORItemNumberFilters.ItemNumberRangeFilter.FromItemNumber.SetValue(6);
            ////Set field value for ToItemNumber
            //ItemInventoryQueryRq.ORItemNumberFilters.ItemNumberRangeFilter.ToItemNumber.SetValue(6);
            //}
            ////Set field value for ItemType
            //ItemInventoryQueryRq.ItemType.SetValue(ENItemType.itInventory);
            //string ORLastReceivedFiltersElementType12 = "LastReceivedFilter";
            //if (ORLastReceivedFiltersElementType12 == "LastReceivedFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORLastReceivedFilters.LastReceivedFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for LastReceived
            //ItemInventoryQueryRq.ORLastReceivedFilters.LastReceivedFilter.LastReceived.SetValue(DateTime.Parse("12/15/2007"));
            //}
            //if (ORLastReceivedFiltersElementType12 == "LastReceivedRangeFilter")
            //{
            ////Set field value for FromLastReceived
            //ItemInventoryQueryRq.ORLastReceivedFilters.LastReceivedRangeFilter.FromLastReceived.SetValue(DateTime.Parse("12/15/2007"));
            ////Set field value for ToLastReceived
            //ItemInventoryQueryRq.ORLastReceivedFilters.LastReceivedRangeFilter.ToLastReceived.SetValue(DateTime.Parse("12/15/2007"));
            //}
            //string ORMSRPFiltersElementType13 = "MSRPFilter";
            //if (ORMSRPFiltersElementType13 == "MSRPFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORMSRPFilters.MSRPFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for MSRP
            //ItemInventoryQueryRq.ORMSRPFilters.MSRPFilter.MSRP.SetValue(10.01);
            //}
            //if (ORMSRPFiltersElementType13 == "MSRPRangeFilter")
            //{
            ////Set field value for FromMSRP
            //ItemInventoryQueryRq.ORMSRPFilters.MSRPRangeFilter.FromMSRP.SetValue(10.01);
            ////Set field value for ToMSRP
            //ItemInventoryQueryRq.ORMSRPFilters.MSRPRangeFilter.ToMSRP.SetValue(10.01);
            //}
            //string OROnHandStore01FiltersElementType14 = "OnHandStore01Filter";
            //if (OROnHandStore01FiltersElementType14 == "OnHandStore01Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore01Filters.OnHandStore01Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore01
            //ItemInventoryQueryRq.OROnHandStore01Filters.OnHandStore01Filter.OnHandStore01.SetValue(2);
            //}
            //if (OROnHandStore01FiltersElementType14 == "OnHandStore01RangeFilter")
            //{
            ////Set field value for FromOnHandStore01
            //ItemInventoryQueryRq.OROnHandStore01Filters.OnHandStore01RangeFilter.FromOnHandStore01.SetValue(2);
            ////Set field value for ToOnHandStore01
            //ItemInventoryQueryRq.OROnHandStore01Filters.OnHandStore01RangeFilter.ToOnHandStore01.SetValue(2);
            //}
            //string OROnHandStore02FiltersElementType15 = "OnHandStore02Filter";
            //if (OROnHandStore02FiltersElementType15 == "OnHandStore02Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore02Filters.OnHandStore02Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore02
            //ItemInventoryQueryRq.OROnHandStore02Filters.OnHandStore02Filter.OnHandStore02.SetValue(2);
            //}
            //if (OROnHandStore02FiltersElementType15 == "OnHandStore02RangeFilter")
            //{
            ////Set field value for FromOnHandStore02
            //ItemInventoryQueryRq.OROnHandStore02Filters.OnHandStore02RangeFilter.FromOnHandStore02.SetValue(2);
            ////Set field value for ToOnHandStore02
            //ItemInventoryQueryRq.OROnHandStore02Filters.OnHandStore02RangeFilter.ToOnHandStore02.SetValue(2);
            //}
            //string OROnHandStore03FiltersElementType16 = "OnHandStore03Filter";
            //if (OROnHandStore03FiltersElementType16 == "OnHandStore03Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore03Filters.OnHandStore03Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore03
            //ItemInventoryQueryRq.OROnHandStore03Filters.OnHandStore03Filter.OnHandStore03.SetValue(2);
            //}
            //if (OROnHandStore03FiltersElementType16 == "OnHandStore03RangeFilter")
            //{
            ////Set field value for FromOnHandStore03
            //ItemInventoryQueryRq.OROnHandStore03Filters.OnHandStore03RangeFilter.FromOnHandStore03.SetValue(2);
            ////Set field value for ToOnHandStore03
            //ItemInventoryQueryRq.OROnHandStore03Filters.OnHandStore03RangeFilter.ToOnHandStore03.SetValue(2);
            //}
            //string OROnHandStore04FiltersElementType17 = "OnHandStore04Filter";
            //if (OROnHandStore04FiltersElementType17 == "OnHandStore04Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore04Filters.OnHandStore04Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore04
            //ItemInventoryQueryRq.OROnHandStore04Filters.OnHandStore04Filter.OnHandStore04.SetValue(2);
            //}
            //if (OROnHandStore04FiltersElementType17 == "OnHandStore04RangeFilter")
            //{
            ////Set field value for FromOnHandStore04
            //ItemInventoryQueryRq.OROnHandStore04Filters.OnHandStore04RangeFilter.FromOnHandStore04.SetValue(2);
            ////Set field value for ToOnHandStore04
            //ItemInventoryQueryRq.OROnHandStore04Filters.OnHandStore04RangeFilter.ToOnHandStore04.SetValue(2);
            //}
            //string OROnHandStore05FiltersElementType18 = "OnHandStore05Filter";
            //if (OROnHandStore05FiltersElementType18 == "OnHandStore05Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore05Filters.OnHandStore05Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore05
            //ItemInventoryQueryRq.OROnHandStore05Filters.OnHandStore05Filter.OnHandStore05.SetValue(2);
            //}
            //if (OROnHandStore05FiltersElementType18 == "OnHandStore05RangeFilter")
            //{
            ////Set field value for FromOnHandStore05
            //ItemInventoryQueryRq.OROnHandStore05Filters.OnHandStore05RangeFilter.FromOnHandStore05.SetValue(2);
            ////Set field value for ToOnHandStore05
            //ItemInventoryQueryRq.OROnHandStore05Filters.OnHandStore05RangeFilter.ToOnHandStore05.SetValue(2);
            //}
            //string OROnHandStore06FiltersElementType19 = "OnHandStore06Filter";
            //if (OROnHandStore06FiltersElementType19 == "OnHandStore06Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore06Filters.OnHandStore06Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore06
            //ItemInventoryQueryRq.OROnHandStore06Filters.OnHandStore06Filter.OnHandStore06.SetValue(2);
            //}
            //if (OROnHandStore06FiltersElementType19 == "OnHandStore06RangeFilter")
            //{
            ////Set field value for FromOnHandStore06
            //ItemInventoryQueryRq.OROnHandStore06Filters.OnHandStore06RangeFilter.FromOnHandStore06.SetValue(2);
            ////Set field value for ToOnHandStore06
            //ItemInventoryQueryRq.OROnHandStore06Filters.OnHandStore06RangeFilter.ToOnHandStore06.SetValue(2);
            //}
            //string OROnHandStore07FiltersElementType20 = "OnHandStore07Filter";
            //if (OROnHandStore07FiltersElementType20 == "OnHandStore07Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore07Filters.OnHandStore07Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore07
            //ItemInventoryQueryRq.OROnHandStore07Filters.OnHandStore07Filter.OnHandStore07.SetValue(2);
            //}
            //if (OROnHandStore07FiltersElementType20 == "OnHandStore07RangeFilter")
            //{
            ////Set field value for FromOnHandStore07
            //ItemInventoryQueryRq.OROnHandStore07Filters.OnHandStore07RangeFilter.FromOnHandStore07.SetValue(2);
            ////Set field value for ToOnHandStore07
            //ItemInventoryQueryRq.OROnHandStore07Filters.OnHandStore07RangeFilter.ToOnHandStore07.SetValue(2);
            //}
            //string OROnHandStore08FiltersElementType21 = "OnHandStore08Filter";
            //if (OROnHandStore08FiltersElementType21 == "OnHandStore08Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore08Filters.OnHandStore08Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore08
            //ItemInventoryQueryRq.OROnHandStore08Filters.OnHandStore08Filter.OnHandStore08.SetValue(2);
            //}
            //if (OROnHandStore08FiltersElementType21 == "OnHandStore08RangeFilter")
            //{
            ////Set field value for FromOnHandStore08
            //ItemInventoryQueryRq.OROnHandStore08Filters.OnHandStore08RangeFilter.FromOnHandStore08.SetValue(2);
            ////Set field value for ToOnHandStore08
            //ItemInventoryQueryRq.OROnHandStore08Filters.OnHandStore08RangeFilter.ToOnHandStore08.SetValue(2);
            //}
            //string OROnHandStore09FiltersElementType22 = "OnHandStore09Filter";
            //if (OROnHandStore09FiltersElementType22 == "OnHandStore09Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore09Filters.OnHandStore09Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore09
            //ItemInventoryQueryRq.OROnHandStore09Filters.OnHandStore09Filter.OnHandStore09.SetValue(2);
            //}
            //if (OROnHandStore09FiltersElementType22 == "OnHandStore09RangeFilter")
            //{
            ////Set field value for FromOnHandStore09
            //ItemInventoryQueryRq.OROnHandStore09Filters.OnHandStore09RangeFilter.FromOnHandStore09.SetValue(2);
            ////Set field value for ToOnHandStore09
            //ItemInventoryQueryRq.OROnHandStore09Filters.OnHandStore09RangeFilter.ToOnHandStore09.SetValue(2);
            //}
            //string OROnHandStore10FiltersElementType23 = "OnHandStore10Filter";
            //if (OROnHandStore10FiltersElementType23 == "OnHandStore10Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore10Filters.OnHandStore10Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore10
            //ItemInventoryQueryRq.OROnHandStore10Filters.OnHandStore10Filter.OnHandStore10.SetValue(2);
            //}
            //if (OROnHandStore10FiltersElementType23 == "OnHandStore10RangeFilter")
            //{
            ////Set field value for FromOnHandStore10
            //ItemInventoryQueryRq.OROnHandStore10Filters.OnHandStore10RangeFilter.FromOnHandStore10.SetValue(2);
            ////Set field value for ToOnHandStore10
            //ItemInventoryQueryRq.OROnHandStore10Filters.OnHandStore10RangeFilter.ToOnHandStore10.SetValue(2);
            //}
            //string OROnHandStore11FiltersElementType24 = "OnHandStore11Filter";
            //if (OROnHandStore11FiltersElementType24 == "OnHandStore11Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore11Filters.OnHandStore11Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore11
            //ItemInventoryQueryRq.OROnHandStore11Filters.OnHandStore11Filter.OnHandStore11.SetValue(2);
            //}
            //if (OROnHandStore11FiltersElementType24 == "OnHandStore11RangeFilter")
            //{
            ////Set field value for FromOnHandStore11
            //ItemInventoryQueryRq.OROnHandStore11Filters.OnHandStore11RangeFilter.FromOnHandStore11.SetValue(2);
            ////Set field value for ToOnHandStore11
            //ItemInventoryQueryRq.OROnHandStore11Filters.OnHandStore11RangeFilter.ToOnHandStore11.SetValue(2);
            //}
            //string OROnHandStore12FiltersElementType25 = "OnHandStore12Filter";
            //if (OROnHandStore12FiltersElementType25 == "OnHandStore12Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore12Filters.OnHandStore12Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore12
            //ItemInventoryQueryRq.OROnHandStore12Filters.OnHandStore12Filter.OnHandStore12.SetValue(2);
            //}
            //if (OROnHandStore12FiltersElementType25 == "OnHandStore12RangeFilter")
            //{
            ////Set field value for FromOnHandStore12
            //ItemInventoryQueryRq.OROnHandStore12Filters.OnHandStore12RangeFilter.FromOnHandStore12.SetValue(2);
            ////Set field value for ToOnHandStore12
            //ItemInventoryQueryRq.OROnHandStore12Filters.OnHandStore12RangeFilter.ToOnHandStore12.SetValue(2);
            //}
            //string OROnHandStore13FiltersElementType26 = "OnHandStore13Filter";
            //if (OROnHandStore13FiltersElementType26 == "OnHandStore13Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore13Filters.OnHandStore13Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore13
            //ItemInventoryQueryRq.OROnHandStore13Filters.OnHandStore13Filter.OnHandStore13.SetValue(2);
            //}
            //if (OROnHandStore13FiltersElementType26 == "OnHandStore13RangeFilter")
            //{
            ////Set field value for FromOnHandStore13
            //ItemInventoryQueryRq.OROnHandStore13Filters.OnHandStore13RangeFilter.FromOnHandStore13.SetValue(2);
            ////Set field value for ToOnHandStore13
            //ItemInventoryQueryRq.OROnHandStore13Filters.OnHandStore13RangeFilter.ToOnHandStore13.SetValue(2);
            //}
            //string OROnHandStore14FiltersElementType27 = "OnHandStore14Filter";
            //if (OROnHandStore14FiltersElementType27 == "OnHandStore14Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore14Filters.OnHandStore14Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore14
            //ItemInventoryQueryRq.OROnHandStore14Filters.OnHandStore14Filter.OnHandStore14.SetValue(2);
            //}
            //if (OROnHandStore14FiltersElementType27 == "OnHandStore14RangeFilter")
            //{
            ////Set field value for FromOnHandStore14
            //ItemInventoryQueryRq.OROnHandStore14Filters.OnHandStore14RangeFilter.FromOnHandStore14.SetValue(2);
            ////Set field value for ToOnHandStore14
            //ItemInventoryQueryRq.OROnHandStore14Filters.OnHandStore14RangeFilter.ToOnHandStore14.SetValue(2);
            //}
            //string OROnHandStore15FiltersElementType28 = "OnHandStore15Filter";
            //if (OROnHandStore15FiltersElementType28 == "OnHandStore15Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore15Filters.OnHandStore15Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore15
            //ItemInventoryQueryRq.OROnHandStore15Filters.OnHandStore15Filter.OnHandStore15.SetValue(2);
            //}
            //if (OROnHandStore15FiltersElementType28 == "OnHandStore15RangeFilter")
            //{
            ////Set field value for FromOnHandStore15
            //ItemInventoryQueryRq.OROnHandStore15Filters.OnHandStore15RangeFilter.FromOnHandStore15.SetValue(2);
            ////Set field value for ToOnHandStore15
            //ItemInventoryQueryRq.OROnHandStore15Filters.OnHandStore15RangeFilter.ToOnHandStore15.SetValue(2);
            //}
            //string OROnHandStore16FiltersElementType29 = "OnHandStore16Filter";
            //if (OROnHandStore16FiltersElementType29 == "OnHandStore16Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore16Filters.OnHandStore16Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore16
            //ItemInventoryQueryRq.OROnHandStore16Filters.OnHandStore16Filter.OnHandStore16.SetValue(2);
            //}
            //if (OROnHandStore16FiltersElementType29 == "OnHandStore16RangeFilter")
            //{
            ////Set field value for FromOnHandStore16
            //ItemInventoryQueryRq.OROnHandStore16Filters.OnHandStore16RangeFilter.FromOnHandStore16.SetValue(2);
            ////Set field value for ToOnHandStore16
            //ItemInventoryQueryRq.OROnHandStore16Filters.OnHandStore16RangeFilter.ToOnHandStore16.SetValue(2);
            //}
            //string OROnHandStore17FiltersElementType30 = "OnHandStore17Filter";
            //if (OROnHandStore17FiltersElementType30 == "OnHandStore17Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore17Filters.OnHandStore17Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore17
            //ItemInventoryQueryRq.OROnHandStore17Filters.OnHandStore17Filter.OnHandStore17.SetValue(2);
            //}
            //if (OROnHandStore17FiltersElementType30 == "OnHandStore17RangeFilter")
            //{
            ////Set field value for FromOnHandStore17
            //ItemInventoryQueryRq.OROnHandStore17Filters.OnHandStore17RangeFilter.FromOnHandStore17.SetValue(2);
            ////Set field value for ToOnHandStore17
            //ItemInventoryQueryRq.OROnHandStore17Filters.OnHandStore17RangeFilter.ToOnHandStore17.SetValue(2);
            //}
            //string OROnHandStore18FiltersElementType31 = "OnHandStore18Filter";
            //if (OROnHandStore18FiltersElementType31 == "OnHandStore18Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore18Filters.OnHandStore18Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore18
            //ItemInventoryQueryRq.OROnHandStore18Filters.OnHandStore18Filter.OnHandStore18.SetValue(2);
            //}
            //if (OROnHandStore18FiltersElementType31 == "OnHandStore18RangeFilter")
            //{
            ////Set field value for FromOnHandStore18
            //ItemInventoryQueryRq.OROnHandStore18Filters.OnHandStore18RangeFilter.FromOnHandStore18.SetValue(2);
            ////Set field value for ToOnHandStore18
            //ItemInventoryQueryRq.OROnHandStore18Filters.OnHandStore18RangeFilter.ToOnHandStore18.SetValue(2);
            //}
            //string OROnHandStore19FiltersElementType32 = "OnHandStore19Filter";
            //if (OROnHandStore19FiltersElementType32 == "OnHandStore19Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore19Filters.OnHandStore19Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore19
            //ItemInventoryQueryRq.OROnHandStore19Filters.OnHandStore19Filter.OnHandStore19.SetValue(2);
            //}
            //if (OROnHandStore19FiltersElementType32 == "OnHandStore19RangeFilter")
            //{
            ////Set field value for FromOnHandStore19
            //ItemInventoryQueryRq.OROnHandStore19Filters.OnHandStore19RangeFilter.FromOnHandStore19.SetValue(2);
            ////Set field value for ToOnHandStore19
            //ItemInventoryQueryRq.OROnHandStore19Filters.OnHandStore19RangeFilter.ToOnHandStore19.SetValue(2);
            //}
            //string OROnHandStore20FiltersElementType33 = "OnHandStore20Filter";
            //if (OROnHandStore20FiltersElementType33 == "OnHandStore20Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROnHandStore20Filters.OnHandStore20Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OnHandStore20
            //ItemInventoryQueryRq.OROnHandStore20Filters.OnHandStore20Filter.OnHandStore20.SetValue(2);
            //}
            //if (OROnHandStore20FiltersElementType33 == "OnHandStore20RangeFilter")
            //{
            ////Set field value for FromOnHandStore20
            //ItemInventoryQueryRq.OROnHandStore20Filters.OnHandStore20RangeFilter.FromOnHandStore20.SetValue(2);
            ////Set field value for ToOnHandStore20
            //ItemInventoryQueryRq.OROnHandStore20Filters.OnHandStore20RangeFilter.ToOnHandStore20.SetValue(2);
            //}
            //string ORReorderPointStore01FiltersElementType34 = "ReorderPointStore01Filter";
            //if (ORReorderPointStore01FiltersElementType34 == "ReorderPointStore01Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore01Filters.ReorderPointStore01Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore01
            //ItemInventoryQueryRq.ORReorderPointStore01Filters.ReorderPointStore01Filter.ReorderPointStore01.SetValue(2);
            //}
            //if (ORReorderPointStore01FiltersElementType34 == "ReorderPointStore01RangeFilter")
            //{
            ////Set field value for FromReorderPointStore01
            //ItemInventoryQueryRq.ORReorderPointStore01Filters.ReorderPointStore01RangeFilter.FromReorderPointStore01.SetValue(2);
            ////Set field value for ToReorderPointStore01
            //ItemInventoryQueryRq.ORReorderPointStore01Filters.ReorderPointStore01RangeFilter.ToReorderPointStore01.SetValue(2);
            //}
            //string ORReorderPointStore02FiltersElementType35 = "ReorderPointStore02Filter";
            //if (ORReorderPointStore02FiltersElementType35 == "ReorderPointStore02Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore02Filters.ReorderPointStore02Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore02
            //ItemInventoryQueryRq.ORReorderPointStore02Filters.ReorderPointStore02Filter.ReorderPointStore02.SetValue(2);
            //}
            //if (ORReorderPointStore02FiltersElementType35 == "ReorderPointStore02RangeFilter")
            //{
            ////Set field value for FromReorderPointStore02
            //ItemInventoryQueryRq.ORReorderPointStore02Filters.ReorderPointStore02RangeFilter.FromReorderPointStore02.SetValue(2);
            ////Set field value for ToReorderPointStore02
            //ItemInventoryQueryRq.ORReorderPointStore02Filters.ReorderPointStore02RangeFilter.ToReorderPointStore02.SetValue(2);
            //}
            //string ORReorderPointStore03FiltersElementType36 = "ReorderPointStore03Filter";
            //if (ORReorderPointStore03FiltersElementType36 == "ReorderPointStore03Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore03Filters.ReorderPointStore03Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore03
            //ItemInventoryQueryRq.ORReorderPointStore03Filters.ReorderPointStore03Filter.ReorderPointStore03.SetValue(2);
            //}
            //if (ORReorderPointStore03FiltersElementType36 == "ReorderPointStore03RangeFilter")
            //{
            ////Set field value for FromReorderPointStore03
            //ItemInventoryQueryRq.ORReorderPointStore03Filters.ReorderPointStore03RangeFilter.FromReorderPointStore03.SetValue(2);
            ////Set field value for ToReorderPointStore03
            //ItemInventoryQueryRq.ORReorderPointStore03Filters.ReorderPointStore03RangeFilter.ToReorderPointStore03.SetValue(2);
            //}
            //string ORReorderPointStore04FiltersElementType37 = "ReorderPointStore04Filter";
            //if (ORReorderPointStore04FiltersElementType37 == "ReorderPointStore04Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore04Filters.ReorderPointStore04Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore04
            //ItemInventoryQueryRq.ORReorderPointStore04Filters.ReorderPointStore04Filter.ReorderPointStore04.SetValue(2);
            //}
            //if (ORReorderPointStore04FiltersElementType37 == "ReorderPointStore04RangeFilter")
            //{
            ////Set field value for FromReorderPointStore04
            //ItemInventoryQueryRq.ORReorderPointStore04Filters.ReorderPointStore04RangeFilter.FromReorderPointStore04.SetValue(2);
            ////Set field value for ToReorderPointStore04
            //ItemInventoryQueryRq.ORReorderPointStore04Filters.ReorderPointStore04RangeFilter.ToReorderPointStore04.SetValue(2);
            //}
            //string ORReorderPointStore05FiltersElementType38 = "ReorderPointStore05Filter";
            //if (ORReorderPointStore05FiltersElementType38 == "ReorderPointStore05Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore05Filters.ReorderPointStore05Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore05
            //ItemInventoryQueryRq.ORReorderPointStore05Filters.ReorderPointStore05Filter.ReorderPointStore05.SetValue(2);
            //}
            //if (ORReorderPointStore05FiltersElementType38 == "ReorderPointStore05RangeFilter")
            //{
            ////Set field value for FromReorderPointStore05
            //ItemInventoryQueryRq.ORReorderPointStore05Filters.ReorderPointStore05RangeFilter.FromReorderPointStore05.SetValue(2);
            ////Set field value for ToReorderPointStore05
            //ItemInventoryQueryRq.ORReorderPointStore05Filters.ReorderPointStore05RangeFilter.ToReorderPointStore05.SetValue(2);
            //}
            //string ORReorderPointStore06FiltersElementType39 = "ReorderPointStore06Filter";
            //if (ORReorderPointStore06FiltersElementType39 == "ReorderPointStore06Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore06Filters.ReorderPointStore06Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore06
            //ItemInventoryQueryRq.ORReorderPointStore06Filters.ReorderPointStore06Filter.ReorderPointStore06.SetValue(2);
            //}
            //if (ORReorderPointStore06FiltersElementType39 == "ReorderPointStore06RangeFilter")
            //{
            ////Set field value for FromReorderPointStore06
            //ItemInventoryQueryRq.ORReorderPointStore06Filters.ReorderPointStore06RangeFilter.FromReorderPointStore06.SetValue(2);
            ////Set field value for ToReorderPointStore06
            //ItemInventoryQueryRq.ORReorderPointStore06Filters.ReorderPointStore06RangeFilter.ToReorderPointStore06.SetValue(2);
            //}
            //string ORReorderPointStore07FiltersElementType40 = "ReorderPointStore07Filter";
            //if (ORReorderPointStore07FiltersElementType40 == "ReorderPointStore07Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore07Filters.ReorderPointStore07Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore07
            //ItemInventoryQueryRq.ORReorderPointStore07Filters.ReorderPointStore07Filter.ReorderPointStore07.SetValue(2);
            //}
            //if (ORReorderPointStore07FiltersElementType40 == "ReorderPointStore07RangeFilter")
            //{
            ////Set field value for FromReorderPointStore07
            //ItemInventoryQueryRq.ORReorderPointStore07Filters.ReorderPointStore07RangeFilter.FromReorderPointStore07.SetValue(2);
            ////Set field value for ToReorderPointStore07
            //ItemInventoryQueryRq.ORReorderPointStore07Filters.ReorderPointStore07RangeFilter.ToReorderPointStore07.SetValue(2);
            //}
            //string ORReorderPointStore08FiltersElementType41 = "ReorderPointStore08Filter";
            //if (ORReorderPointStore08FiltersElementType41 == "ReorderPointStore08Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore08Filters.ReorderPointStore08Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore08
            //ItemInventoryQueryRq.ORReorderPointStore08Filters.ReorderPointStore08Filter.ReorderPointStore08.SetValue(2);
            //}
            //if (ORReorderPointStore08FiltersElementType41 == "ReorderPointStore08RangeFilter")
            //{
            ////Set field value for FromReorderPointStore08
            //ItemInventoryQueryRq.ORReorderPointStore08Filters.ReorderPointStore08RangeFilter.FromReorderPointStore08.SetValue(2);
            ////Set field value for ToReorderPointStore08
            //ItemInventoryQueryRq.ORReorderPointStore08Filters.ReorderPointStore08RangeFilter.ToReorderPointStore08.SetValue(2);
            //}
            //string ORReorderPointStore09FiltersElementType42 = "ReorderPointStore09Filter";
            //if (ORReorderPointStore09FiltersElementType42 == "ReorderPointStore09Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore09Filters.ReorderPointStore09Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore09
            //ItemInventoryQueryRq.ORReorderPointStore09Filters.ReorderPointStore09Filter.ReorderPointStore09.SetValue(2);
            //}
            //if (ORReorderPointStore09FiltersElementType42 == "ReorderPointStore09RangeFilter")
            //{
            ////Set field value for FromReorderPointStore09
            //ItemInventoryQueryRq.ORReorderPointStore09Filters.ReorderPointStore09RangeFilter.FromReorderPointStore09.SetValue(2);
            ////Set field value for ToReorderPointStore09
            //ItemInventoryQueryRq.ORReorderPointStore09Filters.ReorderPointStore09RangeFilter.ToReorderPointStore09.SetValue(2);
            //}
            //string ORReorderPointStore10FiltersElementType43 = "ReorderPointStore10Filter";
            //if (ORReorderPointStore10FiltersElementType43 == "ReorderPointStore10Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore10Filters.ReorderPointStore10Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore10
            //ItemInventoryQueryRq.ORReorderPointStore10Filters.ReorderPointStore10Filter.ReorderPointStore10.SetValue(2);
            //}
            //if (ORReorderPointStore10FiltersElementType43 == "ReorderPointStore10RangeFilter")
            //{
            ////Set field value for FromReorderPointStore10
            //ItemInventoryQueryRq.ORReorderPointStore10Filters.ReorderPointStore10RangeFilter.FromReorderPointStore10.SetValue(2);
            ////Set field value for ToReorderPointStore10
            //ItemInventoryQueryRq.ORReorderPointStore10Filters.ReorderPointStore10RangeFilter.ToReorderPointStore10.SetValue(2);
            //}
            //string ORReorderPointStore11FiltersElementType44 = "ReorderPointStore11Filter";
            //if (ORReorderPointStore11FiltersElementType44 == "ReorderPointStore11Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore11Filters.ReorderPointStore11Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore11
            //ItemInventoryQueryRq.ORReorderPointStore11Filters.ReorderPointStore11Filter.ReorderPointStore11.SetValue(2);
            //}
            //if (ORReorderPointStore11FiltersElementType44 == "ReorderPointStore11RangeFilter")
            //{
            ////Set field value for FromReorderPointStore11
            //ItemInventoryQueryRq.ORReorderPointStore11Filters.ReorderPointStore11RangeFilter.FromReorderPointStore11.SetValue(2);
            ////Set field value for ToReorderPointStore11
            //ItemInventoryQueryRq.ORReorderPointStore11Filters.ReorderPointStore11RangeFilter.ToReorderPointStore11.SetValue(2);
            //}
            //string ORReorderPointStore12FiltersElementType45 = "ReorderPointStore12Filter";
            //if (ORReorderPointStore12FiltersElementType45 == "ReorderPointStore12Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore12Filters.ReorderPointStore12Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore12
            //ItemInventoryQueryRq.ORReorderPointStore12Filters.ReorderPointStore12Filter.ReorderPointStore12.SetValue(2);
            //}
            //if (ORReorderPointStore12FiltersElementType45 == "ReorderPointStore12RangeFilter")
            //{
            ////Set field value for FromReorderPointStore12
            //ItemInventoryQueryRq.ORReorderPointStore12Filters.ReorderPointStore12RangeFilter.FromReorderPointStore12.SetValue(2);
            ////Set field value for ToReorderPointStore12
            //ItemInventoryQueryRq.ORReorderPointStore12Filters.ReorderPointStore12RangeFilter.ToReorderPointStore12.SetValue(2);
            //}
            //string ORReorderPointStore13FiltersElementType46 = "ReorderPointStore13Filter";
            //if (ORReorderPointStore13FiltersElementType46 == "ReorderPointStore13Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore13Filters.ReorderPointStore13Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore13
            //ItemInventoryQueryRq.ORReorderPointStore13Filters.ReorderPointStore13Filter.ReorderPointStore13.SetValue(2);
            //}
            //if (ORReorderPointStore13FiltersElementType46 == "ReorderPointStore13RangeFilter")
            //{
            ////Set field value for FromReorderPointStore13
            //ItemInventoryQueryRq.ORReorderPointStore13Filters.ReorderPointStore13RangeFilter.FromReorderPointStore13.SetValue(2);
            ////Set field value for ToReorderPointStore13
            //ItemInventoryQueryRq.ORReorderPointStore13Filters.ReorderPointStore13RangeFilter.ToReorderPointStore13.SetValue(2);
            //}
            //string ORReorderPointStore14FiltersElementType47 = "ReorderPointStore14Filter";
            //if (ORReorderPointStore14FiltersElementType47 == "ReorderPointStore14Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore14Filters.ReorderPointStore14Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore14
            //ItemInventoryQueryRq.ORReorderPointStore14Filters.ReorderPointStore14Filter.ReorderPointStore14.SetValue(2);
            //}
            //if (ORReorderPointStore14FiltersElementType47 == "ReorderPointStore14RangeFilter")
            //{
            ////Set field value for FromReorderPointStore14
            //ItemInventoryQueryRq.ORReorderPointStore14Filters.ReorderPointStore14RangeFilter.FromReorderPointStore14.SetValue(2);
            ////Set field value for ToReorderPointStore14
            //ItemInventoryQueryRq.ORReorderPointStore14Filters.ReorderPointStore14RangeFilter.ToReorderPointStore14.SetValue(2);
            //}
            //string ORReorderPointStore15FiltersElementType48 = "ReorderPointStore15Filter";
            //if (ORReorderPointStore15FiltersElementType48 == "ReorderPointStore15Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore15Filters.ReorderPointStore15Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore15
            //ItemInventoryQueryRq.ORReorderPointStore15Filters.ReorderPointStore15Filter.ReorderPointStore15.SetValue(2);
            //}
            //if (ORReorderPointStore15FiltersElementType48 == "ReorderPointStore15RangeFilter")
            //{
            ////Set field value for FromReorderPointStore15
            //ItemInventoryQueryRq.ORReorderPointStore15Filters.ReorderPointStore15RangeFilter.FromReorderPointStore15.SetValue(2);
            ////Set field value for ToReorderPointStore15
            //ItemInventoryQueryRq.ORReorderPointStore15Filters.ReorderPointStore15RangeFilter.ToReorderPointStore15.SetValue(2);
            //}
            //string ORReorderPointStore16FiltersElementType49 = "ReorderPointStore16Filter";
            //if (ORReorderPointStore16FiltersElementType49 == "ReorderPointStore16Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore16Filters.ReorderPointStore16Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore16
            //ItemInventoryQueryRq.ORReorderPointStore16Filters.ReorderPointStore16Filter.ReorderPointStore16.SetValue(2);
            //}
            //if (ORReorderPointStore16FiltersElementType49 == "ReorderPointStore16RangeFilter")
            //{
            ////Set field value for FromReorderPointStore16
            //ItemInventoryQueryRq.ORReorderPointStore16Filters.ReorderPointStore16RangeFilter.FromReorderPointStore16.SetValue(2);
            ////Set field value for ToReorderPointStore16
            //ItemInventoryQueryRq.ORReorderPointStore16Filters.ReorderPointStore16RangeFilter.ToReorderPointStore16.SetValue(2);
            //}
            //string ORReorderPointStore17FiltersElementType50 = "ReorderPointStore17Filter";
            //if (ORReorderPointStore17FiltersElementType50 == "ReorderPointStore17Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore17Filters.ReorderPointStore17Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore17
            //ItemInventoryQueryRq.ORReorderPointStore17Filters.ReorderPointStore17Filter.ReorderPointStore17.SetValue(2);
            //}
            //if (ORReorderPointStore17FiltersElementType50 == "ReorderPointStore17RangeFilter")
            //{
            ////Set field value for FromReorderPointStore17
            //ItemInventoryQueryRq.ORReorderPointStore17Filters.ReorderPointStore17RangeFilter.FromReorderPointStore17.SetValue(2);
            ////Set field value for ToReorderPointStore17
            //ItemInventoryQueryRq.ORReorderPointStore17Filters.ReorderPointStore17RangeFilter.ToReorderPointStore17.SetValue(2);
            //}
            //string ORReorderPointStore18FiltersElementType51 = "ReorderPointStore18Filter";
            //if (ORReorderPointStore18FiltersElementType51 == "ReorderPointStore18Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore18Filters.ReorderPointStore18Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore18
            //ItemInventoryQueryRq.ORReorderPointStore18Filters.ReorderPointStore18Filter.ReorderPointStore18.SetValue(2);
            //}
            //if (ORReorderPointStore18FiltersElementType51 == "ReorderPointStore18RangeFilter")
            //{
            ////Set field value for FromReorderPointStore18
            //ItemInventoryQueryRq.ORReorderPointStore18Filters.ReorderPointStore18RangeFilter.FromReorderPointStore18.SetValue(2);
            ////Set field value for ToReorderPointStore18
            //ItemInventoryQueryRq.ORReorderPointStore18Filters.ReorderPointStore18RangeFilter.ToReorderPointStore18.SetValue(2);
            //}
            //string ORReorderPointStore19FiltersElementType52 = "ReorderPointStore19Filter";
            //if (ORReorderPointStore19FiltersElementType52 == "ReorderPointStore19Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore19Filters.ReorderPointStore19Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore19
            //ItemInventoryQueryRq.ORReorderPointStore19Filters.ReorderPointStore19Filter.ReorderPointStore19.SetValue(2);
            //}
            //if (ORReorderPointStore19FiltersElementType52 == "ReorderPointStore19RangeFilter")
            //{
            ////Set field value for FromReorderPointStore19
            //ItemInventoryQueryRq.ORReorderPointStore19Filters.ReorderPointStore19RangeFilter.FromReorderPointStore19.SetValue(2);
            ////Set field value for ToReorderPointStore19
            //ItemInventoryQueryRq.ORReorderPointStore19Filters.ReorderPointStore19RangeFilter.ToReorderPointStore19.SetValue(2);
            //}
            //string ORReorderPointStore20FiltersElementType53 = "ReorderPointStore20Filter";
            //if (ORReorderPointStore20FiltersElementType53 == "ReorderPointStore20Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointStore20Filters.ReorderPointStore20Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPointStore20
            //ItemInventoryQueryRq.ORReorderPointStore20Filters.ReorderPointStore20Filter.ReorderPointStore20.SetValue(2);
            //}
            //if (ORReorderPointStore20FiltersElementType53 == "ReorderPointStore20RangeFilter")
            //{
            ////Set field value for FromReorderPointStore20
            //ItemInventoryQueryRq.ORReorderPointStore20Filters.ReorderPointStore20RangeFilter.FromReorderPointStore20.SetValue(2);
            ////Set field value for ToReorderPointStore20
            //ItemInventoryQueryRq.ORReorderPointStore20Filters.ReorderPointStore20RangeFilter.ToReorderPointStore20.SetValue(2);
            //}
            //string OROrderByUnitFiltersElementType54 = "OrderByUnitFilter";
            //if (OROrderByUnitFiltersElementType54 == "OrderByUnitFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.OROrderByUnitFilters.OrderByUnitFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for OrderByUnit
            //ItemInventoryQueryRq.OROrderByUnitFilters.OrderByUnitFilter.OrderByUnit.SetValue("ab");
            //}
            //if (OROrderByUnitFiltersElementType54 == "OrderByUnitRangeFilter")
            //{
            ////Set field value for FromOrderByUnit
            //ItemInventoryQueryRq.OROrderByUnitFilters.OrderByUnitRangeFilter.FromOrderByUnit.SetValue("ab");
            ////Set field value for ToOrderByUnit
            //ItemInventoryQueryRq.OROrderByUnitFilters.OrderByUnitRangeFilter.ToOrderByUnit.SetValue("ab");
            //}
            //string OROrderCostFiltersElementType55 = "OrderCostFilter";
            //if (OROrderCostFiltersElementType55 == "OrderCostFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.OROrderCostFilters.OrderCostFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for OrderCost
            //ItemInventoryQueryRq.OROrderCostFilters.OrderCostFilter.OrderCost.SetValue(10.01);
            //}
            //if (OROrderCostFiltersElementType55 == "OrderCostRangeFilter")
            //{
            ////Set field value for FromOrderCost
            //ItemInventoryQueryRq.OROrderCostFilters.OrderCostRangeFilter.FromOrderCost.SetValue(10.01);
            ////Set field value for ToOrderCost
            //ItemInventoryQueryRq.OROrderCostFilters.OrderCostRangeFilter.ToOrderCost.SetValue(10.01);
            //}
            //string ORPrice1FiltersElementType56 = "Price1Filter";
            //if (ORPrice1FiltersElementType56 == "Price1Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORPrice1Filters.Price1Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for Price1
            //ItemInventoryQueryRq.ORPrice1Filters.Price1Filter.Price1.SetValue(10.01);
            //}
            //if (ORPrice1FiltersElementType56 == "Price1RangeFilter")
            //{
            ////Set field value for FromPrice1
            //ItemInventoryQueryRq.ORPrice1Filters.Price1RangeFilter.FromPrice1.SetValue(10.01);
            ////Set field value for ToPrice1
            //ItemInventoryQueryRq.ORPrice1Filters.Price1RangeFilter.ToPrice1.SetValue(10.01);
            //}
            //string ORPrice2FiltersElementType57 = "Price2Filter";
            //if (ORPrice2FiltersElementType57 == "Price2Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORPrice2Filters.Price2Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for Price2
            //ItemInventoryQueryRq.ORPrice2Filters.Price2Filter.Price2.SetValue(10.01);
            //}
            //if (ORPrice2FiltersElementType57 == "Price2RangeFilter")
            //{
            ////Set field value for FromPrice2
            //ItemInventoryQueryRq.ORPrice2Filters.Price2RangeFilter.FromPrice2.SetValue(10.01);
            ////Set field value for ToPrice2
            //ItemInventoryQueryRq.ORPrice2Filters.Price2RangeFilter.ToPrice2.SetValue(10.01);
            //}
            //string ORPrice3FiltersElementType58 = "Price3Filter";
            //if (ORPrice3FiltersElementType58 == "Price3Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORPrice3Filters.Price3Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for Price3
            //ItemInventoryQueryRq.ORPrice3Filters.Price3Filter.Price3.SetValue(10.01);
            //}
            //if (ORPrice3FiltersElementType58 == "Price3RangeFilter")
            //{
            ////Set field value for FromPrice3
            //ItemInventoryQueryRq.ORPrice3Filters.Price3RangeFilter.FromPrice3.SetValue(10.01);
            ////Set field value for ToPrice3
            //ItemInventoryQueryRq.ORPrice3Filters.Price3RangeFilter.ToPrice3.SetValue(10.01);
            //}
            //string ORPrice4FiltersElementType59 = "Price4Filter";
            //if (ORPrice4FiltersElementType59 == "Price4Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORPrice4Filters.Price4Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for Price4
            //ItemInventoryQueryRq.ORPrice4Filters.Price4Filter.Price4.SetValue(10.01);
            //}
            //if (ORPrice4FiltersElementType59 == "Price4RangeFilter")
            //{
            ////Set field value for FromPrice4
            //ItemInventoryQueryRq.ORPrice4Filters.Price4RangeFilter.FromPrice4.SetValue(10.01);
            ////Set field value for ToPrice4
            //ItemInventoryQueryRq.ORPrice4Filters.Price4RangeFilter.ToPrice4.SetValue(10.01);
            //}
            //string ORPrice5FiltersElementType60 = "Price5Filter";
            //if (ORPrice5FiltersElementType60 == "Price5Filter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORPrice5Filters.Price5Filter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for Price5
            //ItemInventoryQueryRq.ORPrice5Filters.Price5Filter.Price5.SetValue(10.01);
            //}
            //if (ORPrice5FiltersElementType60 == "Price5RangeFilter")
            //{
            ////Set field value for FromPrice5
            //ItemInventoryQueryRq.ORPrice5Filters.Price5RangeFilter.FromPrice5.SetValue(10.01);
            ////Set field value for ToPrice5
            //ItemInventoryQueryRq.ORPrice5Filters.Price5RangeFilter.ToPrice5.SetValue(10.01);
            //}
            //string ORQuantityOnCustomerOrderFiltersElementType61 = "QuantityOnCustomerOrderFilter";
            //if (ORQuantityOnCustomerOrderFiltersElementType61 == "QuantityOnCustomerOrderFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORQuantityOnCustomerOrderFilters.QuantityOnCustomerOrderFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for QuantityOnCustomerOrder
            //ItemInventoryQueryRq.ORQuantityOnCustomerOrderFilters.QuantityOnCustomerOrderFilter.QuantityOnCustomerOrder.SetValue(2);
            //}
            //if (ORQuantityOnCustomerOrderFiltersElementType61 == "QuantityOnCustomerOrderRangeFilter")
            //{
            ////Set field value for FromQuantityOnCustomerOrder
            //ItemInventoryQueryRq.ORQuantityOnCustomerOrderFilters.QuantityOnCustomerOrderRangeFilter.FromQuantityOnCustomerOrder.SetValue(2);
            ////Set field value for ToQuantityOnCustomerOrder
            //ItemInventoryQueryRq.ORQuantityOnCustomerOrderFilters.QuantityOnCustomerOrderRangeFilter.ToQuantityOnCustomerOrder.SetValue(2);
            //}
            //string ORQuantityOnHandFiltersElementType62 = "QuantityOnHandFilter";
            //if (ORQuantityOnHandFiltersElementType62 == "QuantityOnHandFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORQuantityOnHandFilters.QuantityOnHandFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for QuantityOnHand
            //ItemInventoryQueryRq.ORQuantityOnHandFilters.QuantityOnHandFilter.QuantityOnHand.SetValue(2);
            //}
            //if (ORQuantityOnHandFiltersElementType62 == "QuantityOnHandRangeFilter")
            //{
            ////Set field value for FromQuantityOnHand
            //ItemInventoryQueryRq.ORQuantityOnHandFilters.QuantityOnHandRangeFilter.FromQuantityOnHand.SetValue(2);
            ////Set field value for ToQuantityOnHand
            //ItemInventoryQueryRq.ORQuantityOnHandFilters.QuantityOnHandRangeFilter.ToQuantityOnHand.SetValue(2);
            //}
            //string ORQuantityOnOrderFiltersElementType63 = "QuantityOnOrderFilter";
            //if (ORQuantityOnOrderFiltersElementType63 == "QuantityOnOrderFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORQuantityOnOrderFilters.QuantityOnOrderFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for QuantityOnOrder
            //ItemInventoryQueryRq.ORQuantityOnOrderFilters.QuantityOnOrderFilter.QuantityOnOrder.SetValue(2);
            //}
            //if (ORQuantityOnOrderFiltersElementType63 == "QuantityOnOrderRangeFilter")
            //{
            ////Set field value for FromQuantityOnOrder
            //ItemInventoryQueryRq.ORQuantityOnOrderFilters.QuantityOnOrderRangeFilter.FromQuantityOnOrder.SetValue(2);
            ////Set field value for ToQuantityOnOrder
            //ItemInventoryQueryRq.ORQuantityOnOrderFilters.QuantityOnOrderRangeFilter.ToQuantityOnOrder.SetValue(2);
            //}
            //string ORQuantityOnPendingOrderFiltersElementType64 = "QuantityOnPendingOrderFilter";
            //if (ORQuantityOnPendingOrderFiltersElementType64 == "QuantityOnPendingOrderFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORQuantityOnPendingOrderFilters.QuantityOnPendingOrderFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for QuantityOnPendingOrder
            //ItemInventoryQueryRq.ORQuantityOnPendingOrderFilters.QuantityOnPendingOrderFilter.QuantityOnPendingOrder.SetValue(2);
            //}
            //if (ORQuantityOnPendingOrderFiltersElementType64 == "QuantityOnPendingOrderRangeFilter")
            //{
            ////Set field value for FromQuantityOnPendingOrder
            //ItemInventoryQueryRq.ORQuantityOnPendingOrderFilters.QuantityOnPendingOrderRangeFilter.FromQuantityOnPendingOrder.SetValue(2);
            ////Set field value for ToQuantityOnPendingOrder
            //ItemInventoryQueryRq.ORQuantityOnPendingOrderFilters.QuantityOnPendingOrderRangeFilter.ToQuantityOnPendingOrder.SetValue(2);
            //}
            //string ORReorderPointFiltersElementType65 = "ReorderPointFilter";
            //if (ORReorderPointFiltersElementType65 == "ReorderPointFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORReorderPointFilters.ReorderPointFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for ReorderPoint
            //ItemInventoryQueryRq.ORReorderPointFilters.ReorderPointFilter.ReorderPoint.SetValue(2);
            //}
            //if (ORReorderPointFiltersElementType65 == "ReorderPointRangeFilter")
            //{
            ////Set field value for FromReorderPoint
            //ItemInventoryQueryRq.ORReorderPointFilters.ReorderPointRangeFilter.FromReorderPoint.SetValue(2);
            ////Set field value for ToReorderPoint
            //ItemInventoryQueryRq.ORReorderPointFilters.ReorderPointRangeFilter.ToReorderPoint.SetValue(2);
            //}
            //string ORSellByUnitFiltersElementType66 = "SellByUnitFilter";
            //if (ORSellByUnitFiltersElementType66 == "SellByUnitFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORSellByUnitFilters.SellByUnitFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for SellByUnit
            //ItemInventoryQueryRq.ORSellByUnitFilters.SellByUnitFilter.SellByUnit.SetValue("ab");
            //}
            //if (ORSellByUnitFiltersElementType66 == "SellByUnitRangeFilter")
            //{
            ////Set field value for FromSellByUnit
            //ItemInventoryQueryRq.ORSellByUnitFilters.SellByUnitRangeFilter.FromSellByUnit.SetValue("ab");
            ////Set field value for ToSellByUnit
            //ItemInventoryQueryRq.ORSellByUnitFilters.SellByUnitRangeFilter.ToSellByUnit.SetValue("ab");
            //}
            ////Set field value for SerialFlag
            //ItemInventoryQueryRq.SerialFlag.SetValue(ENSerialFlag.sfOptional);
            //string ORSizeFiltersElementType67 = "SizeFilter";
            //if (ORSizeFiltersElementType67 == "SizeFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORSizeFilters.SizeFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for Size
            //ItemInventoryQueryRq.ORSizeFilters.SizeFilter.Size.SetValue("ab");
            //}
            //if (ORSizeFiltersElementType67 == "SizeRangeFilter")
            //{
            ////Set field value for FromSize
            //ItemInventoryQueryRq.ORSizeFilters.SizeRangeFilter.FromSize.SetValue("ab");
            ////Set field value for ToSize
            //ItemInventoryQueryRq.ORSizeFilters.SizeRangeFilter.ToSize.SetValue("ab");
            //}
            ////Set field value for StoreExchangeStatus
            //ItemInventoryQueryRq.StoreExchangeStatus.SetValue(ENStoreExchangeStatus.sesModified);
            ////Set field value for TaxCode
            //ItemInventoryQueryRq.TaxCode.SetValue("ab");
            //string ORUnitOfMeasureFiltersElementType68 = "UnitOfMeasureFilter";
            //if (ORUnitOfMeasureFiltersElementType68 == "UnitOfMeasureFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORUnitOfMeasureFilters.UnitOfMeasureFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for UnitOfMeasure
            //ItemInventoryQueryRq.ORUnitOfMeasureFilters.UnitOfMeasureFilter.UnitOfMeasure.SetValue("ab");
            //}
            //if (ORUnitOfMeasureFiltersElementType68 == "UnitOfMeasureRangeFilter")
            //{
            ////Set field value for FromUnitOfMeasure
            //ItemInventoryQueryRq.ORUnitOfMeasureFilters.UnitOfMeasureRangeFilter.FromUnitOfMeasure.SetValue("ab");
            ////Set field value for ToUnitOfMeasure
            //ItemInventoryQueryRq.ORUnitOfMeasureFilters.UnitOfMeasureRangeFilter.ToUnitOfMeasure.SetValue("ab");
            //}
            //string ORUPCFiltersElementType69 = "UPCFilter";
            //if (ORUPCFiltersElementType69 == "UPCFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORUPCFilters.UPCFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for UPC
            //ItemInventoryQueryRq.ORUPCFilters.UPCFilter.UPC.SetValue("ab");
            //}
            //if (ORUPCFiltersElementType69 == "UPCRangeFilter")
            //{
            ////Set field value for FromUPC
            //ItemInventoryQueryRq.ORUPCFilters.UPCRangeFilter.FromUPC.SetValue("ab");
            ////Set field value for ToUPC
            //ItemInventoryQueryRq.ORUPCFilters.UPCRangeFilter.ToUPC.SetValue("ab");
            //}
            //string ORVendorCodeFiltersElementType70 = "VendorCodeFilter";
            //if (ORVendorCodeFiltersElementType70 == "VendorCodeFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORVendorCodeFilters.VendorCodeFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for VendorCode
            //ItemInventoryQueryRq.ORVendorCodeFilters.VendorCodeFilter.VendorCode.SetValue("ab");
            //}
            //if (ORVendorCodeFiltersElementType70 == "VendorCodeRangeFilter")
            //{
            ////Set field value for FromVendorCode
            //ItemInventoryQueryRq.ORVendorCodeFilters.VendorCodeRangeFilter.FromVendorCode.SetValue("ab");
            ////Set field value for ToVendorCode
            //ItemInventoryQueryRq.ORVendorCodeFilters.VendorCodeRangeFilter.ToVendorCode.SetValue("ab");
            //}
            ////Set field value for VendorListID
            //ItemInventoryQueryRq.VendorListID.SetValue("200000-1011023419");
            //string ORWebDescFiltersElementType71 = "WebDescFilter";
            //if (ORWebDescFiltersElementType71 == "WebDescFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORWebDescFilters.WebDescFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for WebDesc
            //ItemInventoryQueryRq.ORWebDescFilters.WebDescFilter.WebDesc.SetValue("ab");
            //}
            //if (ORWebDescFiltersElementType71 == "WebDescRangeFilter")
            //{
            ////Set field value for FromWebDesc
            //ItemInventoryQueryRq.ORWebDescFilters.WebDescRangeFilter.FromWebDesc.SetValue("ab");
            ////Set field value for ToWebDesc
            //ItemInventoryQueryRq.ORWebDescFilters.WebDescRangeFilter.ToWebDesc.SetValue("ab");
            //}
            //string ORWebPriceFiltersElementType72 = "WebPriceFilter";
            //if (ORWebPriceFiltersElementType72 == "WebPriceFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORWebPriceFilters.WebPriceFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for WebPrice
            //ItemInventoryQueryRq.ORWebPriceFilters.WebPriceFilter.WebPrice.SetValue(10.01);
            //}
            //if (ORWebPriceFiltersElementType72 == "WebPriceRangeFilter")
            //{
            ////Set field value for FromWebPrice
            //ItemInventoryQueryRq.ORWebPriceFilters.WebPriceRangeFilter.FromWebPrice.SetValue(10.01);
            ////Set field value for ToWebPrice
            //ItemInventoryQueryRq.ORWebPriceFilters.WebPriceRangeFilter.ToWebPrice.SetValue(10.01);
            //}
            //string ORManufacturerFiltersElementType73 = "ManufacturerFilter";
            //if (ORManufacturerFiltersElementType73 == "ManufacturerFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORManufacturerFilters.ManufacturerFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for Manufacturer
            //ItemInventoryQueryRq.ORManufacturerFilters.ManufacturerFilter.Manufacturer.SetValue("ab");
            //}
            //if (ORManufacturerFiltersElementType73 == "ManufacturerRangeFilter")
            //{
            ////Set field value for FromManufacturer
            //ItemInventoryQueryRq.ORManufacturerFilters.ManufacturerRangeFilter.FromManufacturer.SetValue("ab");
            ////Set field value for ToManufacturer
            //ItemInventoryQueryRq.ORManufacturerFilters.ManufacturerRangeFilter.ToManufacturer.SetValue("ab");
            //}
            //string ORWeightFiltersElementType74 = "WeightFilter";
            //if (ORWeightFiltersElementType74 == "WeightFilter")
            //{
            ////Set field value for MatchNumericCriterion
            //ItemInventoryQueryRq.ORWeightFilters.WeightFilter.MatchNumericCriterion.SetValue(ENMatchNumericCriterion.mncLessThan);
            ////Set field value for Weight
            //ItemInventoryQueryRq.ORWeightFilters.WeightFilter.Weight.SetValue("IQBFloatType");
            //}
            //if (ORWeightFiltersElementType74 == "WeightRangeFilter")
            //{
            ////Set field value for FromWeight
            //ItemInventoryQueryRq.ORWeightFilters.WeightRangeFilter.FromWeight.SetValue("IQBFloatType");
            ////Set field value for ToWeight
            //ItemInventoryQueryRq.ORWeightFilters.WeightRangeFilter.ToWeight.SetValue("IQBFloatType");
            //}
            //string ORWebSKUFiltersElementType75 = "WebSKUFilter";
            //if (ORWebSKUFiltersElementType75 == "WebSKUFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORWebSKUFilters.WebSKUFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for WebSKU
            //ItemInventoryQueryRq.ORWebSKUFilters.WebSKUFilter.WebSKU.SetValue("ab");
            //}
            //if (ORWebSKUFiltersElementType75 == "WebSKURangeFilter")
            //{
            ////Set field value for FromWebSKU
            //ItemInventoryQueryRq.ORWebSKUFilters.WebSKURangeFilter.FromWebSKU.SetValue("ab");
            ////Set field value for ToWebSKU
            //ItemInventoryQueryRq.ORWebSKUFilters.WebSKURangeFilter.ToWebSKU.SetValue("ab");
            //}
            //string ORKeywordsFiltersElementType76 = "KeywordsFilter";
            //if (ORKeywordsFiltersElementType76 == "KeywordsFilter")
            //{
            ////Set field value for MatchStringCriterion
            //ItemInventoryQueryRq.ORKeywordsFilters.KeywordsFilter.MatchStringCriterion.SetValue(ENMatchStringCriterion.mscEqual);
            ////Set field value for Keywords
            //ItemInventoryQueryRq.ORKeywordsFilters.KeywordsFilter.Keywords.SetValue("ab");
            //}
            //if (ORKeywordsFiltersElementType76 == "KeywordsRangeFilter")
            //{
            ////Set field value for FromKeywords
            //ItemInventoryQueryRq.ORKeywordsFilters.KeywordsRangeFilter.FromKeywords.SetValue("ab");
            ////Set field value for ToKeywords
            //ItemInventoryQueryRq.ORKeywordsFilters.KeywordsRangeFilter.ToKeywords.SetValue("ab");
            //}
            ////Set field value for IncludeRetElementList
            ////May create more than one of these if needed
            //ItemInventoryQueryRq.IncludeRetElementList.Add("ab");
            //}
            #endregion

        }

        public async Task<Tuple<string>> WalkItemInventoryQueryRs(IMsgSetResponse responseMsgSet)
        {
            if (responseMsgSet == null)
            {
              
                return new Tuple<string>("0");
            }

            var responseList = responseMsgSet.ResponseList;
            if (responseList == null)
            {
                return new Tuple<string>("0");
            }

            //if we sent only one request, there is only one response, we'll walk the list for this sample
            for (var i = 0; i < responseList.Count; i++)
            {
                var response = responseList.GetAt(i);
                //check the status code of the response, 0=ok, >0 is warning
                if (response.StatusCode >= 0)
                {
                    //the request-specific response is in the details, make sure we have some
                    if (response.Detail != null)
                    {
                        //make sure the response is the type we're expecting
                        var responseType = (ENResponseType)response.Type.GetValue();
                        if (responseType == ENResponseType.rtItemInventoryQueryRs)
                        {
                            //upcast to more specific type here, this is safe because we checked with response.Type check above
                            var ItemInventoryRet = (IItemInventoryRetList)response.Detail;
                            await SaveInventoryToDB(WalkItemInventoryRet(ItemInventoryRet)).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        return new Tuple<string>("0");
                    }
                }
            }
            return new Tuple<string>("Complete");
        }

        private async Task SaveInventoryToDB(IEnumerable<ItemInventoryRet> trackableCollection)
        {
            try
            {

                StatusModel.StartStatusUpdate("Importing Inventory Items", trackableCollection.Count());
                using (var ctx = new InventoryDSContext())
                {
                    foreach (var itm in trackableCollection) //.Where(x => x.ItemNumber == 12059).ToList()
                    {
                        var itmnumber = itm.ItemNumber.ToString();
                        var i = ctx.InventoryItems.Find(itmnumber);

                        if (i == null)
                        {
                            i = new InventoryItem(true) {TrackingState = TrackingState.Added};

                        }

                        i.Description = itm.Desc1 + "|" + itm.Desc2 + "|" + itm.Attribute;
                        i.ItemNumber = itm.ItemNumber.ToString(CultureInfo.InvariantCulture);
                        i.Quantity = Convert.ToInt32(itm.QuantityOnHand);
                        StatusModel.StatusUpdate();
                        ctx.ApplyChanges(i);
                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                StatusModel.StopStatusUpdate();
               // MessageBox.Show(@"Inventory Items Import Complete");

            }
            catch (Exception)
            {

                throw;
            }


        }


        private IEnumerable<ItemInventoryRet> WalkItemInventoryRet(IItemInventoryRetList ItemInventoryRetList)
        {
            try
            {

                var itmList = new List<ItemInventoryRet>();

                for (var i = 0; i < ItemInventoryRetList.Count; i++)
                {
                    var positm = ItemInventoryRetList.GetAt(i);
                    var itm = new ItemInventoryRet();

                    if (positm == null) continue;

                    //Go through all the elements of IpositmList
                    //Get value of ListID
                    if (positm.ListID != null)
                    {
                        itm.ListID = (string) positm.ListID.GetValue();
                    }

                    //Get value of ALU
                    if (positm.ALU != null)
                    {
                        itm.ALU = (string) positm.ALU.GetValue();
                    }
                    //Get value of Attribute
                    if (positm.Attribute != null)
                    {
                        itm.Attribute = (string) positm.Attribute.GetValue();
                    }
                    //Get value of DepartmentCode
                    if (positm.DepartmentCode != null)
                    {
                        itm.DepartmentCode = (string) positm.DepartmentCode.GetValue();
                    }
                    //Get value of Desc1
                    if (positm.Desc1 != null)
                    {
                        itm.Desc1 = (string) positm.Desc1.GetValue();
                    }
                    //Get value of Desc2
                    if (positm.Desc2 != null)
                    {
                        itm.Desc2 = (string) positm.Desc2.GetValue();
                    }
                    //Get value of ItemNumber
                    if (positm.ItemNumber != null)
                    {
                        itm.ItemNumber = (int) positm.ItemNumber.GetValue();
                    }
                    //Get value of ItemType
                    if (positm.ItemType != null)
                    {
                        itm.ItemType = ((ENItemType) positm.ItemType.GetValue()).ToString();
                    }
                    //Get value of Size
                    if (positm.Size != null)
                    {
                        itm.Size = (string) positm.Size.GetValue();
                    }

                    //if (positm.DataExtRetList != null)
                    //{
                    //    for (var x = 0; x < positm.DataExtRetList.Count; x++)
                    //    {
                    //        var xd = new ItemInventoryRetDataExtRet();


                    //        var DataExtRet = positm.DataExtRetList.GetAt(x);

                    //        if (DataExtRet.DataExtValue != null)
                    //        {
                    //            xd.DataExtValue = (string) DataExtRet.DataExtValue.GetValue();

                    //            itm.DataExtRetList.Add(xd);
                    //        }
                    //        else
                    //        {
                    //            continue;
                    //        }
                    //        //Get value of OwnerID
                    //        xd.OwnerID = (string) DataExtRet.OwnerID.GetValue();
                    //        //Get value of DataExtName
                    //        xd.DataExtName = (string) DataExtRet.DataExtName.GetValue();
                    //        //Get value of DataExtType
                    //        xd.DataExtType =
                    //            Enum.Parse(typeof (ENDataExtType), DataExtRet.DataExtType.GetValue().ToString(), true)
                    //                .ToString();
                    //        //Get value of DataExtValue

                    //    }
                    //}

                    #region "More Properties"

                    //        //Get value of TimeCreated
                    //if (ItemInventoryRet.TimeCreated != null)
                    //{
                    //DateTime TimeCreated78 = (DateTime)ItemInventoryRet.TimeCreated.GetValue();
                    //}
                    ////Get value of TimeModified
                    //if (ItemInventoryRet.TimeModified != null)
                    //{
                    //DateTime TimeModified79 = (DateTime)ItemInventoryRet.TimeModified.GetValue();
                    //}
                    ////Get value of COGSAccount
                    //if (ItemInventoryRet.COGSAccount != null)
                    //{
                    //string COGSAccount82 = (string)ItemInventoryRet.COGSAccount.GetValue();
                    //}
                    ////Get value of Cost
                    //if (ItemInventoryRet.Cost != null)
                    //{
                    //double Cost83 = (double)ItemInventoryRet.Cost.GetValue();
                    //}
                    ////Get value of DepartmentListID
                    //if (ItemInventoryRet.DepartmentListID != null)
                    //{
                    //string DepartmentListID85 = (string)ItemInventoryRet.DepartmentListID.GetValue();
                    //}
                    ////Get value of IncomeAccount
                    //if (ItemInventoryRet.IncomeAccount != null)
                    //{
                    //string IncomeAccount88 = (string)ItemInventoryRet.IncomeAccount.GetValue();
                    //}
                    ////Get value of IsBelowReorder
                    //if (ItemInventoryRet.IsBelowReorder != null)
                    //{
                    //bool IsBelowReorder89 = (bool)ItemInventoryRet.IsBelowReorder.GetValue();
                    //}
                    ////Get value of IsEligibleForCommission
                    //if (ItemInventoryRet.IsEligibleForCommission != null)
                    //{
                    //bool IsEligibleForCommission90 = (bool)ItemInventoryRet.IsEligibleForCommission.GetValue();
                    //}
                    ////Get value of IsPrintingTags
                    //if (ItemInventoryRet.IsPrintingTags != null)
                    //{
                    //bool IsPrintingTags91 = (bool)ItemInventoryRet.IsPrintingTags.GetValue();
                    //}
                    ////Get value of IsUnorderable
                    //if (ItemInventoryRet.IsUnorderable != null)
                    //{
                    //bool IsUnorderable92 = (bool)ItemInventoryRet.IsUnorderable.GetValue();
                    //}
                    ////Get value of HasPictures
                    //if (ItemInventoryRet.HasPictures != null)
                    //{
                    //bool HasPictures93 = (bool)ItemInventoryRet.HasPictures.GetValue();
                    //}
                    ////Get value of IsEligibleForRewards
                    //if (ItemInventoryRet.IsEligibleForRewards != null)
                    //{
                    //bool IsEligibleForRewards94 = (bool)ItemInventoryRet.IsEligibleForRewards.GetValue();
                    //}
                    ////Get value of IsWebItem
                    //if (ItemInventoryRet.IsWebItem != null)
                    //{
                    //bool IsWebItem95 = (bool)ItemInventoryRet.IsWebItem.GetValue();
                    //}
                    ////Get value of LastReceived
                    //if (ItemInventoryRet.LastReceived != null)
                    //{
                    //DateTime LastReceived98 = (DateTime)ItemInventoryRet.LastReceived.GetValue();
                    //}
                    ////Get value of MarginPercent
                    //if (ItemInventoryRet.MarginPercent != null)
                    //{
                    //int MarginPercent99 = (int)ItemInventoryRet.MarginPercent.GetValue();
                    //}
                    ////Get value of MarkupPercent
                    //if (ItemInventoryRet.MarkupPercent != null)
                    //{
                    //int MarkupPercent100 = (int)ItemInventoryRet.MarkupPercent.GetValue();
                    //}
                    ////Get value of MSRP
                    //if (ItemInventoryRet.MSRP != null)
                    //{
                    //double MSRP101 = (double)ItemInventoryRet.MSRP.GetValue();
                    //}
                    ////Get value of OnHandStore01
                    //if (ItemInventoryRet.OnHandStore01 != null)
                    //{
                    //int OnHandStore01102 = (int)ItemInventoryRet.OnHandStore01.GetValue();
                    //}
                    ////Get value of OnHandStore02
                    //if (ItemInventoryRet.OnHandStore02 != null)
                    //{
                    //int OnHandStore02103 = (int)ItemInventoryRet.OnHandStore02.GetValue();
                    //}
                    ////Get value of OnHandStore03
                    //if (ItemInventoryRet.OnHandStore03 != null)
                    //{
                    //int OnHandStore03104 = (int)ItemInventoryRet.OnHandStore03.GetValue();
                    //}
                    ////Get value of OnHandStore04
                    //if (ItemInventoryRet.OnHandStore04 != null)
                    //{
                    //int OnHandStore04105 = (int)ItemInventoryRet.OnHandStore04.GetValue();
                    //}
                    ////Get value of OnHandStore05
                    //if (ItemInventoryRet.OnHandStore05 != null)
                    //{
                    //int OnHandStore05106 = (int)ItemInventoryRet.OnHandStore05.GetValue();
                    //}
                    ////Get value of OnHandStore06
                    //if (ItemInventoryRet.OnHandStore06 != null)
                    //{
                    //int OnHandStore06107 = (int)ItemInventoryRet.OnHandStore06.GetValue();
                    //}
                    ////Get value of OnHandStore07
                    //if (ItemInventoryRet.OnHandStore07 != null)
                    //{
                    //int OnHandStore07108 = (int)ItemInventoryRet.OnHandStore07.GetValue();
                    //}
                    ////Get value of OnHandStore08
                    //if (ItemInventoryRet.OnHandStore08 != null)
                    //{
                    //int OnHandStore08109 = (int)ItemInventoryRet.OnHandStore08.GetValue();
                    //}
                    ////Get value of OnHandStore09
                    //if (ItemInventoryRet.OnHandStore09 != null)
                    //{
                    //int OnHandStore09110 = (int)ItemInventoryRet.OnHandStore09.GetValue();
                    //}
                    ////Get value of OnHandStore10
                    //if (ItemInventoryRet.OnHandStore10 != null)
                    //{
                    //int OnHandStore10111 = (int)ItemInventoryRet.OnHandStore10.GetValue();
                    //}
                    ////Get value of OnHandStore11
                    //if (ItemInventoryRet.OnHandStore11 != null)
                    //{
                    //int OnHandStore11112 = (int)ItemInventoryRet.OnHandStore11.GetValue();
                    //}
                    ////Get value of OnHandStore12
                    //if (ItemInventoryRet.OnHandStore12 != null)
                    //{
                    //int OnHandStore12113 = (int)ItemInventoryRet.OnHandStore12.GetValue();
                    //}
                    ////Get value of OnHandStore13
                    //if (ItemInventoryRet.OnHandStore13 != null)
                    //{
                    //int OnHandStore13114 = (int)ItemInventoryRet.OnHandStore13.GetValue();
                    //}
                    ////Get value of OnHandStore14
                    //if (ItemInventoryRet.OnHandStore14 != null)
                    //{
                    //int OnHandStore14115 = (int)ItemInventoryRet.OnHandStore14.GetValue();
                    //}
                    ////Get value of OnHandStore15
                    //if (ItemInventoryRet.OnHandStore15 != null)
                    //{
                    //int OnHandStore15116 = (int)ItemInventoryRet.OnHandStore15.GetValue();
                    //}
                    ////Get value of OnHandStore16
                    //if (ItemInventoryRet.OnHandStore16 != null)
                    //{
                    //int OnHandStore16117 = (int)ItemInventoryRet.OnHandStore16.GetValue();
                    //}
                    ////Get value of OnHandStore17
                    //if (ItemInventoryRet.OnHandStore17 != null)
                    //{
                    //int OnHandStore17118 = (int)ItemInventoryRet.OnHandStore17.GetValue();
                    //}
                    ////Get value of OnHandStore18
                    //if (ItemInventoryRet.OnHandStore18 != null)
                    //{
                    //int OnHandStore18119 = (int)ItemInventoryRet.OnHandStore18.GetValue();
                    //}
                    ////Get value of OnHandStore19
                    //if (ItemInventoryRet.OnHandStore19 != null)
                    //{
                    //int OnHandStore19120 = (int)ItemInventoryRet.OnHandStore19.GetValue();
                    //}
                    ////Get value of OnHandStore20
                    //if (ItemInventoryRet.OnHandStore20 != null)
                    //{
                    //int OnHandStore20121 = (int)ItemInventoryRet.OnHandStore20.GetValue();
                    //}
                    ////Get value of ReorderPointStore01
                    //if (ItemInventoryRet.ReorderPointStore01 != null)
                    //{
                    //int ReorderPointStore01122 = (int)ItemInventoryRet.ReorderPointStore01.GetValue();
                    //}
                    ////Get value of ReorderPointStore02
                    //if (ItemInventoryRet.ReorderPointStore02 != null)
                    //{
                    //int ReorderPointStore02123 = (int)ItemInventoryRet.ReorderPointStore02.GetValue();
                    //}
                    ////Get value of ReorderPointStore03
                    //if (ItemInventoryRet.ReorderPointStore03 != null)
                    //{
                    //int ReorderPointStore03124 = (int)ItemInventoryRet.ReorderPointStore03.GetValue();
                    //}
                    ////Get value of ReorderPointStore04
                    //if (ItemInventoryRet.ReorderPointStore04 != null)
                    //{
                    //int ReorderPointStore04125 = (int)ItemInventoryRet.ReorderPointStore04.GetValue();
                    //}
                    ////Get value of ReorderPointStore05
                    //if (ItemInventoryRet.ReorderPointStore05 != null)
                    //{
                    //int ReorderPointStore05126 = (int)ItemInventoryRet.ReorderPointStore05.GetValue();
                    //}
                    ////Get value of ReorderPointStore06
                    //if (ItemInventoryRet.ReorderPointStore06 != null)
                    //{
                    //int ReorderPointStore06127 = (int)ItemInventoryRet.ReorderPointStore06.GetValue();
                    //}
                    ////Get value of ReorderPointStore07
                    //if (ItemInventoryRet.ReorderPointStore07 != null)
                    //{
                    //int ReorderPointStore07128 = (int)ItemInventoryRet.ReorderPointStore07.GetValue();
                    //}
                    ////Get value of ReorderPointStore08
                    //if (ItemInventoryRet.ReorderPointStore08 != null)
                    //{
                    //int ReorderPointStore08129 = (int)ItemInventoryRet.ReorderPointStore08.GetValue();
                    //}
                    ////Get value of ReorderPointStore09
                    //if (ItemInventoryRet.ReorderPointStore09 != null)
                    //{
                    //int ReorderPointStore09130 = (int)ItemInventoryRet.ReorderPointStore09.GetValue();
                    //}
                    ////Get value of ReorderPointStore10
                    //if (ItemInventoryRet.ReorderPointStore10 != null)
                    //{
                    //int ReorderPointStore10131 = (int)ItemInventoryRet.ReorderPointStore10.GetValue();
                    //}
                    ////Get value of ReorderPointStore11
                    //if (ItemInventoryRet.ReorderPointStore11 != null)
                    //{
                    //int ReorderPointStore11132 = (int)ItemInventoryRet.ReorderPointStore11.GetValue();
                    //}
                    ////Get value of ReorderPointStore12
                    //if (ItemInventoryRet.ReorderPointStore12 != null)
                    //{
                    //int ReorderPointStore12133 = (int)ItemInventoryRet.ReorderPointStore12.GetValue();
                    //}
                    ////Get value of ReorderPointStore13
                    //if (ItemInventoryRet.ReorderPointStore13 != null)
                    //{
                    //int ReorderPointStore13134 = (int)ItemInventoryRet.ReorderPointStore13.GetValue();
                    //}
                    ////Get value of ReorderPointStore14
                    //if (ItemInventoryRet.ReorderPointStore14 != null)
                    //{
                    //int ReorderPointStore14135 = (int)ItemInventoryRet.ReorderPointStore14.GetValue();
                    //}
                    ////Get value of ReorderPointStore15
                    //if (ItemInventoryRet.ReorderPointStore15 != null)
                    //{
                    //int ReorderPointStore15136 = (int)ItemInventoryRet.ReorderPointStore15.GetValue();
                    //}
                    ////Get value of ReorderPointStore16
                    //if (ItemInventoryRet.ReorderPointStore16 != null)
                    //{
                    //int ReorderPointStore16137 = (int)ItemInventoryRet.ReorderPointStore16.GetValue();
                    //}
                    ////Get value of ReorderPointStore17
                    //if (ItemInventoryRet.ReorderPointStore17 != null)
                    //{
                    //int ReorderPointStore17138 = (int)ItemInventoryRet.ReorderPointStore17.GetValue();
                    //}
                    ////Get value of ReorderPointStore18
                    //if (ItemInventoryRet.ReorderPointStore18 != null)
                    //{
                    //int ReorderPointStore18139 = (int)ItemInventoryRet.ReorderPointStore18.GetValue();
                    //}
                    ////Get value of ReorderPointStore19
                    //if (ItemInventoryRet.ReorderPointStore19 != null)
                    //{
                    //int ReorderPointStore19140 = (int)ItemInventoryRet.ReorderPointStore19.GetValue();
                    //}
                    ////Get value of ReorderPointStore20
                    //if (ItemInventoryRet.ReorderPointStore20 != null)
                    //{
                    //int ReorderPointStore20141 = (int)ItemInventoryRet.ReorderPointStore20.GetValue();
                    //}
                    ////Get value of OrderByUnit
                    //if (ItemInventoryRet.OrderByUnit != null)
                    //{
                    //string OrderByUnit142 = (string)ItemInventoryRet.OrderByUnit.GetValue();
                    //}
                    ////Get value of OrderCost
                    //if (ItemInventoryRet.OrderCost != null)
                    //{
                    //double OrderCost143 = (double)ItemInventoryRet.OrderCost.GetValue();
                    //}
                    ////Get value of Price1
                    //if (ItemInventoryRet.Price1 != null)
                    //{
                    //double Price1144 = (double)ItemInventoryRet.Price1.GetValue();
                    //}
                    ////Get value of Price2
                    //if (ItemInventoryRet.Price2 != null)
                    //{
                    //double Price2145 = (double)ItemInventoryRet.Price2.GetValue();
                    //}
                    ////Get value of Price3
                    //if (ItemInventoryRet.Price3 != null)
                    //{
                    //double Price3146 = (double)ItemInventoryRet.Price3.GetValue();
                    //}
                    ////Get value of Price4
                    //if (ItemInventoryRet.Price4 != null)
                    //{
                    //double Price4147 = (double)ItemInventoryRet.Price4.GetValue();
                    //}
                    ////Get value of Price5
                    //if (ItemInventoryRet.Price5 != null)
                    //{
                    //double Price5148 = (double)ItemInventoryRet.Price5.GetValue();
                    //}
                    ////Get value of QuantityOnCustomerOrder
                    //if (ItemInventoryRet.QuantityOnCustomerOrder != null)
                    //{
                    //int QuantityOnCustomerOrder149 = (int)ItemInventoryRet.QuantityOnCustomerOrder.GetValue();
                    //}
                    ////Get value of QuantityOnHand
                    //if (ItemInventoryRet.QuantityOnHand != null)
                    //{
                    //int QuantityOnHand150 = (int)ItemInventoryRet.QuantityOnHand.GetValue();
                    //}
                    ////Get value of QuantityOnOrder
                    //if (ItemInventoryRet.QuantityOnOrder != null)
                    //{
                    //int QuantityOnOrder151 = (int)ItemInventoryRet.QuantityOnOrder.GetValue();
                    //}
                    ////Get value of QuantityOnPendingOrder
                    //if (ItemInventoryRet.QuantityOnPendingOrder != null)
                    //{
                    //int QuantityOnPendingOrder152 = (int)ItemInventoryRet.QuantityOnPendingOrder.GetValue();
                    //}
                    //if (ItemInventoryRet.AvailableQtyList != null)
                    //{
                    //for (int i153 = 0; i153 < ItemInventoryRet.AvailableQtyList.Count; i153++)
                    //{
                    //IAvailableQty AvailableQty = ItemInventoryRet.AvailableQtyList.GetAt(i153);
                    ////Get value of StoreNumber
                    //if (AvailableQty.StoreNumber != null)
                    //{
                    //int StoreNumber154 = (int)AvailableQty.StoreNumber.GetValue();
                    //}
                    ////Get value of QuantityOnOrder
                    //if (AvailableQty.QuantityOnOrder != null)
                    //{
                    //int QuantityOnOrder155 = (int)AvailableQty.QuantityOnOrder.GetValue();
                    //}
                    ////Get value of QuantityOnCustomerOrder
                    //if (AvailableQty.QuantityOnCustomerOrder != null)
                    //{
                    //int QuantityOnCustomerOrder156 = (int)AvailableQty.QuantityOnCustomerOrder.GetValue();
                    //}
                    ////Get value of QuantityOnPendingOrder
                    //if (AvailableQty.QuantityOnPendingOrder != null)
                    //{
                    //int QuantityOnPendingOrder157 = (int)AvailableQty.QuantityOnPendingOrder.GetValue();
                    //}
                    //}
                    //}
                    ////Get value of ReorderPoint
                    //if (ItemInventoryRet.ReorderPoint != null)
                    //{
                    //int ReorderPoint158 = (int)ItemInventoryRet.ReorderPoint.GetValue();
                    //}
                    ////Get value of SellByUnit
                    //if (ItemInventoryRet.SellByUnit != null)
                    //{
                    //string SellByUnit159 = (string)ItemInventoryRet.SellByUnit.GetValue();
                    //}
                    ////Get value of SerialFlag
                    //if (ItemInventoryRet.SerialFlag != null)
                    //{
                    //ENSerialFlag SerialFlag160 = (ENSerialFlag)ItemInventoryRet.SerialFlag.GetValue();
                    //}

                    ////Get value of StoreExchangeStatus
                    //if (ItemInventoryRet.StoreExchangeStatus != null)
                    //{
                    //ENStoreExchangeStatus StoreExchangeStatus162 = (ENStoreExchangeStatus)ItemInventoryRet.StoreExchangeStatus.GetValue();
                    //}
                    ////Get value of TaxCode
                    //if (ItemInventoryRet.TaxCode != null)
                    //{
                    //string TaxCode163 = (string)ItemInventoryRet.TaxCode.GetValue();
                    //}
                    ////Get value of UnitOfMeasure
                    //if (ItemInventoryRet.UnitOfMeasure != null)
                    //{
                    //string UnitOfMeasure164 = (string)ItemInventoryRet.UnitOfMeasure.GetValue();
                    //}
                    ////Get value of UPC
                    //if (ItemInventoryRet.UPC != null)
                    //{
                    //string UPC165 = (string)ItemInventoryRet.UPC.GetValue();
                    //}
                    ////Get value of VendorCode
                    //if (ItemInventoryRet.VendorCode != null)
                    //{
                    //string VendorCode166 = (string)ItemInventoryRet.VendorCode.GetValue();
                    //}
                    ////Get value of VendorListID
                    //if (ItemInventoryRet.VendorListID != null)
                    //{
                    //string VendorListID167 = (string)ItemInventoryRet.VendorListID.GetValue();
                    //}
                    ////Get value of WebDesc
                    //if (ItemInventoryRet.WebDesc != null)
                    //{
                    //string WebDesc168 = (string)ItemInventoryRet.WebDesc.GetValue();
                    //}
                    ////Get value of WebPrice
                    //if (ItemInventoryRet.WebPrice != null)
                    //{
                    //double WebPrice169 = (double)ItemInventoryRet.WebPrice.GetValue();
                    //}
                    ////Get value of Manufacturer
                    //if (ItemInventoryRet.Manufacturer != null)
                    //{
                    //string Manufacturer170 = (string)ItemInventoryRet.Manufacturer.GetValue();
                    //}
                    ////Get value of Weight
                    //if (ItemInventoryRet.Weight != null)
                    //{
                    //IQBFloatType Weight171 = (IQBFloatType)ItemInventoryRet.Weight.GetValue();
                    //}
                    ////Get value of WebSKU
                    //if (ItemInventoryRet.WebSKU != null)
                    //{
                    //string WebSKU172 = (string)ItemInventoryRet.WebSKU.GetValue();
                    //}
                    ////Get value of Keywords
                    //if (ItemInventoryRet.Keywords != null)
                    //{
                    //string Keywords173 = (string)ItemInventoryRet.Keywords.GetValue();
                    //}
                    ////Get value of WebCategories
                    //if (ItemInventoryRet.WebCategories != null)
                    //{
                    //string WebCategories174 = (string)ItemInventoryRet.WebCategories.GetValue();
                    //}
                    //if (ItemInventoryRet.UnitOfMeasure1 != null)
                    //{
                    ////Get value of ALU
                    //if (ItemInventoryRet.UnitOfMeasure1.ALU != null)
                    //{
                    //string ALU175 = (string)ItemInventoryRet.UnitOfMeasure1.ALU.GetValue();
                    //}
                    ////Get value of MSRP
                    //if (ItemInventoryRet.UnitOfMeasure1.MSRP != null)
                    //{
                    //double MSRP176 = (double)ItemInventoryRet.UnitOfMeasure1.MSRP.GetValue();
                    //}
                    ////Get value of NumberOfBaseUnits
                    //if (ItemInventoryRet.UnitOfMeasure1.NumberOfBaseUnits != null)
                    //{
                    //int NumberOfBaseUnits177 = (int)ItemInventoryRet.UnitOfMeasure1.NumberOfBaseUnits.GetValue();
                    //}
                    ////Get value of Price1
                    //if (ItemInventoryRet.UnitOfMeasure1.Price1 != null)
                    //{
                    //double Price1178 = (double)ItemInventoryRet.UnitOfMeasure1.Price1.GetValue();
                    //}
                    ////Get value of Price2
                    //if (ItemInventoryRet.UnitOfMeasure1.Price2 != null)
                    //{
                    //double Price2179 = (double)ItemInventoryRet.UnitOfMeasure1.Price2.GetValue();
                    //}
                    ////Get value of Price3
                    //if (ItemInventoryRet.UnitOfMeasure1.Price3 != null)
                    //{
                    //double Price3180 = (double)ItemInventoryRet.UnitOfMeasure1.Price3.GetValue();
                    //}
                    ////Get value of Price4
                    //if (ItemInventoryRet.UnitOfMeasure1.Price4 != null)
                    //{
                    //double Price4181 = (double)ItemInventoryRet.UnitOfMeasure1.Price4.GetValue();
                    //}
                    ////Get value of Price5
                    //if (ItemInventoryRet.UnitOfMeasure1.Price5 != null)
                    //{
                    //double Price5182 = (double)ItemInventoryRet.UnitOfMeasure1.Price5.GetValue();
                    //}
                    ////Get value of UnitOfMeasure
                    //if (ItemInventoryRet.UnitOfMeasure1.UnitOfMeasure != null)
                    //{
                    //string UnitOfMeasure183 = (string)ItemInventoryRet.UnitOfMeasure1.UnitOfMeasure.GetValue();
                    //}
                    ////Get value of UPC
                    //if (ItemInventoryRet.UnitOfMeasure1.UPC != null)
                    //{
                    //string UPC184 = (string)ItemInventoryRet.UnitOfMeasure1.UPC.GetValue();
                    //}
                    //}
                    //if (ItemInventoryRet.UnitOfMeasure2 != null)
                    //{
                    ////Get value of ALU
                    //if (ItemInventoryRet.UnitOfMeasure2.ALU != null)
                    //{
                    //string ALU185 = (string)ItemInventoryRet.UnitOfMeasure2.ALU.GetValue();
                    //}
                    ////Get value of MSRP
                    //if (ItemInventoryRet.UnitOfMeasure2.MSRP != null)
                    //{
                    //double MSRP186 = (double)ItemInventoryRet.UnitOfMeasure2.MSRP.GetValue();
                    //}
                    ////Get value of NumberOfBaseUnits
                    //if (ItemInventoryRet.UnitOfMeasure2.NumberOfBaseUnits != null)
                    //{
                    //int NumberOfBaseUnits187 = (int)ItemInventoryRet.UnitOfMeasure2.NumberOfBaseUnits.GetValue();
                    //}
                    ////Get value of Price1
                    //if (ItemInventoryRet.UnitOfMeasure2.Price1 != null)
                    //{
                    //double Price1188 = (double)ItemInventoryRet.UnitOfMeasure2.Price1.GetValue();
                    //}
                    ////Get value of Price2
                    //if (ItemInventoryRet.UnitOfMeasure2.Price2 != null)
                    //{
                    //double Price2189 = (double)ItemInventoryRet.UnitOfMeasure2.Price2.GetValue();
                    //}
                    ////Get value of Price3
                    //if (ItemInventoryRet.UnitOfMeasure2.Price3 != null)
                    //{
                    //double Price3190 = (double)ItemInventoryRet.UnitOfMeasure2.Price3.GetValue();
                    //}
                    ////Get value of Price4
                    //if (ItemInventoryRet.UnitOfMeasure2.Price4 != null)
                    //{
                    //double Price4191 = (double)ItemInventoryRet.UnitOfMeasure2.Price4.GetValue();
                    //}
                    ////Get value of Price5
                    //if (ItemInventoryRet.UnitOfMeasure2.Price5 != null)
                    //{
                    //double Price5192 = (double)ItemInventoryRet.UnitOfMeasure2.Price5.GetValue();
                    //}
                    ////Get value of UnitOfMeasure
                    //if (ItemInventoryRet.UnitOfMeasure2.UnitOfMeasure != null)
                    //{
                    //string UnitOfMeasure193 = (string)ItemInventoryRet.UnitOfMeasure2.UnitOfMeasure.GetValue();
                    //}
                    ////Get value of UPC
                    //if (ItemInventoryRet.UnitOfMeasure2.UPC != null)
                    //{
                    //string UPC194 = (string)ItemInventoryRet.UnitOfMeasure2.UPC.GetValue();
                    //}
                    //}
                    //if (ItemInventoryRet.UnitOfMeasure3 != null)
                    //{
                    ////Get value of ALU
                    //if (ItemInventoryRet.UnitOfMeasure3.ALU != null)
                    //{
                    //string ALU195 = (string)ItemInventoryRet.UnitOfMeasure3.ALU.GetValue();
                    //}
                    ////Get value of MSRP
                    //if (ItemInventoryRet.UnitOfMeasure3.MSRP != null)
                    //{
                    //double MSRP196 = (double)ItemInventoryRet.UnitOfMeasure3.MSRP.GetValue();
                    //}
                    ////Get value of NumberOfBaseUnits
                    //if (ItemInventoryRet.UnitOfMeasure3.NumberOfBaseUnits != null)
                    //{
                    //int NumberOfBaseUnits197 = (int)ItemInventoryRet.UnitOfMeasure3.NumberOfBaseUnits.GetValue();
                    //}
                    ////Get value of Price1
                    //if (ItemInventoryRet.UnitOfMeasure3.Price1 != null)
                    //{
                    //double Price1198 = (double)ItemInventoryRet.UnitOfMeasure3.Price1.GetValue();
                    //}
                    ////Get value of Price2
                    //if (ItemInventoryRet.UnitOfMeasure3.Price2 != null)
                    //{
                    //double Price2199 = (double)ItemInventoryRet.UnitOfMeasure3.Price2.GetValue();
                    //}
                    ////Get value of Price3
                    //if (ItemInventoryRet.UnitOfMeasure3.Price3 != null)
                    //{
                    //double Price3200 = (double)ItemInventoryRet.UnitOfMeasure3.Price3.GetValue();
                    //}
                    ////Get value of Price4
                    //if (ItemInventoryRet.UnitOfMeasure3.Price4 != null)
                    //{
                    //double Price4201 = (double)ItemInventoryRet.UnitOfMeasure3.Price4.GetValue();
                    //}
                    ////Get value of Price5
                    //if (ItemInventoryRet.UnitOfMeasure3.Price5 != null)
                    //{
                    //double Price5202 = (double)ItemInventoryRet.UnitOfMeasure3.Price5.GetValue();
                    //}
                    ////Get value of UnitOfMeasure
                    //if (ItemInventoryRet.UnitOfMeasure3.UnitOfMeasure != null)
                    //{
                    //string UnitOfMeasure203 = (string)ItemInventoryRet.UnitOfMeasure3.UnitOfMeasure.GetValue();
                    //}
                    ////Get value of UPC
                    //if (ItemInventoryRet.UnitOfMeasure3.UPC != null)
                    //{
                    //string UPC204 = (string)ItemInventoryRet.UnitOfMeasure3.UPC.GetValue();
                    //}
                    //}
                    //if (ItemInventoryRet.VendorInfo2 != null)
                    //{
                    ////Get value of ALU
                    //if (ItemInventoryRet.VendorInfo2.ALU != null)
                    //{
                    //string ALU205 = (string)ItemInventoryRet.VendorInfo2.ALU.GetValue();
                    //}
                    ////Get value of OrderCost
                    //if (ItemInventoryRet.VendorInfo2.OrderCost != null)
                    //{
                    //double OrderCost206 = (double)ItemInventoryRet.VendorInfo2.OrderCost.GetValue();
                    //}
                    ////Get value of UPC
                    //if (ItemInventoryRet.VendorInfo2.UPC != null)
                    //{
                    //string UPC207 = (string)ItemInventoryRet.VendorInfo2.UPC.GetValue();
                    //}
                    ////Get value of VendorListID
                    //string VendorListID208 = (string)ItemInventoryRet.VendorInfo2.VendorListID.GetValue();
                    //}
                    //if (ItemInventoryRet.VendorInfo3 != null)
                    //{
                    ////Get value of ALU
                    //if (ItemInventoryRet.VendorInfo3.ALU != null)
                    //{
                    //string ALU209 = (string)ItemInventoryRet.VendorInfo3.ALU.GetValue();
                    //}
                    ////Get value of OrderCost
                    //if (ItemInventoryRet.VendorInfo3.OrderCost != null)
                    //{
                    //double OrderCost210 = (double)ItemInventoryRet.VendorInfo3.OrderCost.GetValue();
                    //}
                    ////Get value of UPC
                    //if (ItemInventoryRet.VendorInfo3.UPC != null)
                    //{
                    //string UPC211 = (string)ItemInventoryRet.VendorInfo3.UPC.GetValue();
                    //}
                    ////Get value of VendorListID
                    //string VendorListID212 = (string)ItemInventoryRet.VendorInfo3.VendorListID.GetValue();
                    //}
                    //if (ItemInventoryRet.VendorInfo4 != null)
                    //{
                    ////Get value of ALU
                    //if (ItemInventoryRet.VendorInfo4.ALU != null)
                    //{
                    //string ALU213 = (string)ItemInventoryRet.VendorInfo4.ALU.GetValue();
                    //}
                    ////Get value of OrderCost
                    //if (ItemInventoryRet.VendorInfo4.OrderCost != null)
                    //{
                    //double OrderCost214 = (double)ItemInventoryRet.VendorInfo4.OrderCost.GetValue();
                    //}
                    ////Get value of UPC
                    //if (ItemInventoryRet.VendorInfo4.UPC != null)
                    //{
                    //string UPC215 = (string)ItemInventoryRet.VendorInfo4.UPC.GetValue();
                    //}
                    ////Get value of VendorListID
                    //string VendorListID216 = (string)ItemInventoryRet.VendorInfo4.VendorListID.GetValue();
                    //}
                    //if (ItemInventoryRet.VendorInfo5 != null)
                    //{
                    ////Get value of ALU
                    //if (ItemInventoryRet.VendorInfo5.ALU != null)
                    //{
                    //string ALU217 = (string)ItemInventoryRet.VendorInfo5.ALU.GetValue();
                    //}
                    ////Get value of OrderCost
                    //if (ItemInventoryRet.VendorInfo5.OrderCost != null)
                    //{
                    //double OrderCost218 = (double)ItemInventoryRet.VendorInfo5.OrderCost.GetValue();
                    //}
                    ////Get value of UPC
                    //if (ItemInventoryRet.VendorInfo5.UPC != null)
                    //{
                    //string UPC219 = (string)ItemInventoryRet.VendorInfo5.UPC.GetValue();
                    //}
                    ////Get value of VendorListID
                    //string VendorListID220 = (string)ItemInventoryRet.VendorInfo5.VendorListID.GetValue();
                    //}

                    #endregion

                    itmList.Add(itm);
                }
                return itmList;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
