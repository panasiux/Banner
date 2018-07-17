using System;
using AutoMapper;
using BannersRepository;
using Declarations.DomainModel;
using Declarations.Interfaces;
using Xunit;

namespace XUnitTests
{
    public class BaseTest
    {
        public static bool MapperInitialized = false;
        protected const string MongoDbConnectionString = "mongodb://localhost:27017";
        protected const string MongoDbDbName = "BannersTestDb";

        public BaseTest()
        {
            if(MapperInitialized)
                return;
            MapperInitialized = true;
            Mapper.Initialize(cfg => {
                cfg.AddProfile<AutoMapperRepoProfile>();
            });
        }

        private static int _idCounter = 1;
        protected Banner GenerateBanner()
        {
            return new Banner
            {
                Id = _idCounter++,
                Created = DateTime.Now.AddDays(-1),
                Modified = DateTime.Now,
                Html = $"<Id>{_idCounter - 1}</Id>"
            };
        }

        protected static void CheckEqual(IBanner banner, IBanner dbBanner)
        {
            Assert.Equal(banner.Id, dbBanner.Id);
            Assert.Equal(banner.Html, dbBanner.Html);

            Assert.True(banner.Created.Subtract(dbBanner.Created).TotalMilliseconds < 1);
            Assert.True(banner.Modified.HasValue && dbBanner.Modified.HasValue ||
                        !banner.Modified.HasValue && !dbBanner.Modified.HasValue);
            if (banner.Modified.HasValue)
                Assert.True(banner.Modified.Value.Subtract(dbBanner.Modified.Value).TotalMilliseconds < 1);
        }
    }
}
