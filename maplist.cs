using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json;

namespace Maplist;

public class Maplist : BasePlugin, IPluginConfig<MaplistConfig>
{
    public MaplistConfig Config { get; set; } = new MaplistConfig();

    public override string ModuleName => "Maplist";
    public override string ModuleDescription => "rockthevote Maplist &Rrtv";
    public override string ModuleAuthor => "小彩旗";
    public override string ModuleVersion => "0.0.9";

    public void OnConfigParsed(MaplistConfig config)
    {
        Config = config;
    }
    private string maplistPath = string.Empty;
    private Dictionary<ulong, DateTime> _lastCommandUse = new Dictionary<ulong, DateTime>();
    private DateTime _lastMapListRead = DateTime.MinValue;

    public override void Load(bool hotReload)
    {
        string rtvPath = Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "plugins", "RockTheVote", "maplist.txt");
        maplistPath = rtvPath;
        Console.WriteLine(Localizer["MaplistPath", maplistPath]);
    }

    [ConsoleCommand("css_maplist", "Display the map list in the console.")]
    public void OnMaplistCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (!Config.EnableMaplist)
        {
            return;
        }
        try
        {
            if (player != null)
            {
                ulong steamId = player.SteamID;
                if (_lastCommandUse.TryGetValue(steamId, out DateTime lastUse))
                {
                    int secondsLeft = Config.CommandCooldown - (int)(DateTime.Now - lastUse).TotalSeconds;
                    if (secondsLeft > 0)
                    {
                        player.PrintToChat($" {ChatColors.Blue}[Maplist]{ChatColors.Default} {Localizer["WaitCooldown", secondsLeft]}");
                        return;
                    }
                }
                _lastCommandUse[steamId] = DateTime.Now;
            }

            if (!File.Exists(maplistPath))
            {
                ReplyToCommand(player, Localizer["MaplistFileNotExist"]);
                return;
            }

            if (player != null)
            {
                player.PrintToChat($" {ChatColors.Blue}[Maplist]{ChatColors.Default} {Localizer["MaplistPrintConsole"]}");
                player.PrintToConsole("echo ====================================");
                player.PrintToConsole($"echo {Localizer["ServerMaplist"]}");
                player.PrintToConsole("echo ====================================");
                
                string[] maps = File.ReadAllLines(maplistPath);
                foreach (string map in maps)
                {
                    if (!string.IsNullOrWhiteSpace(map))
                    {
                        string mapName = map.Split(':')[0];
                        player.PrintToConsole(mapName);
                    }
                }
                
                player.PrintToConsole("echo ====================================");
                player.PrintToConsole($"echo {Localizer["TotalMaps", maps.Length]}");
                player.PrintToConsole($"echo {Localizer["MaplistEnd"]}");
                player.PrintToConsole("echo ====================================");
            }
            else
            {
                Console.WriteLine("====================================");
                Console.WriteLine($"             {Localizer["ServerMaplist"]}");
                Console.WriteLine("====================================");
                
                string[] maps = File.ReadAllLines(maplistPath);
                foreach (string map in maps)
                {
                    if (!string.IsNullOrWhiteSpace(map))
                    {
                        string mapName = map.Split(':')[0];
                        Console.WriteLine(mapName);
                    }
                }
                
                Console.WriteLine("====================================");
                Console.WriteLine($"             {Localizer["TotalMaps", maps.Length]}");
                Console.WriteLine($"             {Localizer["MaplistEnd"]}");
                Console.WriteLine("====================================");
            }
        }
        catch (Exception ex)
        {
            ReplyToCommand(player, Localizer["ReadMaplistError", ex.Message]);
        }
    }

    private void ReplyToCommand(CCSPlayerController? player, string message)
    {
        if (player == null)
        {
            Console.WriteLine(message);
        }
        else
        {
            player.PrintToChat($" {ChatColors.Blue}[Maplist]{ChatColors.Default} {message}");
        }
    }

    private HookResult OnMapStart(EventGameStart @event, GameEventInfo info)
    {
        _lastMapListRead = DateTime.MinValue;
        _lastCommandUse.Clear();
        
        return HookResult.Continue;
    }
}