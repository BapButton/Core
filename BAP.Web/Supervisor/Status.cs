namespace BAP.Web.Supervisor
{
    public class Container
    {
        public string status { get; set; }
        public string serviceName { get; set; }
        public int appId { get; set; }
        public int imageId { get; set; }
        public int serviceId { get; set; }
        public string containerId { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class Image
    {
        public string name { get; set; }
        public int appId { get; set; }
        public string serviceName { get; set; }
        public int imageId { get; set; }
        public string dockerImageId { get; set; }
        public string status { get; set; }
        public double? downloadProgress { get; set; }
    }

    public class BalenaStatus
    {
        public string status { get; set; }
        public string appState { get; set; }
        public double? overallDownloadProgress { get; set; }
        public List<Container> containers { get; set; }
        public List<Image> images { get; set; }
        public string release { get; set; }
    }


}
