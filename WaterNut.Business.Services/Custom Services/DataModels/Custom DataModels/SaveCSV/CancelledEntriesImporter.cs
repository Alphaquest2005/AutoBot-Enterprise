using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<bool> Process(DataFile dataFile)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext())
                {
                    await ctx.Database.ExecuteSqlCommandAsync("delete from CancelledEntriesLst").ConfigureAwait(false);
                    foreach (var expireditm in dataFile.Data.Select(itm => new CancelledEntriesLst(true)
                             {
                                 Office = itm.Office,
                                 ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                                 RegistrationDate = itm.RegistrationDate,
                                 RegistrationNumber = itm.RegistrationNumber,
                                 TrackingState = TrackingState.Added
                             }))
                    {
                        ctx.CancelledEntriesLst.Add(expireditm);
                    }

                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                    await ctx.Database.ExecuteSqlCommandAsync($@"UPDATE xcuda_ASYCUDA_ExtendedProperties
                                                        SET         Cancelled = 1
                                                        FROM    (SELECT AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code
                                                                            FROM     CancelledEntriesLst INNER JOIN
                                                                                            AsycudaDocument ON CancelledEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND CancelledEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate AND 
                                                                                            CancelledEntriesLst.RegistrationNumber = AsycudaDocument.CNumber ) AS exp INNER JOIN
                                                                            xcuda_ASYCUDA_ExtendedProperties ON exp.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id").ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
                //throw;
            }
        }
    }
}