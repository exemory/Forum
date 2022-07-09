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
        private readonly ISession _session;

        /// <summary>
        /// Constructor for initializing a <see cref="ThreadService"/> class instance
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="mapper">Mapper</param>
        /// <param name="session">Current session</param>
        public ThreadService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper, ISession session)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _session = session;
        }

        public async Task<ThreadWithDetailsDto> GetByIdAsync(Guid id)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdWithDetailsAsync(id);
            if (thread == null)
            {
                throw new NotFoundException($"Thread with id '{id}' not found");
            }

            return _mapper.Map<ThreadWithDetailsDto>(thread);
        }

        public async Task<IEnumerable<ThreadWithDetailsDto>> GetAllAsync()
        {
            var threads = await _unitOfWork.ThreadRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ThreadWithDetailsDto>>(threads);
        }

        public async Task<ThreadWithDetailsDto> CreateAsync(ThreadCreationDto threadDto)
        {
            var author = await _userManager.FindByIdAsync(_session.UserId.ToString());
            if (author == null)
            {
                throw new ForumException($"User with id '{_session.UserId}' does not exist");
            }

            var thread = _mapper.Map<Thread>(threadDto);
            thread.Author = author;

            _unitOfWork.ThreadRepository.Add(thread);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ThreadWithDetailsDto>(thread);
        }

        public async Task UpdateAsync(Guid id, ThreadUpdateDto threadDto)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdAsync(id);
            if (thread == null)
            {
                throw new NotFoundException($"Thread with id '{id}' not found");
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
                throw new NotFoundException($"Thread with id '{id}' not found");
            }

            _unitOfWork.ThreadRepository.Delete(thread);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateStatusAsync(Guid id, ThreadStatusUpdateDto statusDto)
        {
            var thread = await _unitOfWork.ThreadRepository.GetByIdAsync(id);
            if (thread == null)
            {
                throw new NotFoundException($"Thread with id '{id}' not found");
            }

            _mapper.Map(statusDto, thread);

            _unitOfWork.ThreadRepository.Update(thread);
            await _unitOfWork.SaveAsync();
        }
    }
}