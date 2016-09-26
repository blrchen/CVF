using System;
using System.Collections.Generic;
using System.Linq;
using CVF.Contract.Requests;
using CVF.Contract.Responses;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CVF.App.ApiControllers
{

    public class Plugin2Controller : Controller
    {
        private static Plugin2Controller GlobalSettings;
        private static readonly List<DemoPluginRawData> SampleDatas = ParallelEnumerable.Range(0, 1000)
                    .Select(i => new DemoPluginRawData() { Id = i, Name = "demo data " + i.ToString(), Type = (DemoEnumType)(i % 3) })
                    .ToList();

        [HttpPost]
        [Route("api/p2/table")]
        public PluginPaginationResponse<DemoPluginRawData> GetDataForDemoList([FromBody]SearchRequestInfo<DemoPluginRawData> request)
        {
            if (request.Method == PluginRequestMethod.Update)
            {
                var updateRawData = request.RawData;
                if (updateRawData != null)
                {
                    var dbRawData = SampleDatas.Where(d => d.Id == updateRawData.Id).FirstOrDefault();
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

        private double[] RandomNumbers(int length)
        {
            Random random = new Random();
            return new int[length].Select(i => Math.Floor(random.NextDouble() * 10000) / 100).ToArray();
        }

    }
}
