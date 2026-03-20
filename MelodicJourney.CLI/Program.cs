using MelodicJourney.CLI.Models;
using MelodicJourney.CLI.Services;
using MelodicJourney.CLI.UI;
using Spectre.Console;

namespace MelodicJourney.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        var audioService = new AudioService();
        var tracks = MusicService.LocalPlaylist;

        TerminalUI.RenderHeader();

        if (tracks.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No tracks found in the local library![/]");
            return;
        }

        while (true)
        {
            var selectedTrack = TerminalUI.PromptTrackSelection(tracks);
            if (selectedTrack == null) break;

            await audioService.PlayAsync(selectedTrack);

            TerminalUI.RenderHeader();
            TerminalUI.ShowPlayerStatus(selectedTrack, true);

            AnsiConsole.MarkupLine("\n[blue]Controls: [P] Pause/Resume, [S] Stop, [Enter] New Selection, [X] Exit[/]");

            var exitPlayback = false;
            while (!exitPlayback)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.P:
                            if (audioService.IsPlaying) await audioService.PauseAsync();
                            else await audioService.ResumeAsync();
                            TerminalUI.RenderHeader();
                            TerminalUI.ShowPlayerStatus(selectedTrack, audioService.IsPlaying);
                            AnsiConsole.MarkupLine("\n[blue]Controls: [P] Pause/Resume, [S] Stop, [Enter] New Selection, [X] Exit[/]");
                            break;

                        case ConsoleKey.S:
                            await audioService.StopAsync();
                            exitPlayback = true;
                            break;

                        case ConsoleKey.Enter:
                            exitPlayback = true;
                            break;

                        case ConsoleKey.X:
                            await audioService.StopAsync();
                            return;
                    }
                }
                await Task.Delay(100);
            }
        }
    }
}
