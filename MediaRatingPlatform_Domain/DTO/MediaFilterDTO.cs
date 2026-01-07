using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class MediaFilterDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string mediaType { get; set; }
        public string genres { get; set; }
        public int releaseYear { get; set; }
        public int ageRestriction { get; set; }
        public decimal? averageStar { get; set; }
        public long? totalRatings { get; set; }
    }
}
