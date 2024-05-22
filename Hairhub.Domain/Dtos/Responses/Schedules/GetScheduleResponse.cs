﻿using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Schedules
{
    public class GetScheduleResponse
    {
        public GetScheduleResponse()
        {
            
        }

        public GetScheduleResponse(Guid id, Guid employeeId, string? date, TimeOnly? startTime, TimeOnly? endTime, bool? isActive)
        {
            Id = id;
            EmployeeId = employeeId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            IsActive = isActive;
        }

        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool? IsActive { get; set; }
        public SalonEmployeeResponseS SalonEmployee { get; set; }
    }

    public class SalonEmployeeResponseS
    {
        public SalonEmployeeResponseS()
        {

        }
        public SalonEmployeeResponseS(Guid id, Guid salonInformationId, string? fullName, DateTime? dayOfBirth, string? gender, string? email, string? phone, string? address, string? humanId, string? img, bool? isActive)
        {
            Id = id;
            SalonInformationId = salonInformationId;
            FullName = fullName;
            DayOfBirth = dayOfBirth;
            Gender = gender;
            Email = email;
            Phone = phone;
            Address = address;
            HumanId = humanId;
            Img = img;
            IsActive = isActive;
        }

        public Guid Id { get; set; }
        public Guid SalonInformationId { get; set; }
        public string? FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? HumanId { get; set; }
        public string? Img { get; set; }
        public bool? IsActive { get; set; }
    }
}