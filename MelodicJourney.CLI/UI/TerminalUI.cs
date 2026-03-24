using MelodicJourney.CLI.Models;
using MelodicJourney.CLI.Services;
using Spectre.Console;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MelodicJourney.CLI.UI;

public static class TerminalUI
{
    public static void RenderHeader()
    {
        AnsiConsole.Clear();
        var rule = new Rule("[red]MELODIC JOURNEYS OF COLLIN[/]")
        {
            Style = Style.Parse("red"),
            Justification = Justify.Center
        };
        AnsiConsole.Write(rule);
        var welcomeText = new Text("Welcome to the Melodic Journeys of Collin CLI!",
            new Style(Color.White, Color.Black, Decoration.Bold));
        AnsiConsole.Write(welcomeText);
        AnsiConsole.WriteLine();
    }

    public static void ShowLoading(string message)
    {
        AnsiConsole.Status()
            .Start(message, ctx =>
            {
                // Simulate some work
                Thread.Sleep(2000);
            });
    }

    public static MusicInfoModel? PromptTrackSelection(List<MusicInfoModel> tracks)
    {

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<MusicInfoModel>()
                .Title("[yellow]Select a track to play (or press Esc to exit)[/]")
                .PageSize(10)
                .AddChoices(tracks)
                .UseConverter(t =>
                {
                    var name = Markup.Escape(Markup.Escape(t.Name ?? "Unknown"));
                    var genre = Markup.Escape(Markup.Escape(t.Genre ?? "Unknown"));
                    return $"{name} - [grey]{genre}[/]";
                }));



        return choice;
    }

    public static void ShowPlayerStatus(MusicInfoModel track, bool isPlaying)
    {
        var status = isPlaying ? "[green]PLAYING[/]" : "[yellow]PAUSED[/]";
        var panel = new Panel(
            new Rows(
                new Text($"Track: {track.Name}", new Style(Color.White, Color.Black, Decoration.Bold)),
                new Text($"Genre: {track.Genre}", new Style(Color.Grey)),
                new Text($"Status: {status}")
            ))
        {
            Header = new PanelHeader("[red]Current Deck[/]"),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Red)
        };

        AnsiConsole.Write(panel);
    }



    public static async Task ChooseOptionsAsync(AudioService audioService)
    {
        while (true)
        {
            RenderHeader();
            var options = new[] { "Play Music", "View Albums", "About", "Exit" };

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Choose an option:[/]")
                .AddChoices(options));

            switch (choice)
            {
                case "Play Music":
                    var tracks = MusicService.LocalPlaylist;
                    if (!tracks.Any())
                    {
                        AnsiConsole.MarkupLine("[red]No music found in the local playlist![/]");
                        AnsiConsole.MarkupLine("\n[grey]Press any key to return to main menu...[/]");
                        Console.ReadKey(true);
                        break;
                    }

                    var selectedTrack = PromptTrackSelection(tracks);
                    if (selectedTrack == null) break;

                    try
                    {
                        await audioService.PlayAsync(selectedTrack);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
                        AnsiConsole.MarkupLine("\n[grey]Press any key to return to main menu...[/]");
                        Console.ReadKey(true);
                        break;
                    }

                    var exitPlayback = false;
                    while (!exitPlayback)
                    {
                        RenderHeader();
                        ShowPlayerStatus(selectedTrack, audioService.IsPlaying);
                        AnsiConsole.MarkupLine("\n[blue]Controls: [[P]] Pause/Resume, [[S]] Stop, [[Enter]] New Selection, [[X]] Exit back to menu[/]");

                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true).Key;
                            switch (key)
                            {
                                case ConsoleKey.P:
                                    if (audioService.IsPlaying) await audioService.PauseAsync();
                                    else await audioService.ResumeAsync();
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
                                    exitPlayback = true;
                                    break;
                            }
                        }
                        await Task.Delay(100);
                    }
                    break;
                case "View Albums":
                    // Handle view library
                    break;
                case "About":
                    break;
                case "Exit":
                    await audioService.StopAsync();
                    AnsiConsole.MarkupLine("[green]Thank you for using Melodic Journeys of Collin CLI! Goodbye![/]");
                    await Task.Delay(1500);
                    AnsiConsole.Clear();
                    break;
            }
        }
    }


}
