namespace WaterNut.Business.Services.Importers
{
    public interface IImporter
    {
        void Import(string fileName, bool overWrite);
    }
}