using MediaRatingPlatform_Domain.ENUM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.Entities
{
    public class MediaRatingEntity
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("star")]
        public int star { get; set; }

        [JsonPropertyName("comment")]
        public string? comment { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime createdAt { get; set; }

        [JsonPropertyName("user_id")]
        public int userId { get; set; }

        [JsonPropertyName("media_id")]
        public int mediaId { get; set; }

        [JsonPropertyName("is_confirmed")]
        public bool isConfirmed { get; set; } = false;


        public MediaRatingEntity(int star, string? comment, int userId, int mediaId, bool isConfirmed)
        {
            this.star = star;
            this.comment = comment;
            this.userId = userId;
            this.mediaId = mediaId;
            this.createdAt = DateTime.UtcNow;
            this.isConfirmed = isConfirmed;
        }

    }
}
