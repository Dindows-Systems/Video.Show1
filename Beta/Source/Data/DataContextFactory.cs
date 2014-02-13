using System.Configuration;

namespace VideoShow.Data
{
    public class DataContextFactory
    {
        public static VideoShowDataContext DataContext()
        {
            return new VideoShowDataContext(ConfigurationManager.ConnectionStrings["VideoShowConnectionString"].ConnectionString);
        }
        public static VideoShowDataContext DataContext(string connectionString)
        {
            return new VideoShowDataContext(connectionString);
        }
    }
}