﻿using System;
using System.Collections.Generic;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace
{
    public class ExpiredEntriesImporter
    {
        static ExpiredEntriesImporter()
        {
        }

        public ExpiredEntriesImporter()
        {
        }

        public void ProcessCsvExpiredEntries(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<dynamic> eslst)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.ExecuteSqlCommand("delete from ExpiredEntriesLst");
                    foreach (var itm in eslst)
                    {
                        var expireditm = new ExpiredEntriesLst(true)
                        {
                            Office = itm.Office,
                            GeneralProcedure = itm.GeneralProcedure,
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            AssessmentDate = itm.AssessmentDate,
                            AssessmentNumber = itm.AssessmentNumber,
                            AssessmentSerial = itm.AssessmentSerial,
                            RegistrationDate = itm.RegistrationDate,
                            RegistrationNumber = itm.RegistrationNumber,
                            RegistrationSerial = itm.RegistrationSerial,
                            Consignee = itm.Consignee,
                            Exporter = itm.Exporter,
                            DeclarantCode = itm.DeclarantCode,
                            DeclarantReference = itm.DeclarantReference,
                            Expiration = itm.Expiration,
                            TrackingState = TrackingState.Added
                        };
                        ctx.ExpiredEntriesLst.Add(expireditm);
                    }

                    ctx.SaveChanges();
                    ctx.Database.ExecuteSqlCommand($@"UPDATE xcuda_ASYCUDA_ExtendedProperties
                                                    SET         EffectiveExpiryDate = exp.Expiration
                                                    FROM    (SELECT AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code, 
                                                                                      CAST(ExpiredEntriesLst.Expiration AS datetime) AS Expiration
                                                                     FROM     ExpiredEntriesLst INNER JOIN
                                                                                      AsycudaDocument ON ExpiredEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND ExpiredEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate AND 
                                                                                      ExpiredEntriesLst.RegistrationNumber = AsycudaDocument.CNumber AND ExpiredEntriesLst.DeclarantReference = AsycudaDocument.ReferenceNumber) AS exp INNER JOIN
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