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
    public override string ModuleVersion => "V. 1.0.6";



    public float[] Time = new float[3];
    public string[] Text = new string[3];
    public string color = "";
    public bool Countdown_enable;
    public bool[] Countdown_enable_text = new bool[3];
    public bool Stopwatch_enable;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_1;
    //public CounterStrikeSharp.API.Modules.Timers.Timer? timer_2;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_2a;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_2b;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_2c;
    public CounterStrikeSharp.API.Modules.Timers.Timer? timer_3;
    public int index = 0;


    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnMapStart>(name =>
        {
            index = 0;
            Countdown_enable = false;
            for (int i = 0; i < Countdown_enable_text.Length; i++) 
            { 
                Countdown_enable_text[i] = false;
                Text[i] = "";
                Time[i] = 0;
            }           
            Stopwatch_enable = false;
            timer_1?.Kill();
            //timer_2?.Kill();
            timer_2a?.Kill();
            timer_2b?.Kill();
            timer_2c?.Kill();
            timer_3?.Kill();
        });
        //kill timers in case of server crash on map change
        RegisterListener<Listeners.OnMapEnd>(() =>
        {
            Countdown_enable = false;
            for (int i = 0; i < Countdown_enable_text.Length; i++)
            {
                Countdown_enable_text[i] = false;
                Text[i] = "";
                Time[i] = 0;
            }
            Stopwatch_enable = false;
            timer_1?.Kill();
            //timer_2?.Kill();
            timer_2a?.Kill();
            timer_2b?.Kill();
            timer_2c?.Kill();
            timer_3?.Kill();

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
                    $"<font color='gray'>----</font> <font class='fontSize-m' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time} seconds]</font> <font color='gray'>◄</font>"
                    //$"<font color='gray'>----</font> <font class='fontSize-l' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>"
                    );
                }
                if (Countdown_enable_text[0] && Countdown_enable_text[1] && Countdown_enable_text[2])
                {
                    client.PrintToCenterHtml(
                    $"<font class='fontSize-m' color='gold'>[{Text[0]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[0]} seconds]</font> <font color='gray'>◄</font><br>" +
                    $"<font class='fontSize-m' color='gold'>[{Text[1]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[1]} seconds]</font> <font color='gray'>◄</font><br>" +
                    $"<font class='fontSize-m' color='gold'>[{Text[2]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[2]} seconds]</font> <font color='gray'>◄</font>"
                    );

                }
                if (Countdown_enable_text[0] && Countdown_enable_text[1] && !Countdown_enable_text[2])
                {
                    client.PrintToCenterHtml(
                    $"<font class='fontSize-m' color='gold'>[{Text[0]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[0]} seconds]</font> <font color='gray'>◄</font><br>" +
                    $"<font class='fontSize-m' color='gold'>[{Text[1]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[1]} seconds]</font> <font color='gray'>◄</font>"
                    );

                }
                if (!Countdown_enable_text[0] && Countdown_enable_text[1] && Countdown_enable_text[2])
                {
                    client.PrintToCenterHtml(
                    $"<font class='fontSize-m' color='gold'>[{Text[1]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[1]} seconds]</font> <font color='gray'>◄</font><br>" +
                    $"<font class='fontSize-m' color='gold'>[{Text[2]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[2]} seconds]</font> <font color='gray'>◄</font>"
                    );

                }
                if (Countdown_enable_text[0] && !Countdown_enable_text[1] && Countdown_enable_text[2])
                {
                    client.PrintToCenterHtml(
                    $"<font class='fontSize-m' color='gold'>[{Text[0]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[0]} seconds]</font> <font color='gray'>◄</font><br>" +
                    $"<font class='fontSize-m' color='gold'>[{Text[2]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[2]} seconds]</font> <font color='gray'>◄</font>"
                    );

                }
                if (Countdown_enable_text[0] && !Countdown_enable_text[1] && !Countdown_enable_text[2])
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-m' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>" +
                    $"<font class='fontSize-m' color='gold'>[{Text[0]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[0]} seconds]</font> <font color='gray'>◄</font>"
                    );

                }
                if (!Countdown_enable_text[0] && Countdown_enable_text[1] && !Countdown_enable_text[2])
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-m' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>" +
                    $"<font class='fontSize-m' color='gold'>[{Text[1]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[1]} seconds]</font> <font color='gray'>◄</font>"
                    );

                }
                if (!Countdown_enable_text[0] && !Countdown_enable_text[1] && Countdown_enable_text[2])
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-m' color='{color}'>COUNTDOWN</font><font color='gray'>----</font><br>" +
                    $"<font class='fontSize-m' color='gold'>[{Text[2]}]</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='white'>[{Time[2]} seconds]</font> <font color='gray'>◄</font>"
                    );

                }
                if (Stopwatch_enable)
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-m' color='{color}'>STOPWATCH</font><font color='gray'>----</font><br>" +
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
            player.PrintToChat($"  [{ChatColors.Lime}CountDown{ChatColors.Default}] StopWatch has turned off!");
            Stopwatch_enable = false;
            timer_3?.Kill();
            return;
        }
        if (Countdown_enable == true || Countdown_enable_text[0] == true || Countdown_enable_text[1] == true || Countdown_enable_text[2] == true)
        {
            //    player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must wait for end one countdown.");
            return;
        }
        Stopwatch_enable = true;
        index = 0;
        timer_3 = AddTimer(1.0f, () =>
        {
            RandomColor();
            Time[0] = Time[0] + 1.0f;
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
        if (Countdown_enable == true || Countdown_enable_text[0] == true || Countdown_enable_text[1] == true || Countdown_enable_text[2] == true || Stopwatch_enable == true)
        {
            Countdown_enable = false;
            Countdown_enable_text[0] = false;
            Countdown_enable_text[1] = false;
            Countdown_enable_text[2] = false;
            timer_1?.Kill();
            timer_2a?.Kill();
            timer_2b?.Kill();
            timer_2c?.Kill();
        }
        var time_convert = Convert.ToInt32(TimeSec);
        Time[0] = time_convert;
        Countdown_enable = true;
        index = 0;
        timer_1 = AddTimer(1.0f, () =>
        {
            if (Time[0] <= 0.0)
            {
                Countdown_enable = false;
                timer_1?.Kill();

                return;
            }
            RandomColor();
            Time[0] = Time[0] - 1.0f;
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
        var Text_var = info.ArgByIndex(2);
        Text_var = SanitizeHTML(Text_var);

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
        if (Countdown_enable == true || Countdown_enable_text[index] == true || Stopwatch_enable == true)
        {
            Countdown_enable_text[index] = false;
            if (index == 0)
            {
                timer_2a?.Kill();
            }
            if (index == 1)
            {
                timer_2b?.Kill();
            }
            if (index == 2)
            {
                timer_2c?.Kill();
            }
        }
        var time_convert = Convert.ToInt32(TimeSec);
        Time[index] = time_convert;
        Text[index] = Text_var;
        Countdown_enable_text[index] = true;
        if (index < Time.Length)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        if (index == 0)
        {
            timer_2a = AddTimer(1.0f, () =>
            {
                if (Time[index] <= 0.0)
                {
                    Countdown_enable_text[index] = false;
                    timer_2a?.Kill();

                    return;
                }
                RandomColor();
                Time[index] = Time[index] - 1.0f;
            }, TimerFlags.REPEAT);
        }
        if (index == 1)
        {
            timer_2b = AddTimer(1.0f, () =>
            {
                if (Time[index] <= 0.0)
                {
                    Countdown_enable_text[index] = false;
                    timer_2b?.Kill();

                    return;
                }
                RandomColor();
                Time[index] = Time[index] - 1.0f;
            }, TimerFlags.REPEAT);
        }
        if (index == 2)
        {
            timer_2c = AddTimer(1.0f, () =>
            {
                if (Time[index] <= 0.0)
                {
                    Countdown_enable_text[index] = false;
                    timer_2c?.Kill();

                    return;
                }
                RandomColor();
                Time[index] = Time[index] - 1.0f;
            }, TimerFlags.REPEAT);
        }
    }

    public void RandomColor()
    {
        Random rnd = new Random();
        int random_num = rnd.Next(5);
        if (random_num == 0) { color = "green"; }
        if (random_num == 1) { color = "red"; }
        if (random_num == 2) { color = "blue"; }
        if (random_num == 3) { color = "gold"; }
        if (random_num == 4) { color = "orange"; }
    }

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
        timer_2a = AddTimer(1.0f, () =>
        {
            if (Time <= 0.0)
            {
                Countdown_enable_text = false;
                timer_2a?.Kill();

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
//}
