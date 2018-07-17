using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Declarations.DomainModel;
using Declarations.Interfaces;
using MongoBannersContext.Documents;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BannersRepository
{
    public class BannersRepository : IBannersRepository
    {
        private readonly MongoBannersContext.MongoBannersContext _context;

        public BannersRepository(MongoBannersContext.MongoBannersContext context)
        {
            _context = context;
        }

        public void ClearAll()
        {
            _context.ClearAll();
        }

        public async Task<IBanner> Add(IBanner banner)
        {
            var doc = new BannerDoc();
            Mapper.Map(banner, doc);
            await _context.Banners.InsertOneAsync(doc);

            return Mapper.Map<Banner>(doc);
        }

        public async Task<IBanner> FindById(int id)
        {
            var filter = Builders<BannerDoc>.Filter.Eq(nameof(BannerDoc.Id), id);
            var res = await _context.Banners.FindAsync(filter);
            return await res.FirstOrDefaultAsync();
        }

        public async Task<IBanner> Update(int id, IBanner banner)
        {
            var filter = Builders<BannerDoc>.Filter.Eq(nameof(BannerDoc.Id), id);
            var res = await _context.Banners.FindAsync(filter);
            var item = await res.FirstOrDefaultAsync();
            if (item == null)
                return null;

            banner.Id = item.Id;
            Mapper.Map(banner, item);
            
            await _context.Banners.ReplaceOneAsync(filter, item);

            return Mapper.Map<Banner>(item);
        }

        public async Task Delete(int id)
        {
            var filter = Builders<BannerDoc>.Filter.Eq(nameof(BannerDoc.Id), id);

            await _context.Banners.DeleteOneAsync(filter);
        }

        public async Task<IList<IBanner>> List(int take, int skip)
        {
            if(take <= 0 || skip < 0)
                throw new Exception($"wrong params: {nameof(take)}={take}, {nameof(skip)}={skip}");

            var res = await _context.Banners.AsQueryable().Skip(skip).Take(take).ToListAsync();
            
            return res.Select(Mapper.Map<Banner>).Cast<IBanner>().ToList();
        }

        public IEnumerable<IBanner> All()
        {
            foreach (var item in _context.Banners.AsQueryable())
                yield return Mapper.Map(item, new BannerDoc());

        }
    }
}
