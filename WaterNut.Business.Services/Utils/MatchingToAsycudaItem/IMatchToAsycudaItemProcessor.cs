using System.Collections.Generic;
using System;
using AdjustmentQS.Business.Entities;
using CoreEntities.Business.Entities;

namespace WaterNut.Business.Services.Utils.MatchingToAsycudaItem
{
    public interface IMatchToAsycudaItemProcessor
    {
        void Execute(AdjustmentDetail os, List<AsycudaDocumentItem> alst, EntryDataDetail ed);
    }
}