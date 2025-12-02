using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class RatingDTO
    {
        public int userId { get; set; }
        public int mediaId { get; set; }
        // can only be 1 - 5
        public decimal ratingValue { get; set; }
        // optional
        public string? comment { get; set; }
        public DateTime timestamp { get; set; }
        public bool confirmation { get; set; }

    }
}
