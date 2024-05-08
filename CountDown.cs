using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Timers;



namespace CountDown;
[MinimumApiVersion(215)]

public static class GetUnixTime
{
    public static int GetUnixEpoch(this DateTime dateTime)
    {
        var unixTime = dateTime.ToUniversalTime() -
                       new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return (int)unixTime.TotalSeconds;
    }
}
public partial class CountDown : BasePlugin
{
    public override string ModuleName => "Countdown";
    public override string ModuleAuthor => "DeadSwim";
    public override string ModuleDescription => "Simple plugin for countdown and stopwatch.";
    public override string ModuleVersion => "V. 1.0.0";



    public float Time;
    public string Text;
    public string color;
    public bool Countdown_enable;
    public bool Countdown_enable_text;
    public bool Stopwatch_enable;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_1;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_2;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_3;


    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnMapStart>(name =>
        {
            Countdown_enable = false;
            Countdown_enable_text = false;
            Stopwatch_enable = false;
            Text = "";
            Time = 0;

        });
        RegisterListener<Listeners.OnTick>(() =>
        {
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                var ent = NativeAPI.GetEntityFromIndex(i);
                if (ent == 0)
                    continue;

                var client = new CCSPlayerController(ent);
                if (client == null || !client.IsValid)
                    continue;



                if (Countdown_enable)
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='green'>[{Time} seconds]</font> <font color='gray'>◄</font><br>" +
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>"
                    );
                }
                if (Countdown_enable_text)
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-l' color='gold'>[{Text}]</font><font color='gray'>◄</font><br>"+
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='green'>[{Time} seconds]</font> <font color='gray'>◄</font><br>" +
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>"
                    );
                }
                if (Stopwatch_enable)
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>STOPWATCH</font><font color='gray'>----</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='green'>[{Time} seconds]</font> <font color='gray'>◄</font><br>" +
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>STOPWATCH</font><font color='gray'>----</font><br>"
                    );
                }
            }
        });
    }
    private bool IsInt(string sVal)
    {
        foreach (char c in sVal)
        {
            int iN = (int)c;
            if ((iN > 57) || (iN < 48))
                return false;
        }
        return true;
    }
    [ConsoleCommand("css_stopwatch", "Start stopwatch")]
    public void CommandStartStopwatch(CCSPlayerController? player, CommandInfo info)
    {
        if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You are not admin..");
            return;
        }
        if (Stopwatch_enable == true)
        {
            player.PrintToChat($"  [{ChatColors.Lime}CountDown{ChatColors.Default}] StopWatch be turned off!");
            Stopwatch_enable = false;
            timer_3?.Kill();
            return;
        }
        if (Countdown_enable == true || Countdown_enable_text == true)
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must wait for end one countdown.");
            return;
        }
        Stopwatch_enable = true;
        timer_3 = AddTimer(1.0f, () =>
        {
            Random rnd = new Random();
            int random_num = rnd.Next(5);
            if (random_num == 0) { color = "green"; }
            if (random_num == 1) { color = "red"; }
            if (random_num == 2) { color = "blue"; }
            if (random_num == 3) { color = "gold"; }
            if (random_num == 4) { color = "orange"; }
            Time = Time + 1.0f;
        }, TimerFlags.REPEAT);
    }
    [ConsoleCommand("css_countdown", "Start countdown")]
    public void CommandStartCountDown(CCSPlayerController? player, CommandInfo info)
    {
        if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You are not admin..");
            return;
        }
        var TimeSec = info.ArgByIndex(1);
        if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec))
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must use that {ChatColors.Lime}/countdown <TIME_SECONDS>{ChatColors.Default}.");
            return;
        }
        if(Countdown_enable == true || Countdown_enable_text == true || Stopwatch_enable == true)
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must wait for end one countdown.");
            return;
        }
        var time_convert = Convert.ToInt32(TimeSec);
        Time = time_convert;
        Countdown_enable = true;
        timer_1 = AddTimer(1.0f, () =>
        {
            if (Time <= 0.0)
            {
                Countdown_enable = false;
                timer_1?.Kill();

                return;
            }
            Random rnd = new Random();
            int random_num = rnd.Next(5);
            if (random_num == 0) { color = "green"; }
            if (random_num == 1) { color = "red"; }
            if (random_num == 2) { color = "blue"; }
            if (random_num == 3) { color = "gold"; }
            if (random_num == 4) { color = "orange"; }
            Time = Time - 1.0f;
        }, TimerFlags.REPEAT);
    }
    [ConsoleCommand("css_countdown_text", "Start countdown with text")]
    public void CommandStartTextCountDown(CCSPlayerController? player, CommandInfo info)
    {
        if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You are not admin..");
            return;
        }
        var TimeSec = info.ArgByIndex(1);
        var Text_var = info.ArgByIndex(2);

        if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec))
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must use that {ChatColors.Lime}/countdown_text <TIME_SECONDS> <TEXT>{ChatColors.Default}.");
            return;
        }
        if (Text_var == null || Text_var == "")
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must use that {ChatColors.Lime}/countdown_text <TIME_SECONDS> <TEXT>{ChatColors.Default}.");
            return;
        }
        if (Countdown_enable == true || Countdown_enable_text == true || Stopwatch_enable == true)
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must wait for end one countdown.");
            return;
        }
        var time_convert = Convert.ToInt32(TimeSec);
        Time = time_convert;
        Text = Text_var;
        Countdown_enable_text = true;
        timer_2 = AddTimer(1.0f, () =>
        {
            if (Time <= 0.0)
            {
                Countdown_enable_text = false;
                timer_2?.Kill();

                return;
            }
            Random rnd = new Random();
            int random_num = rnd.Next(5);
            if (random_num == 0) { color = "green"; }
            if (random_num == 1) { color = "red"; }
            if (random_num == 2) { color = "blue"; }
            if (random_num == 3) { color = "gold"; }
            if (random_num == 4) { color = "orange"; }
            Time = Time - 1.0f;
        }, TimerFlags.REPEAT);
    }
}
