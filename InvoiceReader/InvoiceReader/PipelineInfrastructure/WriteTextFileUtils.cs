namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        public static string WriteTextFile(string file, string pdftxt)
        {
            var txtFile = file + ".txt";
            //if (File.Exists(txtFile)) return;
            File.WriteAllText(txtFile, pdftxt);
            return txtFile;
        }
    }
}