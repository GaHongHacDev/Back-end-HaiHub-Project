﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Authentication
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; }
    }
}
