using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class MediaDeleteDTO
    {
        public int Id { get; set; }
        public DateTime? DeletedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = true;
    }
}
