using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Payment;
using Hairhub.Domain.Dtos.Responses.Reports;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Dtos.Responses.StaticFile;
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
using System.Linq.Expressions;
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
                include: x => x.Include(s => s.StaticFiles).Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails).ThenInclude(s => s.SalonEmployee.SalonInformation).Include(s => s.Customer),

                page: page,
                size: size);
            foreach (var feedback in feedbacks.Items)
            {
                feedback.StaticFiles = await _unitOfWork.GetRepository<StaticFile>().GetListAsync(predicate: x => x.FeedbackId == feedback.Id);
            }

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
                    include: x => x.Include(s => s.StaticFiles).Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails).ThenInclude(s => s.SalonEmployee.SalonInformation).Include(s => s.Customer)
                );


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
                int ratingSum = request.FeedbackDetailRequests.Sum(s => s.Rating);
                Feedback newFeedback = new Feedback()
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    AppointmentId = request.AppointmentId,
                    Rating = ratingSum / request.FeedbackDetailRequests.Count,
                    Comment = request.Comment,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                };

                await _unitOfWork.GetRepository<Feedback>().InsertAsync(newFeedback);

                foreach (var item in request.FeedbackDetailRequests)
                {
                    var appointmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                                .SingleOrDefaultAsync(
                                                                                      predicate: x=>x.Id == item.AppointmentDetailId 
                                                                                                && x.Status.Equals(AppointmentStatus.Successed)
                                                                                     );
                    if (appointmentDetail == null)
                    {
                        throw new NotFoundException($"Không tìm thấy lịch hẹn chi tiết có trạng thái thành công với id {item.AppointmentDetailId}");
                    }
                    var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x=>x.Id == appointmentDetail.SalonEmployeeId);
                    if (employee == null)
                    {
                        throw new NotFoundException($"Không tìm thấy nhân viên với id {employee!.Id}");
                    }
                    employee.RatingCount++;
                    employee.RatingSum += item.Rating;
                    employee.Rating = employee.RatingSum/employee.RatingCount;
                    _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(employee);
                }

                for (int i = 0; i < request.ImgFeedbacks.Count; i++)
                {
                    var urlImg = await _mediaservice.UploadAnImage(request.ImgFeedbacks[i], MediaPath.FEEDBACK_IMG, newFeedback.Id.ToString() + "/" + i.ToString());
                    //var urlVideo = await _mediaservice.UploadAVideo(request.Video, MediaPath.FEEDBACK_VIDEO, newFeedback.Id.ToString());
                    StaticFile staticFile = new StaticFile()
                    {
                        Id = Guid.NewGuid(),
                        FeedbackId = newFeedback.Id,
                        Img = urlImg,
                    };
                    await _unitOfWork.GetRepository<StaticFile>().InsertAsync(staticFile);
                }

                decimal totalRating = existingSalon.TotalRating;
                int totalReview = existingSalon.TotalReviewer + 1;
                existingSalon.Rate = (totalRating + (decimal)(ratingSum / request.FeedbackDetailRequests.Count)) / totalReview;
                existingSalon.TotalReviewer = totalReview;
                existingSalon.TotalRating = totalRating + (decimal)ratingSum/request.FeedbackDetailRequests.Count;

                var salon = _mapper.Map<SalonInformation>(existingSalon);

                _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salon);

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

        public async Task<IPaginate<GetFeedbackResponse>> GetFeedBackBySalonId(Guid id, int? rating, int page, int size)
        {
            try
            {
                IPaginate<Feedback> feedbacks;

                if (rating == null)
                {
                    feedbacks = await _unitOfWork.GetRepository<Feedback>()
                       .GetPagingListAsync(
                       predicate: x => x.IsActive == true && x.Appointment.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == id),
                        include: x => x.Include(s => s.StaticFiles).Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails).ThenInclude(s => s.SalonEmployee.SalonInformation).Include(s => s.Customer),
                       page: page,
                       size: size);
                }
                else
                {
                    feedbacks = await _unitOfWork.GetRepository<Feedback>()
                       .GetPagingListAsync(
                       predicate: x => x.IsActive == true && x.Rating == rating && x.Appointment.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == id),
                       include: x => x.Include(s => s.StaticFiles).Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails).ThenInclude(s => s.SalonEmployee.SalonInformation).Include(s => s.Customer),
                       page: page,
                       size: size);
                }

                if (feedbacks == null || feedbacks.Items == null)
                {
                    throw new InvalidOperationException("Không tìm thấy đánh giá");
                }

                foreach (var feedback in feedbacks.Items)
                {
                    feedback.StaticFiles = await _unitOfWork.GetRepository<StaticFile>().GetListAsync(predicate: x => x.FeedbackId == feedback.Id);
                }

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
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }

        public async Task<GetFeedbackResponse> GetFeedBackByAppointmentId(Guid id)
        {
            try
            {
                var feedbacks = await _unitOfWork.GetRepository<Feedback>()
                    .SingleOrDefaultAsync(
                        predicate: x => x.IsActive == true && x.AppointmentId == id,
                        include: x => x.Include(s => s.StaticFiles).Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails).ThenInclude(s => s.SalonEmployee.SalonInformation).Include(s => s.Customer)
                    );
                if (feedbacks != null)
                {
                    feedbacks.StaticFiles = await _unitOfWork.GetRepository<StaticFile>().GetListAsync(predicate: x => x.FeedbackId == feedbacks.Id);
                }
                return _mapper.Map<GetFeedbackResponse>(feedbacks);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }

        public async Task<IPaginate<GetFeedbackResponse>> GetFeedbackByCustomerId(Guid id, int page, int size)
        {
            try
            {
                var cus = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: p => p.Id == id);
                if(cus == null) {
                    throw new InvalidOperationException("Không tìm thấy khách hàng này");
                }
                IPaginate<Feedback> feedbacks;                
                feedbacks = await _unitOfWork.GetRepository<Feedback>()
                    .GetPagingListAsync(
                       predicate: x => x.IsActive == true && x.CustomerId == id,
                       include: x => x.Include(s => s.StaticFiles).Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails).ThenInclude(s => s.SalonEmployee.SalonInformation).Include(s => s.Customer),
                       page: page,
                       size: size);
                

                if (feedbacks == null || feedbacks.Items == null)
                {
                    throw new InvalidOperationException("Không tìm thấy đánh giá");
                }

                foreach (var feedback in feedbacks.Items)
                {
                    feedback.StaticFiles = await _unitOfWork.GetRepository<StaticFile>().GetListAsync(predicate: x => x.FeedbackId == feedback.Id);
                }

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
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }
    }
}

