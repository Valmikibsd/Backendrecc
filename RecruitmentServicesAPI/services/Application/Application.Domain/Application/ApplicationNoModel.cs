using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Application
{
    public class DtoApplicationNo
    {
        public int Year { get; set; }

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public int AppNo { get; set; }
    }


    public class ApplicationNoModel
    {
        public int SrNo { get; set; }

        public int Year { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public int AdvertisementId { get; set; }

        public string? AdvertisementName { get; set; }

        public int PostId { get; set; }

        public string? PostName { get; set; }

        public int AppNo { get; set; }
    }
}
