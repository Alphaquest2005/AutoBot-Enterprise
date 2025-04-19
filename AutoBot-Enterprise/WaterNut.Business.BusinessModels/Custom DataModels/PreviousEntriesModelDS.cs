using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PreviousDocumentQS.Business.Entities;

namespace WaterNut.DataSpace
{
	public class PreviousDocumentItemsModel
	{

        private static readonly PreviousDocumentItemsModel instance;
        static PreviousDocumentItemsModel()
        {
            instance = new PreviousDocumentItemsModel();
        }

        public static PreviousDocumentItemsModel Instance
        {
            get { return instance; }
        }


        internal  void RemoveSelectedItems(List<PreviousDocumentItem> list)
        {
            throw new NotImplementedException();
        }

        internal  void SavePreviousDocumentItem(PreviousDocumentItem previousDocumentItem)
        {
            throw new NotImplementedException();
        }
    }
}