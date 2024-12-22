using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace Maplist;

public class MaplistConfig : BasePluginConfig
{
    public override int Version { get; set; } = 1;

    [JsonPropertyName("CommandCooldown")]
    public int CommandCooldown { get; set; } = 30;

    [JsonPropertyName("EnableMaplist")]
    public bool EnableMaplist { get; set; } = true;

    [JsonPropertyName("EnableRrtv")]
    public bool EnableRrtv { get; set; } = true;

    [JsonPropertyName("DefaultMapId")]
    public string DefaultMapId { get; set; } = "1889424174";

    [JsonPropertyName("RequiredVotePercentage")]
    public float RequiredVotePercentage { get; set; } = 60.0f;
}
