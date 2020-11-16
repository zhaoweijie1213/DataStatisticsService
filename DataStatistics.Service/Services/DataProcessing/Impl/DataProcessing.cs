using DataStatistics.Service.Services.DataProcessing;
using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStatistics.Service.Services.DataProcessingl.Impl
{
    public class DataProcessing: ICapSubscribe, IDataProcessing
    {
        [CapSubscribe("m.test")]
        public void SubscribeWithnoController(string date)
        {
            Console.WriteLine($"SubscribeWithnoController接收到订阅:{date}");
        }
    }
}
