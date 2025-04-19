
//using System.CodeDom.Compiler;
//using QuickBooks;

//namespace QuickBooks
//{
//    using System;
//    using System.Diagnostics;
//    using System.Xml.Serialization;
//    using System.Collections;
//    using System.Xml.Schema;
//    using System.ComponentModel;
//    using System.IO;
//    using System.Text;
//    using System.Xml;
//    using System.Collections.ObjectModel;
//    using System.Runtime.Serialization;


//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [XmlRoot(Namespace = "", IsNullable = false)]
//    [DataContract(Name = "ItemInventoryRet")]
//    public partial class ItemInventoryRet : INotifyPropertyChanged
//    {

//        private string listIDField;

//        private DateTime timeCreatedField;

//        private DateTime timeModifiedField;

//        private string aLUField;

//        private string attributeField;

//        private string cOGSAccountField;

//        private decimal costField;

//        private string departmentCodeField;

//        private string departmentListIDField;

//        private string desc1Field;

//        private string desc2Field;

//        private string incomeAccountField;

//        private bool isBelowReorderField;

//        private bool isEligibleForCommissionField;

//        private bool isPrintingTagsField;

//        private bool isUnorderableField;

//        private bool hasPicturesField;

//        private bool isEligibleForRewardsField;

//        private bool isWebItemField;

//        private decimal itemNumberField;

//        private string itemTypeField;

//        private string lastReceivedField;

//        private decimal marginPercentField;

//        private decimal markupPercentField;

//        private decimal mSRPField;

//        private decimal onHandStore01Field;

//        private decimal onHandStore02Field;

//        private decimal onHandStore03Field;

//        private decimal onHandStore04Field;

//        private decimal onHandStore05Field;

//        private decimal onHandStore06Field;

//        private decimal onHandStore07Field;

//        private decimal onHandStore08Field;

//        private decimal onHandStore09Field;

//        private decimal onHandStore10Field;

//        private decimal onHandStore11Field;

//        private decimal onHandStore12Field;

//        private decimal onHandStore13Field;

//        private decimal onHandStore14Field;

//        private decimal onHandStore15Field;

//        private decimal onHandStore16Field;

//        private decimal onHandStore17Field;

//        private decimal onHandStore18Field;

//        private decimal onHandStore19Field;

//        private decimal onHandStore20Field;

//        private decimal reorderPointStore01Field;

//        private decimal reorderPointStore02Field;

//        private decimal reorderPointStore03Field;

//        private decimal reorderPointStore04Field;

//        private decimal reorderPointStore05Field;

//        private decimal reorderPointStore06Field;

//        private decimal reorderPointStore07Field;

//        private decimal reorderPointStore08Field;

//        private decimal reorderPointStore09Field;

//        private decimal reorderPointStore10Field;

//        private decimal reorderPointStore11Field;

//        private decimal reorderPointStore12Field;

//        private decimal reorderPointStore13Field;

//        private decimal reorderPointStore14Field;

//        private decimal reorderPointStore15Field;

//        private decimal reorderPointStore16Field;

//        private decimal reorderPointStore17Field;

//        private decimal reorderPointStore18Field;

//        private decimal reorderPointStore19Field;

//        private decimal reorderPointStore20Field;

//        private string orderByUnitField;

//        private decimal orderCostField;

//        private decimal price1Field;

//        private decimal price2Field;

//        private decimal price3Field;

//        private decimal price4Field;

//        private decimal price5Field;

//        private decimal quantityOnCustomerOrderField;

//        private decimal quantityOnHandField;

//        private decimal quantityOnOrderField;

//        private decimal quantityOnPendingOrderField;

//        private ItemInventoryRetAvailableQty availableQtyField;

//        private decimal reorderPointField;

//        private string sellByUnitField;

//        private string serialFlagField;

//        private string sizeField;

//        private string storeExchangeStatusField;

//        private string taxCodeField;

//        private string unitOfMeasureField;

//        private string uPCField;

//        private string vendorCodeField;

//        private string vendorListIDField;

//        private string webDescField;

//        private decimal webPriceField;

//        private string manufacturerField;

//        private decimal weightField;

//        private string webSKUField;

//        private string keywordsField;

//        private string webCategoriesField;

//        private ItemInventoryRetUnitOfMeasure1 unitOfMeasure1Field;

//        private ItemInventoryRetUnitOfMeasure2 unitOfMeasure2Field;

//        private ItemInventoryRetUnitOfMeasure3 unitOfMeasure3Field;

//        private ItemInventoryRetVendorInfo2 vendorInfo2Field;

//        private ItemInventoryRetVendorInfo3 vendorInfo3Field;

//        private ItemInventoryRetVendorInfo4 vendorInfo4Field;

//        private ItemInventoryRetVendorInfo5 vendorInfo5Field;

//        private TrackableCollection<ItemInventoryRetDataExtRet> dataExtRetField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        public ItemInventoryRet()
//        {
//            dataExtRetField = new TrackableCollection<ItemInventoryRetDataExtRet>(null);
//            vendorInfo5Field = new ItemInventoryRetVendorInfo5();
//            vendorInfo4Field = new ItemInventoryRetVendorInfo4();
//            vendorInfo3Field = new ItemInventoryRetVendorInfo3();
//            vendorInfo2Field = new ItemInventoryRetVendorInfo2();
//            unitOfMeasure3Field = new ItemInventoryRetUnitOfMeasure3();
//            unitOfMeasure2Field = new ItemInventoryRetUnitOfMeasure2();
//            unitOfMeasure1Field = new ItemInventoryRetUnitOfMeasure1();
//            availableQtyField = new ItemInventoryRetAvailableQty();
//        }

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string ListID
//        {
//            get
//            {
//                return listIDField;
//            }
//            set
//            {
//                if (((listIDField == null)
//                            || (listIDField.Equals(value) != true)))
//                {
//                    listIDField = value;
//                    OnPropertyChanged("ListID", value);
//                }
//            }
//        }

//        [XmlElement(DataType = "date", Order = 1)]
//        [DataMember()]
//        public DateTime TimeCreated
//        {
//            get
//            {
//                return timeCreatedField;
//            }
//            set
//            {
//                if ((timeCreatedField.Equals(value) != true))
//                {
//                    timeCreatedField = value;
//                    OnPropertyChanged("TimeCreated", value);
//                }
//            }
//        }

//        [XmlElement(DataType = "date", Order = 2)]
//        [DataMember()]
//        public DateTime TimeModified
//        {
//            get
//            {
//                return timeModifiedField;
//            }
//            set
//            {
//                if ((timeModifiedField.Equals(value) != true))
//                {
//                    timeModifiedField = value;
//                    OnPropertyChanged("TimeModified", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public string ALU
//        {
//            get
//            {
//                return aLUField;
//            }
//            set
//            {
//                if (((aLUField == null)
//                            || (aLUField.Equals(value) != true)))
//                {
//                    aLUField = value;
//                    OnPropertyChanged("ALU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 4)]
//        [DataMember()]
//        public string Attribute
//        {
//            get
//            {
//                return attributeField;
//            }
//            set
//            {
//                if (((attributeField == null)
//                            || (attributeField.Equals(value) != true)))
//                {
//                    attributeField = value;
//                    OnPropertyChanged("Attribute", value);
//                }
//            }
//        }

//        [XmlElement(Order = 5)]
//        [DataMember()]
//        public string COGSAccount
//        {
//            get
//            {
//                return cOGSAccountField;
//            }
//            set
//            {
//                if (((cOGSAccountField == null)
//                            || (cOGSAccountField.Equals(value) != true)))
//                {
//                    cOGSAccountField = value;
//                    OnPropertyChanged("COGSAccount", value);
//                }
//            }
//        }

//        [XmlElement(Order = 6)]
//        [DataMember()]
//        public decimal Cost
//        {
//            get
//            {
//                return costField;
//            }
//            set
//            {
//                if ((costField.Equals(value) != true))
//                {
//                    costField = value;
//                    OnPropertyChanged("Cost", value);
//                }
//            }
//        }

//        [XmlElement(Order = 7)]
//        [DataMember()]
//        public string DepartmentCode
//        {
//            get
//            {
//                return departmentCodeField;
//            }
//            set
//            {
//                if (((departmentCodeField == null)
//                            || (departmentCodeField.Equals(value) != true)))
//                {
//                    departmentCodeField = value;
//                    OnPropertyChanged("DepartmentCode", value);
//                }
//            }
//        }

//        [XmlElement(Order = 8)]
//        [DataMember()]
//        public string DepartmentListID
//        {
//            get
//            {
//                return departmentListIDField;
//            }
//            set
//            {
//                if (((departmentListIDField == null)
//                            || (departmentListIDField.Equals(value) != true)))
//                {
//                    departmentListIDField = value;
//                    OnPropertyChanged("DepartmentListID", value);
//                }
//            }
//        }

//        [XmlElement(Order = 9)]
//        [DataMember()]
//        public string Desc1
//        {
//            get
//            {
//                return desc1Field;
//            }
//            set
//            {
//                if (((desc1Field == null)
//                            || (desc1Field.Equals(value) != true)))
//                {
//                    desc1Field = value;
//                    OnPropertyChanged("Desc1", value);
//                }
//            }
//        }

//        [XmlElement(Order = 10)]
//        [DataMember()]
//        public string Desc2
//        {
//            get
//            {
//                return desc2Field;
//            }
//            set
//            {
//                if (((desc2Field == null)
//                            || (desc2Field.Equals(value) != true)))
//                {
//                    desc2Field = value;
//                    OnPropertyChanged("Desc2", value);
//                }
//            }
//        }

//        [XmlElement(Order = 11)]
//        [DataMember()]
//        public string IncomeAccount
//        {
//            get
//            {
//                return incomeAccountField;
//            }
//            set
//            {
//                if (((incomeAccountField == null)
//                            || (incomeAccountField.Equals(value) != true)))
//                {
//                    incomeAccountField = value;
//                    OnPropertyChanged("IncomeAccount", value);
//                }
//            }
//        }

//        [XmlElement(Order = 12)]
//        [DataMember()]
//        public bool IsBelowReorder
//        {
//            get
//            {
//                return isBelowReorderField;
//            }
//            set
//            {
//                if ((isBelowReorderField.Equals(value) != true))
//                {
//                    isBelowReorderField = value;
//                    OnPropertyChanged("IsBelowReorder", value);
//                }
//            }
//        }

//        [XmlElement(Order = 13)]
//        [DataMember()]
//        public bool IsEligibleForCommission
//        {
//            get
//            {
//                return isEligibleForCommissionField;
//            }
//            set
//            {
//                if ((isEligibleForCommissionField.Equals(value) != true))
//                {
//                    isEligibleForCommissionField = value;
//                    OnPropertyChanged("IsEligibleForCommission", value);
//                }
//            }
//        }

//        [XmlElement(Order = 14)]
//        [DataMember()]
//        public bool IsPrintingTags
//        {
//            get
//            {
//                return isPrintingTagsField;
//            }
//            set
//            {
//                if ((isPrintingTagsField.Equals(value) != true))
//                {
//                    isPrintingTagsField = value;
//                    OnPropertyChanged("IsPrintingTags", value);
//                }
//            }
//        }

//        [XmlElement(Order = 15)]
//        [DataMember()]
//        public bool IsUnorderable
//        {
//            get
//            {
//                return isUnorderableField;
//            }
//            set
//            {
//                if ((isUnorderableField.Equals(value) != true))
//                {
//                    isUnorderableField = value;
//                    OnPropertyChanged("IsUnorderable", value);
//                }
//            }
//        }

//        [XmlElement(Order = 16)]
//        [DataMember()]
//        public bool HasPictures
//        {
//            get
//            {
//                return hasPicturesField;
//            }
//            set
//            {
//                if ((hasPicturesField.Equals(value) != true))
//                {
//                    hasPicturesField = value;
//                    OnPropertyChanged("HasPictures", value);
//                }
//            }
//        }

//        [XmlElement(Order = 17)]
//        [DataMember()]
//        public bool IsEligibleForRewards
//        {
//            get
//            {
//                return isEligibleForRewardsField;
//            }
//            set
//            {
//                if ((isEligibleForRewardsField.Equals(value) != true))
//                {
//                    isEligibleForRewardsField = value;
//                    OnPropertyChanged("IsEligibleForRewards", value);
//                }
//            }
//        }

//        [XmlElement(Order = 18)]
//        [DataMember()]
//        public bool IsWebItem
//        {
//            get
//            {
//                return isWebItemField;
//            }
//            set
//            {
//                if ((isWebItemField.Equals(value) != true))
//                {
//                    isWebItemField = value;
//                    OnPropertyChanged("IsWebItem", value);
//                }
//            }
//        }

//        [XmlElement(Order = 19)]
//        [DataMember()]
//        public decimal ItemNumber
//        {
//            get
//            {
//                return itemNumberField;
//            }
//            set
//            {
//                if ((itemNumberField.Equals(value) != true))
//                {
//                    itemNumberField = value;
//                    OnPropertyChanged("ItemNumber", value);
//                }
//            }
//        }

//        [XmlElement(Order = 20)]
//        [DataMember()]
//        public string ItemType
//        {
//            get
//            {
//                return itemTypeField;
//            }
//            set
//            {
//                if (((itemTypeField == null)
//                            || (itemTypeField.Equals(value) != true)))
//                {
//                    itemTypeField = value;
//                    OnPropertyChanged("ItemType", value);
//                }
//            }
//        }

//        [XmlElement(Order = 21)]
//        [DataMember()]
//        public string LastReceived
//        {
//            get
//            {
//                return lastReceivedField;
//            }
//            set
//            {
//                if (((lastReceivedField == null)
//                            || (lastReceivedField.Equals(value) != true)))
//                {
//                    lastReceivedField = value;
//                    OnPropertyChanged("LastReceived", value);
//                }
//            }
//        }

//        [XmlElement(Order = 22)]
//        [DataMember()]
//        public decimal MarginPercent
//        {
//            get
//            {
//                return marginPercentField;
//            }
//            set
//            {
//                if ((marginPercentField.Equals(value) != true))
//                {
//                    marginPercentField = value;
//                    OnPropertyChanged("MarginPercent", value);
//                }
//            }
//        }

//        [XmlElement(Order = 23)]
//        [DataMember()]
//        public decimal MarkupPercent
//        {
//            get
//            {
//                return markupPercentField;
//            }
//            set
//            {
//                if ((markupPercentField.Equals(value) != true))
//                {
//                    markupPercentField = value;
//                    OnPropertyChanged("MarkupPercent", value);
//                }
//            }
//        }

//        [XmlElement(Order = 24)]
//        [DataMember()]
//        public decimal MSRP
//        {
//            get
//            {
//                return mSRPField;
//            }
//            set
//            {
//                if ((mSRPField.Equals(value) != true))
//                {
//                    mSRPField = value;
//                    OnPropertyChanged("MSRP", value);
//                }
//            }
//        }

//        [XmlElement(Order = 25)]
//        [DataMember()]
//        public decimal OnHandStore01
//        {
//            get
//            {
//                return onHandStore01Field;
//            }
//            set
//            {
//                if ((onHandStore01Field.Equals(value) != true))
//                {
//                    onHandStore01Field = value;
//                    OnPropertyChanged("OnHandStore01", value);
//                }
//            }
//        }

//        [XmlElement(Order = 26)]
//        [DataMember()]
//        public decimal OnHandStore02
//        {
//            get
//            {
//                return onHandStore02Field;
//            }
//            set
//            {
//                if ((onHandStore02Field.Equals(value) != true))
//                {
//                    onHandStore02Field = value;
//                    OnPropertyChanged("OnHandStore02", value);
//                }
//            }
//        }

//        [XmlElement(Order = 27)]
//        [DataMember()]
//        public decimal OnHandStore03
//        {
//            get
//            {
//                return onHandStore03Field;
//            }
//            set
//            {
//                if ((onHandStore03Field.Equals(value) != true))
//                {
//                    onHandStore03Field = value;
//                    OnPropertyChanged("OnHandStore03", value);
//                }
//            }
//        }

//        [XmlElement(Order = 28)]
//        [DataMember()]
//        public decimal OnHandStore04
//        {
//            get
//            {
//                return onHandStore04Field;
//            }
//            set
//            {
//                if ((onHandStore04Field.Equals(value) != true))
//                {
//                    onHandStore04Field = value;
//                    OnPropertyChanged("OnHandStore04", value);
//                }
//            }
//        }

//        [XmlElement(Order = 29)]
//        [DataMember()]
//        public decimal OnHandStore05
//        {
//            get
//            {
//                return onHandStore05Field;
//            }
//            set
//            {
//                if ((onHandStore05Field.Equals(value) != true))
//                {
//                    onHandStore05Field = value;
//                    OnPropertyChanged("OnHandStore05", value);
//                }
//            }
//        }

//        [XmlElement(Order = 30)]
//        [DataMember()]
//        public decimal OnHandStore06
//        {
//            get
//            {
//                return onHandStore06Field;
//            }
//            set
//            {
//                if ((onHandStore06Field.Equals(value) != true))
//                {
//                    onHandStore06Field = value;
//                    OnPropertyChanged("OnHandStore06", value);
//                }
//            }
//        }

//        [XmlElement(Order = 31)]
//        [DataMember()]
//        public decimal OnHandStore07
//        {
//            get
//            {
//                return onHandStore07Field;
//            }
//            set
//            {
//                if ((onHandStore07Field.Equals(value) != true))
//                {
//                    onHandStore07Field = value;
//                    OnPropertyChanged("OnHandStore07", value);
//                }
//            }
//        }

//        [XmlElement(Order = 32)]
//        [DataMember()]
//        public decimal OnHandStore08
//        {
//            get
//            {
//                return onHandStore08Field;
//            }
//            set
//            {
//                if ((onHandStore08Field.Equals(value) != true))
//                {
//                    onHandStore08Field = value;
//                    OnPropertyChanged("OnHandStore08", value);
//                }
//            }
//        }

//        [XmlElement(Order = 33)]
//        [DataMember()]
//        public decimal OnHandStore09
//        {
//            get
//            {
//                return onHandStore09Field;
//            }
//            set
//            {
//                if ((onHandStore09Field.Equals(value) != true))
//                {
//                    onHandStore09Field = value;
//                    OnPropertyChanged("OnHandStore09", value);
//                }
//            }
//        }

//        [XmlElement(Order = 34)]
//        [DataMember()]
//        public decimal OnHandStore10
//        {
//            get
//            {
//                return onHandStore10Field;
//            }
//            set
//            {
//                if ((onHandStore10Field.Equals(value) != true))
//                {
//                    onHandStore10Field = value;
//                    OnPropertyChanged("OnHandStore10", value);
//                }
//            }
//        }

//        [XmlElement(Order = 35)]
//        [DataMember()]
//        public decimal OnHandStore11
//        {
//            get
//            {
//                return onHandStore11Field;
//            }
//            set
//            {
//                if ((onHandStore11Field.Equals(value) != true))
//                {
//                    onHandStore11Field = value;
//                    OnPropertyChanged("OnHandStore11", value);
//                }
//            }
//        }

//        [XmlElement(Order = 36)]
//        [DataMember()]
//        public decimal OnHandStore12
//        {
//            get
//            {
//                return onHandStore12Field;
//            }
//            set
//            {
//                if ((onHandStore12Field.Equals(value) != true))
//                {
//                    onHandStore12Field = value;
//                    OnPropertyChanged("OnHandStore12", value);
//                }
//            }
//        }

//        [XmlElement(Order = 37)]
//        [DataMember()]
//        public decimal OnHandStore13
//        {
//            get
//            {
//                return onHandStore13Field;
//            }
//            set
//            {
//                if ((onHandStore13Field.Equals(value) != true))
//                {
//                    onHandStore13Field = value;
//                    OnPropertyChanged("OnHandStore13", value);
//                }
//            }
//        }

//        [XmlElement(Order = 38)]
//        [DataMember()]
//        public decimal OnHandStore14
//        {
//            get
//            {
//                return onHandStore14Field;
//            }
//            set
//            {
//                if ((onHandStore14Field.Equals(value) != true))
//                {
//                    onHandStore14Field = value;
//                    OnPropertyChanged("OnHandStore14", value);
//                }
//            }
//        }

//        [XmlElement(Order = 39)]
//        [DataMember()]
//        public decimal OnHandStore15
//        {
//            get
//            {
//                return onHandStore15Field;
//            }
//            set
//            {
//                if ((onHandStore15Field.Equals(value) != true))
//                {
//                    onHandStore15Field = value;
//                    OnPropertyChanged("OnHandStore15", value);
//                }
//            }
//        }

//        [XmlElement(Order = 40)]
//        [DataMember()]
//        public decimal OnHandStore16
//        {
//            get
//            {
//                return onHandStore16Field;
//            }
//            set
//            {
//                if ((onHandStore16Field.Equals(value) != true))
//                {
//                    onHandStore16Field = value;
//                    OnPropertyChanged("OnHandStore16", value);
//                }
//            }
//        }

//        [XmlElement(Order = 41)]
//        [DataMember()]
//        public decimal OnHandStore17
//        {
//            get
//            {
//                return onHandStore17Field;
//            }
//            set
//            {
//                if ((onHandStore17Field.Equals(value) != true))
//                {
//                    onHandStore17Field = value;
//                    OnPropertyChanged("OnHandStore17", value);
//                }
//            }
//        }

//        [XmlElement(Order = 42)]
//        [DataMember()]
//        public decimal OnHandStore18
//        {
//            get
//            {
//                return onHandStore18Field;
//            }
//            set
//            {
//                if ((onHandStore18Field.Equals(value) != true))
//                {
//                    onHandStore18Field = value;
//                    OnPropertyChanged("OnHandStore18", value);
//                }
//            }
//        }

//        [XmlElement(Order = 43)]
//        [DataMember()]
//        public decimal OnHandStore19
//        {
//            get
//            {
//                return onHandStore19Field;
//            }
//            set
//            {
//                if ((onHandStore19Field.Equals(value) != true))
//                {
//                    onHandStore19Field = value;
//                    OnPropertyChanged("OnHandStore19", value);
//                }
//            }
//        }

//        [XmlElement(Order = 44)]
//        [DataMember()]
//        public decimal OnHandStore20
//        {
//            get
//            {
//                return onHandStore20Field;
//            }
//            set
//            {
//                if ((onHandStore20Field.Equals(value) != true))
//                {
//                    onHandStore20Field = value;
//                    OnPropertyChanged("OnHandStore20", value);
//                }
//            }
//        }

//        [XmlElement(Order = 45)]
//        [DataMember()]
//        public decimal ReorderPointStore01
//        {
//            get
//            {
//                return reorderPointStore01Field;
//            }
//            set
//            {
//                if ((reorderPointStore01Field.Equals(value) != true))
//                {
//                    reorderPointStore01Field = value;
//                    OnPropertyChanged("ReorderPointStore01", value);
//                }
//            }
//        }

//        [XmlElement(Order = 46)]
//        [DataMember()]
//        public decimal ReorderPointStore02
//        {
//            get
//            {
//                return reorderPointStore02Field;
//            }
//            set
//            {
//                if ((reorderPointStore02Field.Equals(value) != true))
//                {
//                    reorderPointStore02Field = value;
//                    OnPropertyChanged("ReorderPointStore02", value);
//                }
//            }
//        }

//        [XmlElement(Order = 47)]
//        [DataMember()]
//        public decimal ReorderPointStore03
//        {
//            get
//            {
//                return reorderPointStore03Field;
//            }
//            set
//            {
//                if ((reorderPointStore03Field.Equals(value) != true))
//                {
//                    reorderPointStore03Field = value;
//                    OnPropertyChanged("ReorderPointStore03", value);
//                }
//            }
//        }

//        [XmlElement(Order = 48)]
//        [DataMember()]
//        public decimal ReorderPointStore04
//        {
//            get
//            {
//                return reorderPointStore04Field;
//            }
//            set
//            {
//                if ((reorderPointStore04Field.Equals(value) != true))
//                {
//                    reorderPointStore04Field = value;
//                    OnPropertyChanged("ReorderPointStore04", value);
//                }
//            }
//        }

//        [XmlElement(Order = 49)]
//        [DataMember()]
//        public decimal ReorderPointStore05
//        {
//            get
//            {
//                return reorderPointStore05Field;
//            }
//            set
//            {
//                if ((reorderPointStore05Field.Equals(value) != true))
//                {
//                    reorderPointStore05Field = value;
//                    OnPropertyChanged("ReorderPointStore05", value);
//                }
//            }
//        }

//        [XmlElement(Order = 50)]
//        [DataMember()]
//        public decimal ReorderPointStore06
//        {
//            get
//            {
//                return reorderPointStore06Field;
//            }
//            set
//            {
//                if ((reorderPointStore06Field.Equals(value) != true))
//                {
//                    reorderPointStore06Field = value;
//                    OnPropertyChanged("ReorderPointStore06", value);
//                }
//            }
//        }

//        [XmlElement(Order = 51)]
//        [DataMember()]
//        public decimal ReorderPointStore07
//        {
//            get
//            {
//                return reorderPointStore07Field;
//            }
//            set
//            {
//                if ((reorderPointStore07Field.Equals(value) != true))
//                {
//                    reorderPointStore07Field = value;
//                    OnPropertyChanged("ReorderPointStore07", value);
//                }
//            }
//        }

//        [XmlElement(Order = 52)]
//        [DataMember()]
//        public decimal ReorderPointStore08
//        {
//            get
//            {
//                return reorderPointStore08Field;
//            }
//            set
//            {
//                if ((reorderPointStore08Field.Equals(value) != true))
//                {
//                    reorderPointStore08Field = value;
//                    OnPropertyChanged("ReorderPointStore08", value);
//                }
//            }
//        }

//        [XmlElement(Order = 53)]
//        [DataMember()]
//        public decimal ReorderPointStore09
//        {
//            get
//            {
//                return reorderPointStore09Field;
//            }
//            set
//            {
//                if ((reorderPointStore09Field.Equals(value) != true))
//                {
//                    reorderPointStore09Field = value;
//                    OnPropertyChanged("ReorderPointStore09", value);
//                }
//            }
//        }

//        [XmlElement(Order = 54)]
//        [DataMember()]
//        public decimal ReorderPointStore10
//        {
//            get
//            {
//                return reorderPointStore10Field;
//            }
//            set
//            {
//                if ((reorderPointStore10Field.Equals(value) != true))
//                {
//                    reorderPointStore10Field = value;
//                    OnPropertyChanged("ReorderPointStore10", value);
//                }
//            }
//        }

//        [XmlElement(Order = 55)]
//        [DataMember()]
//        public decimal ReorderPointStore11
//        {
//            get
//            {
//                return reorderPointStore11Field;
//            }
//            set
//            {
//                if ((reorderPointStore11Field.Equals(value) != true))
//                {
//                    reorderPointStore11Field = value;
//                    OnPropertyChanged("ReorderPointStore11", value);
//                }
//            }
//        }

//        [XmlElement(Order = 56)]
//        [DataMember()]
//        public decimal ReorderPointStore12
//        {
//            get
//            {
//                return reorderPointStore12Field;
//            }
//            set
//            {
//                if ((reorderPointStore12Field.Equals(value) != true))
//                {
//                    reorderPointStore12Field = value;
//                    OnPropertyChanged("ReorderPointStore12", value);
//                }
//            }
//        }

//        [XmlElement(Order = 57)]
//        [DataMember()]
//        public decimal ReorderPointStore13
//        {
//            get
//            {
//                return reorderPointStore13Field;
//            }
//            set
//            {
//                if ((reorderPointStore13Field.Equals(value) != true))
//                {
//                    reorderPointStore13Field = value;
//                    OnPropertyChanged("ReorderPointStore13", value);
//                }
//            }
//        }

//        [XmlElement(Order = 58)]
//        [DataMember()]
//        public decimal ReorderPointStore14
//        {
//            get
//            {
//                return reorderPointStore14Field;
//            }
//            set
//            {
//                if ((reorderPointStore14Field.Equals(value) != true))
//                {
//                    reorderPointStore14Field = value;
//                    OnPropertyChanged("ReorderPointStore14", value);
//                }
//            }
//        }

//        [XmlElement(Order = 59)]
//        [DataMember()]
//        public decimal ReorderPointStore15
//        {
//            get
//            {
//                return reorderPointStore15Field;
//            }
//            set
//            {
//                if ((reorderPointStore15Field.Equals(value) != true))
//                {
//                    reorderPointStore15Field = value;
//                    OnPropertyChanged("ReorderPointStore15", value);
//                }
//            }
//        }

//        [XmlElement(Order = 60)]
//        [DataMember()]
//        public decimal ReorderPointStore16
//        {
//            get
//            {
//                return reorderPointStore16Field;
//            }
//            set
//            {
//                if ((reorderPointStore16Field.Equals(value) != true))
//                {
//                    reorderPointStore16Field = value;
//                    OnPropertyChanged("ReorderPointStore16", value);
//                }
//            }
//        }

//        [XmlElement(Order = 61)]
//        [DataMember()]
//        public decimal ReorderPointStore17
//        {
//            get
//            {
//                return reorderPointStore17Field;
//            }
//            set
//            {
//                if ((reorderPointStore17Field.Equals(value) != true))
//                {
//                    reorderPointStore17Field = value;
//                    OnPropertyChanged("ReorderPointStore17", value);
//                }
//            }
//        }

//        [XmlElement(Order = 62)]
//        [DataMember()]
//        public decimal ReorderPointStore18
//        {
//            get
//            {
//                return reorderPointStore18Field;
//            }
//            set
//            {
//                if ((reorderPointStore18Field.Equals(value) != true))
//                {
//                    reorderPointStore18Field = value;
//                    OnPropertyChanged("ReorderPointStore18", value);
//                }
//            }
//        }

//        [XmlElement(Order = 63)]
//        [DataMember()]
//        public decimal ReorderPointStore19
//        {
//            get
//            {
//                return reorderPointStore19Field;
//            }
//            set
//            {
//                if ((reorderPointStore19Field.Equals(value) != true))
//                {
//                    reorderPointStore19Field = value;
//                    OnPropertyChanged("ReorderPointStore19", value);
//                }
//            }
//        }

//        [XmlElement(Order = 64)]
//        [DataMember()]
//        public decimal ReorderPointStore20
//        {
//            get
//            {
//                return reorderPointStore20Field;
//            }
//            set
//            {
//                if ((reorderPointStore20Field.Equals(value) != true))
//                {
//                    reorderPointStore20Field = value;
//                    OnPropertyChanged("ReorderPointStore20", value);
//                }
//            }
//        }

//        [XmlElement(Order = 65)]
//        [DataMember()]
//        public string OrderByUnit
//        {
//            get
//            {
//                return orderByUnitField;
//            }
//            set
//            {
//                if (((orderByUnitField == null)
//                            || (orderByUnitField.Equals(value) != true)))
//                {
//                    orderByUnitField = value;
//                    OnPropertyChanged("OrderByUnit", value);
//                }
//            }
//        }

//        [XmlElement(Order = 66)]
//        [DataMember()]
//        public decimal OrderCost
//        {
//            get
//            {
//                return orderCostField;
//            }
//            set
//            {
//                if ((orderCostField.Equals(value) != true))
//                {
//                    orderCostField = value;
//                    OnPropertyChanged("OrderCost", value);
//                }
//            }
//        }

//        [XmlElement(Order = 67)]
//        [DataMember()]
//        public decimal Price1
//        {
//            get
//            {
//                return price1Field;
//            }
//            set
//            {
//                if ((price1Field.Equals(value) != true))
//                {
//                    price1Field = value;
//                    OnPropertyChanged("Price1", value);
//                }
//            }
//        }

//        [XmlElement(Order = 68)]
//        [DataMember()]
//        public decimal Price2
//        {
//            get
//            {
//                return price2Field;
//            }
//            set
//            {
//                if ((price2Field.Equals(value) != true))
//                {
//                    price2Field = value;
//                    OnPropertyChanged("Price2", value);
//                }
//            }
//        }

//        [XmlElement(Order = 69)]
//        [DataMember()]
//        public decimal Price3
//        {
//            get
//            {
//                return price3Field;
//            }
//            set
//            {
//                if ((price3Field.Equals(value) != true))
//                {
//                    price3Field = value;
//                    OnPropertyChanged("Price3", value);
//                }
//            }
//        }

//        [XmlElement(Order = 70)]
//        [DataMember()]
//        public decimal Price4
//        {
//            get
//            {
//                return price4Field;
//            }
//            set
//            {
//                if ((price4Field.Equals(value) != true))
//                {
//                    price4Field = value;
//                    OnPropertyChanged("Price4", value);
//                }
//            }
//        }

//        [XmlElement(Order = 71)]
//        [DataMember()]
//        public decimal Price5
//        {
//            get
//            {
//                return price5Field;
//            }
//            set
//            {
//                if ((price5Field.Equals(value) != true))
//                {
//                    price5Field = value;
//                    OnPropertyChanged("Price5", value);
//                }
//            }
//        }

//        [XmlElement(Order = 72)]
//        [DataMember()]
//        public decimal QuantityOnCustomerOrder
//        {
//            get
//            {
//                return quantityOnCustomerOrderField;
//            }
//            set
//            {
//                if ((quantityOnCustomerOrderField.Equals(value) != true))
//                {
//                    quantityOnCustomerOrderField = value;
//                    OnPropertyChanged("QuantityOnCustomerOrder", value);
//                }
//            }
//        }

//        [XmlElement(Order = 73)]
//        [DataMember()]
//        public decimal QuantityOnHand
//        {
//            get
//            {
//                return quantityOnHandField;
//            }
//            set
//            {
//                if ((quantityOnHandField.Equals(value) != true))
//                {
//                    quantityOnHandField = value;
//                    OnPropertyChanged("QuantityOnHand", value);
//                }
//            }
//        }

//        [XmlElement(Order = 74)]
//        [DataMember()]
//        public decimal QuantityOnOrder
//        {
//            get
//            {
//                return quantityOnOrderField;
//            }
//            set
//            {
//                if ((quantityOnOrderField.Equals(value) != true))
//                {
//                    quantityOnOrderField = value;
//                    OnPropertyChanged("QuantityOnOrder", value);
//                }
//            }
//        }

//        [XmlElement(Order = 75)]
//        [DataMember()]
//        public decimal QuantityOnPendingOrder
//        {
//            get
//            {
//                return quantityOnPendingOrderField;
//            }
//            set
//            {
//                if ((quantityOnPendingOrderField.Equals(value) != true))
//                {
//                    quantityOnPendingOrderField = value;
//                    OnPropertyChanged("QuantityOnPendingOrder", value);
//                }
//            }
//        }

//        [XmlElement(Order = 76)]
//        [DataMember()]
//        public ItemInventoryRetAvailableQty AvailableQty
//        {
//            get
//            {
//                return availableQtyField;
//            }
//            set
//            {
//                if (((availableQtyField == null)
//                            || (availableQtyField.Equals(value) != true)))
//                {
//                    availableQtyField = value;
//                    OnPropertyChanged("AvailableQty", value);
//                }
//            }
//        }

//        [XmlElement(Order = 77)]
//        [DataMember()]
//        public decimal ReorderPoint
//        {
//            get
//            {
//                return reorderPointField;
//            }
//            set
//            {
//                if ((reorderPointField.Equals(value) != true))
//                {
//                    reorderPointField = value;
//                    OnPropertyChanged("ReorderPoint", value);
//                }
//            }
//        }

//        [XmlElement(Order = 78)]
//        [DataMember()]
//        public string SellByUnit
//        {
//            get
//            {
//                return sellByUnitField;
//            }
//            set
//            {
//                if (((sellByUnitField == null)
//                            || (sellByUnitField.Equals(value) != true)))
//                {
//                    sellByUnitField = value;
//                    OnPropertyChanged("SellByUnit", value);
//                }
//            }
//        }

//        [XmlElement(Order = 79)]
//        [DataMember()]
//        public string SerialFlag
//        {
//            get
//            {
//                return serialFlagField;
//            }
//            set
//            {
//                if (((serialFlagField == null)
//                            || (serialFlagField.Equals(value) != true)))
//                {
//                    serialFlagField = value;
//                    OnPropertyChanged("SerialFlag", value);
//                }
//            }
//        }

//        [XmlElement(Order = 80)]
//        [DataMember()]
//        public string Size
//        {
//            get
//            {
//                return sizeField;
//            }
//            set
//            {
//                if (((sizeField == null)
//                            || (sizeField.Equals(value) != true)))
//                {
//                    sizeField = value;
//                    OnPropertyChanged("Size", value);
//                }
//            }
//        }

//        [XmlElement(Order = 81)]
//        [DataMember()]
//        public string StoreExchangeStatus
//        {
//            get
//            {
//                return storeExchangeStatusField;
//            }
//            set
//            {
//                if (((storeExchangeStatusField == null)
//                            || (storeExchangeStatusField.Equals(value) != true)))
//                {
//                    storeExchangeStatusField = value;
//                    OnPropertyChanged("StoreExchangeStatus", value);
//                }
//            }
//        }

//        [XmlElement(Order = 82)]
//        [DataMember()]
//        public string TaxCode
//        {
//            get
//            {
//                return taxCodeField;
//            }
//            set
//            {
//                if (((taxCodeField == null)
//                            || (taxCodeField.Equals(value) != true)))
//                {
//                    taxCodeField = value;
//                    OnPropertyChanged("TaxCode", value);
//                }
//            }
//        }

//        [XmlElement(Order = 83)]
//        [DataMember()]
//        public string UnitOfMeasure
//        {
//            get
//            {
//                return unitOfMeasureField;
//            }
//            set
//            {
//                if (((unitOfMeasureField == null)
//                            || (unitOfMeasureField.Equals(value) != true)))
//                {
//                    unitOfMeasureField = value;
//                    OnPropertyChanged("UnitOfMeasure", value);
//                }
//            }
//        }

//        [XmlElement(Order = 84)]
//        [DataMember()]
//        public string UPC
//        {
//            get
//            {
//                return uPCField;
//            }
//            set
//            {
//                if (((uPCField == null)
//                            || (uPCField.Equals(value) != true)))
//                {
//                    uPCField = value;
//                    OnPropertyChanged("UPC", value);
//                }
//            }
//        }

//        [XmlElement(Order = 85)]
//        [DataMember()]
//        public string VendorCode
//        {
//            get
//            {
//                return vendorCodeField;
//            }
//            set
//            {
//                if (((vendorCodeField == null)
//                            || (vendorCodeField.Equals(value) != true)))
//                {
//                    vendorCodeField = value;
//                    OnPropertyChanged("VendorCode", value);
//                }
//            }
//        }

//        [XmlElement(Order = 86)]
//        [DataMember()]
//        public string VendorListID
//        {
//            get
//            {
//                return vendorListIDField;
//            }
//            set
//            {
//                if (((vendorListIDField == null)
//                            || (vendorListIDField.Equals(value) != true)))
//                {
//                    vendorListIDField = value;
//                    OnPropertyChanged("VendorListID", value);
//                }
//            }
//        }

//        [XmlElement(Order = 87)]
//        [DataMember()]
//        public string WebDesc
//        {
//            get
//            {
//                return webDescField;
//            }
//            set
//            {
//                if (((webDescField == null)
//                            || (webDescField.Equals(value) != true)))
//                {
//                    webDescField = value;
//                    OnPropertyChanged("WebDesc", value);
//                }
//            }
//        }

//        [XmlElement(Order = 88)]
//        [DataMember()]
//        public decimal WebPrice
//        {
//            get
//            {
//                return webPriceField;
//            }
//            set
//            {
//                if ((webPriceField.Equals(value) != true))
//                {
//                    webPriceField = value;
//                    OnPropertyChanged("WebPrice", value);
//                }
//            }
//        }

//        [XmlElement(Order = 89)]
//        [DataMember()]
//        public string Manufacturer
//        {
//            get
//            {
//                return manufacturerField;
//            }
//            set
//            {
//                if (((manufacturerField == null)
//                            || (manufacturerField.Equals(value) != true)))
//                {
//                    manufacturerField = value;
//                    OnPropertyChanged("Manufacturer", value);
//                }
//            }
//        }

//        [XmlElement(Order = 90)]
//        [DataMember()]
//        public decimal Weight
//        {
//            get
//            {
//                return weightField;
//            }
//            set
//            {
//                if ((weightField.Equals(value) != true))
//                {
//                    weightField = value;
//                    OnPropertyChanged("Weight", value);
//                }
//            }
//        }

//        [XmlElement(Order = 91)]
//        [DataMember()]
//        public string WebSKU
//        {
//            get
//            {
//                return webSKUField;
//            }
//            set
//            {
//                if (((webSKUField == null)
//                            || (webSKUField.Equals(value) != true)))
//                {
//                    webSKUField = value;
//                    OnPropertyChanged("WebSKU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 92)]
//        [DataMember()]
//        public string Keywords
//        {
//            get
//            {
//                return keywordsField;
//            }
//            set
//            {
//                if (((keywordsField == null)
//                            || (keywordsField.Equals(value) != true)))
//                {
//                    keywordsField = value;
//                    OnPropertyChanged("Keywords", value);
//                }
//            }
//        }

//        [XmlElement(Order = 93)]
//        [DataMember()]
//        public string WebCategories
//        {
//            get
//            {
//                return webCategoriesField;
//            }
//            set
//            {
//                if (((webCategoriesField == null)
//                            || (webCategoriesField.Equals(value) != true)))
//                {
//                    webCategoriesField = value;
//                    OnPropertyChanged("WebCategories", value);
//                }
//            }
//        }

//        [XmlElement(Order = 94)]
//        [DataMember()]
//        public ItemInventoryRetUnitOfMeasure1 UnitOfMeasure1
//        {
//            get
//            {
//                return unitOfMeasure1Field;
//            }
//            set
//            {
//                if (((unitOfMeasure1Field == null)
//                            || (unitOfMeasure1Field.Equals(value) != true)))
//                {
//                    unitOfMeasure1Field = value;
//                    OnPropertyChanged("UnitOfMeasure1", value);
//                }
//            }
//        }

//        [XmlElement(Order = 95)]
//        [DataMember()]
//        public ItemInventoryRetUnitOfMeasure2 UnitOfMeasure2
//        {
//            get
//            {
//                return unitOfMeasure2Field;
//            }
//            set
//            {
//                if (((unitOfMeasure2Field == null)
//                            || (unitOfMeasure2Field.Equals(value) != true)))
//                {
//                    unitOfMeasure2Field = value;
//                    OnPropertyChanged("UnitOfMeasure2", value);
//                }
//            }
//        }

//        [XmlElement(Order = 96)]
//        [DataMember()]
//        public ItemInventoryRetUnitOfMeasure3 UnitOfMeasure3
//        {
//            get
//            {
//                return unitOfMeasure3Field;
//            }
//            set
//            {
//                if (((unitOfMeasure3Field == null)
//                            || (unitOfMeasure3Field.Equals(value) != true)))
//                {
//                    unitOfMeasure3Field = value;
//                    OnPropertyChanged("UnitOfMeasure3", value);
//                }
//            }
//        }

//        [XmlElement(Order = 97)]
//        [DataMember()]
//        public ItemInventoryRetVendorInfo2 VendorInfo2
//        {
//            get
//            {
//                return vendorInfo2Field;
//            }
//            set
//            {
//                if (((vendorInfo2Field == null)
//                            || (vendorInfo2Field.Equals(value) != true)))
//                {
//                    vendorInfo2Field = value;
//                    OnPropertyChanged("VendorInfo2", value);
//                }
//            }
//        }

//        [XmlElement(Order = 98)]
//        [DataMember()]
//        public ItemInventoryRetVendorInfo3 VendorInfo3
//        {
//            get
//            {
//                return vendorInfo3Field;
//            }
//            set
//            {
//                if (((vendorInfo3Field == null)
//                            || (vendorInfo3Field.Equals(value) != true)))
//                {
//                    vendorInfo3Field = value;
//                    OnPropertyChanged("VendorInfo3", value);
//                }
//            }
//        }

//        [XmlElement(Order = 99)]
//        [DataMember()]
//        public ItemInventoryRetVendorInfo4 VendorInfo4
//        {
//            get
//            {
//                return vendorInfo4Field;
//            }
//            set
//            {
//                if (((vendorInfo4Field == null)
//                            || (vendorInfo4Field.Equals(value) != true)))
//                {
//                    vendorInfo4Field = value;
//                    OnPropertyChanged("VendorInfo4", value);
//                }
//            }
//        }

//        [XmlElement(Order = 100)]
//        [DataMember()]
//        public ItemInventoryRetVendorInfo5 VendorInfo5
//        {
//            get
//            {
//                return vendorInfo5Field;
//            }
//            set
//            {
//                if (((vendorInfo5Field == null)
//                            || (vendorInfo5Field.Equals(value) != true)))
//                {
//                    vendorInfo5Field = value;
//                    OnPropertyChanged("VendorInfo5", value);
//                }
//            }
//        }

//        [XmlElement(Order = 101)]
//        [DataMember()]
//        public TrackableCollection<ItemInventoryRetDataExtRet> DataExtRetList
//        {
//            get
//            {
//                return dataExtRetField;
//            }
//            set
//            {
//                if (((dataExtRetField == null)
//                            || (dataExtRetField.Equals(value) != true)))
//                {
//                    dataExtRetField = value;
//                    OnPropertyChanged("DataExtRet", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRet));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRet object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRet object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRet object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRet obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRet);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRet obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRet Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRet Deserialize(Stream s)
//        {
//            return ((ItemInventoryRet)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRet object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRet object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRet object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRet obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRet);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRet obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRet obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRet LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRet LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRet object
//        /// </summary>
//        public virtual ItemInventoryRet Clone()
//        {
//            return ((ItemInventoryRet)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetAvailableQty")]
//    public partial class ItemInventoryRetAvailableQty : INotifyPropertyChanged
//    {

//        private decimal storeNumberField;

//        private decimal quantityOnOrderField;

//        private decimal quantityOnCustomerOrderField;

//        private decimal quantityOnPendingOrderField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public decimal StoreNumber
//        {
//            get
//            {
//                return storeNumberField;
//            }
//            set
//            {
//                if ((storeNumberField.Equals(value) != true))
//                {
//                    storeNumberField = value;
//                    OnPropertyChanged("StoreNumber", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public decimal QuantityOnOrder
//        {
//            get
//            {
//                return quantityOnOrderField;
//            }
//            set
//            {
//                if ((quantityOnOrderField.Equals(value) != true))
//                {
//                    quantityOnOrderField = value;
//                    OnPropertyChanged("QuantityOnOrder", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public decimal QuantityOnCustomerOrder
//        {
//            get
//            {
//                return quantityOnCustomerOrderField;
//            }
//            set
//            {
//                if ((quantityOnCustomerOrderField.Equals(value) != true))
//                {
//                    quantityOnCustomerOrderField = value;
//                    OnPropertyChanged("QuantityOnCustomerOrder", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public decimal QuantityOnPendingOrder
//        {
//            get
//            {
//                return quantityOnPendingOrderField;
//            }
//            set
//            {
//                if ((quantityOnPendingOrderField.Equals(value) != true))
//                {
//                    quantityOnPendingOrderField = value;
//                    OnPropertyChanged("QuantityOnPendingOrder", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetAvailableQty));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetAvailableQty object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetAvailableQty object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetAvailableQty object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetAvailableQty obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetAvailableQty);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetAvailableQty obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetAvailableQty Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetAvailableQty)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetAvailableQty Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetAvailableQty)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetAvailableQty object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetAvailableQty object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetAvailableQty object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetAvailableQty obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetAvailableQty);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetAvailableQty obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetAvailableQty obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetAvailableQty LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetAvailableQty LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetAvailableQty object
//        /// </summary>
//        public virtual ItemInventoryRetAvailableQty Clone()
//        {
//            return ((ItemInventoryRetAvailableQty)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetUnitOfMeasure1")]
//    public partial class ItemInventoryRetUnitOfMeasure1 : INotifyPropertyChanged
//    {

//        private string aLUField;

//        private decimal mSRPField;

//        private decimal numberOfBaseUnitsField;

//        private decimal price1Field;

//        private decimal price2Field;

//        private decimal price3Field;

//        private decimal price4Field;

//        private decimal price5Field;

//        private string unitOfMeasureField;

//        private string uPCField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string ALU
//        {
//            get
//            {
//                return aLUField;
//            }
//            set
//            {
//                if (((aLUField == null)
//                            || (aLUField.Equals(value) != true)))
//                {
//                    aLUField = value;
//                    OnPropertyChanged("ALU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public decimal MSRP
//        {
//            get
//            {
//                return mSRPField;
//            }
//            set
//            {
//                if ((mSRPField.Equals(value) != true))
//                {
//                    mSRPField = value;
//                    OnPropertyChanged("MSRP", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public decimal NumberOfBaseUnits
//        {
//            get
//            {
//                return numberOfBaseUnitsField;
//            }
//            set
//            {
//                if ((numberOfBaseUnitsField.Equals(value) != true))
//                {
//                    numberOfBaseUnitsField = value;
//                    OnPropertyChanged("NumberOfBaseUnits", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public decimal Price1
//        {
//            get
//            {
//                return price1Field;
//            }
//            set
//            {
//                if ((price1Field.Equals(value) != true))
//                {
//                    price1Field = value;
//                    OnPropertyChanged("Price1", value);
//                }
//            }
//        }

//        [XmlElement(Order = 4)]
//        [DataMember()]
//        public decimal Price2
//        {
//            get
//            {
//                return price2Field;
//            }
//            set
//            {
//                if ((price2Field.Equals(value) != true))
//                {
//                    price2Field = value;
//                    OnPropertyChanged("Price2", value);
//                }
//            }
//        }

//        [XmlElement(Order = 5)]
//        [DataMember()]
//        public decimal Price3
//        {
//            get
//            {
//                return price3Field;
//            }
//            set
//            {
//                if ((price3Field.Equals(value) != true))
//                {
//                    price3Field = value;
//                    OnPropertyChanged("Price3", value);
//                }
//            }
//        }

//        [XmlElement(Order = 6)]
//        [DataMember()]
//        public decimal Price4
//        {
//            get
//            {
//                return price4Field;
//            }
//            set
//            {
//                if ((price4Field.Equals(value) != true))
//                {
//                    price4Field = value;
//                    OnPropertyChanged("Price4", value);
//                }
//            }
//        }

//        [XmlElement(Order = 7)]
//        [DataMember()]
//        public decimal Price5
//        {
//            get
//            {
//                return price5Field;
//            }
//            set
//            {
//                if ((price5Field.Equals(value) != true))
//                {
//                    price5Field = value;
//                    OnPropertyChanged("Price5", value);
//                }
//            }
//        }

//        [XmlElement(Order = 8)]
//        [DataMember()]
//        public string UnitOfMeasure
//        {
//            get
//            {
//                return unitOfMeasureField;
//            }
//            set
//            {
//                if (((unitOfMeasureField == null)
//                            || (unitOfMeasureField.Equals(value) != true)))
//                {
//                    unitOfMeasureField = value;
//                    OnPropertyChanged("UnitOfMeasure", value);
//                }
//            }
//        }

//        [XmlElement(Order = 9)]
//        [DataMember()]
//        public string UPC
//        {
//            get
//            {
//                return uPCField;
//            }
//            set
//            {
//                if (((uPCField == null)
//                            || (uPCField.Equals(value) != true)))
//                {
//                    uPCField = value;
//                    OnPropertyChanged("UPC", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetUnitOfMeasure1));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetUnitOfMeasure1 object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetUnitOfMeasure1 object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetUnitOfMeasure1 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetUnitOfMeasure1 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetUnitOfMeasure1);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetUnitOfMeasure1 obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetUnitOfMeasure1 Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetUnitOfMeasure1)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetUnitOfMeasure1 Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetUnitOfMeasure1)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetUnitOfMeasure1 object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetUnitOfMeasure1 object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetUnitOfMeasure1 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetUnitOfMeasure1 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetUnitOfMeasure1);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetUnitOfMeasure1 obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetUnitOfMeasure1 obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetUnitOfMeasure1 LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetUnitOfMeasure1 LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetUnitOfMeasure1 object
//        /// </summary>
//        public virtual ItemInventoryRetUnitOfMeasure1 Clone()
//        {
//            return ((ItemInventoryRetUnitOfMeasure1)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetUnitOfMeasure2")]
//    public partial class ItemInventoryRetUnitOfMeasure2 : INotifyPropertyChanged
//    {

//        private string aLUField;

//        private decimal mSRPField;

//        private decimal numberOfBaseUnitsField;

//        private decimal price1Field;

//        private decimal price2Field;

//        private decimal price3Field;

//        private decimal price4Field;

//        private decimal price5Field;

//        private string unitOfMeasureField;

//        private string uPCField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string ALU
//        {
//            get
//            {
//                return aLUField;
//            }
//            set
//            {
//                if (((aLUField == null)
//                            || (aLUField.Equals(value) != true)))
//                {
//                    aLUField = value;
//                    OnPropertyChanged("ALU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public decimal MSRP
//        {
//            get
//            {
//                return mSRPField;
//            }
//            set
//            {
//                if ((mSRPField.Equals(value) != true))
//                {
//                    mSRPField = value;
//                    OnPropertyChanged("MSRP", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public decimal NumberOfBaseUnits
//        {
//            get
//            {
//                return numberOfBaseUnitsField;
//            }
//            set
//            {
//                if ((numberOfBaseUnitsField.Equals(value) != true))
//                {
//                    numberOfBaseUnitsField = value;
//                    OnPropertyChanged("NumberOfBaseUnits", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public decimal Price1
//        {
//            get
//            {
//                return price1Field;
//            }
//            set
//            {
//                if ((price1Field.Equals(value) != true))
//                {
//                    price1Field = value;
//                    OnPropertyChanged("Price1", value);
//                }
//            }
//        }

//        [XmlElement(Order = 4)]
//        [DataMember()]
//        public decimal Price2
//        {
//            get
//            {
//                return price2Field;
//            }
//            set
//            {
//                if ((price2Field.Equals(value) != true))
//                {
//                    price2Field = value;
//                    OnPropertyChanged("Price2", value);
//                }
//            }
//        }

//        [XmlElement(Order = 5)]
//        [DataMember()]
//        public decimal Price3
//        {
//            get
//            {
//                return price3Field;
//            }
//            set
//            {
//                if ((price3Field.Equals(value) != true))
//                {
//                    price3Field = value;
//                    OnPropertyChanged("Price3", value);
//                }
//            }
//        }

//        [XmlElement(Order = 6)]
//        [DataMember()]
//        public decimal Price4
//        {
//            get
//            {
//                return price4Field;
//            }
//            set
//            {
//                if ((price4Field.Equals(value) != true))
//                {
//                    price4Field = value;
//                    OnPropertyChanged("Price4", value);
//                }
//            }
//        }

//        [XmlElement(Order = 7)]
//        [DataMember()]
//        public decimal Price5
//        {
//            get
//            {
//                return price5Field;
//            }
//            set
//            {
//                if ((price5Field.Equals(value) != true))
//                {
//                    price5Field = value;
//                    OnPropertyChanged("Price5", value);
//                }
//            }
//        }

//        [XmlElement(Order = 8)]
//        [DataMember()]
//        public string UnitOfMeasure
//        {
//            get
//            {
//                return unitOfMeasureField;
//            }
//            set
//            {
//                if (((unitOfMeasureField == null)
//                            || (unitOfMeasureField.Equals(value) != true)))
//                {
//                    unitOfMeasureField = value;
//                    OnPropertyChanged("UnitOfMeasure", value);
//                }
//            }
//        }

//        [XmlElement(Order = 9)]
//        [DataMember()]
//        public string UPC
//        {
//            get
//            {
//                return uPCField;
//            }
//            set
//            {
//                if (((uPCField == null)
//                            || (uPCField.Equals(value) != true)))
//                {
//                    uPCField = value;
//                    OnPropertyChanged("UPC", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetUnitOfMeasure2));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetUnitOfMeasure2 object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetUnitOfMeasure2 object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetUnitOfMeasure2 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetUnitOfMeasure2 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetUnitOfMeasure2);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetUnitOfMeasure2 obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetUnitOfMeasure2 Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetUnitOfMeasure2)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetUnitOfMeasure2 Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetUnitOfMeasure2)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetUnitOfMeasure2 object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetUnitOfMeasure2 object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetUnitOfMeasure2 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetUnitOfMeasure2 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetUnitOfMeasure2);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetUnitOfMeasure2 obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetUnitOfMeasure2 obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetUnitOfMeasure2 LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetUnitOfMeasure2 LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetUnitOfMeasure2 object
//        /// </summary>
//        public virtual ItemInventoryRetUnitOfMeasure2 Clone()
//        {
//            return ((ItemInventoryRetUnitOfMeasure2)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetUnitOfMeasure3")]
//    public partial class ItemInventoryRetUnitOfMeasure3 : INotifyPropertyChanged
//    {

//        private string aLUField;

//        private decimal mSRPField;

//        private decimal numberOfBaseUnitsField;

//        private decimal price1Field;

//        private decimal price2Field;

//        private decimal price3Field;

//        private decimal price4Field;

//        private decimal price5Field;

//        private string unitOfMeasureField;

//        private string uPCField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string ALU
//        {
//            get
//            {
//                return aLUField;
//            }
//            set
//            {
//                if (((aLUField == null)
//                            || (aLUField.Equals(value) != true)))
//                {
//                    aLUField = value;
//                    OnPropertyChanged("ALU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public decimal MSRP
//        {
//            get
//            {
//                return mSRPField;
//            }
//            set
//            {
//                if ((mSRPField.Equals(value) != true))
//                {
//                    mSRPField = value;
//                    OnPropertyChanged("MSRP", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public decimal NumberOfBaseUnits
//        {
//            get
//            {
//                return numberOfBaseUnitsField;
//            }
//            set
//            {
//                if ((numberOfBaseUnitsField.Equals(value) != true))
//                {
//                    numberOfBaseUnitsField = value;
//                    OnPropertyChanged("NumberOfBaseUnits", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public decimal Price1
//        {
//            get
//            {
//                return price1Field;
//            }
//            set
//            {
//                if ((price1Field.Equals(value) != true))
//                {
//                    price1Field = value;
//                    OnPropertyChanged("Price1", value);
//                }
//            }
//        }

//        [XmlElement(Order = 4)]
//        [DataMember()]
//        public decimal Price2
//        {
//            get
//            {
//                return price2Field;
//            }
//            set
//            {
//                if ((price2Field.Equals(value) != true))
//                {
//                    price2Field = value;
//                    OnPropertyChanged("Price2", value);
//                }
//            }
//        }

//        [XmlElement(Order = 5)]
//        [DataMember()]
//        public decimal Price3
//        {
//            get
//            {
//                return price3Field;
//            }
//            set
//            {
//                if ((price3Field.Equals(value) != true))
//                {
//                    price3Field = value;
//                    OnPropertyChanged("Price3", value);
//                }
//            }
//        }

//        [XmlElement(Order = 6)]
//        [DataMember()]
//        public decimal Price4
//        {
//            get
//            {
//                return price4Field;
//            }
//            set
//            {
//                if ((price4Field.Equals(value) != true))
//                {
//                    price4Field = value;
//                    OnPropertyChanged("Price4", value);
//                }
//            }
//        }

//        [XmlElement(Order = 7)]
//        [DataMember()]
//        public decimal Price5
//        {
//            get
//            {
//                return price5Field;
//            }
//            set
//            {
//                if ((price5Field.Equals(value) != true))
//                {
//                    price5Field = value;
//                    OnPropertyChanged("Price5", value);
//                }
//            }
//        }

//        [XmlElement(Order = 8)]
//        [DataMember()]
//        public string UnitOfMeasure
//        {
//            get
//            {
//                return unitOfMeasureField;
//            }
//            set
//            {
//                if (((unitOfMeasureField == null)
//                            || (unitOfMeasureField.Equals(value) != true)))
//                {
//                    unitOfMeasureField = value;
//                    OnPropertyChanged("UnitOfMeasure", value);
//                }
//            }
//        }

//        [XmlElement(Order = 9)]
//        [DataMember()]
//        public string UPC
//        {
//            get
//            {
//                return uPCField;
//            }
//            set
//            {
//                if (((uPCField == null)
//                            || (uPCField.Equals(value) != true)))
//                {
//                    uPCField = value;
//                    OnPropertyChanged("UPC", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetUnitOfMeasure3));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetUnitOfMeasure3 object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetUnitOfMeasure3 object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetUnitOfMeasure3 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetUnitOfMeasure3 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetUnitOfMeasure3);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetUnitOfMeasure3 obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetUnitOfMeasure3 Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetUnitOfMeasure3)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetUnitOfMeasure3 Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetUnitOfMeasure3)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetUnitOfMeasure3 object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetUnitOfMeasure3 object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetUnitOfMeasure3 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetUnitOfMeasure3 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetUnitOfMeasure3);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetUnitOfMeasure3 obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetUnitOfMeasure3 obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetUnitOfMeasure3 LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetUnitOfMeasure3 LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetUnitOfMeasure3 object
//        /// </summary>
//        public virtual ItemInventoryRetUnitOfMeasure3 Clone()
//        {
//            return ((ItemInventoryRetUnitOfMeasure3)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetVendorInfo2")]
//    public partial class ItemInventoryRetVendorInfo2 : INotifyPropertyChanged
//    {

//        private string aLUField;

//        private decimal orderCostField;

//        private string uPCField;

//        private ItemInventoryRetVendorInfo2VendorListID vendorListIDField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        public ItemInventoryRetVendorInfo2()
//        {
//            vendorListIDField = new ItemInventoryRetVendorInfo2VendorListID();
//        }

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string ALU
//        {
//            get
//            {
//                return aLUField;
//            }
//            set
//            {
//                if (((aLUField == null)
//                            || (aLUField.Equals(value) != true)))
//                {
//                    aLUField = value;
//                    OnPropertyChanged("ALU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public decimal OrderCost
//        {
//            get
//            {
//                return orderCostField;
//            }
//            set
//            {
//                if ((orderCostField.Equals(value) != true))
//                {
//                    orderCostField = value;
//                    OnPropertyChanged("OrderCost", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public string UPC
//        {
//            get
//            {
//                return uPCField;
//            }
//            set
//            {
//                if (((uPCField == null)
//                            || (uPCField.Equals(value) != true)))
//                {
//                    uPCField = value;
//                    OnPropertyChanged("UPC", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public ItemInventoryRetVendorInfo2VendorListID VendorListID
//        {
//            get
//            {
//                return vendorListIDField;
//            }
//            set
//            {
//                if (((vendorListIDField == null)
//                            || (vendorListIDField.Equals(value) != true)))
//                {
//                    vendorListIDField = value;
//                    OnPropertyChanged("VendorListID", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetVendorInfo2));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo2 object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetVendorInfo2 object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo2 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo2 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo2);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo2 obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetVendorInfo2 Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetVendorInfo2)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetVendorInfo2 Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetVendorInfo2)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo2 object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetVendorInfo2 object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo2 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetVendorInfo2 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo2);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo2 obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo2 obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetVendorInfo2 LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetVendorInfo2 LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetVendorInfo2 object
//        /// </summary>
//        public virtual ItemInventoryRetVendorInfo2 Clone()
//        {
//            return ((ItemInventoryRetVendorInfo2)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetVendorInfo2VendorListID")]
//    public partial class ItemInventoryRetVendorInfo2VendorListID : INotifyPropertyChanged
//    {

//        private string useMacroField;

//        private string valueField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlAttribute()]
//        [DataMember()]
//        public string useMacro
//        {
//            get
//            {
//                return useMacroField;
//            }
//            set
//            {
//                if (((useMacroField == null)
//                            || (useMacroField.Equals(value) != true)))
//                {
//                    useMacroField = value;
//                    OnPropertyChanged("useMacro", value);
//                }
//            }
//        }

//        [XmlText()]
//        [DataMember()]
//        public string Value
//        {
//            get
//            {
//                return valueField;
//            }
//            set
//            {
//                if (((valueField == null)
//                            || (valueField.Equals(value) != true)))
//                {
//                    valueField = value;
//                    OnPropertyChanged("Value", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetVendorInfo2VendorListID));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo2VendorListID object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetVendorInfo2VendorListID object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo2VendorListID object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo2VendorListID obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo2VendorListID);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo2VendorListID obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetVendorInfo2VendorListID Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetVendorInfo2VendorListID)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetVendorInfo2VendorListID Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetVendorInfo2VendorListID)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo2VendorListID object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetVendorInfo2VendorListID object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo2VendorListID object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetVendorInfo2VendorListID obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo2VendorListID);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo2VendorListID obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo2VendorListID obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetVendorInfo2VendorListID LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetVendorInfo2VendorListID LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetVendorInfo2VendorListID object
//        /// </summary>
//        public virtual ItemInventoryRetVendorInfo2VendorListID Clone()
//        {
//            return ((ItemInventoryRetVendorInfo2VendorListID)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetVendorInfo3")]
//    public partial class ItemInventoryRetVendorInfo3 : INotifyPropertyChanged
//    {

//        private string aLUField;

//        private decimal orderCostField;

//        private string uPCField;

//        private ItemInventoryRetVendorInfo3VendorListID vendorListIDField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        public ItemInventoryRetVendorInfo3()
//        {
//            vendorListIDField = new ItemInventoryRetVendorInfo3VendorListID();
//        }

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string ALU
//        {
//            get
//            {
//                return aLUField;
//            }
//            set
//            {
//                if (((aLUField == null)
//                            || (aLUField.Equals(value) != true)))
//                {
//                    aLUField = value;
//                    OnPropertyChanged("ALU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public decimal OrderCost
//        {
//            get
//            {
//                return orderCostField;
//            }
//            set
//            {
//                if ((orderCostField.Equals(value) != true))
//                {
//                    orderCostField = value;
//                    OnPropertyChanged("OrderCost", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public string UPC
//        {
//            get
//            {
//                return uPCField;
//            }
//            set
//            {
//                if (((uPCField == null)
//                            || (uPCField.Equals(value) != true)))
//                {
//                    uPCField = value;
//                    OnPropertyChanged("UPC", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public ItemInventoryRetVendorInfo3VendorListID VendorListID
//        {
//            get
//            {
//                return vendorListIDField;
//            }
//            set
//            {
//                if (((vendorListIDField == null)
//                            || (vendorListIDField.Equals(value) != true)))
//                {
//                    vendorListIDField = value;
//                    OnPropertyChanged("VendorListID", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetVendorInfo3));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo3 object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetVendorInfo3 object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo3 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo3 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo3);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo3 obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetVendorInfo3 Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetVendorInfo3)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetVendorInfo3 Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetVendorInfo3)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo3 object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetVendorInfo3 object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo3 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetVendorInfo3 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo3);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo3 obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo3 obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetVendorInfo3 LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetVendorInfo3 LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetVendorInfo3 object
//        /// </summary>
//        public virtual ItemInventoryRetVendorInfo3 Clone()
//        {
//            return ((ItemInventoryRetVendorInfo3)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetVendorInfo3VendorListID")]
//    public partial class ItemInventoryRetVendorInfo3VendorListID : INotifyPropertyChanged
//    {

//        private string useMacroField;

//        private string valueField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlAttribute()]
//        [DataMember()]
//        public string useMacro
//        {
//            get
//            {
//                return useMacroField;
//            }
//            set
//            {
//                if (((useMacroField == null)
//                            || (useMacroField.Equals(value) != true)))
//                {
//                    useMacroField = value;
//                    OnPropertyChanged("useMacro", value);
//                }
//            }
//        }

//        [XmlText()]
//        [DataMember()]
//        public string Value
//        {
//            get
//            {
//                return valueField;
//            }
//            set
//            {
//                if (((valueField == null)
//                            || (valueField.Equals(value) != true)))
//                {
//                    valueField = value;
//                    OnPropertyChanged("Value", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetVendorInfo3VendorListID));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo3VendorListID object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetVendorInfo3VendorListID object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo3VendorListID object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo3VendorListID obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo3VendorListID);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo3VendorListID obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetVendorInfo3VendorListID Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetVendorInfo3VendorListID)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetVendorInfo3VendorListID Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetVendorInfo3VendorListID)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo3VendorListID object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetVendorInfo3VendorListID object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo3VendorListID object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetVendorInfo3VendorListID obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo3VendorListID);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo3VendorListID obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo3VendorListID obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetVendorInfo3VendorListID LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetVendorInfo3VendorListID LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetVendorInfo3VendorListID object
//        /// </summary>
//        public virtual ItemInventoryRetVendorInfo3VendorListID Clone()
//        {
//            return ((ItemInventoryRetVendorInfo3VendorListID)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetVendorInfo4")]
//    public partial class ItemInventoryRetVendorInfo4 : INotifyPropertyChanged
//    {

//        private string aLUField;

//        private decimal orderCostField;

//        private string uPCField;

//        private ItemInventoryRetVendorInfo4VendorListID vendorListIDField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        public ItemInventoryRetVendorInfo4()
//        {
//            vendorListIDField = new ItemInventoryRetVendorInfo4VendorListID();
//        }

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string ALU
//        {
//            get
//            {
//                return aLUField;
//            }
//            set
//            {
//                if (((aLUField == null)
//                            || (aLUField.Equals(value) != true)))
//                {
//                    aLUField = value;
//                    OnPropertyChanged("ALU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public decimal OrderCost
//        {
//            get
//            {
//                return orderCostField;
//            }
//            set
//            {
//                if ((orderCostField.Equals(value) != true))
//                {
//                    orderCostField = value;
//                    OnPropertyChanged("OrderCost", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public string UPC
//        {
//            get
//            {
//                return uPCField;
//            }
//            set
//            {
//                if (((uPCField == null)
//                            || (uPCField.Equals(value) != true)))
//                {
//                    uPCField = value;
//                    OnPropertyChanged("UPC", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public ItemInventoryRetVendorInfo4VendorListID VendorListID
//        {
//            get
//            {
//                return vendorListIDField;
//            }
//            set
//            {
//                if (((vendorListIDField == null)
//                            || (vendorListIDField.Equals(value) != true)))
//                {
//                    vendorListIDField = value;
//                    OnPropertyChanged("VendorListID", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetVendorInfo4));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo4 object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetVendorInfo4 object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo4 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo4 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo4);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo4 obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetVendorInfo4 Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetVendorInfo4)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetVendorInfo4 Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetVendorInfo4)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo4 object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetVendorInfo4 object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo4 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetVendorInfo4 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo4);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo4 obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo4 obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetVendorInfo4 LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetVendorInfo4 LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetVendorInfo4 object
//        /// </summary>
//        public virtual ItemInventoryRetVendorInfo4 Clone()
//        {
//            return ((ItemInventoryRetVendorInfo4)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetVendorInfo4VendorListID")]
//    public partial class ItemInventoryRetVendorInfo4VendorListID : INotifyPropertyChanged
//    {

//        private string useMacroField;

//        private string valueField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlAttribute()]
//        [DataMember()]
//        public string useMacro
//        {
//            get
//            {
//                return useMacroField;
//            }
//            set
//            {
//                if (((useMacroField == null)
//                            || (useMacroField.Equals(value) != true)))
//                {
//                    useMacroField = value;
//                    OnPropertyChanged("useMacro", value);
//                }
//            }
//        }

//        [XmlText()]
//        [DataMember()]
//        public string Value
//        {
//            get
//            {
//                return valueField;
//            }
//            set
//            {
//                if (((valueField == null)
//                            || (valueField.Equals(value) != true)))
//                {
//                    valueField = value;
//                    OnPropertyChanged("Value", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetVendorInfo4VendorListID));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo4VendorListID object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetVendorInfo4VendorListID object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo4VendorListID object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo4VendorListID obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo4VendorListID);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo4VendorListID obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetVendorInfo4VendorListID Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetVendorInfo4VendorListID)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetVendorInfo4VendorListID Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetVendorInfo4VendorListID)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo4VendorListID object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetVendorInfo4VendorListID object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo4VendorListID object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetVendorInfo4VendorListID obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo4VendorListID);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo4VendorListID obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo4VendorListID obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetVendorInfo4VendorListID LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetVendorInfo4VendorListID LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetVendorInfo4VendorListID object
//        /// </summary>
//        public virtual ItemInventoryRetVendorInfo4VendorListID Clone()
//        {
//            return ((ItemInventoryRetVendorInfo4VendorListID)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetVendorInfo5")]
//    public partial class ItemInventoryRetVendorInfo5 : INotifyPropertyChanged
//    {

//        private string aLUField;

//        private decimal orderCostField;

//        private string uPCField;

//        private ItemInventoryRetVendorInfo5VendorListID vendorListIDField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        public ItemInventoryRetVendorInfo5()
//        {
//            vendorListIDField = new ItemInventoryRetVendorInfo5VendorListID();
//        }

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string ALU
//        {
//            get
//            {
//                return aLUField;
//            }
//            set
//            {
//                if (((aLUField == null)
//                            || (aLUField.Equals(value) != true)))
//                {
//                    aLUField = value;
//                    OnPropertyChanged("ALU", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public decimal OrderCost
//        {
//            get
//            {
//                return orderCostField;
//            }
//            set
//            {
//                if ((orderCostField.Equals(value) != true))
//                {
//                    orderCostField = value;
//                    OnPropertyChanged("OrderCost", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public string UPC
//        {
//            get
//            {
//                return uPCField;
//            }
//            set
//            {
//                if (((uPCField == null)
//                            || (uPCField.Equals(value) != true)))
//                {
//                    uPCField = value;
//                    OnPropertyChanged("UPC", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public ItemInventoryRetVendorInfo5VendorListID VendorListID
//        {
//            get
//            {
//                return vendorListIDField;
//            }
//            set
//            {
//                if (((vendorListIDField == null)
//                            || (vendorListIDField.Equals(value) != true)))
//                {
//                    vendorListIDField = value;
//                    OnPropertyChanged("VendorListID", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetVendorInfo5));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo5 object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetVendorInfo5 object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo5 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo5 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo5);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo5 obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetVendorInfo5 Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetVendorInfo5)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetVendorInfo5 Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetVendorInfo5)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo5 object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetVendorInfo5 object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo5 object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetVendorInfo5 obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo5);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo5 obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo5 obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetVendorInfo5 LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetVendorInfo5 LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetVendorInfo5 object
//        /// </summary>
//        public virtual ItemInventoryRetVendorInfo5 Clone()
//        {
//            return ((ItemInventoryRetVendorInfo5)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetVendorInfo5VendorListID")]
//    public partial class ItemInventoryRetVendorInfo5VendorListID : INotifyPropertyChanged
//    {

//        private string useMacroField;

//        private string valueField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlAttribute()]
//        [DataMember()]
//        public string useMacro
//        {
//            get
//            {
//                return useMacroField;
//            }
//            set
//            {
//                if (((useMacroField == null)
//                            || (useMacroField.Equals(value) != true)))
//                {
//                    useMacroField = value;
//                    OnPropertyChanged("useMacro", value);
//                }
//            }
//        }

//        [XmlText()]
//        [DataMember()]
//        public string Value
//        {
//            get
//            {
//                return valueField;
//            }
//            set
//            {
//                if (((valueField == null)
//                            || (valueField.Equals(value) != true)))
//                {
//                    valueField = value;
//                    OnPropertyChanged("Value", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetVendorInfo5VendorListID));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo5VendorListID object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetVendorInfo5VendorListID object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo5VendorListID object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo5VendorListID obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo5VendorListID);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetVendorInfo5VendorListID obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetVendorInfo5VendorListID Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetVendorInfo5VendorListID)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetVendorInfo5VendorListID Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetVendorInfo5VendorListID)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetVendorInfo5VendorListID object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetVendorInfo5VendorListID object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetVendorInfo5VendorListID object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetVendorInfo5VendorListID obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetVendorInfo5VendorListID);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo5VendorListID obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetVendorInfo5VendorListID obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetVendorInfo5VendorListID LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetVendorInfo5VendorListID LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetVendorInfo5VendorListID object
//        /// </summary>
//        public virtual ItemInventoryRetVendorInfo5VendorListID Clone()
//        {
//            return ((ItemInventoryRetVendorInfo5VendorListID)(MemberwiseClone()));
//        }
//        #endregion
//    }

//    [GeneratedCode("System.Xml", "4.0.30319.17929")]
//    [Serializable()]
//    [DesignerCategory("code")]
//    [XmlType(AnonymousType = true)]
//    [DataContract(Name = "ItemInventoryRetDataExtRet")]
//    public partial class ItemInventoryRetDataExtRet : INotifyPropertyChanged
//    {

//        private string ownerIDField;

//        private string dataExtNameField;

//        private string dataExtTypeField;

//        private string dataExtValueField;

//        private static XmlSerializer serializer;

//        private ObjectChangeTracker changeTrackerField;

//        [XmlElement(Order = 0)]
//        [DataMember()]
//        public string OwnerID
//        {
//            get
//            {
//                return ownerIDField;
//            }
//            set
//            {
//                if (((ownerIDField == null)
//                            || (ownerIDField.Equals(value) != true)))
//                {
//                    ownerIDField = value;
//                    OnPropertyChanged("OwnerID", value);
//                }
//            }
//        }

//        [XmlElement(Order = 1)]
//        [DataMember()]
//        public string DataExtName
//        {
//            get
//            {
//                return dataExtNameField;
//            }
//            set
//            {
//                if (((dataExtNameField == null)
//                            || (dataExtNameField.Equals(value) != true)))
//                {
//                    dataExtNameField = value;
//                    OnPropertyChanged("DataExtName", value);
//                }
//            }
//        }

//        [XmlElement(Order = 2)]
//        [DataMember()]
//        public string DataExtType
//        {
//            get
//            {
//                return dataExtTypeField;
//            }
//            set
//            {
//                if (((dataExtTypeField == null)
//                            || (dataExtTypeField.Equals(value) != true)))
//                {
//                    dataExtTypeField = value;
//                    OnPropertyChanged("DataExtType", value);
//                }
//            }
//        }

//        [XmlElement(Order = 3)]
//        [DataMember()]
//        public string DataExtValue
//        {
//            get
//            {
//                return dataExtValueField;
//            }
//            set
//            {
//                if (((dataExtValueField == null)
//                            || (dataExtValueField.Equals(value) != true)))
//                {
//                    dataExtValueField = value;
//                    OnPropertyChanged("DataExtValue", value);
//                }
//            }
//        }

//        private static XmlSerializer Serializer
//        {
//            get
//            {
//                if ((serializer == null))
//                {
//                    serializer = new XmlSerializer(typeof(ItemInventoryRetDataExtRet));
//                }
//                return serializer;
//            }
//        }

//        [XmlIgnore()]
//        public ObjectChangeTracker ChangeTracker
//        {
//            get
//            {
//                if ((changeTrackerField == null))
//                {
//                    changeTrackerField = new ObjectChangeTracker(this);
//                }
//                return changeTrackerField;
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        public virtual void OnPropertyChanged(string propertyName, object value)
//        {
//            ChangeTracker.RecordCurrentValue(propertyName, value);
//            var handler = PropertyChanged;
//            if ((handler != null))
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }

//        #region Serialize/Deserialize
//        /// <summary>
//        /// Serializes current ItemInventoryRetDataExtRet object into an XML document
//        /// </summary>
//        /// <returns>string XML value</returns>
//        public virtual string Serialize(Encoding encoding)
//        {
//            StreamReader streamReader = null;
//            MemoryStream memoryStream = null;
//            try
//            {
//                memoryStream = new MemoryStream();
//                var xmlWriterSettings = new XmlWriterSettings();
//                xmlWriterSettings.Encoding = encoding;
//                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
//                Serializer.Serialize(xmlWriter, this);
//                memoryStream.Seek(0, SeekOrigin.Begin);
//                streamReader = new StreamReader(memoryStream, encoding);
//                return streamReader.ReadToEnd();
//            }
//            finally
//            {
//                if ((streamReader != null))
//                {
//                    streamReader.Dispose();
//                }
//                if ((memoryStream != null))
//                {
//                    memoryStream.Dispose();
//                }
//            }
//        }

//        public virtual string Serialize()
//        {
//            return Serialize(Encoding.UTF8);
//        }

//        /// <summary>
//        /// Deserializes workflow markup into an ItemInventoryRetDataExtRet object
//        /// </summary>
//        /// <param name="xml">string workflow markup to deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetDataExtRet object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool Deserialize(string xml, out ItemInventoryRetDataExtRet obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetDataExtRet);
//            try
//            {
//                obj = Deserialize(xml);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool Deserialize(string xml, out ItemInventoryRetDataExtRet obj)
//        {
//            Exception exception = null;
//            return Deserialize(xml, out obj, out exception);
//        }

//        public new static ItemInventoryRetDataExtRet Deserialize(string xml)
//        {
//            StringReader stringReader = null;
//            try
//            {
//                stringReader = new StringReader(xml);
//                return ((ItemInventoryRetDataExtRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
//            }
//            finally
//            {
//                if ((stringReader != null))
//                {
//                    stringReader.Dispose();
//                }
//            }
//        }

//        public static ItemInventoryRetDataExtRet Deserialize(Stream s)
//        {
//            return ((ItemInventoryRetDataExtRet)(Serializer.Deserialize(s)));
//        }

//        /// <summary>
//        /// Serializes current ItemInventoryRetDataExtRet object into file
//        /// </summary>
//        /// <param name="fileName">full path of outupt xml file</param>
//        /// <param name="exception">output Exception value if failed</param>
//        /// <returns>true if can serialize and save into file; otherwise, false</returns>
//        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
//        {
//            exception = null;
//            try
//            {
//                SaveToFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public virtual bool SaveToFile(string fileName, out Exception exception)
//        {
//            return SaveToFile(fileName, Encoding.UTF8, out exception);
//        }

//        public virtual void SaveToFile(string fileName)
//        {
//            SaveToFile(fileName, Encoding.UTF8);
//        }

//        public virtual void SaveToFile(string fileName, Encoding encoding)
//        {
//            StreamWriter streamWriter = null;
//            try
//            {
//                var xmlString = Serialize(encoding);
//                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
//                streamWriter.WriteLine(xmlString);
//                streamWriter.Close();
//            }
//            finally
//            {
//                if ((streamWriter != null))
//                {
//                    streamWriter.Dispose();
//                }
//            }
//        }

//        /// <summary>
//        /// Deserializes xml markup from file into an ItemInventoryRetDataExtRet object
//        /// </summary>
//        /// <param name="fileName">string xml file to load and deserialize</param>
//        /// <param name="obj">Output ItemInventoryRetDataExtRet object</param>
//        /// <param name="exception">output Exception value if deserialize failed</param>
//        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
//        public static bool LoadFromFile(string fileName, Encoding encoding, out ItemInventoryRetDataExtRet obj, out Exception exception)
//        {
//            exception = null;
//            obj = default(ItemInventoryRetDataExtRet);
//            try
//            {
//                obj = LoadFromFile(fileName, encoding);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                return false;
//            }
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetDataExtRet obj, out Exception exception)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
//        }

//        public static bool LoadFromFile(string fileName, out ItemInventoryRetDataExtRet obj)
//        {
//            Exception exception = null;
//            return LoadFromFile(fileName, out obj, out exception);
//        }

//        public static ItemInventoryRetDataExtRet LoadFromFile(string fileName)
//        {
//            return LoadFromFile(fileName, Encoding.UTF8);
//        }

//        public new static ItemInventoryRetDataExtRet LoadFromFile(string fileName, Encoding encoding)
//        {
//            FileStream file = null;
//            StreamReader sr = null;
//            try
//            {
//                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
//                sr = new StreamReader(file, encoding);
//                var xmlString = sr.ReadToEnd();
//                sr.Close();
//                file.Close();
//                return Deserialize(xmlString);
//            }
//            finally
//            {
//                if ((file != null))
//                {
//                    file.Dispose();
//                }
//                if ((sr != null))
//                {
//                    sr.Dispose();
//                }
//            }
//        }
//        #endregion

//        #region Clone method
//        /// <summary>
//        /// Create a clone of this ItemInventoryRetDataExtRet object
//        /// </summary>
//        public virtual ItemInventoryRetDataExtRet Clone()
//        {
//            return ((ItemInventoryRetDataExtRet)(MemberwiseClone()));
//        }
//        #endregion
//    }
//}
