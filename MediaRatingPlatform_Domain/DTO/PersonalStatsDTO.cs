using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class PersonalStatsDTO
    {
        public int ratingCount { get; set; }
        public double averageRating { get; set; }
        public string favoriteGenre { get; set; }
    }
}
