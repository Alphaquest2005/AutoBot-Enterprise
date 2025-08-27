namespace WaterNut.DataSpace
{
    public partial class Template
    {
        private static int GetSectionPrecedence(string sectionName)
        {
            switch (sectionName)
            {
                case "Single": return 1;
                case "Ripped": return 2;
                case "Sparse": return 3;
                default: return 99;
            }
        }
    }
}