using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class StatisticsOfSalonsParticipating
    { 
        public List<InYear>? InYears {  get; set; }
        public List<InMonth>? InMonths { get; set; }
        public List<InWeek>? InWeeks { get; set; }
    }

    public class InYear
    {
        public string? NumofMonth { get; set; }

        public decimal? value { get; set; }
    }

    public class InMonth
    {
        public string? NumofDate { get; set; }

        public decimal? value { get; set; }
    }

    public class InWeek
    {
        public string? NumofDate { get; set; }

        public decimal? value { get; set; }
    }
}
