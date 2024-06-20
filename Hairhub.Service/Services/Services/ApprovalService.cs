using AutoMapper;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hairhub.Domain.Dtos.Requests.Approval;
using Hairhub.Domain.Dtos.Responses.Approval;
using Hairhub.Domain.Specifications;

namespace Hairhub.Service.Services.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApprovalService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IPaginate<GetApprovalResponse>> GetApprovals(int page, int size)
        {
            var approvals = await _unitOfWork.GetRepository<Approval>()
            .GetPagingListAsync(
                include: query => query.Include(a => a.SalonInformation).Include(a => a.Admin),
                page: page,
                size: size
            );

            var approvalResponses = new Paginate<GetApprovalResponse>()
            {
                Page = approvals.Page,
                Size = approvals.Size,
                Total = approvals.Total,
                TotalPages = approvals.TotalPages,
                Items = _mapper.Map<IList<GetApprovalResponse>>(approvals.Items),
            };

            return approvalResponses;
        }

        public async Task<List<GetApprovalResponse>> GetSalonApprovals(Guid salonId)
        {
            var approvals = await _unitOfWork.GetRepository<Approval>()
            .GetListAsync(
                predicate: x => x.SalonInformationId == salonId,
                include: query => query.Include(a => a.SalonInformation).Include(a => a.Admin)
            );

            if (approvals == null)
            {
                throw new NotFoundException($"Không tìm thấy phê duyệt của salon với id {salonId}");
            }

            return _mapper.Map<List<GetApprovalResponse>>(approvals);
        }

        public async Task<GetApprovalResponse> GetApprovalById(Guid id)
        {
            var approval = await _unitOfWork.GetRepository<Approval>()
               .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id),
               include: x => x.Include(a => a.SalonInformation).Include(a => a.Admin));

            if (approval == null)
            {
                throw new NotFoundException("Approval not found");
            }

            var approvalResponse = _mapper.Map<GetApprovalResponse>(approval);
            return approvalResponse;
        }

        public async Task<bool> CreateApproval(CreateApprovalRequest request)
        {
            var salonInformation = await _unitOfWork.GetRepository<SalonInformation>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(request.SalonInformationId));

            if (salonInformation == null)
            {
                throw new NotFoundException($"Salon information with id {request.SalonInformationId} not found");
            }

            if (request.Status != "approved" && request.Status != "rejected")
            {
                throw new ArgumentException("Invalid status value. Allowed values are 'approved' or 'rejected'.");
            }

            salonInformation.Status = request.Status;

            Approval newApproval = new Approval()
            {
                Id = Guid.NewGuid(),
                SalonInformationId = request.SalonInformationId,
                AdminId = request.AdminId,
                ReasonReject = request.Status == "rejected" ? request.ReasonReject : null,
                CreateDate = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Approval>().InsertAsync(newApproval);

            _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salonInformation);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }



        public async Task<bool> UpdateApproval(Guid id, UpdateApprovalRequest request)
        {
            var approval = await _unitOfWork.GetRepository<Approval>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (approval == null)
            {
                throw new Exception("Approval does not exist");
            }

            var salonInformation = await _unitOfWork.GetRepository<SalonInformation>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(request.SalonInformationId));

            if (salonInformation == null)
            {
                throw new NotFoundException($"Salon information with id {request.SalonInformationId} not found");
            }

            if (request.Status != "approved" && request.Status != "rejected")
            {
                throw new ArgumentException("Invalid status value. Allowed values are 'approved' or 'rejected'.");
            }

            salonInformation.Status = request.Status;

            approval.SalonInformationId = request.SalonInformationId;
            approval.AdminId = request.AdminId;
            approval.ReasonReject = request.Status == "rejected" ? request.ReasonReject : null;
            approval.UpdateDate = request.UpdateDate ?? DateTime.UtcNow;

            _unitOfWork.GetRepository<Approval>().UpdateAsync(approval);
            _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salonInformation);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }



        public async Task<bool> DeleteApproval(Guid id)
        {
            var approval = await _unitOfWork.GetRepository<Approval>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (approval == null)
            {
                throw new Exception("Approval does not exist");
            }

            _unitOfWork.GetRepository<Approval>().DeleteAsync(approval);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}
