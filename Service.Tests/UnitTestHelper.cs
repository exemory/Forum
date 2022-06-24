using AutoMapper;

namespace Service.Tests
{
    public static class UnitTestHelper
    {
        public static IMapper CreateMapper()
        {
            var profile = new AutomapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            
            return new Mapper(configuration);
        }
    }
}