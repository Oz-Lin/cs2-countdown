using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Timers;
using System;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;



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
    public override string ModuleAuthor => "DeadSwim, Oz-Lin";
    public override string ModuleDescription => "Simple plugin for countdown and stopwatch.";
    public override string ModuleVersion => "V. 1.0.6b";



    public float Time;
    public string Text = "";
    public string color = "";
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
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time} seconds]</font> <font color='gray'>◄</font>" 
                    //$"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>"
                    );
                }
                if (Countdown_enable_text)
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-m' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='gold'>[{Text}]</font><font color='gray'>◄</font><br>"+
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time} seconds]</font> <font color='gray'>◄</font>" 
                    //$"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>"
                    );
                }
                if (Stopwatch_enable)
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>STOPWATCH</font><font color='gray'>----</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time} seconds]</font> <font color='gray'>◄</font>" 
                    //$"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>STOPWATCH</font><font color='gray'>----</font><br>"
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

    private string ParseInt(string sVal)
    {
        string strNumber = "";
        foreach (char c in sVal)
        {
            int iN = (int)c;
            if ((iN > 57) || (iN < 48))
                return strNumber;
            strNumber += c;
        }
        return strNumber;
    }

    // Sanitise HTML due to centre text is using HTML format.
    private string SanitizeHTML(string inputString)
    {
        // Hardcoded removal of specific tags
        inputString = inputString.Replace("<script>", "");
        inputString = inputString.Replace("</script>", "");
        inputString = inputString.Replace("<style>", "");
        inputString = inputString.Replace("</style>", "");

        //Encode existing HTML entities
        inputString = inputString.Replace("&", "&amp;");
        inputString = inputString.Replace("<", "&lt;");
        inputString = inputString.Replace(">", "&gt;");

        return inputString;
    }

    [ConsoleCommand("css_stopwatch", "Start stopwatch")]
    public void CommandStartStopwatch(CCSPlayerController? player, CommandInfo info)
    {
        // add null check
        // however this prevents accepting commands from server console
        //if (player == null)
        //{
        //    return;
        //}
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
        //    player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must wait for end one countdown.");
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
        // add null check
        // however this prevents accepting commands from server console
        //if (player == null)
        //{
        //    return;
        //}
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
            Countdown_enable = false;
            timer_1?.Kill();
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
        // add null check
        // however this prevents accepting commands from server console
        //if (player == null)
        //{
        //    return;
        //}
        if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You are not admin..");
            return;
        }
        var TimeSec = info.ArgByIndex(1);
        TimeSec = ParseInt(TimeSec);
        var Text_var = info.ArgByIndex(2);
        Text_var = SanitizeHTML(Text_var);

        //if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec))
        //{
            //player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must use that {ChatColors.Lime}/countdown_text <TIME_SECONDS> <TEXT>{ChatColors.Default}.");
        //    return;
        //}
        if (Text_var == null || Text_var == "")
        {
            //player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must use that {ChatColors.Lime}/countdown_text <TIME_SECONDS> <TEXT>{ChatColors.Default}.");
            return;
        }
        if (Countdown_enable == true || Countdown_enable_text == true || Stopwatch_enable == true)
        {
            Countdown_enable_text = false;
            timer_2?.Kill();
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

    /*
    // Future Extension: 
    // Planning for separate ChatHUD plugin as in 
    // https://github.com/Oz-Lin/Chat-Hud-Countdown-Translator-Anubis-Edition

    [ConsoleCommand("say", "Trigger by Map ChatHUD")]
    public void CommandStartMapTextCountDown(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null)
        {
            return;
        }
        if (player.IsBot && player.PlayerName.Equals("Console") && player.UserId.Equals(0))
        {
           
        }
    

        var TimeSec = info.ArgByIndex(1);
        var Text_var = info.ArgByIndex(1);

        if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec))
        {
            
            return;
        }
        if (Text_var == null || Text_var == "")
        {
            
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

        string filteredText = Regex.Replace(Text_var.ToLower(), @"[^a-z0-9 ]", "");

        string[] words = filteredText.Split(' ');
        int wordCount = words.Length;
        uint triggerTimerLength = 0;

        if (wordCount == 2)
        {
            uint.TryParse(words[1], out triggerTimerLength);
        }

        for (int i = 1; i < wordCount && triggerTimerLength == 0; i++)
        {
            uint currentValue = 0;
            uint.TryParse(words[i], out currentValue);

            if (i + 1 < wordCount)
            {
                string nextWord = words[i + 1];

                if (nextWord.Length > 2 && currentValue > 0)
                {
                    if (nextWord.StartsWith("sec"))
                    {
                        triggerTimerLength = currentValue;
                    }
                    else if (nextWord.StartsWith("min"))
                    {
                        triggerTimerLength = currentValue * 60;
                    }
                }
            }

            if (currentValue == 0)
            {
                string currentWord = words[i];
                int currentScanLength = Math.Min(currentWord.Length, 4);

                for (int j = 0; j < currentScanLength; j++)
                {
                    if (char.IsDigit(currentWord[j]))
                        continue;

                    if (currentWord[j] == 's')
                    {
                        currentWord = currentWord.Substring(0, j);
                        uint.TryParse(currentWord, out triggerTimerLength);
                    }
                    break;
                }
            }
        }
        
        float currentRoundClock = CCSGameRules. - (gpGlobals.curtime - CCSGameRules.m_fRoundStartTime.Value);

        if (triggerTimerLength > 4 && currentRoundClock > triggerTimerLength)
        {
            int triggerTime = (int)(currentRoundClock - triggerTimerLength + 0.5f);
            int mins = triggerTime / 60;
            int secs = triggerTime % 60;

  
            ClientPrintAll(HUD_PRINTCENTER, buf);
        }
        else
        {

            UTIL_SayTextFilter(filter, buf, player, messageType);
        }
    }
    */
}
