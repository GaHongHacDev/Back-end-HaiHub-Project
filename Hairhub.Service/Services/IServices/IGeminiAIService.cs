using Hairhub.Domain.Dtos.Requests.AI;
using Hairhub.Domain.Dtos.Responses.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IGeminiAIService
    {
        public Task<string> ChatMessage(AIChatMessageRequest request);
    }
}
