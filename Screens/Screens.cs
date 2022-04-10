using Spectre.Console;
using System;
using System.Linq;
using System.Threading.Tasks;
using TogglHelper.Controllers;
using TogglHelper.Enums;

namespace TogglHelper.Screens
{
    internal static class Screens
    {
        private static void MainHeader()
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"Hello [blue]{Globals.TogglUser.Username}[/]");
            AnsiConsole.MarkupLine($"Current Workspace: [blue]{Globals.TogglUser.CurrentWorkspace.Name}[/]");
            if (Globals.DateFilter != DateTime.MinValue)
                AnsiConsole.MarkupLine($"Filter Date: [blue]{Globals.DateFilter:dd/MM/yyyy}[/]");

            if (Globals.TimeEntries?.Count > 0)
            {
                var totalWithoutID = Globals.TimeEntries.Count(x => x.TicketID.Equals(0));
                var str = $"Time Entries: {Globals.TimeEntries.Count}";
                if (totalWithoutID > 0)
                    str.Concat($" ({totalWithoutID} Without ID)");

                var synced = Globals.TimeEntries.Count(x => x.Status.Equals("synced"));
                if (synced > 0)
                    str.Concat($" ({synced} synced)");

                AnsiConsole.MarkupLine(str);
                AnsiConsole.MarkupLine($"Total time: {Helpers.TimeHelper.ConvertSecondsToString(Globals.TimeEntries.Sum(x => x.DurationInSeconds))}");
            }

            AnsiConsole.MarkupLine("");
        }

        internal static void MainMenu()
        {
            string op;
            do
            {
                MainHeader();

                op = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[] {
                        MainMenuOptions.Today, MainMenuOptions.Yesterday, MainMenuOptions.InformDate,
                        MainMenuOptions.SwitchWorkspace, MainMenuOptions.Logout, MainMenuOptions.Close
                    }));

                switch (op)
                {
                    case MainMenuOptions.Today:
                        Globals.DateFilter = DateTime.Now;
                        LoadTimeEntries();
                        break;
                    case MainMenuOptions.Yesterday:
                        Globals.DateFilter = DateTime.Now.AddDays(-1);
                        LoadTimeEntries();
                        break;
                    case MainMenuOptions.InformDate:
                        ShowCalendarMenu();
                        break;
                    case MainMenuOptions.SwitchWorkspace:
                        TogglController.SetWorkspace();
                        break;
                    case MainMenuOptions.Logout:
                        LogoutToggl();
                        break;
                    case MainMenuOptions.Close:
                        Environment.Exit(-1);
                        break;
                }
            } while (op != MainMenuOptions.Close);
        }

        private static void MenuTimeEntriesLoaded()
        {
            string op;
            do
            {
                MainHeader();

                op = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[] {
                        MenuTimeEntriesLoadedOptions.SendToKayako, MenuTimeEntriesLoadedOptions.AddNote, MenuTimeEntriesLoadedOptions.ListTimeEntries,
                        MenuTimeEntriesLoadedOptions.ReloadTimesEntries, MenuTimeEntriesLoadedOptions.Return
                    }));

                switch (op)
                {
                    case MenuTimeEntriesLoadedOptions.SendToKayako:
                        break;
                    case MenuTimeEntriesLoadedOptions.AddNote:
                        AddNoteMenu();
                        break;
                    case MenuTimeEntriesLoadedOptions.ListTimeEntries:
                        break;
                    case MenuTimeEntriesLoadedOptions.ReloadTimesEntries:
                        break;
                }
            } while (op != MenuTimeEntriesLoadedOptions.Return);
        }

        private static void AddNoteMenu()
        {
            string op;
            do
            {
                Console.Clear();
                var options = Globals.TimeEntries.Where(x => !string.IsNullOrEmpty(x.Description)).Select(x => x.Description.Replace("[", "[[").Replace("]", "]]")).ToArray();
                options.Append("Return");
                op = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(options));

                var entry = Globals.TimeEntries.FirstOrDefault(x => x.Description == op.Replace("[[", "[").Replace("]]", "]"));
                if (entry != null)
                {
                    AnsiConsole.Ask<string>($"[blue]Current Note[/]:{entry.Note}");
                    var note = AnsiConsole.Ask<string>("[blue]Note[/]:");
                    entry.Note = note;
                }
            } while (op != "Return");
        }

        internal static async Task LoginToggl()
        {
            var isValid = false;
            var biShowInvalidAlert = false;
            do
            {
                Console.Clear();
                AnsiConsole.MarkupLine("Please, inform the Toggl [green]API Token[/]");

                if (biShowInvalidAlert)
                    AnsiConsole.MarkupLine("[red]Invalid API Token![/]");

                var key = AnsiConsole.Ask<string>("[green]API Token:[/]");
                if (!string.IsNullOrEmpty(key) && key.Length == 32)
                {
                    if (Globals.TogglUser == null)
                        Globals.TogglUser = new Models.Toggl.User();

                    Globals.TogglUser.ApiToken = key;

                    await AnsiConsole.Status()
                        .StartAsync("Loading...", async ctx =>
                        {
                            Console.Clear();
                            isValid = await TogglController.SetTogglData();
                        });

                    if (isValid)
                        TogglController.SetWorkspace();
                }
                else
                {
                    biShowInvalidAlert = true;
                }
            } while (!isValid);
        }

        internal static object WorkspaceSelection(string[] workspaces)
        {
            Console.Clear();
            var workspace = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a [blue]Workspace[/]:")
                .AddChoices(workspaces));

            return workspace;
        }

        private static void LogoutToggl()
        {
            Globals.TogglUser = null;
            IsolatedFileController.SetIsolatedTogglUser();
            var task = LoginToggl();
            task.Wait();
            MainMenu();
        }

        internal static void ShowCalendarMenu()
        {
            ConsoleKeyInfo key;
            Globals.DateFilter = DateTime.Now;
            do
            {
                ShowCalendar();
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        Globals.DateFilter = Globals.DateFilter.AddDays(-1);
                        break;
                    case ConsoleKey.RightArrow:
                        Globals.DateFilter = Globals.DateFilter.AddDays(1);
                        break;
                    case ConsoleKey.UpArrow:
                        Globals.DateFilter = Globals.DateFilter.AddDays(-7);
                        break;
                    case ConsoleKey.DownArrow:
                        Globals.DateFilter = Globals.DateFilter.AddDays(7);
                        break;
                    case ConsoleKey.Enter:
                        break;
                    default:
                        ShowErrorMessage("Invalid key Pressed");
                        break;
                }
            } while (key.Key != ConsoleKey.Enter);
            LoadTimeEntries();
        }

        private static void ShowCalendar()
        {
            Console.Clear();
            var calendar = new Calendar(Globals.DateFilter.Year, Globals.DateFilter.Month);
            calendar.AddCalendarEvent(Globals.DateFilter.Year, Globals.DateFilter.Month, Globals.DateFilter.Day);
            AnsiConsole.Write(calendar);
        }

        internal static void ShowErrorMessage(string message)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[red]{message}[/]");
        }

        private static void LoadTimeEntries()
        {
            Console.Clear();

            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("green bold"))
                .Start("Loading [blue]time entries[/]...", ctx =>
                {
                    try
                    {
                        Task task = TogglController.GetTimeEntriesAsync();
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage("Fail");
                        throw;
                    }
                });

            MenuTimeEntriesLoaded();
        }
    }
}