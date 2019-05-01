using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using QBPOSFC3Lib;
using DocumentDS.Business.Entities;
using Core.Common.UI;

namespace QuickBooks
{
    public class QBPOS
    {
       
        public static async Task DownloadQbData(DateTime startDate, DateTime endDate, AsycudaDocumentSet currentAsycudaDocumentSet,bool ImportSales, bool ImportInventory)
        {
            StatusModel.Timer("Connecting to QuickBooks");

            var sessionBegun = false;
            var connectionOpen = false;
            QBPOSSessionManager sessionManager = null;

            //try
            //{
            // get qbpos filename
            var qbposfile = "";

            //Create the session Manager object
            sessionManager = new QBPOSSessionManager();
            sessionManager.OpenConnection("1", "Insight's Asycuda Toolkit");
            short majorVersion;
            short minorVersion;
            ENReleaseLevel releaseLevel;
            short releaseNumber;

            // sessionManager.GetVersion(out majorVersion, out minorVersion, out releaseLevel, out releaseNumber);



            connectionOpen = true;
            sessionManager.BeginSession(qbposfile);
            sessionBegun = true;

            try
            {
                if (ImportInventory)
                {
                    var ItemInventoryRequestMsgSet = sessionManager.CreateMsgSetRequest(3, 0);
                    ItemInventoryRequestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                    var inventoryVM = new ItemInventoryViewModel();
                    int increment = 1000;
                    int toItemNumber = 0;
                    int fromItemNumber = 0;
                    string errstring = null;

                    while (errstring == null)
                    {

                        toItemNumber += increment;
                        ItemInventoryRequestMsgSet.ClearRequests();
                        inventoryVM.BuildItemInventoryQueryRq(ItemInventoryRequestMsgSet, fromItemNumber, toItemNumber);
                        fromItemNumber = toItemNumber;
                        ItemInventoryRequestMsgSet.Verify(out errstring);

                        IMsgSetResponse ItemInventoryResponseMsgSet = null;
                        ItemInventoryResponseMsgSet = sessionManager.DoRequests(ItemInventoryRequestMsgSet);
                        var responseStatus = new Tuple<string>(null);
                        if (ItemInventoryResponseMsgSet != null)
                         responseStatus =    await inventoryVM.WalkItemInventoryQueryRs(ItemInventoryResponseMsgSet).ConfigureAwait(false);

                        if (errstring != null || responseStatus.Item1 == "0") break;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
          

           
            

          

            if (ImportSales)
            {
                StatusModel.Timer("Getting Data Request");
                var SalesReceiptRequestMsgSet = sessionManager.CreateMsgSetRequest(3, 0);
                SalesReceiptRequestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;
                var SalesReceiptVM = new SalesReceiptViewModel();
                SalesReceiptVM.BuildSalesReceiptQueryRq(SalesReceiptRequestMsgSet, startDate, endDate);
                IMsgSetResponse SalesReceiptResponseMsgSet = null;

                SalesReceiptResponseMsgSet = sessionManager.DoRequests(SalesReceiptRequestMsgSet);

                if (SalesReceiptResponseMsgSet != null)
                    await SalesReceiptVM.WalkSalesReceiptQueryRs(SalesReceiptResponseMsgSet, currentAsycudaDocumentSet).ConfigureAwait(false);


            }
           

            
            


            //End the session and close the connection to QuickBooks
            sessionManager.EndSession();
            sessionBegun = false;
            sessionManager.CloseConnection();
            connectionOpen = false;

            

            StatusModel.StopStatusUpdate();
        }

    }
}
