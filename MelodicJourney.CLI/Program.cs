using MelodicJourney.CLI.Models;
using MelodicJourney.CLI.Services;
using MelodicJourney.CLI.UI;
using Spectre.Console;
using System.IO;

namespace MelodicJourney.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        var audioService = new AudioService();

        // Initial Startup
        TerminalUI.RenderHeader();
        TerminalUI.ShowLoading("Initializing Melodic Journeys CLI...");

        // Let the menu take over
        await TerminalUI.ChooseOptionsAsync(audioService);
    }

}
