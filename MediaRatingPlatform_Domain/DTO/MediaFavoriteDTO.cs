using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    // unnötig
    public class MediaFavoriteDTO
    {
        [JsonPropertyName("media_id")]
        public int mediaId { get; set; }
        
    }
}
