﻿using Hairhub.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class SMSService : ISMSService
    {
        public Task<bool> SendOtpSMS()
        {
            throw new NotImplementedException();
        }
    }
}