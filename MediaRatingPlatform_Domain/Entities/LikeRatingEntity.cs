using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.Entities
{
    public class LikeRatingEntity
    {

        [JsonPropertyName("rating_id")]
        public int ratingId { get; set; }

        [JsonPropertyName("user_id")]
        public int userId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime createdAt { get; set; }

        public LikeRatingEntity(int ratingId, int userId)
        {
            this.ratingId = ratingId;
            this.userId = userId;
            this.createdAt = DateTime.UtcNow;
        }
    }
}
