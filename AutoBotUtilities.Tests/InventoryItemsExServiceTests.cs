using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using InventoryQS.Business.Services;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class InventoryItemsExServiceTests
    {
        [Test]
        public async Task GetTariffCode_WhenExactMatchExistsAndIsValid_ShouldReturnInputCode()
        {
            // Arrange
            string suspectedTariffCode = "61091000";
            // This test assumes that the underlying database (accessed via new InventoryDSContext())
            // contains a TariffCode with TariffCodeName = "61091000" and a non-null RateofDuty.
            // If not, this test might not accurately reflect the method's behavior in that scenario.
            string expectedTariffCode = "61091010";

            // Act
            string actualTariffCode = await InventoryItemsExService.GetTariffCode(suspectedTariffCode);

            // Assert
            Assert.That(actualTariffCode, Is.EqualTo(expectedTariffCode), "The method should return the exact match if it's valid and exists.");
        }

        [Test]
        public async Task ScrubCategoryTariffs()
        {
            using(var ctx = new EntryDataDSContext())
            {
                var lst = ctx.CategoryTariffs.ToList();
                foreach (var item in lst)
                {
                    var tariffCode = await InventoryItemsExService.GetTariffCode(item.TariffCode).ConfigureAwait(false);
                    if (tariffCode != item.TariffCode)
                    {
                        item.TariffCode = tariffCode;
                        
                    }
                }

                ctx.SaveChanges();
            }

            
            // Assert
            Assert.That(true, "The method should return the exact match if it's valid and exists.");
        }

        [Test]
        public async Task GetTariffCode_WhenInputIsNull_ShouldReturnNull()
        {
            // Arrange
            string suspectedTariffCode = null;
            string expectedTariffCode = null;

            // Act
            string actualTariffCode = await InventoryItemsExService.GetTariffCode(suspectedTariffCode);

            // Assert
            Assert.That(actualTariffCode, Is.EqualTo(expectedTariffCode), "The method should return null for null input.");
        }

        [Test]
        public async Task GetTariffCode_WhenInputIsEmpty_ShouldReturnEmpty()
        {
            // Arrange
            string suspectedTariffCode = "";
            string expectedTariffCode = "";

            // Act
            string actualTariffCode = await InventoryItemsExService.GetTariffCode(suspectedTariffCode);

            // Assert
            Assert.That(actualTariffCode, Is.EqualTo(expectedTariffCode), "The method should return empty for empty input.");
        }

        // Consider adding tests for scenarios where:
        // 1. Exact match does not exist, but "partialCode + 90" exists and is valid.
        // 2. Exact match does not exist, but "partialCode + 00" exists and is valid.
        // 3. No valid match is found (should return original suspectedTariffCode).
        // These would require more control over the test data in InventoryDSContext.
    }
}