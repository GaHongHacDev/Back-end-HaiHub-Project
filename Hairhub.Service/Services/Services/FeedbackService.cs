using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Payment;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
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
        private readonly ISalonInformationService _salonInformationService;
        private readonly IMediaService _mediaservice;
        public FeedbackService(IUnitOfWork _unitOfWork, IMapper _mapper, ISalonInformationService salonInformationService, IMediaService mediaservice)
        {
            this._unitOfWork = _unitOfWork;
            this._mapper = _mapper;
            _salonInformationService = salonInformationService;
            _mediaservice = mediaservice;
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
            try
            {
                var existingSalon = await _salonInformationService.GetSalonInformationById(request.SalonId);
                if (existingSalon == null)
                {
                    throw new NotFoundException("Salon, barber shop không tồn tại");
                }

                Feedback newFeedback = new Feedback()
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    AppointmentId = request.AppointmentId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    IsActive = true,
                };

                for(int i=0; i<request.ImgFeedbacks.Count; i++)
                {
                    var urlImg = await _mediaservice.UploadAnImage(request.ImgFeedbacks[i], MediaPath.FEEDBACK_IMG, newFeedback.Id.ToString()+"/"+i.ToString());
                    //var urlVideo = await _mediaservice.UploadAVideo(request.Video, MediaPath.FEEDBACK_VIDEO, newFeedback.Id.ToString());
                    StaticFile staticFile = new StaticFile()
                    {
                        Id = Guid.NewGuid(),
                        FeedbackId = newFeedback.Id,
                        Img = urlImg,
                    };
                    await _unitOfWork.GetRepository<StaticFile>().InsertAsync(staticFile);
                }
                              
                int totalRating = existingSalon.TotalRating;
                int totalReview = existingSalon.TotalReviewer + 1;
                existingSalon.Rate = (int)(totalRating + request.Rating) / totalReview;
                existingSalon.TotalReviewer = totalReview;
                existingSalon.TotalRating = (int)(totalRating + request.Rating);

                var salon = _mapper.Map<SalonInformation>(existingSalon);

                _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salon);
                await _unitOfWork.GetRepository<Feedback>().InsertAsync(newFeedback);

                bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
                return isSuccessful;
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
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
