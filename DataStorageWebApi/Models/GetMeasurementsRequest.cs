namespace DataStorageWebApi.Models
{
    public class GetMeasurementsRequest
    {
        public required int DeviceID { get; set; }

        public required DateTime MinDate { get; set; }

        public required DateTime MaxDate { get; set; }
    }
}
