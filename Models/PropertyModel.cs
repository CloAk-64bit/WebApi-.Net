using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PropertyApi.Models
{
    [DataContract]
    public class PropertyModel
    {

        public string PropertyId { get; set; }

        public string PropertyTitle { get; set; }

        public string PropertyDesc { get; set; }

        public string Price { get; set; }

        public string totalBedRooms { get; set; }

        public string totalBathRooms { get; set; }

        public string ErfSize { get; set; }

        public string totalParkingSpace { get; set; }

        public string PropertyLink { get; set; }

        public PropertyModel(string propertyId, string propertyTitle, string propertyLink, string propertyDesc,
            string propertyPrice, string totalBedRooms, string totalBathRooms,
            string erfSize, string totalParkingSpace)
        {
            PropertyId = propertyId;
            PropertyTitle = propertyTitle;
            PropertyDesc = propertyDesc;
            Price = propertyPrice;
            this.totalBedRooms = totalBedRooms;
            this.totalBathRooms = totalBathRooms;
            ErfSize = erfSize;
            this.totalParkingSpace = totalParkingSpace;
            PropertyLink = propertyLink;
        }

      
    }
}
