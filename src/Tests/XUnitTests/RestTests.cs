using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Declarations.DomainModel;
using Declarations.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace XUnitTests
{
    public class RestTests : BaseTest
    {
        private static Mock<IBannersRepository> _mockRepo;
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private readonly List<IBanner> _banners = new List<IBanner>();
        public RestTests()
        {
            _mockRepo = new Mock<IBannersRepository>();
            _mockRepo.Setup(x => x.Add(It.IsAny<IBanner>())).Returns((IBanner b) =>
            {
                _banners.Add(b);
                return Task.FromResult<IBanner>(b);
            });

            _mockRepo.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<IBanner>())).Returns((int id, IBanner b) =>
            {
                var existing = _banners.FirstOrDefault(x => x.Id == id);
                if(existing == null)
                    return Task.FromResult<IBanner>(null);

                _banners.Remove(existing);
                b.Id = existing.Id;
                _banners.Add(b);
                return Task.FromResult<IBanner>(b);
            });

            _mockRepo.Setup(x => x.Delete(It.IsAny<int>())).Returns((int id) =>
            {
                var existing = _banners.FirstOrDefault(x => x.Id == id);
                if (existing != null)
                    _banners.Remove(existing);
                return Task.Delay(0);
            });

            _mockRepo.Setup(x => x.FindById(It.IsAny<int>())).Returns((int id) =>
            {
                var existing = _banners.FirstOrDefault(x => x.Id == id);
                if (existing == null)
                    return Task.FromResult<IBanner>(null);
                
                return Task.FromResult<IBanner>(existing);
            });

            _mockRepo.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>())).Returns((int take, int skip) =>
            {
                return Task.FromResult<IList<IBanner>>(_banners.Select(x => (IBanner)x).Skip(skip).Take(take).ToList());
            });

            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            _client = _server.CreateClient();
        }

        [Fact]
        public async Task AddingSimple()
        {
            var banner = GenerateBanner();

            var ret = await Post(banner);
            CheckEqual(banner, ret.Item1);
        }

        [Fact]
        public async Task AddingAndGetting()
        {
            var banner = GenerateBanner();

            var ret = await Post(banner);
            CheckEqual(banner, ret.Item1);

            ret = await Get(banner.Id);
            CheckEqual(banner, ret.Item1);

            ret = await Get(banner.Id + 10000);
            Assert.True(ret.Item2 == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddingDuplicate()
        {
            var banner = GenerateBanner();
            var ret = await Post(banner);
            CheckEqual(banner, ret.Item1);

            ret = await Post(banner);
            Assert.True(ret.Item2 == HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Updating()
        {
            var banner = GenerateBanner();
            var ret = await Post(banner);
            CheckEqual(banner, ret.Item1);

            ret = await Put(banner);
            CheckEqual(banner, ret.Item1);

            banner.Html = "hej";
            ret = await Put(banner);
            CheckEqual(banner, ret.Item1);
        }

        [Fact]
        public async Task Deleting()
        {
            var banner = GenerateBanner();
            var ret = await Post(banner);
            CheckEqual(banner, ret.Item1);

            var ret2 = await Delete(banner.Id);
            Assert.True(ret2 == HttpStatusCode.OK);

            ret2 = await Delete(banner.Id);
            Assert.True(ret2 == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Rendering()
        {
            var banner = GenerateBanner();
            var ret = await Post(banner);
            CheckEqual(banner, ret.Item1);

            var ret2 = await Render(banner.Id);
            Assert.True(ret2.Item1 == banner.Html);

            ret2 = await Render(banner.Id + 10000);
            Assert.True(ret2.Item2 == HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Listing()
        {
            var banners = new List<Banner>()
            {
                GenerateBanner(),
                GenerateBanner(),
                GenerateBanner(),
                GenerateBanner(),
                GenerateBanner(),
                GenerateBanner()
            };
            banners.ForEach(async b => await Post(b));
            
            var ret = await List(0, 100);
            Assert.True(ret.Item1.Count >= 6);

            ret = await List(3, -1);
            Assert.True(ret.Item2 == HttpStatusCode.BadRequest);
        }

        #region Helpers

        public async Task<Tuple<Banner, HttpStatusCode>> Post(Banner banner)
        {
            var output = JsonConvert.SerializeObject(banner);

            var content = new StringContent(output, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/banners", content);
            var code = response.StatusCode;
            if (response.StatusCode != HttpStatusCode.OK)
                return new Tuple<Banner, HttpStatusCode>(null, code);
            var responseString = await response.Content.ReadAsStringAsync();
            return new Tuple<Banner, HttpStatusCode>(JsonConvert.DeserializeObject<Banner>(responseString), code); 
        }

        public async Task<Tuple<Banner, HttpStatusCode>> Put(Banner banner)
        {
            var output = JsonConvert.SerializeObject(banner);

            var content = new StringContent(output, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/banners/{banner.Id}", content);
            var code = response.StatusCode;
            if (response.StatusCode != HttpStatusCode.OK)
                return new Tuple<Banner, HttpStatusCode>(null, code);
            var responseString = await response.Content.ReadAsStringAsync();
            return new Tuple<Banner, HttpStatusCode>(JsonConvert.DeserializeObject<Banner>(responseString), code);
        }

        public async Task<HttpStatusCode> Delete(int id)
        {
            var response = await _client.DeleteAsync($"/api/banners/{id}");
            var code = response.StatusCode;
            return code;
        }

        public async Task<Tuple<Banner, HttpStatusCode>> Get(int id)
        {
            var response = await _client.GetAsync($"/api/banners/{id}");
            var code = response.StatusCode;
            if (response.StatusCode != HttpStatusCode.OK)
                return new Tuple<Banner, HttpStatusCode>(null, code);
            var responseString = await response.Content.ReadAsStringAsync();
            return new Tuple<Banner, HttpStatusCode>(JsonConvert.DeserializeObject<Banner>(responseString), code);
        }

        public async Task<Tuple<string, HttpStatusCode>> Render(int id)
        {
            var response = await _client.GetAsync($"/api/banners/render/{id}");
            var code = response.StatusCode;
            if (response.StatusCode != HttpStatusCode.OK)
                return new Tuple<string, HttpStatusCode>(null, code);
            var responseString = await response.Content.ReadAsStringAsync();
            return new Tuple<string, HttpStatusCode>(responseString, code);
        }

        public async Task<Tuple<List<Banner>, HttpStatusCode>> List(int skip, int take)
        {
            var response = await _client.GetAsync($"/api/banners?take={take}&skip={skip}");
            var code = response.StatusCode;
            if (response.StatusCode != HttpStatusCode.OK)
                return new Tuple<List<Banner>, HttpStatusCode>(null, code);
            var responseString = await response.Content.ReadAsStringAsync();
            return new Tuple<List<Banner>, HttpStatusCode>(JsonConvert.DeserializeObject<List<Banner>>(responseString), code);
        }

        #endregion

        #region test Startup

        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc();
                services.AddSingleton<IBannersRepository>(_mockRepo.Object);
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseMvc();
            }
        }

        #endregion
    }
}
