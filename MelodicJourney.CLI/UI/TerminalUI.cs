using Spectre.Console;
using MelodicJourney.CLI.Models;
using MelodicJourney.CLI.Services;

namespace MelodicJourney.CLI.UI;

public static class TerminalUI
{
    public static void RenderHeader()
    {
        AnsiConsole.Clear();
        var rule = new Rule("[red]🏴‍☠️ MELODIC JOURNEYS OF COLLIN 🏴‍☠️[/]")
        {
            Style = Style.Parse("red"),
            Justification = Justify.Center
        };
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }

    public static MusicInfoModel? PromptTrackSelection(List<MusicInfoModel> tracks)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<MusicInfoModel>()
                .Title("[yellow]Select a track to play (or press Esc to exit)[/]")
                .PageSize(10)
                .AddChoices(tracks)
                .UseConverter(t => $"{t.Name} - [grey]{t.Genre}[/]")
        );

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
}
