using AutoMapper;
using Xunit;

namespace Service.Tests
{
    public class AutomapperTests
    {
        [Fact]
        public void ValidateConfiguration()
        {
            var profile = new AutomapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            
            configuration.AssertConfigurationIsValid();
        }
    }
}