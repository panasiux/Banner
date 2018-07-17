using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Declarations.DomainModel;
using Declarations.Interfaces;
using Xunit;

namespace XUnitTests
{
    public class MongoDbRepoTests : BaseTest
    {
        private readonly BannersRepository.BannersRepository _repository;

        public MongoDbRepoTests()
        {
            var bannersContext = new MongoBannersContext.MongoBannersContext(MongoDbConnectionString, MongoDbDbName);
            _repository = new BannersRepository.BannersRepository(bannersContext);
            _repository.ClearAll();
        }

        [Fact]
        public async Task TestAddFindOne()
        {
            var banner = GenerateBanner();

            await _repository.Add(banner);

            var found = await _repository.FindById(banner.Id);

            CheckEqual(banner, found);
        }

        [Fact]
        public void TestFindMany()
        {
            _repository.ClearAll();
            var list = new List<Banner>
            {
                GenerateBanner(),
                GenerateBanner(),
                GenerateBanner()
            };

            list[0].Created = list[0].Created.AddDays(10);

            list.ForEach(async b => await _repository.Add(b));

            var res = _repository.All().ToList();
            Assert.True(res.Count == 3);
        }

   
    }
}
