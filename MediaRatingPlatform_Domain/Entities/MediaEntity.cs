using MediaRatingPlatform_Domain.ENUM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.Entities
{
    public class MediaEntity
    {
        public int id { get; set; }
        public string title { get; set; } 
        public string description { get; set; }
        [JsonPropertyName("media_type")]
        public EMediaType mediaType { get; set; }
        [JsonPropertyName("release_year")]
        public int releaseYear { get; set; }
        public string genres { get; set; }
        [JsonPropertyName("age_restriction")]
        public int ageRestriction { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int userId { get; set; }

        public MediaEntity(string title, string description, EMediaType mediaType, int releaseYear,
            string genres, int ageRestriction, int userId)
        {
            this.title = title;
            this.description = description;
            this.mediaType = mediaType;
            this.releaseYear = releaseYear;
            this.genres = genres;
            this.ageRestriction = ageRestriction;
            this.userId = userId;
        }
    }
}
