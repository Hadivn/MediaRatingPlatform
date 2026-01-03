using MediaRatingPlatform_Domain.ENUM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class MediaDTO
    {
        [JsonPropertyName("title")]
        public string title { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("media_type")]
        public EMediaType mediaType { get; set; }

        [JsonPropertyName("release_year")]
        public int releaseYear { get; set; }

        [JsonPropertyName("age_restriction")]
        public int ageRestriction { get; set; }

        [JsonPropertyName("genres")]
        public string genres { get; set; }

        
    }

}

