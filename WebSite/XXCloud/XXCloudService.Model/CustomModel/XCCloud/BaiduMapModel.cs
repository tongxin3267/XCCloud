using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{

    [DataContract]
    public class CoordinateAnalysisModel
    {
        [DataMember(Name = "status", Order = 1)]
        public int Status { set; get; }

        [DataMember(Name = "result", Order = 2)]
        public AnalysisResult Result { set; get; } 
    }

    [DataContract]
    public class AnalysisResult
    {
        [DataMember(Name = "location", Order = 1)]
        public Location Location { set; get; }

        [DataMember(Name = "addressComponent", Order = 2)]
        public AddressComponent AddressComponent { set; get; }

        [DataMember(Name = "formatted_address", Order = 3)]
        public string Formatted_Address {set;get;}

        [DataMember(Name = "business", Order = 4)]
        public string Business {set;get;}
    }

    [DataContract]
    public class AddressComponent
    { 
        [DataMember(Name = "country", Order = 1)]
        public string Country {set;get;}

        [DataMember(Name = "country_code", Order = 2)]
        public string Country_Code {set;get;}

        [DataMember(Name = "province", Order = 3)]
        public string Province {set;get;}

        [DataMember(Name = "city", Order = 4)]
        public string City {set;get;}

        [DataMember(Name = "district", Order = 5)]
        public string District {set;get;}

        [DataMember(Name = "town", Order = 6)]
        public string Town {set;get;}

        [DataMember(Name = "adcode", Order = 7)]
        public string Adcode {set;get;}

        [DataMember(Name = "street", Order = 8)]
        public string Street {set;get;}

        [DataMember(Name = "street_number", Order = 9)]
        public string Street_Number {set;get;}

        [DataMember(Name = "direction", Order = 10)]
        public string Direction {set;get;}

        [DataMember(Name = "distance", Order = 11)]
        public string Distance {set;get;}
    }

    [DataContract]
    public class Location
    { 
        [DataMember(Name = "lat", Order = 1)]
        public string Latitude {set;get;}
        [DataMember(Name = "lng", Order = 2)]
        public string Longitude {set;get;}
    }

    [DataContract]
    public class IPAnalysisModel
    {
        [DataMember(Name = "status", Order = 1)]
        public int Status { set; get; }

        [DataMember(Name = "address", Order = 2)]
        public string Address { set; get; }

        [DataMember(Name = "content", Order = 3)]
        public AnalysisContent Content { set; get; }
    }

    [DataContract]
    public class AnalysisContent
    {
        [DataMember(Name = "address", Order = 1)]
        public string Address { set; get; }

        [DataMember(Name = "address_detail", Order = 2)]
        public AddressDetail AddressDetail { set; get; }

        [DataMember(Name = "point", Order = 3)]
        public Point Point { set; get; }
    }

    [DataContract]
    public class AddressDetail
    {
        [DataMember(Name = "city", Order = 1)]
        public string City { set; get; }

        [DataMember(Name = "city_code", Order = 2)]
        public int CityCode { set; get; }

        [DataMember(Name = "district", Order = 3)]
        public string District { set; get; }

        [DataMember(Name = "province", Order = 4)]
        public string Province { set; get; }

        [DataMember(Name = "street", Order = 5)]
        public string Street { set; get; }

        [DataMember(Name = "street_number", Order = 6)]
        public string StreetNumber { set; get; }
    }

    [DataContract]
    public class Point
    {
        [DataMember(Name = "x", Order = 1)]
        public string X { set; get; }

        [DataMember(Name = "y", Order = 2)]
        public string Y { set; get; }
    }
}
