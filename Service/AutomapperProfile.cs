using AutoMapper;
using Data.Entities;
using Service.DataTransferObjects;

namespace Service
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<SignUpDto, User>(MemberList.Source)
                .ForSourceMember(d => d.Password, o => o.DoNotValidate());
            CreateMap<AccountUpdateDto, User>(MemberList.Source)
                .ForSourceMember(d => d.CurrentPassword, o => o.DoNotValidate());

            CreateMap<User, UserDto>();
            CreateMap<User, UserWithDetailsDto>()
                .ForMember(u => u.Roles, o => o.Ignore());
            
            CreateMap<Thread, ThreadWithDetailsDto>();
            CreateMap<ThreadCreationDto, Thread>(MemberList.Source);
            CreateMap<ThreadUpdateDto, Thread>(MemberList.Source);
            CreateMap<ThreadStatusUpdateDto, Thread>(MemberList.Source);

            CreateMap<Post, PostWithDetailsDto>();
            CreateMap<PostCreationDto, Post>(MemberList.Source);
            CreateMap<PostUpdateDto, Post>(MemberList.Source);
        }
    }
}