using AutoMapper;
using Data.Entities;
using Service.DataTransferObjects;

namespace Service
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<SignUpDto, User>();
        }
    }
}