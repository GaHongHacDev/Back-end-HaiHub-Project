﻿using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Dtos.Responses.SalonInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Reports
{
    public class GetReportResponse
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public string RoleNameReport { get; set; } // Customer or SalonOwner
        public string? ReasonReport { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? TimeConfirm { get; set; }
        public string? DescriptionAdmin { get; set; }
        public string Status { get; set; }
        public GetAppointmentResponse Appointment { get; set; }
        public List<FileReportResponse> FileReports { get; set; }
        public GetSalonInformationResponse SalonInformation { get; set; }
        public CustomerAppointment Customer { get; set; }
    }

    public class FileReportResponse
    {
        public Guid Id { get; set; }
        public Guid? ReportId { get; set; }
        public string? Img { get; set; }
        public string? Video { get; set; }
    }
}
