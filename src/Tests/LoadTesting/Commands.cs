using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Declarations.DomainModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LoadTesting
{
    public class Commands
    {
        private readonly string _endpoint;
        private readonly Stat _stat = new Stat();
        private readonly Random _random = new Random();
        private readonly HttpClient _client = new HttpClient();

        public Commands()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .Build();
            _endpoint = configuration["serviceEndpoint"];
        }

        public void Run(
            int writersCount, 
            int readersCount,
            int updatersCount,
            int callsPerSec)
        {
            var working = new List<Task>();

            for (var i = 0; i < writersCount; i++)
            {
                working.Add(Task.Run(async () =>
                {
                    var sw = new Stopwatch();
                    long totalMs = 0;

                    while (true)
                    {
                        sw.Start();
                        await Post(_client, GenerateBanner());
                        totalMs += sw.ElapsedMilliseconds;
                        
                        _stat.AveragePostTimeMs = totalMs / ++_stat.PostCount;
                        sw.Reset();
                        
                        await Task.Delay(1000/callsPerSec);
                    }
                }));
            }

            for (var i = 0; i < readersCount; i++)
            {
                working.Add(Task.Run(async () =>
                {
                    var sw = new Stopwatch();
                    long totalMs = 0;

                    while (true)
                    {
                        sw.Start();
                        await Get(_client, _random.Next(1, _idCounter));

                        totalMs += sw.ElapsedMilliseconds;
                        _stat.AverageGetTimeMs = totalMs / ++_stat.GetCount;
                        sw.Reset();

                        await Task.Delay(1000 / callsPerSec);
                    }
                }));
            }

            for (var i = 0; i < updatersCount; i++)
            {
                working.Add(Task.Run(async () =>
                {
                    var sw = new Stopwatch();
                    long totalMs = 0;

                    while (true)
                    {
                        var b = GenerateBanner();
                        b.Id = _random.Next(1, _idCounter);
                        b.Modified = DateTime.Now;
                        sw.Start();

                        await Put(_client, b);

                        totalMs += sw.ElapsedMilliseconds;
                        _stat.AveragePutTimeMs = totalMs / ++_stat.PutCount;
                        sw.Reset();

                        await Task.Delay(1000 / callsPerSec);
                    }
                }));
            }

            working.Add(Task.Run(async () =>
            {
                while (true)
                {
                    Log($"[{Thread.CurrentThread.ManagedThreadId}] " +
                        $"posts={_stat.PostCount} (avgMs={_stat.AveragePutTimeMs}), " +
                        $"gets={_stat.GetCount} (avgMs={_stat.AverageGetTimeMs}), " +
                        $"puts={_stat.PutCount} (avgMs={_stat.AveragePutTimeMs})");

                    await Task.Delay(1000);
                }
            }));

            Task.WaitAll(working.ToArray());
        }

        public class Stat
        {
            public long AveragePostTimeMs { set; get; }
            public int PostCount { set; get; }

            public long AverageGetTimeMs { set; get; }
            public int GetCount { set; get; }

            public long AveragePutTimeMs { set; get; }
            public int PutCount { set; get; }
        }

        #region rest helpers

        private static void Log(string msg)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} {msg}");
        }

        private static int _idCounter = 1;

        private static Banner GenerateBanner()
        {
            return new Banner
            {
                Id = _idCounter++,
                Created = DateTime.Now.AddDays(-1),
                Html = $"<Id>{_idCounter - 1}</Id>"
            };
        }

        private async Task<HttpStatusCode> Post(HttpClient client, Banner banner)
        {
            var output = JsonConvert.SerializeObject(banner);

            var content = new StringContent(output, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_endpoint, content);
            return response.StatusCode;
        }

        private async Task<HttpStatusCode> Get(HttpClient client, int id)
        {
            var response = await client.GetAsync($"{_endpoint}/{id}");
            return response.StatusCode;
        }

        private async Task<HttpStatusCode> Put(HttpClient client, Banner banner)
        {
            var output = JsonConvert.SerializeObject(banner);

            var content = new StringContent(output, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_endpoint}/{banner.Id}", content);
            return response.StatusCode;
        }

        #endregion
    }
}
