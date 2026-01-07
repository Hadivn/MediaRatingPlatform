using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class MediaRecommendationDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string mediaType { get; set; }
        public string genres { get; set; }
        public int releaseYear { get; set; }
        public int ageRestriction { get; set; }
        public decimal? star { get; set; }
    }
}
