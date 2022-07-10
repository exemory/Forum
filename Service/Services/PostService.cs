using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Service.DataTransferObjects;
using Service.Exceptions;
using Service.Interfaces;

namespace Service.Services
{
    /// <inheritdoc />
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ISession _session;

        /// <summary>
        /// Constructor for initializing a <see cref="PostService"/> class instance
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="mapper">Mapper</param>
        /// <param name="session">Current session</param>
        public PostService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper, ISession session)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _session = session;
        }

        public async Task<PostWithDetailsDto> GetByIdAsync(Guid id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdWithDetailsAsync(id);
            if (post == null)
            {
                throw new NotFoundException($"Post with id '{id}' not found");
            }

            return _mapper.Map<PostWithDetailsDto>(post);
        }

        public async Task<IEnumerable<PostWithDetailsDto>> GetByThreadAsync(Guid threadId)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdAsync(threadId);
            if (thread == null)
            {
                throw new NotFoundException($"Thread with id '{threadId}' not found");
            }

            var posts = await _unitOfWork.PostRepository.GetByThreadIdWithDetailsAsync(threadId);
            return _mapper.Map<IEnumerable<PostWithDetailsDto>>(posts);
        }

        public async Task<PostWithDetailsDto> CreateAsync(PostCreationDto postDto)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdAsync(postDto.ThreadId);
            if (thread == null)
            {
                throw new NotFoundException($"Thread with id '{postDto.ThreadId}' not found");
            }

            var author = await _userManager.FindByIdAsync(_session.UserId.ToString());
            if (author == null)
            {
                throw new ForumException($"User with id '{_session.UserId}' does not exist");
            }

            if (thread.Closed)
            {
                throw new ForumException("Thread is closed for posting");
            }

            var post = _mapper.Map<Post>(postDto);
            post.Author = author;

            _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<PostWithDetailsDto>(post);
        }

        public async Task UpdateAsync(Guid id, PostUpdateDto postDto)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null)
            {
                throw new NotFoundException($"Post with id '{id}' not found");
            }

            if (post.AuthorId != _session.UserId &&
                !_session.UserRoles.Contains("Moderator") && 
                !_session.UserRoles.Contains("Administrator"))
            {
                throw new AccessDeniedException("Access denied");
            }

            _mapper.Map(postDto, post);

            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null)
            {
                throw new NotFoundException($"Post with id '{id}' not found");
            }
            
            if (post.AuthorId != _session.UserId &&
                !_session.UserRoles.Contains("Moderator") && 
                !_session.UserRoles.Contains("Administrator"))
            {
                throw new AccessDeniedException("Access denied");
            }

            _unitOfWork.PostRepository.Delete(post);
            await _unitOfWork.SaveAsync();
        }
    }
}