using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;


namespace WaterNut.DataLayer
{
    public enum DocumentSetApportionMethod
        {
            ByValue,
            Equal
        }

    public partial class AsycudaDocumentSet : IHasEntryTimeStamp
    {



        public AsycudaDocumentSet()
        {
            xcuda_ASYCUDA_ExtendedProperties.AssociationChanged += xcuda_ASYCUDA_ExtendedProperties_AssociationChanged;

        }

        private void xcuda_ASYCUDA_ExtendedProperties_AssociationChanged(object sender, CollectionChangeEventArgs e)
        {
            if (xcuda_ASYCUDA_ExtendedProperties != null)
            {
                SetFileNumber();
                OnPropertyChanged("AsycudaDocuments");
            }
        }


        public void SetFileNumber()
        {
            var xlist = xcuda_ASYCUDA_ExtendedProperties.Where(xe => xe.xcuda_ASYCUDA != null);
            var seed = Convert.ToInt32(StartingFileCount);
            for (var i = 0; i < xlist.Count(); i++)
            {
                var xe = xlist.ElementAt(i);
                xe.FileNumber = seed + i + 1;
                //if (xe.xcuda_ASYCUDAReference.IsLoaded == false)
                //{
                //    xe.xcuda_ASYCUDAReference.Load();
                //}
                //if (xe.xcuda_ASYCUDA.xcuda_DeclarantReference.IsLoaded == false && xe.xcuda_ASYCUDA.EntityState != System.Data.EntityState.Added)
                //    xe.xcuda_ASYCUDA.xcuda_DeclarantReference.Load();

                //if (xe.xcuda_ASYCUDA.xcuda_Declarant != null)
                //    xe.xcuda_ASYCUDA.xcuda_Declarant.Number = xe.xcuda_ASYCUDA.xcuda_Declarant.Number.Substring(0,xe.xcuda_ASYCUDA.xcuda_Declarant.Number.Length - 3) + "-F" + xe.FileNumber.ToString();

            }
        }

        public ObservableCollection<xcuda_ASYCUDA> Documents
        {
            get
            {
                var alist = from a in xcuda_ASYCUDA_ExtendedProperties.Where(x => x.xcuda_ASYCUDA != null)
                    select a.xcuda_ASYCUDA;
                return new ObservableCollection<xcuda_ASYCUDA>(alist);
            }
        }


    }
}
