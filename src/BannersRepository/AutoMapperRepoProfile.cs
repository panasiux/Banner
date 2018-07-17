using AutoMapper;
using Declarations.DomainModel;
using Declarations.Interfaces;
using MongoBannersContext.Documents;

namespace BannersRepository
{
    public class AutoMapperRepoProfile : Profile
    {
        public AutoMapperRepoProfile()
        {
            CreateMap<Banner, BannerDoc>()
                .ForMember(x => x._id, opt => opt.Ignore());;
            CreateMap<IBanner, BannerDoc>()
                .ForMember(x => x._id, opt => opt.Ignore());
        }
    }
}
