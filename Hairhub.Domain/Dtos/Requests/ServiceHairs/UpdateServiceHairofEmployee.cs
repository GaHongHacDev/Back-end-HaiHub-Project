﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.ServiceHairs
{
    public class UpdateServiceHairofEmployee
    {
        public List<Guid>? removeServiceID {  get; set; }

        public List<Guid>? addServiceID { get; set;}

    }
}
