using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.Entities
{
    public class MediaFavoriteEntity
    {

        [JsonPropertyName("media_id")]
        public int mediaId { get; set; }

        [JsonPropertyName("usre_id")]
        public int userId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime createdAt { get; set; }

        public MediaFavoriteEntity(int mediaId, int userId)
        {
            this.mediaId = mediaId;
            this.userId = userId;
            this.createdAt = DateTime.UtcNow;
        }

    }
}
