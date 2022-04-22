using System.Linq;
using EmailDownloader;

namespace WaterNut.DataSpace
{
    public class Utils
    {
        public static Client GetClient()
        {
            return new EmailDownloader.Client
            {
                CompanyName = BaseDataModel.Instance.CurrentApplicationSettings.CompanyName,
                DataFolder = BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                Password = BaseDataModel.Instance.CurrentApplicationSettings.EmailPassword,
                Email = BaseDataModel.Instance.CurrentApplicationSettings.Email,
                EmailMappings = BaseDataModel.Instance.CurrentApplicationSettings.EmailMapping.ToList()
            };
        }
    }
}