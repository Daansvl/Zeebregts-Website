using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeebregtsLogic
{
    [Serializable()]
    public class APIError
    {
        public string message { get; set; }
    }

    [Serializable()]
    public class APIResult
    {
        public string street { get; set; }
        public string city { get; set; }
        public string nl_sixpp { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string areacode { get; set; }
    }

    [Serializable()]
    public class APIResult_suggest
    {
        public string city_key { get; set; }
        public string city { get; set; }
        public string official_city { get; set; }
        public string nl_fourpps { get; set; }

    }

    [Serializable()]
    public class APIResult_street
    {
        public string street { get; set; }
        public string nl_sixpps { get; set; }

    }

    [Serializable()]
    public class APIResponse
    {
        public string status { get; set; }
        public List<APIResult> results { get; set; }
        public APIError error { get; set; }
    }
    public class APIResponse_suggest
    {
        public string status { get; set; }
        public List<APIResult_suggest> results { get; set; }
        public APIError error { get; set; }
    }

    public class APIResponse_street
    {
        public string status { get; set; }
        public List<APIResult_street> results { get; set; }
        public APIError error { get; set; }
    }

}
