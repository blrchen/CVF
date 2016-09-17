using System;
using System.Collections.Generic;
using System.Linq;
using CVF.Contract.Requests;
using CVF.Contract.Responses;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CVF.App.ApiControllers
{

    public class DemoController : Controller
    {
        private static DemoPluginSettings GlobalSettings;

        [HttpPost]
        [Route("api/demo/table")]
        public PluginPaginationResponse<DemoPluginRawData> GetDataForDemoList([FromBody]SearchRequestInfo request)
        {
            var list = ParallelEnumerable.Range(0, 1000)
                .Select(i => new DemoPluginRawData() { Id = i, Name = "demo data " + i.ToString(), Type = (DemoEnumType)(i % 3) })
                .ToList();

            var keyword = request.Keyword;
            var page = request.Page;
            var pageSize = request.PageSize;

            IEnumerable<DemoPluginRawData> raws = list;
            if (!string.IsNullOrEmpty(keyword))
            {
                raws = raws.Where(r => r.Name.Contains(keyword));
            }

            var total = raws.Count();

            raws = raws.Skip((page - 1) * pageSize).Take(pageSize);

            return new PluginPaginationResponse<DemoPluginRawData>()
            {
                Page = page,
                PageSize = pageSize,
                Raws = raws.ToList(),
                Total = total
            };
        }

        [HttpPost]
        [Route("api/demo/form")]
        public DemoPluginSettings GetDataForDemoUpdate([FromBody]UpdateRequestInfo<DemoPluginSettings> request)
        {
            if (GlobalSettings == null)
            {
                GlobalSettings = new DemoPluginSettings();
                GlobalSettings.FormData1 = "FormData1-Default";
                GlobalSettings.FormData2 = "FormData2-Default";
                GlobalSettings.UpdateTime = DateTime.UtcNow;
            }

            if (request.Method == PluginRequestMethod.Update)
            {
                GlobalSettings.FormData1 = request.Body.FormData1;
                GlobalSettings.FormData2 = request.Body.FormData2;
                GlobalSettings.UpdateTime = DateTime.UtcNow;
            }

            return GlobalSettings;
        }

        [HttpPost]
        [Route("api/demo/chart")]
        public IReadOnlyList<object> GetDataForDemoReport([FromBody]PluginRequestInfo request)
        {
            var pieData = new
            {
                Labels = ParallelEnumerable.Range(0, 24).Select(i => i.ToString()).ToArray(),
                Series = new string[] { "小时设备" },
                Datas = this.RandomNumbers(24),
            };
            var lineData = new
            {
                Labels = new string[] { "男", "女" },
                Datas = this.RandomNumbers(2),
            };

            return new object[] { pieData, lineData };
        }

        private double[] RandomNumbers(int length)
        {
            Random random = new Random();
            return new int[length].Select(i => Math.Floor(random.NextDouble() * 10000) / 100).ToArray();
        }

    }

    public class DemoPluginRawData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DemoEnumType Type { get; set; }
    }

    public class DemoPluginSettings
    {
        public string FormData1 { get; set; }

        public string FormData2 { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    public enum DemoEnumType
    {
        Type1,
        Type2,
        Type3
    }
}
