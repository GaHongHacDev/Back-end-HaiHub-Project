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

        public static class Voucher
        {
            public const string VoucherEndpoint = ApiEndpoint + "/vouchers";

        }
        public static class AppointmentDetail
        {
            public const string AppointmentDetailsEndpoint = ApiEndpoint + "/appointment_details";
        }

        public static class Otp
        {
            public const string OtpsEndpoint = ApiEndpoint + "/otps";
        }

        public static class Config
        {
            public const string ConfigEndpoint = ApiEndpoint + "/otps";
        }

    }
    }
