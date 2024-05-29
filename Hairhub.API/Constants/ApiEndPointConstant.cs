namespace Hairhub.API.Constants
{
    public static class ApiEndPointConstant
    {
        static ApiEndPointConstant()
        {
        }

        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public static class Authentication
        {
            public const string AuthenticationEndpoint = ApiEndpoint + "/auth";
            public const string Login = AuthenticationEndpoint + "/login";
            public const string Info = AuthenticationEndpoint + "/info";
        }

        public static class Account
        {
            public const string AccountsEndpoint = ApiEndpoint + "/accounts";
            public const string AccountEndpoint = AccountsEndpoint + "/{id}";
        }

        public static class Customer
        {
            public const string CustomersEndpoint = ApiEndpoint + "/customers";
        }

        public static class Appointment
        {
            public const string AppointmentsEndpoint = ApiEndpoint + "/appointments";
        }
        public static class AppointmentDetail
        {
            public const string AppointmentDetailsEndpoint = ApiEndpoint + "/appointment_details";
        }

        public static class Role
        {
            public const string RolesEndpoint = ApiEndpoint + "/roles";
            public const string RoleEndpoint = RolesEndpoint + "/{id}";
        }
        public static class Schedule
        {
            public const string SchedulesEndpoint = ApiEndpoint + "/roles";
            public const string ScheduleEndpoint = SchedulesEndpoint + "/{id}";
        }

        public static class Feedback
        {
            public const string FeedbacksEndpoint = ApiEndpoint + "/roles";
            public const string FeedbackEndpoint = FeedbacksEndpoint + "/{id}";
        }

        public static class AppointmentDetailVoucher
        {
            public const string AppointmentDetailVouchersEndpoint = ApiEndpoint + "/roles";
            public const string AppointmentDetailVoucherEndpoint = AppointmentDetailVouchersEndpoint + "/{id}";
        }

        public static class Otp
        {
            public const string OtpsEndpoint = ApiEndpoint + "/otps";
        }
    }
}
