using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            this._unitOfWork = _unitOfWork;
            this._mapper = _mapper;
        }

        public async Task<IPaginate<GetFeedbackResponse>> GetFeedbacks(int page, int size)
        {
            var feedbacks = await _unitOfWork.GetRepository<Feedback>()
                .GetPagingListAsync(
                predicate: x => x.IsActive == true,
                include: query => query.Include(s => s.Customer).Include(s => s.Appointment),
                page: page,
                size: size);

            var feedbackResponses = new Paginate<GetFeedbackResponse>()
            {
                Page = feedbacks.Page,
                Size = feedbacks.Size,
                Total = feedbacks.Total,
                TotalPages = feedbacks.TotalPages,
                Items = _mapper.Map<IList<GetFeedbackResponse>>(feedbacks.Items),
            };
            return feedbackResponses;
        }

        public async Task<GetFeedbackResponse> GetFeedbackById(Guid id)
        {
            var feedback = await _unitOfWork.GetRepository<Feedback>()
                .SingleOrDefaultAsync(
                predicate: predicate => predicate.Id.Equals(id),
                include: query => query.Include(s => s.Customer).Include(s => s.Appointment));

            var feedbackResponse = _mapper.Map<GetFeedbackResponse>(feedback);

            return feedbackResponse;
        }

        public async Task<bool> CreateFeedback(CreateFeedbackRequest request)
        {
            Feedback newFeedback = new Feedback()
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                AppointmentId = request.AppointmentId,
                Rating = request.Rating,
                Comment = request.Comment,
                IsActive = true,
            };
            await _unitOfWork.GetRepository<Feedback>().InsertAsync(newFeedback);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
        public async Task<bool> UpdateFeedback(Guid id, UpdateFeedbackRequest request)
        {
            var feedback = await _unitOfWork.GetRepository<Feedback>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (feedback == null) throw new Exception("Feedback is not exist!!!");

            feedback.Rating = request.Rating;
            feedback.Comment = request.Comment;

            _unitOfWork.GetRepository<Feedback>().UpdateAsync(feedback);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteFeedback(Guid id)
        {
            var feedback = await _unitOfWork.GetRepository<Feedback>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (feedback == null) throw new Exception("Feedback is not exist!!!");

            feedback.IsActive = false;

            _unitOfWork.GetRepository<Feedback>().UpdateAsync(feedback);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

    }
}
