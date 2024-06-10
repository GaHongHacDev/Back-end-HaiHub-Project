using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;
        public VoucherService(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
        }

        public async Task<CreateVoucherResponse> CreateVoucherAsync(CreateVoucherRequest request)
        {
            var voucherRequest = new Voucher()
            {
                SalonInformationId = (Guid)request.SalonInformationId,
                Code = request.Code,
                Description = request.Description,
                MinimumOrderAmount = request.MinimumOrderAmount,
                DiscountPercentage = request.DiscountPercentage,
                ExpiryDate = request.ExpiryDate,
                CreatedDate = request.CreatedDate,
                ModifiedDate = request.ModifiedDate,
                IsSystemCreated = request.IsSystemCreated,
                IsActive = request.IsActive,
            };

            await _unitofwork.GetRepository<Voucher>().InsertAsync(voucherRequest);
            await _unitofwork.CommitAsync();

            return _mapper.Map<CreateVoucherResponse>(voucherRequest);            
                
        }

        public async Task DeleteVoucherAsync(Guid id)
        {
            var existvoucher = await _unitofwork.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: e => e.Id == id,
               orderBy: null,
               include: null);
            if (existvoucher == null)
            {
                throw new Exception("Voucher Not Found");
            }

            _unitofwork.GetRepository<Voucher>().DeleteAsync(existvoucher);
            _unitofwork.Commit();
        }

        public async Task<IPaginate<GetVoucherResponse>> GetAllVoucherAsync(int page, int size)
        {
            IPaginate<GetVoucherResponse> voucher = await _unitofwork.GetRepository<Voucher>()
                .GetPagingListAsync(selector: x => new GetVoucherResponse(x.Id, x.SalonInformationId, 
                x.Description, x.Code, x.MinimumOrderAmount, x.DiscountPercentage, x.ExpiryDate, x.CreatedDate
                , x.ModifiedDate, x.IsSystemCreated, x.IsActive) , page: page, size: size, orderBy: x => x.OrderBy(x => x.CreatedDate));

            return voucher;
        }

        public async Task<GetVoucherResponse>? GetVoucherbyCodeAsync(string code)
        {
            GetVoucherResponse voucherresponse = await _unitofwork.GetRepository<Voucher>().
                SingleOrDefaultAsync(selector: x => new GetVoucherResponse(x.Id, x.SalonInformationId,
                x.Description, x.Code, x.MinimumOrderAmount, x.DiscountPercentage, x.ExpiryDate, x.CreatedDate
                , x.ModifiedDate, x.IsSystemCreated, x.IsActive), predicate: x => x.Code.Equals(code));

            if (voucherresponse == null) return null;
            return voucherresponse;

        }

        public async Task<GetVoucherResponse>? GetVoucherbyIdAsync(Guid id)
        {
            GetVoucherResponse voucherresponse = await _unitofwork.GetRepository<Voucher>().
                SingleOrDefaultAsync(selector: x => new GetVoucherResponse(x.Id, x.SalonInformationId,
                x.Description, x.Code, x.MinimumOrderAmount, x.DiscountPercentage, x.ExpiryDate, x.CreatedDate
                , x.ModifiedDate, x.IsSystemCreated, x.IsActive), predicate: x => x.Id.Equals(id));

            if (voucherresponse == null) return null;
            return voucherresponse;
        }

        public async Task<UpdateVoucherResponse> UpdateVoucherAsync(Guid id, UpdateVoucherRequest request)
        {
            var vouchermap = _mapper.Map<Voucher>(request);
            var existVoucher = await _unitofwork.GetRepository<Voucher>().SingleOrDefaultAsync(
            predicate: e => e.Id == id,
            orderBy: null,
            include: null);
            
            if (existVoucher == null)
            {
                throw new KeyNotFoundException("Cannot Find Voucher");
            }

            existVoucher.SalonInformationId = vouchermap.SalonInformationId;
            existVoucher.Description = vouchermap.Description;
            existVoucher.Code = vouchermap.Code;
            existVoucher.MinimumOrderAmount = vouchermap.MinimumOrderAmount;
            existVoucher.DiscountPercentage = vouchermap.DiscountPercentage;
            existVoucher.ExpiryDate = vouchermap.ExpiryDate;
            existVoucher.CreatedDate = vouchermap.CreatedDate;
            existVoucher.ModifiedDate = vouchermap.ModifiedDate;
            existVoucher.IsSystemCreated = vouchermap.IsSystemCreated;
            existVoucher.IsActive = vouchermap.IsActive;

            _unitofwork.GetRepository<Voucher>().UpdateAsync(existVoucher);
            _unitofwork.Commit();
            return _mapper.Map<UpdateVoucherResponse>(existVoucher);
        }
    }
}
