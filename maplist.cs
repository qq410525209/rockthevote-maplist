using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using CounterStrikeSharp.API.Modules.Utils;

namespace maplist
{
    public class Maplist : BasePlugin
    {
        public override string ModuleName => "Maplist";
        public override string ModuleDescription => "";
        public override string ModuleAuthor => "小彩旗";
        public override string ModuleVersion => "0.0.1";

        private string maplistPath = string.Empty;
        private Dictionary<ulong, DateTime> _lastCommandUse = new Dictionary<ulong, DateTime>();
        private const int CommandCooldown = 30; // 冷却时间（秒）

        public override void Load(bool hotReload)
        {
            // 构建完整的 RockTheVote 插件地图列表路径
            string rtvPath = Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "plugins", "RockTheVote", "maplist.txt");
            maplistPath = rtvPath;
            Console.WriteLine($"RTV地图列表文件路径: {maplistPath}");
        }

        [ConsoleCommand("css_maplist", "显示可用地图列表")]
        public void OnMaplistCommand(CCSPlayerController? player, CommandInfo command)
        {
            try
            {
                // 检查CD（仅对玩家生效，控制台不受限制）
                if (player != null)
                {
                    ulong steamId = player.SteamID;
                    if (_lastCommandUse.TryGetValue(steamId, out DateTime lastUse))
                    {
                        int secondsLeft = CommandCooldown - (int)(DateTime.Now - lastUse).TotalSeconds;
                        if (secondsLeft > 0)
                        {
                            player.PrintToChat($"{ChatColors.Blue}[Maplist]{ChatColors.Default} 请等待 {secondsLeft} 秒后再使用此命令");
                            return;
                        }
                    }
                    _lastCommandUse[steamId] = DateTime.Now;
                }

                if (!File.Exists(maplistPath))
                {
                    ReplyToCommand(player, "地图列表文件不存在!");
                    return;
                }

                if (player != null)
                {
                    player.PrintToChat($" {ChatColors.Blue}[Maplist]{ChatColors.Default} 已将地图列表打印至控制台，请按 ~ 键查看");
                    player.PrintToConsole("echo ====================================");
                    player.PrintToConsole("echo             服务器地图列表");
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
                    player.PrintToConsole($"echo             总地图数量: {maps.Length}");
                    player.PrintToConsole("echo             地图列表显示完毕");
                    player.PrintToConsole("echo ====================================");
                }
                else
                {
                    Console.WriteLine("====================================");
                    Console.WriteLine("             服务器地图列表");
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
                    Console.WriteLine($"             总地图数量: {maps.Length}");
                    Console.WriteLine("             地图列表显示完毕");
                    Console.WriteLine("====================================");
                }
            }
            catch (Exception ex)
            {
                ReplyToCommand(player, $"读取地图列表时发生错误: {ex.Message}");
            }
        }

        private void ReplyToCommand(CCSPlayerController? player, string message)
        {
            if (player == null)
            {
                // 如果是从服务器控制台执行的命令
                Console.WriteLine(message);
            }
            else
            {
                // 如果是玩家执行的命令
                player.PrintToChat($" {ChatColors.Blue}[Maplist]{ChatColors.Default} {message}");
            }
        }
    }
}