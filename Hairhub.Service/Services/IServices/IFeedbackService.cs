using Hairhub.Domain.Dtos.Requests.Feedbacks;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Dtos.Responses.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IFeedbackService
    {
        Task<IPaginate<GetFeedbackResponse>> GetFeedbacks(int page, int size);
        Task<GetFeedbackResponse> GetFeedbackById(Guid id);
        Task<bool> CreateFeedback(CreateFeedbackRequest request);
        Task<bool> UpdateFeedback(Guid id, UpdateFeedbackRequest request);
        Task<bool> DeleteFeedback(Guid id);
    }
}
