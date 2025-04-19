using System;
using System.Linq;
using CoreEntities.Business.Entities;

namespace WaterNut.Business.Services.Importers
{
    public static class ValidateFileType
    {
        public static void Execute(FileTypes fileType)
        {
            DuplicateOrginalNameColumnCheck(fileType);
        }

        private static void DuplicateOrginalNameColumnCheck(FileTypes fileType)
        {
            var duplicateColumns = fileType.FileTypeMappings.GroupBy(x => x.OriginalName).Where(x => x.Count() > 1)
                .Select(x => x.Key).ToList();
            if (duplicateColumns.Any())
                throw new ApplicationException(
                    $"Duplicate Columns found for FileTypeId:{fileType.Id} - Columns{duplicateColumns.Aggregate((o, n) => $"{o},{n}")}");
        }
    }
}