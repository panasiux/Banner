using AutoMapper;
using BannersRepository;
using Declarations.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BannerWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            InitAutoMapper();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IBannersRepository>(new BannersRepository.BannersRepository(
                new MongoBannersContext.MongoBannersContext(
                    Configuration.GetSection("Db")["connectionString"], 
                    Configuration.GetSection("Db")["dbName"])));
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

        private void InitAutoMapper()
        {
            Mapper.Initialize(cfg => {
                cfg.AddProfile<AutoMapperRepoProfile>();
            });
        }
    }
}
