using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class LeaderboardDTO
    {
        public int userId { get; set; }
        public int ratingCount { get; set; }
    }
}
