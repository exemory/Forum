using System;
using System.Collections.Generic;
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
    public class ThreadService : IThreadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for initializing a <see cref="ThreadService"/> class instance
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="mapper">Mapper</param>
        public ThreadService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ThreadWithDetailsDto> GetByIdAsync(Guid id)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdWithDetailsAsync(id);
            if (thread == null)
            {
                throw new NotFoundException();
            }

            return _mapper.Map<ThreadWithDetailsDto>(thread);
        }

        public async Task<IEnumerable<ThreadWithDetailsDto>> GetAllAsync()
        {
            var threads = await _unitOfWork.ThreadRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ThreadWithDetailsDto>>(threads);
        }

        public async Task<ThreadWithDetailsDto> CreateAsync(ThreadCreationDto threadDto, Guid authorId)
        {
            var user = await _userManager.FindByIdAsync(authorId.ToString());
            if (user == null)
            {
                throw new ForumException($"User with id '{authorId}' does not exist");
            }

            var thread = _mapper.Map<Thread>(threadDto);
            thread.Author = user;

            _unitOfWork.ThreadRepository.Add(thread);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ThreadWithDetailsDto>(thread);
        }

        public async Task UpdateAsync(Guid id, ThreadUpdateDto threadDto)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdAsync(id);
            if (thread == null)
            {
                throw new NotFoundException();
            }

            _mapper.Map(threadDto, thread);

            _unitOfWork.ThreadRepository.Update(thread);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdAsync(id);
            if (thread == null)
            {
                throw new NotFoundException();
            }

            _unitOfWork.ThreadRepository.Delete(thread);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateStatusAsync(Guid id, ThreadStatusUpdateDto statusDto)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdAsync(id);
            if (thread == null)
            {
                throw new NotFoundException();
            }

            _mapper.Map(statusDto, thread);

            _unitOfWork.ThreadRepository.Update(thread);
            await _unitOfWork.SaveAsync();
        }
    }
}