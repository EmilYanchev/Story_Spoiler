using System.Text.Json.Serialization;

namespace StorySpoiler.Models;

public class ApiResponseDto
{
    [JsonPropertyName ("Msg")]
    public string Msg { get; set; }
    
    [JsonPropertyName ("StoryId")]
    public int StoryId { get; set; }
}