using System;
using CoreEntities.Business.Enums;
using DocumentDS.Business.Entities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public static Customs_Procedure GetCustomsProcedure(string dfp, string DocumentType)
    {
        Customs_Procedure customsProcedure;
        var isPaid = dfp == "Duty Paid";
        Func<Customs_Procedure, bool> dtpredicate = x => false;
        switch (DocumentType)
        {
            case "PO":
                var defaultCustomsOperation = BaseDataModel.GetDefaultCustomsOperation();
                dtpredicate = x =>
                {
                    return x.CustomsOperationId == defaultCustomsOperation
                           && x.Sales != true && x.Stock != true;
                };

                break;
            case "Sales":
                dtpredicate = x =>
                    x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Sales == true && x.IsPaid == isPaid;
                break;
            case "DIS":
                dtpredicate = x =>
                    x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Discrepancy == true &&
                    x.IsPaid == isPaid;
                break;
            case "ADJ":
                dtpredicate = x =>
                    x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Adjustment == true &&
                    x.IsPaid == isPaid;
                break;
            case "IM9":
                dtpredicate = x =>
                    x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Stock == true;
                break;
            default:
                throw new ApplicationException("Document Type");
        }


        return GetCustoms_Procedure(dtpredicate);
    }
}