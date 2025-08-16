using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StorySpoiler.Models;

public class StoryDto
{
    [JsonPropertyName ("Title")]
    public string Title { get; set; }
    
    [JsonPropertyName ("Description")]
    public string Description { get; set; }
    
    [JsonPropertyName ("Url")]
    public string Url { get; set; }
    
}