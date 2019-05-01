using System;
using System.Collections.Generic;
using CoreEntities.Business.Entities;



namespace WaterNut.DataSpace
{
    public class AsycudaEntrySummaryListModel
    {
        private static readonly AsycudaEntrySummaryListModel instance;
        static AsycudaEntrySummaryListModel()
        {
            instance = new AsycudaEntrySummaryListModel();
        }

        public static AsycudaEntrySummaryListModel Instance
        {
            get { return instance; }
        }

        internal void RemoveSelectedItems(List<AsycudaDocumentItem> list)
        {
            throw new NotImplementedException();
            foreach (var item in list)
            {
                
            }
        }


    }
}