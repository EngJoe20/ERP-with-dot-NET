using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application.DTOs.Finance
{
    public class MainSafeDto
    {
        public int Id { get; set; }
        public string SafeName { get; set; } = string.Empty;
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateMainSafeDto
    {
        public string SafeName { get; set; } = string.Empty;
        public decimal OpeningBalance { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateMainSafeDto
    {
        public string? SafeName { get; set; }
        public decimal? OpeningBalance { get; set; }
        public bool? IsActive { get; set; }
    }
}
