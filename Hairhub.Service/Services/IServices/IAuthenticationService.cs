﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IAuthenticationService
    {
        Task<string> Login(string userName, string password);
        Task<string> Logout(string token);
    }
}