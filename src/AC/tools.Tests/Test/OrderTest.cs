using System;
using System.Collections.Generic;
using System.Linq;
using AC.Util;
using Common.Logging;
using NUnit.Framework;
using tools.Tests.API;

namespace tools.Tests.Test
{
    [TestFixture]
    public class OrderTest
    {
        private readonly ILog logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void OrderStatusGetResultTest()
        {
            ResponseResult result = new ResponseResult();
            result.code = "0";
            result.response = new OrderStatusGetResult()
                {
                    status = "1"
                };
           
            string jsonResult = JsonHelper.ToJson(result);
            logger.Info(jsonResult);
        }

        [Test]
        public void OrderStatusGetParamTest()
        {
            OrderStatusGetParam param = new OrderStatusGetParam();
            param.order_id = "12121212121";
            string jsonResult = JsonHelper.ToJson(param);
            //logger.Info(jsonResult);

            ApiParams parameter = new ApiParams();

            parameter.app_key = "wanda";
            parameter.v = "1.0";
            parameter.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            parameter.sign = "D58E3582AFA99040E27B92B13C8F2280";
            parameter.fields = param;

            logger.Info(JsonHelper.ToJson(parameter));
        }
        [Test]
        public void OrderCreateParamTest()
        {
            StoreInfo storeInfo = createStore();
            AddressInfo addressInfo = createAddress();
            OrderDetailInfo[] orderDetailInfos = createDetails();

            OrderCreateParam orderCreateParam = createOrder();
            orderCreateParam.store_info = storeInfo;
            orderCreateParam.address = addressInfo;
            orderCreateParam.order_details = orderDetailInfos;

            ApiParams parameter = new ApiParams();

            parameter.app_key = "wanda";
            parameter.v = "1.0";
            parameter.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            parameter.sign = "D58E3582AFA99040E27B92B13C8F2280";
            parameter.fields = orderCreateParam;

            logger.Info(JsonHelper.ToJson(parameter));
        }

        [Test]
        public void orderCreateResponseTest()
        {
            ResponseResult result = new ResponseResult();
            result.code = "0";
            result.response = new OrderCreateResponse()
            {
                order_id = "订单号001",
            };

            string jsonResult = JsonHelper.ToJson(result);
            logger.Info(jsonResult);
            
        }
        private OrderCreateParam createOrder()
        {
            return new OrderCreateParam()
                {
                    create_time = DateTime.Now,
                    receive_time = DateTime.Now.AddHours(1),
                    is_pay = true,
                    order_id = "123456",
                    package_count = 1,
                    remark = "快点，饿",
                    total_price = 55,
                    weight = 0,
                };
        }

        private StoreInfo createStore()
        {
            return new StoreInfo()
                {
                    address = "建国路93号院万达广场4号楼2705",
                    area_code = "000000",
                    city_code = "000000",
                    delivery_fee = 0,
                    commission_type = "1",
                    group = "3",
                    id_card = "130212201503101234",
                    latitude = 11.11,
                    longitude = 11.11,
                    phone = "010-12345678",
                    phone2 = "13612345678",
                    province_code = "000000",
                    store_id = "1",
                    store_name = "天下第一名厨大望路店",
                };
        }

        private AddressInfo createAddress()
        {
            AddressInfo address = new AddressInfo()
                {
                    address = "建国路93号院万达广场4号楼2705",
                    area_code = "000000",
                    city_code = "000000",
                    province_code = "000000",
                    latitude = 11.11,
                    longitude = 11.11,
                    user_name = "张三",
                    user_phone = "13612345678",
                };
            return address;
        }
        private OrderDetailInfo[] createDetails()
        {
            IList<OrderDetailInfo> result = new List<OrderDetailInfo>();

            OrderDetailInfo orderDetail1 = new OrderDetailInfo()
                {
                    detail_id = 1,
                    product_name = "鱼香肉丝",
                    quantity = 1,
                    unit_price = 15,
                };
            OrderDetailInfo orderDetail2 = new OrderDetailInfo()
                {
                    detail_id = 2,
                    product_name = "宫爆鸡丁",
                    quantity = 2,
                    unit_price = 20,
                };
            result.Add(orderDetail1);
            result.Add(orderDetail2);

            return result.ToArray();
        }
    }
}