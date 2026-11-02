using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Application
{
    public class HallTicketRequest
    {
        public long UserId { get; set; }

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public string ApplicationId { get; set; } = string.Empty;
    }
}
