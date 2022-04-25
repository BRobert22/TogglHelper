using Spectre.Console;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TogglHelper.Controllers;
using TogglHelper.Enums;
using TogglHelper.Services;
using static TogglHelper.Enums.ScreenOptions;

namespace TogglHelper.Screens
{
    internal static class Screens
    {
        private static void MainHeader()
        {
            Console.Clear();

            var panelContent = new StringBuilder($"Hello [blue]{Globals.TogglUser.Username}[/]");
            panelContent.Append($"{Environment.NewLine}Workspace: [blue]{Globals.TogglUser.CurrentWorkspace.Name}[/]");

            if (Globals.DateFilter != DateTime.MinValue)
                panelContent.Append($"{Environment.NewLine}Filter Date: [blue]{Globals.DateFilter:dd/MM/yyyy}[/]");

            if (Globals.TimeEntries?.Count > 0)
            {
                var totalWithoutID = Globals.TimeEntries.Count(x => x.TicketID.Equals(0));
                var str = $"{Environment.NewLine}Time Entries: [blue]{Globals.TimeEntries.Count}[/]";
                if (totalWithoutID > 0)
                    str.Concat($" ([red]{totalWithoutID} Without ID[/])");

                var synced = Globals.TimeEntries.Count(x => x.Status.Equals("synced"));
                if (synced > 0)
                    str.Concat($" ([green]{synced} synced[/])");

                panelContent.Append(str);
                panelContent.Append($"{Environment.NewLine}Total time: [blue]{Helpers.TimeHelper.ConvertSecondsToString(Globals.TimeEntries.Sum(x => x.DurationInSeconds))}[/]");
            }

            var panel = new Panel(panelContent.ToString());
            panel.Border = BoxBorder.Rounded;
            AnsiConsole.Write(panel);
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
                        MainMenuOptions.SwitchWorkspace, MainMenuOptions.LogoutToggl, MainMenuOptions.LogoutKayako,
                        MainMenuOptions.Close
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
                    case MainMenuOptions.LogoutToggl:
                        LogoutToggl();
                        break;
                    case MainMenuOptions.LogoutKayako:
                        LogoutKayako();
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
                        SendToKayakoMenu();
                        break;
                    case MenuTimeEntriesLoadedOptions.AddNote:
                        AddNoteMenu();
                        break;
                    case MenuTimeEntriesLoadedOptions.ListTimeEntries:
                        ShowTimeEntriesTable();
                        break;
                    case MenuTimeEntriesLoadedOptions.ReloadTimesEntries:
                        LoadTimeEntries();
                        break;
                }
            } while (op != MenuTimeEntriesLoadedOptions.Return);
        }

        private static void SendToKayakoMenu()
        {
            AnsiConsole.Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask("[green]Sending time entries[/]");

                    foreach (var TimeEntry in Globals.TimeEntries)
                    {
                        KayakoController.SendTimeEntries(TimeEntry);

                        while (!ctx.IsFinished)
                            task.Increment(100 / Globals.TimeEntries.Count);
                    }
                });
        }

        private static void ShowTimeEntriesTable()
        {
            var table = new Table();

            table.AddColumn("Description");
            table.AddColumn("Duration");
            table.AddColumn("Status");
            table.AddColumn("Has note?");

            foreach (var entry in Globals.TimeEntries)
                table.AddRow(Markup.Escape(entry.Description), entry.PrintDuration(), entry.Status, entry.hasNote());

            AnsiConsole.Write(table);
            Console.ReadKey();
        }

        private static void AddNoteMenu()
        {
            string op;
            var index = 0;
            do
            {
                Console.Clear();
                MainHeader();
                var table = new Table();

                table.AddColumn("Description");
                table.AddColumn("Duration");
                table.AddColumn("Status");
                table.AddColumn("Has note?");

                for (int i = 0; i < Globals.TimeEntries.Count; i++)
                    AddItemToTable(index, table, i);

                AnsiConsole.Write(table);
                var keyPressed = Console.ReadKey();

                switch (keyPressed.Key)
                {
                    case ConsoleKey.Escape:
                    case ConsoleKey.Backspace:
                        return;
                    case ConsoleKey.Enter:
                        var entry = Globals.TimeEntries[index]; // Globals.TimeEntries.FirstOrDefault(x => x.Description.Equals(op.Replace("[[", "[").Replace("]]", "]")));
                        if (entry != null)
                        {
                            if (!string.IsNullOrEmpty(entry.Note))
                                AnsiConsole.MarkupLine($"[blue]Current Note[/]:{entry.Note}");
                            var note = AnsiConsole.Ask<string>("[blue]New Note[/]:");
                            entry.Note = note;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        index--;
                        break;
                    case ConsoleKey.DownArrow:
                        index++;
                        break;
                    default:
                        break;
                }
            } while (true);
        }

        private static void AddItemToTable(int index, Table table, int i)
        {
            var isSelectedRow = i == index;
            var item = Globals.TimeEntries[i];
            if (isSelectedRow)
                table.AddRow($"[blue][bold]{Markup.Escape(item.Description)}[/][/]", $"[blue][bold]{Markup.Escape(item.PrintDuration())}[/][/]", $"[blue][bold]{Markup.Escape(item.Status)}[/][/]", $"[blue][bold]{Markup.Escape(item.hasNote())}[/][/]");
            else
                table.AddRow(Markup.Escape(item.Description), item.PrintDuration(), item.Status, item.hasNote());
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

        private static void LogoutKayako()
        {
            Globals.KayakoUser = null;
            IsolatedFileController.SetIsolatedKayakolUser();
            KayakoController.LoginKayako();
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