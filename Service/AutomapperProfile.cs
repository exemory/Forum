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

            CreateMap<User, UserDto>();
            CreateMap<User, UserWithDetailsDto>();
            
            CreateMap<Thread, ThreadWithDetailsDto>();
            CreateMap<ThreadCreationDto, Thread>();
            CreateMap<ThreadUpdateDto, Thread>();

            CreateMap<Post, PostWithDetailsDto>();
            CreateMap<PostCreationDto, Post>();
            CreateMap<PostUpdateDto, Post>();
        }
    }
}