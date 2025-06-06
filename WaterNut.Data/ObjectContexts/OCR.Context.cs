﻿// <autogenerated>
//   This file was generated by T4 code generator AllObjectContext.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

            


using System.Data.Entity;
using CoreEntities.Business.Entities;
using System.Data.Entity.Infrastructure;
using OCR.Business.Entities.Mapping;
using WaterNut.Data;
using System.Data.Entity.Core.Objects;



namespace OCR.Business.Entities
{
    [DbConfigurationType(typeof(DBConfiguration))] 
    public partial class OCRContext : DbContext
    {
        static OCRContext()
        {
            var x = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
            Database.SetInitializer<OCRContext>(null);
        }

        public OCRContext()
            : base("Name=OCR")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
               // Get the ObjectContext related to this DbContext
            var objectContext = (this as IObjectContextAdapter).ObjectContext;

            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = 120;

            objectContext.ObjectMaterialized += ObjectContext_OnObjectMaterialized;
        }
        
        public bool StartTracking { get; set; }

        private void ObjectContext_OnObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            if (StartTracking == true) ((dynamic)e.Entity).StartTracking();
        }

        public DbSet<End> End { get; set; }
     
        public DbSet<Invoices> Invoices { get; set; }
     
        public DbSet<Parts> Parts { get; set; }
     
        public DbSet<PartTypes> PartTypes { get; set; }
     
        public DbSet<RegularExpressions> RegularExpressions { get; set; }
     
        public DbSet<Start> Start { get; set; }
     
        public DbSet<Lines> Lines { get; set; }
     
        public DbSet<RecuringPart> RecuringPart { get; set; }
     
        public DbSet<ChildParts> ChildParts { get; set; }
     
        public DbSet<OCR_FieldValue> OCR_FieldValue { get; set; }
     
        public DbSet<InvoiceRegEx> OCR_InvoiceRegEx { get; set; }
     
        public DbSet<FieldFormatRegEx> OCR_FieldFormatRegEx { get; set; }
     
        public DbSet<InvoiceIdentificatonRegEx> InvoiceIdentificatonRegEx { get; set; }
     
        public DbSet<ImportErrors> ImportErrors { get; set; }
     
        public DbSet<OCR_FailedFields> OCR_FailedFields { get; set; }
     
        public DbSet<OCR_FailedLines> OCR_FailedLines { get; set; }
     
        public DbSet<OCR_FieldMappings> OCR_FieldMappings { get; set; }
     
        public DbSet<OCR_PartLineFields> OCR_PartLineFields { get; set; }
     
        public DbSet<Fields> Fields { get; set; }
     


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new EndMap());
         
            modelBuilder.Configurations.Add(new InvoicesMap());
         
            modelBuilder.Configurations.Add(new PartsMap());
         
            modelBuilder.Configurations.Add(new PartTypesMap());
         
            modelBuilder.Configurations.Add(new RegularExpressionsMap());
         
            modelBuilder.Configurations.Add(new StartMap());
         
            modelBuilder.Configurations.Add(new LinesMap());
         
            modelBuilder.Configurations.Add(new RecuringPartMap());
         
            modelBuilder.Configurations.Add(new ChildPartsMap());
         
            modelBuilder.Configurations.Add(new OCR_FieldValueMap());
         
            modelBuilder.Configurations.Add(new InvoiceRegExMap());
         
            modelBuilder.Configurations.Add(new FieldFormatRegExMap());
         
            modelBuilder.Configurations.Add(new InvoiceIdentificatonRegExMap());
         
            modelBuilder.Configurations.Add(new ImportErrorsMap());
         
            modelBuilder.Configurations.Add(new OCR_FailedFieldsMap());
         
            modelBuilder.Configurations.Add(new OCR_FailedLinesMap());
         
            modelBuilder.Configurations.Add(new OCR_FieldMappingsMap());
         
            modelBuilder.Configurations.Add(new OCR_PartLineFieldsMap());
         
            modelBuilder.Configurations.Add(new FieldsMap());
         
			OnModelCreatingExtentsion(modelBuilder);

        }
		partial void OnModelCreatingExtentsion(DbModelBuilder modelBuilder);
    }
}

 	
