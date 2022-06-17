using System.Threading.Tasks;

namespace WaterNut.DataSpace
{
    public interface IRawDataExtractor
    {
        Task Extract(RawDataFile rawDataFile);
    }
}