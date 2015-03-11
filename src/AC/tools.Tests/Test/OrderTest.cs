using System;
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
    }
}