using System;
using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace
{
    public class CancelledEntriesImporter
    {
        static CancelledEntriesImporter()
        {
        }

        public CancelledEntriesImporter()
        {
        }

        public void Process(DataFile dataFile)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.ExecuteSqlCommand("delete from CancelledEntriesLst");
                    foreach (var itm in dataFile.Data)
                    {
                        var expireditm = new CancelledEntriesLst(true)
                        {
                            Office = itm.Office,
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            RegistrationDate = itm.RegistrationDate,
                            RegistrationNumber = itm.RegistrationNumber,
                            TrackingState = TrackingState.Added
                        };
                        ctx.CancelledEntriesLst.Add(expireditm);
                    }

                    ctx.SaveChanges();
                    ctx.Database.ExecuteSqlCommand($@"UPDATE xcuda_ASYCUDA_ExtendedProperties
                                                        SET         Cancelled = 1
                                                        FROM    (SELECT AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code
                                                                            FROM     CancelledEntriesLst INNER JOIN
                                                                                            AsycudaDocument ON CancelledEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND CancelledEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate AND 
                                                                                            CancelledEntriesLst.RegistrationNumber = AsycudaDocument.CNumber ) AS exp INNER JOIN
                                                                            xcuda_ASYCUDA_ExtendedProperties ON exp.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}