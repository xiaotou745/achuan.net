using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tools.Tests.API
{
    public class ResponseResult
    {
        public string code { get; set; }
        public object response { get; set; }
    }
    

    public class ApiParams
    {
        public string app_key { get; set; }
        public string timestamp { get; set; }
        public string v { get; set; }
        public string sign { get; set; }
        public object fields { get; set; }
    }

    public class OrderCreateParam
    {
        
    }

    public class StoreInfo
    {
        public string name { get; set; }
        public string id { get; set; }
        public string 
    }

    public class OrderStatusGetParam
    {
        public string order_id { get; set; }
    }

    public class OrderStatusGetResult
    {
        public string status { get; set; }
    }
}
