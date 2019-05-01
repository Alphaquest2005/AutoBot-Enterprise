//using System.Linq;
//using AdjustmentQS.Client.Repositories;


//namespace WaterNut.QuerySpace.AdjustmentQS.ViewModels
//{
//    public class OverShortDetailAllocationsModel : OverShortAllocationsEXViewModel_AutoGen
//    {
//           private static readonly OverShortDetailAllocationsModel instance;
//           static OverShortDetailAllocationsModel()
//        {
//            instance = new OverShortDetailAllocationsModel(){ViewCurrentOverShortDetailsEX = true,
//                                                            ViewCurrentAsycudaDocumentItem = true};
//        }

//        public static  OverShortDetailAllocationsModel Instance
//        {
//            get { return instance; }
//        }

//        bool _viewErrors = false;
//        public bool ViewErrors
//        {
//            get
//            {
//                return _viewErrors;
//            }
//            set
//            {
//                _viewErrors = value;
//                FilterData();
//                NotifyPropertyChanged(x => this.ViewErrors);
//            }
//        }


       

//        bool _viewMatches = false;
//        public bool ViewMatches
//        {
//            get
//            {
//                return _viewMatches;
//            }
//            set
//            {
//                _viewMatches = value;
//                FilterData();
//                NotifyPropertyChanged(x => this.ViewMatches);
//            }
//        }


//        bool _viewSelected = false;
//        public bool ViewSelected
//        {
//            get
//            {
//                return _viewSelected;
//            }
//            set
//            {
//                _viewSelected = value;
//                FilterData();
//                NotifyPropertyChanged(x => this.ViewSelected);
//            }
//        }

        

//        bool _viewBadMatches = false;
//        public bool ViewNoMatches
//        {
//            get
//            {
//                return _viewBadMatches;
//            }
//            set
//            {
//                _viewBadMatches = value;
//                FilterData();
//                NotifyPropertyChanged(x => this.ViewNoMatches);
//            }
//        }


//        public override void FilterData()
//        {
//            var res = GetAutoPropertyFilterString();
//            if (_viewErrors)
//            {
//                res.Append(@" && Status != null");
//            }
//            if (_viewMatches)
//            {
//                res.Append(@" && Duration < 15 && AsycudaMonth == InvoiceMonth");
//            }
//            if (_viewSelected)
//            {
//                var lst = AdjustmentsModel.Instance.SelectedAdjustments.ToList();
//                if (lst.Any())
//                {
//                    var slst = OversShortEXRepository.Instance.BuildOSLst(lst.Select(x => x.AdjustmentsId).ToList());
//                    //remove comma

//                    res.Append(slst);
//                }
//            }

//            if (_viewBadMatches)
//            {
//                //vloader.SetNavigationExpression("EX");
//                res.Append(@" && Duration > 15 && InvoiceMonth != AsycudaMonth");

//            }

//            FilterData(res);

//        }







       
//    }
//}