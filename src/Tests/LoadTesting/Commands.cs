using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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

        public Commands()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .Build();
            _endpoint = configuration["serviceEndpoint"];
        }

        public void Run(int writersCount, int readersCount, int callsPerSec)
        {
            var working = new List<Task>();

            for (var i = 0; i < writersCount; i++)
            {
                var threadNumber = i;
                working.Add(Task.Run(async () =>
                {
                    while (true)
                    {
                        using (var client = new HttpClient())
                            await Post(client, GenerateBanner());

                        _stat.PostCount++;
                        await Task.Delay(1000/callsPerSec);
                    }
                }));
            }

            for (var i = 0; i < readersCount; i++)
            {
                var threadNumber = i;
                working.Add(Task.Run(async () =>
                {
                    while (true)
                    {
                        
                        using (var client = new HttpClient())
                            await Get(client, _random.Next(1, _idCounter));
                        _stat.GetCount++;
                        await Task.Delay(1000 / callsPerSec);
                    }
                }));
            }

            working.Add(Task.Run(async () =>
            {
                while (true)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss}posts={_stat.PostCount}, gets={_stat.GetCount}");

                    await Task.Delay(1000);
                }
            }));

            Task.WaitAll(working.ToArray());
        }

        public class Stat
        {
            public int PostCount { set; get; }
            public int GetCount { set; get; }
        }

        #region rest helpers

        private static int _idCounter = 1;

        private Banner GenerateBanner()
        {
            return new Banner
            {
                Id = _idCounter++,
                Created = DateTime.Now.AddDays(-1),
                Modified = DateTime.Now,
                Html = $"<Id>{_idCounter - 1}</Id>"
            };
        }

        private async Task<Tuple<Banner, HttpStatusCode>> Post(HttpClient client, Banner banner)
        {
            var output = JsonConvert.SerializeObject(banner);

            var content = new StringContent(output, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_endpoint, content);
            var code = response.StatusCode;
            if (response.StatusCode != HttpStatusCode.OK)
                return new Tuple<Banner, HttpStatusCode>(null, code);
            var responseString = await response.Content.ReadAsStringAsync();
            return new Tuple<Banner, HttpStatusCode>(JsonConvert.DeserializeObject<Banner>(responseString), code);
        }

        private async Task<Tuple<Banner, HttpStatusCode>> Get(HttpClient client, int id)
        {
            var response = await client.GetAsync($"{_endpoint}/{id}");
            var code = response.StatusCode;
            if (response.StatusCode != HttpStatusCode.OK)
                return new Tuple<Banner, HttpStatusCode>(null, code);
            var responseString = await response.Content.ReadAsStringAsync();
            return new Tuple<Banner, HttpStatusCode>(JsonConvert.DeserializeObject<Banner>(responseString), code);
        }

        //public async Task<Tuple<Banner, HttpStatusCode>> Put(HttpClient client, Banner banner)
        //{
        //    var output = JsonConvert.SerializeObject(banner);

        //    var content = new StringContent(output, Encoding.UTF8, "application/json");
        //    var response = await client.PutAsync($"{_endpoint}/{banner.Id}", content);
        //    var code = response.StatusCode;
        //    if (response.StatusCode != HttpStatusCode.OK)
        //        return new Tuple<Banner, HttpStatusCode>(null, code);
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    return new Tuple<Banner, HttpStatusCode>(JsonConvert.DeserializeObject<Banner>(responseString), code);
        //}

        //public async Task<HttpStatusCode> Delete(HttpClient client, int id)
        //{
        //    var response = await client.DeleteAsync($"{_endpoint}/{id}");
        //    var code = response.StatusCode;
        //    return code;
        //}

        #endregion
    }
}
