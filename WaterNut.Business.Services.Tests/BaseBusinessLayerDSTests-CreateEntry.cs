using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using InventoryDS.Business.Entities;
using NUnit.Framework;
// Assuming BaseBusinessLayerDS is here or in a sub-namespace
// Assuming CoreEntitiesContext and other interfaces are here
// Assuming AsycudaDocumentSet, EntryDataDetails etc. are here
// Required for mocking IQueryable extensions like ToListAsync

// Required for DbSet

namespace WaterNut.Business.Services.Tests
{
    [TestFixture]
    public class BaseBusinessLayerDSTests
    {
        private Mock<CoreEntitiesContext> _mockContext;
        // Add mocks for any other direct dependencies of BaseBusinessLayerDS if needed
        // private Mock<ISomeOtherService> _mockOtherService;

        private BaseBusinessLayerDS _sut; // System Under Test

        // Test Data
        private List<string> _testEntryDataList; // Input identifiers for GetSelectedPODetails
        private AsycudaDocumentSet _testAsycudaDocumentSet;
        private List<EntryDataDetails> _mockEntryDataDetailsResult; // Realistic mock data based on DB query
        private const int TestAsycudaDocumentSetId = 5428;

        // Mock DbSets - Adjust entity types as needed based on actual implementation
        private Mock<DbSet<EntryDataDetails>> _mockEntryDataDetailsDbSet;
        private Mock<DbSet<AsycudaDocument>> _mockAsycudaDocumentDbSet; // Example, if CreateEntryItems modifies AsycudaDocument
        private Mock<DbSet<InventoryItem>> _mockInventoryItemDbSet; // Example, if CreateEntryItems modifies InventoryItem

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<CoreEntitiesContext>();
            // _mockOtherService = new Mock<ISomeOtherService>();

            // Initialize Test Data
            _testEntryDataList = new List<string> { "KEY1", "KEY2", "KEY3", "KEY4", "KEY5", "KEY6", "KEY7", "KEY8", "KEY1" }; // Example keys

            _testAsycudaDocumentSet = new AsycudaDocumentSet { AsycudaDocumentSetId = TestAsycudaDocumentSetId /*, other properties if needed */ };

            // Realistic mock data based on the more diverse DB query results
            _mockEntryDataDetailsResult = new List<EntryDataDetails>
            {
                new EntryDataDetails {
                    EntryDataDetailsId = 1859007, EntryData_Id = 507371, LineNumber = 1, ItemNumber = "SILV-ADJ-350LB", Quantity = 2.0, Units = "",
                    ItemDescription = "eniors and Adults Weighing Up To 350 Pounds, Adjustable Height, Silver", Cost = 21.99m, TotalCost = 43.98m,
                    QtyAllocated = 0, UnitWeight = 0, DoNotAllocate = false, Freight = 0, Weight = 0, InternalFreight = 0, Status = "Active",
                    InvoiceQty = 0, ReceivedQty = 0, InventoryItemId = 69547, FileLineNumber = 1, VolumeLiters = 0, EntryDataDetailsKey = "|1",
                    TotalValue = 43.98, Category = "medical equipment", CategoryTariffCode = "90189090"
                 },
                new EntryDataDetails {
                    EntryDataDetailsId = 1859008, EntryData_Id = 507371, LineNumber = 2, ItemNumber = "MED-SHWR-CHAIR350", Quantity = 3.0, Units = "",
                    ItemDescription = "Medline Shower Chair with Backrest and Padded Armrests - 350 Ib. capacity, Bath", Cost = 19.99m, TotalCost = 59.97m,
                    QtyAllocated = 0, UnitWeight = 0, DoNotAllocate = false, Freight = 0, Weight = 0, InternalFreight = 0, Status = "Active",
                    InvoiceQty = 0, ReceivedQty = 0, InventoryItemId = 69548, FileLineNumber = 2, VolumeLiters = 0, EntryDataDetailsKey = "|2",
                    TotalValue = 59.97, Category = "medical equipment", CategoryTariffCode = "90189090"
                 },
                new EntryDataDetails {
                    EntryDataDetailsId = 1859009, EntryData_Id = 507371, LineNumber = 3, ItemNumber = "DRV-MED-11148-1", Quantity = 1.0, Units = "",
                    ItemDescription = "Drive Medical 11148-1 Folding Steel Bedside Commode Chair, Portable Toilet, Supports Individuals Weighing Up To 350 Lbs,", Cost = 24.00m, TotalCost = 24.00m,
                    QtyAllocated = 0, UnitWeight = 0, DoNotAllocate = false, Freight = 0, Weight = 0, InternalFreight = 0, Status = "Active",
                    InvoiceQty = 0, ReceivedQty = 0, InventoryItemId = 69549, FileLineNumber = 3, VolumeLiters = 0, EntryDataDetailsKey = "|3",
                    TotalValue = 24.00, Category = "medical equipment", CategoryTariffCode = "90189090"
                 },
                new EntryDataDetails {
                    EntryDataDetailsId = 1859010, EntryData_Id = 507371, LineNumber = 4, ItemNumber = "MP-WC-BLK-300-MED", Quantity = 1.0, Units = "",
                    ItemDescription = "Mobility Plus Wheelchair by Medline, Black Frame, Weighs 38.5lbs., Weight Capacity 300 lbs. Cup Holder & 2 Storage Bags", Cost = 55.19m, TotalCost = 55.19m,
                    QtyAllocated = 0, UnitWeight = 0, DoNotAllocate = false, Freight = 0, Weight = 0, InternalFreight = 0, Status = "Active",
                    InvoiceQty = 0, ReceivedQty = 0, InventoryItemId = 69550, FileLineNumber = 4, VolumeLiters = 0, EntryDataDetailsKey = "|4",
                    TotalValue = 55.19, Category = "medical equipment", CategoryTariffCode = "90189090"
                 },
                new EntryDataDetails { // Duplicate ItemNumber from ID 1859009
                    EntryDataDetailsId = 1859011, EntryData_Id = 507371, LineNumber = 5, ItemNumber = "DRV-MED-11148-1", Quantity = 1.0, Units = "",
                    ItemDescription = "Drive Medical 11148-1 Folding Steel Bedside Commode Chair, Portable Toilet, Supports Individuals Weighing Up To 350 Lbs,", Cost = 24.00m, TotalCost = 24.00m,
                    QtyAllocated = 0, UnitWeight = 0, DoNotAllocate = false, Freight = 0, Weight = 0, InternalFreight = 0, Status = "Active",
                    InvoiceQty = 0, ReceivedQty = 0, InventoryItemId = 69549, FileLineNumber = 5, VolumeLiters = 0, EntryDataDetailsKey = "|5",
                    TotalValue = 24.00, Category = "medical equipment", CategoryTariffCode = "90189090"
                 },
                 new EntryDataDetails { // Duplicate ItemNumber from ID 1859009
                    EntryDataDetailsId = 1859012, EntryData_Id = 507371, LineNumber = 6, ItemNumber = "DRV-MED-11148-1", Quantity = 1.0, Units = "",
                    ItemDescription = "Drive Medical 11148-1 Folding Steel Bedside Commode Chair, Portable Toilet, Supports Individuals Weighing Up To 350 Lbs,", Cost = 24.00m, TotalCost = 24.00m,
                    QtyAllocated = 0, UnitWeight = 0, DoNotAllocate = false, Freight = 0, Weight = 0, InternalFreight = 0, Status = "Active",
                    InvoiceQty = 0, ReceivedQty = 0, InventoryItemId = 69549, FileLineNumber = 6, VolumeLiters = 0, EntryDataDetailsKey = "|6",
                    TotalValue = 24.00, Category = "medical equipment", CategoryTariffCode = "90189090"
                 },
                 new EntryDataDetails { // Duplicate ItemNumber from ID 1859009
                    EntryDataDetailsId = 1859013, EntryData_Id = 507371, LineNumber = 7, ItemNumber = "DRV-MED-11148-1", Quantity = 1.0, Units = "",
                    ItemDescription = "Drive Medical 11148-1 Folding Steel Bedside Commode Chair, Portable Toilet, Supports Individuals Weighing Up To 350 Lbs,", Cost = 24.00m, TotalCost = 24.00m,
                    QtyAllocated = 0, UnitWeight = 0, DoNotAllocate = false, Freight = 0, Weight = 0, InternalFreight = 0, Status = "Active",
                    InvoiceQty = 0, ReceivedQty = 0, InventoryItemId = 69549, FileLineNumber = 7, VolumeLiters = 0, EntryDataDetailsKey = "|7",
                    TotalValue = 24.00, Category = "medical equipment", CategoryTariffCode = "90189090"
                 },
                 new EntryDataDetails { // Different Category
                    EntryDataDetailsId = 1859014, EntryData_Id = 507371, LineNumber = 8, ItemNumber = "ELEG-PAPER-PILLAR", Quantity = 1.0, Units = "",
                    ItemDescription = "Elegant Folding Column Decorations, Paper Roman Pillar Accents for Pathway and Birthday Party Welcome, Round, Welcome", Cost = 12.99m, TotalCost = 12.99m,
                    QtyAllocated = 0, UnitWeight = 0, DoNotAllocate = false, Freight = 0, Weight = 0, InternalFreight = 0, Status = "Active",
                    InvoiceQty = 0, ReceivedQty = 0, InventoryItemId = 69551, FileLineNumber = 8, VolumeLiters = 0, EntryDataDetailsKey = "|8",
                    TotalValue = 12.99, Category = "party decorations", CategoryTariffCode = "95059000"
                 }
            };

            // Setup Mock DbSets
            _mockEntryDataDetailsDbSet = _mockEntryDataDetailsResult.AsQueryable().BuildMockDbSet();
            _mockAsycudaDocumentDbSet = new List<AsycudaDocument>().AsQueryable().BuildMockDbSet();
            _mockInventoryItemDbSet = new List<InventoryItem>().AsQueryable().BuildMockDbSet();


            // Configure Mock Context to return Mock DbSets
            _mockContext.Setup(c => c.EntryDataDetails).Returns(_mockEntryDataDetailsDbSet.Object);
            _mockContext.Setup(c => c.AsycudaDocuments).Returns(_mockAsycudaDocumentDbSet.Object);
            _mockContext.Setup(c => c.InventoryItems).Returns(_mockInventoryItemDbSet.Object);

            // --- Mocking Dependencies for GetSelectedPODetails ---
             _mockContext.Setup(c => c.EntryDataDetails.Where(It.IsAny<Expression<Func<EntryDataDetails, bool>>>()))
                         .Returns((Expression<Func<EntryDataDetails, bool>> predicate) =>
                         {
                             return _mockEntryDataDetailsResult.Where(predicate.Compile()).AsQueryable().BuildMockDbSet().Object;
                         });
             _mockContext.Setup(c => c.EntryDataDetails.ToListAsync(It.IsAny<System.Threading.CancellationToken>()))
                         .ReturnsAsync((Expression<Func<EntryDataDetails, bool>> predicate) =>
                         {
                             // This mock assumes GetSelectedPODetails might call ToListAsync directly on the DbSet
                             // or after a Where clause. Adjust if the pattern is different.
                             if (predicate != null)
                             {
                                 return _mockEntryDataDetailsResult.Where(predicate.Compile()).ToList();
                             }
                             return _mockEntryDataDetailsResult; // Return all if no predicate
                         });


            // --- Mocking Dependencies for CreateEntryItems ---
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(1);
            _mockContext.Setup(c => c.Set<AsycudaDocument>().Add(It.IsAny<AsycudaDocument>()));
            _mockContext.Setup(c => c.Set<InventoryItem>().Add(It.IsAny<InventoryItem>()));


            // --- Instantiate SUT ---
            _sut = new BaseBusinessLayerDS(_mockContext.Object);

        }

        // Test Case: All boolean flags false
        [Test]
        public async Task AddToEntry_AllParamsFalse_Test()
        {
            // Arrange
            bool perInvoice = false;
            bool combineEntryDataInSameFile = false;
            bool groupItems = false;
            bool checkPackages = false;

            // Act
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

            // Assert
             _mockContext.Verify(c => c.EntryDataDetails.Where(It.IsAny<Expression<Func<EntryDataDetails, bool>>>()), Times.AtLeastOnce);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.AtLeastOnce);
        }

        // Test Case: perInvoice = true, others false
        [Test]
        public async Task AddToEntry_PerInvoiceTrue_CombineFalse_GroupFalse_CheckFalse_Test()
        {
            // Arrange
            bool perInvoice = true;
            bool combineEntryDataInSameFile = false;
            bool groupItems = false;
            bool checkPackages = false;

            // Act
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

            // Assert
             _mockContext.Verify(c => c.EntryDataDetails.Where(It.IsAny<Expression<Func<EntryDataDetails, bool>>>()), Times.AtLeastOnce);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.AtLeastOnce);
        }

        // Test Case: combineEntryDataInSameFile = true, others false
        [Test]
        public async Task AddToEntry_PerInvoiceFalse_CombineTrue_GroupFalse_CheckFalse_Test()
        {
            // Arrange
            bool perInvoice = false;
            bool combineEntryDataInSameFile = true;
            bool groupItems = false;
            bool checkPackages = false;

            // Act
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

            // Assert
             _mockContext.Verify(c => c.EntryDataDetails.Where(It.IsAny<Expression<Func<EntryDataDetails, bool>>>()), Times.AtLeastOnce);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.AtLeastOnce);
        }

        // Test Case: groupItems = true, others false
        [Test]
        public async Task AddToEntry_PerInvoiceFalse_CombineFalse_GroupTrue_CheckFalse_Test()
        {
            // Arrange
            bool perInvoice = false;
            bool combineEntryDataInSameFile = false;
            bool groupItems = true;
            bool checkPackages = false;

            // Act
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

            // Assert
             _mockContext.Verify(c => c.EntryDataDetails.Where(It.IsAny<Expression<Func<EntryDataDetails, bool>>>()), Times.AtLeastOnce);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.AtLeastOnce);
        }

        // Test Case: checkPackages = true, others false
        [Test]
        public async Task AddToEntry_PerInvoiceFalse_CombineFalse_GroupFalse_CheckTrue_Test()
        {
            // Arrange
            bool perInvoice = false;
            bool combineEntryDataInSameFile = false;
            bool groupItems = false;
            bool checkPackages = true;

            // Act
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

            // Assert
             _mockContext.Verify(c => c.EntryDataDetails.Where(It.IsAny<Expression<Func<EntryDataDetails, bool>>>()), Times.AtLeastOnce);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.AtLeastOnce);
        }

        // Test Case: All boolean flags true
        [Test]
        public async Task AddToEntry_AllParamsTrue_Test()
        {
            // Arrange
            bool perInvoice = true;
            bool combineEntryDataInSameFile = true;
            bool groupItems = true;
            bool checkPackages = true;

            // Act
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

            // Assert
             _mockContext.Verify(c => c.EntryDataDetails.Where(It.IsAny<Expression<Func<EntryDataDetails, bool>>>()), Times.AtLeastOnce);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.AtLeastOnce);
        }
    }

   
}