using System.Configuration;

namespace Turkok.Core.Settings
{
    public class ApplicationSettings : IApplicationSettings
    {  
        public bool EnableCaching
        {
            get
            {
                bool enableCaching;
                if (bool.TryParse(ConfigurationManager.AppSettings["EnableCaching"], out enableCaching))
                    return enableCaching;

                return true;
            }
        }

        public int IndexPageSize
        {
            get
            {
                int indexPageSize;
                if (int.TryParse(ConfigurationManager.AppSettings["FrontEndPageSize"], out indexPageSize))
                    return indexPageSize;

                return 5;
            }
        }

        public int PageSize
        {
            get
            {
                int pagesize;
                if (int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pagesize))
                    return pagesize;

                return 5;
            }
        }

        public string AnnouncementServerPath
        {
            get
            { 
                return ConfigurationManager.AppSettings["AnnouncementServerPath"]; 
            }
        }
        public string AnnouncementAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["AnnouncementServerPath"];
            }
        }

        public string DocumentsServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DocumentsServerPath"];
            }
        }
        public string DocumentsAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DocumentsAccessPath"];
            }
        }

        public string ProjectServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectServerPath"];
            }
        }
        public string ProjectAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectAccessPath"];
            }
        }
        public string TaskServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["TaskServerPath"];
            }
        }
        public string TaskAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["TaskAccessPath"];
            }
        }
        public string MeetingServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MeetingServerPath"];
            }
        }
        public string MeetingAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MeetingAccessPath"];
            }
        }
    }
}
