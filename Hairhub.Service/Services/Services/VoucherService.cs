using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using LinqKit;
using Microsoft.EntityFrameworkCore;
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
        public static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<bool> CreateVoucherAsync(CreateVoucherRequest request)
        {
            var existingsalon = await _unitofwork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: e => e.Id == request.SalonInformationId);
            if(existingsalon == null)
            {
                var voucherbyAdmin = _mapper.Map<Voucher>(request);
                voucherbyAdmin.Id = Guid.NewGuid();
                //voucherbyAdmin.SalonInformationId = null;                
                voucherbyAdmin.Code = GenerateRandomCode(10);
                voucherbyAdmin.StartDate = request.StartDate;
                voucherbyAdmin.Quantity = request.Quantity;
                voucherbyAdmin.MaximumDiscount = request.MaximumDiscount;
                voucherbyAdmin.MaximumOrderAmount = request.MaximumOrderAmount;
                voucherbyAdmin.MinimumOrderAmount = request.MinimumOrderAmount;
                voucherbyAdmin.ExpiryDate = (DateTime)request.ExpiryDate;
                voucherbyAdmin.CreatedDate = DateTime.Now;
                voucherbyAdmin.ModifiedDate = DateTime.Now;
                voucherbyAdmin.IsSystemCreated = true;
                voucherbyAdmin.IsActive = true;
                await _unitofwork.GetRepository<Voucher>().InsertAsync(voucherbyAdmin);
                bool isSystemCreated = await _unitofwork.CommitAsync() > 0;
                return isSystemCreated;
            }
            var voucher = _mapper.Map<Voucher>(request);    
            voucher.Id = Guid.NewGuid();
            voucher.SalonInformationId = request.SalonInformationId;
            voucher.Code = GenerateRandomCode(10);
            voucher.StartDate = request.StartDate;
            voucher.Quantity = request.Quantity;
            voucher.MaximumDiscount = request.MaximumDiscount;
            voucher.MaximumOrderAmount = request.MaximumOrderAmount;
            voucher.MinimumOrderAmount = request.MinimumOrderAmount;
            voucher.ExpiryDate = (DateTime)request.ExpiryDate;
            voucher.CreatedDate = DateTime.Now;
            voucher.ModifiedDate = DateTime.Now;
            voucher.IsActive = true;
            voucher.IsSystemCreated = false;
            await _unitofwork.GetRepository<Voucher>().InsertAsync(voucher);
            bool isCreated = await _unitofwork.CommitAsync() > 0;

            return isCreated;          
                
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            var existvoucher = await _unitofwork.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: e => e.Id == id,
               orderBy: null,
               include: null);
            if (existvoucher == null)
            {
                throw new NotFoundException("Voucher not found!");
            }
            existvoucher.IsActive = false;
            _unitofwork.GetRepository<Voucher>().UpdateAsync(existvoucher);
            bool isDelete = await _unitofwork.CommitAsync() > 0;
            return isDelete;
        }

        public async Task<IPaginate<GetVoucherResponse>> GetAdminVoucher(int page, int size)
        {
            var voucher = await _unitofwork.GetRepository<Voucher>()
           .GetPagingListAsync(
                predicate: x=>x.IsSystemCreated==true,
               page: page,
               size: size
           );

            var voucherResponses = new Paginate<GetVoucherResponse>()
            {
                Page = voucher.Page,
                Size = voucher.Size,
                Total = voucher.Total,
                TotalPages = voucher.TotalPages,
                Items = _mapper.Map<IList<GetVoucherResponse>>(voucher.Items),
            };
            return voucherResponses;
        }

        public async Task<IPaginate<GetVoucherResponse>> GetVoucherAsync(int page, int size)
        {
            var voucher = await _unitofwork.GetRepository<Voucher>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.SalonInformation),
               page: page,
               size: size
           );

            var voucherResponses = new Paginate<GetVoucherResponse>()
            {
                Page = voucher.Page,
                Size = voucher.Size,
                Total = voucher.Total,
                TotalPages = voucher.TotalPages,
                Items = _mapper.Map<IList<GetVoucherResponse>>(voucher.Items),
            };
            return voucherResponses;
        }

        public async Task<GetVoucherResponse>? GetVoucherbyCodeAsync(string code)
        {
            Voucher voucherResponse = await _unitofwork
                .GetRepository<Voucher>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Code.Equals(code)
                 );

            if (voucherResponse == null) return null;
            return _mapper.Map<GetVoucherResponse>(voucherResponse);

        }

        public async Task<GetVoucherResponse>? GetVoucherbyIdAsync(Guid id)
        {
            Voucher voucherResponse = await _unitofwork
                .GetRepository<Voucher>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id == id
                 );

            if (voucherResponse == null) return null;
            return _mapper.Map<GetVoucherResponse>(voucherResponse);
        }

        public async Task<List<GetVoucherResponse>> GetVoucherbySalonId(Guid id)
        {
            List<Voucher> voucherResponse = new List<Voucher>();
            voucherResponse = (List<Voucher>)await _unitofwork
                .GetRepository<Voucher>()
                .GetListAsync(
                    predicate: x => x.SalonInformationId == id && x.IsSystemCreated != true,
                    include: query => query.Include(s => s.SalonInformation)
                 );

            return _mapper.Map<List<GetVoucherResponse>>(voucherResponse);
        }

        public async Task<IPaginate<GetVoucherResponse?>> GetVoucherbySalonId(Guid id, int page, int size, string? orderby, bool? filter, string? search)
        {
            var predicate = PredicateBuilder.New<Voucher>(s => s.SalonInformationId == id);

            if (!string.IsNullOrWhiteSpace(search))
            {
                predicate = predicate.And(s => s.Code.Contains(search));
            }

            if (filter.HasValue)
            {
                predicate = predicate.And(x => x.IsActive == filter.Value);
            }


            Func<IQueryable<Voucher>, IOrderedQueryable<Voucher>> orderBy = null;

            if (!string.IsNullOrWhiteSpace(orderby))
            {
                if (orderby.Equals("giá tăng dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderBy(s => s.MinimumOrderAmount);
                }
                else if (orderby.Equals("giá giảm dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderByDescending(s => s.MinimumOrderAmount);
                }
                else if (orderby.Equals("ngày hết hạn tăng dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderBy(s => s.ExpiryDate);
                }
                else if (orderby.Equals("ngày hết hạn giảm dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderByDescending(s => s.ExpiryDate);
                }
                else if (orderby.Equals("phần trăm tăng dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderBy(s => s.DiscountPercentage);
                }
                else if (orderby.Equals("phần trăm giảm dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderByDescending(s => s.DiscountPercentage);
                }
            }


            var vouchers = await _unitofwork.GetRepository<Voucher>()
                                                .GetPagingListAsync(
                                                    predicate: predicate,
                                                    orderBy: orderBy,
                                                    page: page,
                                                    size: size
                                                );


            var voucherResponses = new Paginate<GetVoucherResponse>()
            {
                Page = vouchers.Page,
                Size = vouchers.Size,
                Total = vouchers.Total,
                TotalPages = vouchers.TotalPages,
                Items = _mapper.Map<IList<GetVoucherResponse>>(vouchers.Items),
            };

            return voucherResponses;
        }

        public async Task<IPaginate<GetVoucherResponse?>> GetVouchersByExpiredDate(Guid salonId, int page, int size)
        {
            var now = DateTime.UtcNow;
            var voucher = await _unitofwork.GetRepository<Voucher>()
          .GetPagingListAsync(
              predicate: x => x.SalonInformationId == salonId && x.ExpiryDate > now,
              include: query => query.Include(s => s.SalonInformation),
              page: page,
              size: size
          );

            var voucherResponses = new Paginate<GetVoucherResponse>()
            {
                Page = voucher.Page,
                Size = voucher.Size,
                Total = voucher.Total,
                TotalPages = voucher.TotalPages,
                Items = _mapper.Map<IList<GetVoucherResponse>>(voucher.Items),
            };
            return voucherResponses;
        }
        public async Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherRequest request)
        {
            
            var existVoucher = await _unitofwork.GetRepository<Voucher>().SingleOrDefaultAsync(
            predicate: e => e.Id == id);
            
            if (existVoucher == null)
            {
                throw new Exception("Cannot Find Voucher");
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                existVoucher.Description = request.Description;
            }
            if (request.MinimumOrderAmount != default(decimal))
            {
                existVoucher.MinimumOrderAmount = request.MinimumOrderAmount;
            }
            if (request.MaximumDiscount != default(decimal))
            {
                existVoucher.MaximumDiscount = request.MaximumDiscount;
            }
            if (request.MaximumOrderAmount != default(decimal))
            {
                existVoucher.MaximumOrderAmount = request.MaximumOrderAmount;
            }
            if (request.DiscountPercentage != default(decimal))
            {
                existVoucher.DiscountPercentage = request.DiscountPercentage;
            }
            if (request.Quantity != default(int))
            {
                existVoucher.Quantity = request.Quantity;
            }
            if (request.ExpiryDate != default(DateTime))
            {
                existVoucher.ExpiryDate = request.ExpiryDate;
            }
            if (request.StartDate != default(DateTime))
            {
                existVoucher.StartDate = request.StartDate;
            }
            existVoucher.ModifiedDate = DateTime.Now;
            existVoucher.Id = id;
            _unitofwork.GetRepository<Voucher>().UpdateAsync(existVoucher);
            bool isUpdate = await _unitofwork.CommitAsync()>0;
            return isUpdate;
        }
    }
}
