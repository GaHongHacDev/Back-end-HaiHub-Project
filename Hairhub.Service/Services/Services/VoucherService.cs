using AutoMapper;
using CloudinaryDotNet.Actions;
using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Entitities;
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
            var voucher = _mapper.Map<Voucher>(request);    
            voucher.Id = Guid.NewGuid();
            voucher.Code = GenerateRandomCode(10);

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
            _unitofwork.GetRepository<Voucher>().DeleteAsync(existvoucher);
            bool isDelete = await _unitofwork.CommitAsync() > 0;
            return isDelete;
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
                    predicate: x => x.Id.Equals(id)
                 );

            if (voucherResponse == null) return null;
            return _mapper.Map<GetVoucherResponse>(voucherResponse);
        }

        public async Task<GetVoucherResponse?> GetVoucherbySalonId(Guid id)
        {
            Voucher voucherResponse = await _unitofwork
                .GetRepository<Voucher>()
                .SingleOrDefaultAsync(
                    predicate: x => x.SalonInformationId.Equals(id),
                    include: query => query.Include(s => s.SalonInformation)
                 );

            if (voucherResponse == null) return null;
            return _mapper.Map<GetVoucherResponse>(voucherResponse);
        }

        public async Task<IPaginate<GetVoucherResponse?>> GetVoucherbySalonId(Guid id, int page, int size)
        {
            var voucher = await _unitofwork.GetRepository<Voucher>()
          .GetPagingListAsync(
                predicate: x => x.SalonInformationId.Equals(id) && (x.IsSystemCreated || x.IsSystemCreated == true),
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

        public async Task<IPaginate<GetVoucherResponse?>> GetVouchersByExpiredDate(Guid salonId, int page, int size)
        {
            var now = DateTime.UtcNow;
            var voucher = await _unitofwork.GetRepository<Voucher>()
          .GetPagingListAsync(
              predicate: x => x.SalonInformationId.Equals(salonId) && x.ExpiryDate > now,
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

            existVoucher = _mapper.Map<Voucher>(request);

            _unitofwork.GetRepository<Voucher>().UpdateAsync(existVoucher);
            bool isUpdate = await _unitofwork.CommitAsync()>0;
            return isUpdate;
        }
    }
}
