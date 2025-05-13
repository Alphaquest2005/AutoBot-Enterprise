namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        public const string CommandsTxt = "Commands:\r\n" +
                                          "UpdateRegex: RegId: 000, Regex: 'xyz', IsMultiline: True\r\n" +
                                          "AddFieldRegEx: RegId: 000,  Field: Name, Regex: '', ReplaceRegex: ''\r\n" +
                                          "RequestInvoice: Name:'xyz'\r\n" +
                                          "AddInvoice: Name:'', IDRegex:''\r\n" +
                                          "AddPart: Template:'', Name: '', StartRegex: '', ParentPart:'', IsRecurring: True, IsComposite: False, IsMultiLine: True \r\n" +
                                          "AddLine: Template:'',  Part: '', Name: '', Regex: ''\r\n" +
                                          "UpdateLine: Template:'',  Part: '', Name: '', Regex: ''\r\n" +
                                          "AddFieldFormatRegex: RegexId: 000, Keyword:'', Regex:'', ReplaceRegex:'', ReplacementRegexIsMultiLine: True, RegexIsMultiLine: True\r\n";
    }


}