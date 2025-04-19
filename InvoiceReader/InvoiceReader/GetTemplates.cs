using System.Data.Entity;
using System.Linq.Expressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class InvoiceReader
{
    public static List<Invoice> GetTemplates(Expression<Func<Invoices, bool>> filter)
    {
        List<Invoice> templates;
        using (var ctx = new OCRContext())
        {
            templates = ctx.Invoices
                .Include(x => x.Parts)
                .Include("InvoiceIdentificatonRegEx.OCR_RegularExpressions")
                .Include("RegEx.RegEx")
                .Include("RegEx.ReplacementRegEx")
                .Include("Parts.RecuringPart")
                .Include("Parts.Start.RegularExpressions")
                .Include("Parts.End.RegularExpressions")
                .Include("Parts.PartTypes")
                .Include("Parts.ChildParts.ChildPart.Start.RegularExpressions")
                .Include("Parts.ParentParts.ParentPart.Start.RegularExpressions")
                .Include("Parts.Lines.RegularExpressions")
                .Include("Parts.Lines.Fields.FieldValue")
                .Include("Parts.Lines.Fields.FormatRegEx.RegEx")
                .Include("Parts.Lines.Fields.FormatRegEx.ReplacementRegEx")
                .Include("Parts.Lines.Fields.ChildFields.FieldValue")
                .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.RegEx")
                .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.ReplacementRegEx")
                .Where(x => x.IsActive)
                .Where(x => x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .Where(filter) //BaseDataModel.Instance.CurrentApplicationSettings.TestMode != true ||
                .ToList()
                .Select(x => new Invoice(x)).ToList();
        }

        return templates;
    }
}