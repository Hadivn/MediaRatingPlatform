using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaRatingPlatform_Domain.DTO
{
    public class MediaRatingUpdateDTO
    {
        // 1 to 5 stars
        [JsonPropertyName("star")]
        public int? star { get; set; }

        [JsonPropertyName("comment")]
        public string? comment { get; set; }

        [JsonPropertyName("is_confirmed")]
        public bool? isConfirmed { get; set; } = false;

    }
}
