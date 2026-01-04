using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class LikeRatingDTO
    {
        [JsonPropertyName("rating_id")]
        public int ratingId { get; set; }
    }
}
