//using System.Data.Entity.Validation;
//using InsightPresentationLayer;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data.Objects.DataClasses;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using WaterNut.DataLayer;
//using System.Data.Entity;
//using System.Data.Objects;
//using System.Diagnostics;
//using System.Data;

//namespace WaterNut
//{
//    public partial class BaseViewModel :  INotifyPropertyChanged
//    {
//        public static volatile WaterNutDBEntities db = new WaterNutDBEntities(Properties.Settings.Default.WaterNutDBEntitiesConnection); 
      
//        public static SliderPanel slider { get; set; }

//        public BaseViewModel()
//        {
//          //  StatusModel.Timer("Loading ...");
//            if (db.ApplicationSettings.Count() > 0)
//            {
//                //if (isloaded == false)
//                //{
//                CurrentApplicationSettings = db.ApplicationSettings.FirstOrDefault();
//                Debug.WriteLine("load Database Start" + DateTime.Now.ToString());

//               // LoadAllData();

//                db.InventoryItems.Include(x => x.TariffCodes)
//                             .Include(x => x.TariffCodes.TariffCategory)
//                             .Load();
                

//                Debug.WriteLine("load Database End" + DateTime.Now.ToString());
                
//            }
//            else
//            {
//                MessageBox.Show("No Default Application Settings Defined");
//            }


//        }

//        public static DataLayer.ApplicationSettings _currentApplicationSettings;

//        public DataLayer.ApplicationSettings CurrentApplicationSettings
//        {
//            get
//            {
//                return _currentApplicationSettings;
//            }
//            set
//            {
//                _currentApplicationSettings = value;
//                BaseViewModel.OnStaticPropertyChanged("CurrentApplicationSettings");
//            }
//        }
//        //private static void LoadAllData()
//        //{
//        //    LoadAsycudaDocumentSet();

//        //    LoadDocument();

//        //    LoadItems();

//        //}

//        //private static async void LoadItems()
//        //{
//        //  await  db.xcuda_Item.Include(x => x.AsycudaDocumentSetPreviousEntries)
//        //                 .Include(x => x.AsycudaSalesAllocations)
//        //                 .Include(x => x.EntryDataDetails)
//        //                 .Include(x => x.xcuda_Goods_description)
//        //                 .Include(x => x.xcuda_Tarification)
//        //                 .Include(x => x.xcuda_Tarification.xcuda_HScode)
//        //                 .Include(x => x.xcuda_Tarification.xcuda_Supplementary_unit)
//        //                 .Include(x => x.xcuda_Valuation_item)
//        //                 .LoadAsync();
//        //}

//        //private static async void LoadDocument()
//        //{

//        //   await  db.xcuda_ASYCUDA.Include(x => x.AsycudaDocumentSetPreviousDocuments)
//        //                    .Include(x => x.xcuda_ASYCUDA_ExtendedProperties)
//        //                    .Include(x => x.xcuda_Declarant)
//        //                    .Include(x => x.xcuda_Identification)
//        //                    .Include(x => x.xcuda_Identification.xcuda_Office_segment)
//        //                    .Include(x => x.xcuda_Identification.xcuda_Registration)
//        //                    .Include(x => x.xcuda_Identification.xcuda_Type)
//        //                    .Include(x => x.xcuda_Property)
//        //                    .Include(x => x.xcuda_Valuation).LoadAsync();
//        //}






//        private static  void ResetDatabase()
//        {
           
//           // db = null;
//          //  db = new WaterNutDBEntities();
//        }

//        public static async Task SaveDatabase()
//        {
//            try
//            {
//                await Task.Run(() => { db.SaveChanges(); });
//            }
//            catch (OptimisticConcurrencyException oce)
//            {

//                db.Refresh(RefreshMode.StoreWins, oce.StateEntries.Select(e => e.Entity));
//                SaveDatabase();
//            }
//            catch (UpdateException ue)
//            {


//                foreach (var item in ue.StateEntries)
//                {
//                    db.DeleteObject(item.Entity);
//                    SaveDatabase();
//                }

//            }
//            catch (DbEntityValidationException e)
//            {
//                foreach (var eve in e.EntityValidationErrors)
//                {
//                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
//                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
//                    foreach (var ve in eve.ValidationErrors)
//                    {
//                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
//                            ve.PropertyName, ve.ErrorMessage);
//                    }
//                }
//                throw;
//            }
//            catch (Exception e)
//            {
//                ResetDatabase();
//            }
//        }

//        public static void SaveDatabase(WaterNutDBEntities mydb)
//        {
//            try
//            {
//                mydb.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
//            }
//            catch (OptimisticConcurrencyException oce)
//            {

//                mydb.Refresh(RefreshMode.ClientWins, oce.StateEntries.Select(e => e.EntitySet));
//                SaveDatabase(mydb);
//            }
//            catch (UpdateException ue)
//            {


//                foreach (var item in ue.StateEntries)
//                {
//                    mydb.DeleteObject(item.Entity);
//                    SaveDatabase(mydb);
//                }

//            }
//            catch (Exception e)
//            {
//                ResetDatabase();
//            }
//        }

       

       

//        #region INotifyPropertyChanged
//        internal static Boolean DoStaticEvents = true;
//        public static event PropertyChangedEventHandler staticPropertyChanged;
//        public static void OnStaticPropertyChanged(String info)
//        {

//            if (staticPropertyChanged != null && DoStaticEvents == true)
//            {
//                staticPropertyChanged(null, new PropertyChangedEventArgs(info));
//            }
//        }


//        public event PropertyChangedEventHandler PropertyChanged;
//        public void OnPropertyChanged(String info)
//        {
//            if (PropertyChanged != null)
//            {
//                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
//                //OnStaticPropertyChanged(info);
//            }
//        }
//        #endregion

//        public static T CopyEntity<T>(WaterNutDBEntities ctx, T entity, bool copyKeys = false) where T : EntityObject
//        {
//            if (Object.ReferenceEquals(entity, null))
//            {
//                return default(T);
//            }
//            T clone = ctx.CreateObject<T>();
//            PropertyInfo[] pis = entity.GetType().GetProperties();

//            foreach (PropertyInfo pi in pis)
//            {
//                EdmScalarPropertyAttribute[] attrs = (EdmScalarPropertyAttribute[])
//                              pi.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false);

//                foreach (EdmScalarPropertyAttribute attr in attrs)
//                {
//                    if (!copyKeys && attr.EntityKeyProperty)
//                        continue;
//                    try
//                    {
//                        pi.SetValue(clone, pi.GetValue(entity, null), null);
//                    }
//                    catch
//                    {
//                    }
//                }

//                EdmRelationshipNavigationPropertyAttribute[] Nattrs = (EdmRelationshipNavigationPropertyAttribute[])
//                              pi.GetCustomAttributes(typeof(EdmRelationshipNavigationPropertyAttribute), false);

//                foreach (EdmRelationshipNavigationPropertyAttribute attr in Nattrs)
//                {
                    
//                    try
//                    {
//                        pi.SetValue(clone, pi.GetValue(entity, null), null);
//                    }
//                    catch
//                    {
//                    }
//                }

//            }

//            return clone;
//        }

//        public static void UpdateEntity<T>(WaterNutDBEntities ctx, T cdoc, T ndoc, bool copyKeys = false) where T : EntityObject
//        {
//            if (Object.ReferenceEquals(cdoc, null))
//            {
//                return;
//            }
//           // T clone = ctx.CreateObject<T>();
//            PropertyInfo[] pis = cdoc.GetType().GetProperties();

//            foreach (PropertyInfo pi in pis)
//            {
//                EdmScalarPropertyAttribute[] attrs = (EdmScalarPropertyAttribute[])
//                              pi.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false);

//                foreach (EdmScalarPropertyAttribute attr in attrs)
//                {
//                    if (!copyKeys && attr.EntityKeyProperty)
//                        continue;

//                    pi.SetValue(ndoc, pi.GetValue(cdoc, null), null);
//                }
//            }

           
//        }


//        public static void CopyProperty<T>(WaterNutDBEntities ctx, T cdoc, T ndoc, string typ, string prp) where T: EntityObject
//        {
//            if (Object.ReferenceEquals(cdoc, null))
//            {
//                return; // default(T);
//            }
//           // T clone = ctx.CreateObject<T>();
//            PropertyInfo  pi = cdoc.GetType().GetProperty(typ);

           
//                    EdmRelationshipNavigationPropertyAttribute[] attrs = (EdmRelationshipNavigationPropertyAttribute[])
//                                  pi.GetCustomAttributes(typeof(EdmRelationshipNavigationPropertyAttribute), false);

//                    foreach (EdmRelationshipNavigationPropertyAttribute attr in attrs)
//                    {
//                        pi.SetValue(ndoc, pi.GetValue(cdoc, null), null);
//                    }
             
//        }


//        //internal static void UpdateProperty<T>(T cdoc, T ndoc,string typ, string propName) where T : EntityObject
//        //{
//        //    WaterNutDBEntities db = new WaterNutDBEntities(Properties.Settings.Default.WaterNutDBEntitiesConnection);
//        //    CopyProperty<T>(db, cdoc, ndoc, typ, propName);
//        //}

//        internal static void xcuda_ASYCUDA_PropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
            
//            //if (_currentAsycudaDocument == null) return;
//            //if (_currentAsycudaDocument.xcuda_ASYCUDA_ExtendedProperties != null && _currentAsycudaDocument.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId != null)
//            //{
//            //    if (_currentAsycudaDocument.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetReference.IsLoaded == false) _currentAsycudaDocument.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetReference.Load();

//            //    foreach (var item in _currentAsycudaDocument.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Where(xe => xe.ASYCUDA_Id != _currentAsycudaDocument.ASYCUDA_Id))
//            //    {
//            //        UpdateProperty<EntityObject>( _currentAsycudaDocument,item, sender.GetType().ToString(), e.PropertyName);
//            //    }
//            //}
//        }





//    }
//}
