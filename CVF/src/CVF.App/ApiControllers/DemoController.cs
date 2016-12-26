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
        private static readonly List<DemoPluginRawData> SampleDatas = ParallelEnumerable.Range(0, 1000)
                    .Select(i => new DemoPluginRawData() { Id = i, Name = "demo data " + i.ToString(), Type = (DemoEnumType)(i % 3) })
                    .ToList();

        private static List<int> CreationDatas = new List<int>();

        [HttpPost]
        [Route("api/demo/table")]
        public PluginPaginationResponse<DemoPluginRawData> GetDataForDemoList([FromBody]SearchRequestInfo<DemoPluginRawData> request)
        {
            if (request.Method == PluginRequestMethod.Update)
            {
                var updateRawData = request.RawData;
                if (CreationDatas.Any(x => x == updateRawData.Id))
                {
                    CreationDatas.Remove(updateRawData.Id);
                    SampleDatas.Add(new DemoPluginRawData {
                        Id = updateRawData.Id
                    });
                }

                if (updateRawData != null)
                {
                    var dbRawData = SampleDatas.FirstOrDefault(d => d.Id == updateRawData.Id);
                    if (dbRawData != null)
                    {
                        dbRawData.Name = updateRawData.Name;
                        dbRawData.Type = updateRawData.Type;
                    }

                    // update raw data here.
                }
            }
            else if (request.Method == PluginRequestMethod.Delete)
            {
                var deleteRawData = request.RawData;
                if (deleteRawData != null)
                {
                    if(SampleDatas.Any(d=>d.Id == deleteRawData.Id))
                    {
                        // delete the raw data here.
                        SampleDatas.Remove(SampleDatas.First(d => d.Id == deleteRawData.Id));
                    }
                }
            }
            else if (request.Method == PluginRequestMethod.Create)
            {
                var id = SampleDatas.Max(x => x.Id) + 1;
                var Raws = new List<DemoPluginRawData>();
                Raws.Add(new DemoPluginRawData {
                    Id = id
                } );
                CreationDatas.Add(id);

                return new PluginPaginationResponse<DemoPluginRawData>()
                {
                    Raws = Raws
                };

            }

            var keyword = request.Keyword;
            var page = request.Page;
            var pageSize = request.PageSize;

            IEnumerable<DemoPluginRawData> raws = SampleDatas;
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
                Series = new string[] { "小时物品" },
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
