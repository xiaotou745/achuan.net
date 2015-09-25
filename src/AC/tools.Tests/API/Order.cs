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
        public string order_id { get; set; }
        public DateTime create_time { get; set; }
        public DateTime receive_time { get; set; }
        public bool is_pay { get; set; }
        public double total_price { get; set; }
        public AddressInfo address { get; set; }
        public StoreInfo store_info { get; set; }
        public OrderDetailInfo[] order_details { get; set; }
        public string remark { get; set; }
        public double weight { get; set; }
        public double delivery_fee { get; set; }
        public int package_count { get; set; }
    }

    public class OrderCreateResponse
    {
        public string order_id { get; set; }
    }

    public class StoreInfo
    {
        public string store_id { get; set; }
        public string store_name { get; set; }
        public string group{get;set;}
        public string id_card { get; set; }
        public string phone { get; set; }
        public string phone2 { get; set; }
        public string province_code { get; set; }
        public string city_code { get; set; }
        public string area_code { get; set; }
        public string address { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public double delivery_fee { get; set; }
        public string commission_type { get; set; }
    }

    public class AddressInfo
    {
        public string user_name { get; set; }
        public string user_phone { get; set; }
        public string province_code { get; set; }
        public string city_code { get; set; }
        public string area_code { get; set; }
        public string address { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
    }

    public class OrderDetailInfo
    {
        public long detail_id { get; set; }
        public string product_name { get; set; }
        public double unit_price { get; set; }
        public int quantity { get; set; }
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
