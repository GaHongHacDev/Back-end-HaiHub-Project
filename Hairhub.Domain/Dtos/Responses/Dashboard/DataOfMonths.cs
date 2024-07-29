using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace Hairhub.Domain.Dtos.Responses.Dashboard
    {
        public class DataOfMonths
        {
            public int? Jan { get; set; }
            public int? Feb { get; set; }
            public int? March { get; set; }
            public int? April { get; set; }
            public int? May { get; set; }
            public int? June { get; set; }
            public int? July { get; set; }
            public int? August { get; set; }
            public int? September { get; set; }
            public int? October { get; set; }
            public int? November { get; set; }
            public int? December { get; set; }


        }

    public class RatioData
    {
        public string Status { get; set; }
        public double Percentage { get; set; }
    }


    public class MonthlyRatioData
    {
        public string Month { get; set; }
        public double Success { get; set; }
        public double Failed { get; set; }
        public double Canceled { get; set; }
    }
}
