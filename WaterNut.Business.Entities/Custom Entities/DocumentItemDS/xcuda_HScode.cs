
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using WaterNut.Interfaces;

namespace DocumentItemDS.Business.Entities
{
    public partial class xcuda_HScode
    {
        //partial void AutoGenStartUp()
        //{
        //    PropertyChanged += xcuda_HScode_PropertyChanged;
        //}

        //private void xcuda_HScode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "Precision_4")
        //    {
        //        using (var ctx = new InventoryItemService())
        //        {

        //            InventoryItems = new InventoryItem(ctx.GetInventoryItemByKey(Precision_4).Result);
        //        }
        //    }
        //}

        IInventoryItem _InventoryItems = null;
        [IgnoreDataMember]
        [NotMapped]
        public IInventoryItem InventoryItems
        {
            get
            {
                return _InventoryItems;
            }
            set
            {
                if (value != null)
                {
                    _InventoryItems = value;

                    Precision_4 = _InventoryItems.ItemNumber;

                   
                }
            }

        }
    }

}
