using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

using GalavisorCli.Constants;
using GalavisorCli.Utils;
using GalavisorCli.Models;

namespace GalavisorCli.Commands.Activities;

internal sealed class AddActivityCommand : AsyncCommand<AddActivityCommand.Settings>
{
    [Description("Add an activity to the database")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [Description("Name of the activity to add")]
        public string Name { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            name = settings.Name,
            planetName = string.Empty
        };

        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/activity";
            var response = await HttpUtils.Post(url, requestBody);
            
            var activity = response.GetProperty("activity");
            var status = response.GetProperty("status");
            var isNewlyCreated = status.GetProperty("isNewlyCreated").GetBoolean();
            
            var table = new Table();
            table.AddColumn("Field");
            table.AddColumn("Value");

            table.AddRow("Name", activity.GetProperty("name").GetString());
            
            string title;
            string messageColor;
            Style panelStyle;
            
            if (isNewlyCreated)
            {
                title = "Activity Added Successfully";
                messageColor = "green";
                panelStyle = Style.Parse("green");
            }
            else
            {
                title = "Activity Already Exists";
                messageColor = "yellow";
                panelStyle = Style.Parse("yellow");
            }

            // Output the message above the panel
            AnsiConsole.MarkupLine($"[{messageColor}]{title}[/]");

            
            var panel = new Panel(table)
                .Header("Activity Details", Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderStyle(panelStyle);

            AnsiConsole.Write(panel);

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class GetActivityCommand : AsyncCommand<GetActivityCommand.Settings>
{
    [Description("Get all activities or get activities for a planet")]
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-p|--planet <PLANET>")]
        [Description("Get activities for a specific planet")]
        public string? PlanetName { get; set; }

        [CommandOption("-a|--all")]
        [Description("Get all activities")]
        public bool GetAll { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!settings.GetAll && string.IsNullOrEmpty(settings.PlanetName))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] You must specify either --all or --planet");
            return 1;
        }

        try
        {
            var url = settings.GetAll ? 
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/activity" : 
                $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/activity/planet/{HttpUtility.UrlEncode(settings.PlanetName)}";
            
            var response = await HttpUtils.Get(url);
            
            var table = new Table
            {
                Border = TableBorder.Rounded,
                BorderStyle = Style.Parse("green")
            };

            table.AddColumn("Name");

            var activities = JsonSerializer.Deserialize<List<ActivityModel>>(response.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (activities != null && activities.Any())
            {
                string title = settings.GetAll ? 
                    "All Activities" : 
                    $"Activities for Planet: {settings.PlanetName}";
                
                AnsiConsole.MarkupLine($"[green]{title}[/]");
                
                foreach (var activity in activities)
                {
                    table.AddRow(activity.Name);
                }
                AnsiConsole.Write(table);
            }
            else
            {
                string message = settings.GetAll ? 
                    "No activities found" : 
                    $"No activities found for planet {settings.PlanetName}";
                    
                AnsiConsole.MarkupLine($"[yellow]{message}[/]");
            }

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class UpdateActivityCommand : AsyncCommand<UpdateActivityCommand.Settings>
{
    [Description("Update the name of an existing activity")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<CURRENT_NAME>")]
        public string CurrentName { get; set; } = string.Empty;

        [CommandArgument(1, "<NEW_NAME>")]
        public string NewName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/activity";
            var requestBody = new
            {
                CurrentName = settings.CurrentName,
                NewName = settings.NewName
            };
            var response = await HttpUtils.Put(url, requestBody);
            
            var message = response.GetProperty("message").GetString();
            AnsiConsole.MarkupLine($"[green]{message}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class DeleteActivityCommand : AsyncCommand<DeleteActivityCommand.Settings>
{
    [Description("Delete an activity from the database")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [Description("Name of the activity to delete")]
        public string Name { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/activity";
            var response = await HttpUtils.DeleteWithBody(url, settings.Name);
            
            var message = response.GetProperty("message").GetString();
            AnsiConsole.MarkupLine($"[green]{message}[/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
}

internal sealed class LinkActivityCommand : AsyncCommand<LinkActivityCommand.Settings>
{
    [Description("Link an activity to a planet")]
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<ACTIVITY>")]
        [Description("Name of the existing activity to link")]
        public string ActivityName { get; set; } = string.Empty;

        [CommandArgument(1, "<PLANET>")]
        [Description("Name of the planet to link the activity to")]
        public string PlanetName { get; set; } = string.Empty;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var requestBody = new
        {
            name = settings.ActivityName,
            planetName = settings.PlanetName
        };

        try
        {
            var url = $"{ConfigStore.Get(ConfigKeys.ServerUri)}/api/activity/link";
            var response = await HttpUtils.Post(url, requestBody);
            
            // Parse the response
            var activity = response.GetProperty("activity");
            var status = response.GetProperty("status");
            var isNewlyLinked = status.GetProperty("isNewlyLinked").GetBoolean();
            
            var table = new Table();
            table.AddColumn("Field");
            table.AddColumn("Value");
            
            table.AddRow("Activity", activity.GetProperty("name").GetString());
            table.AddRow("Planet", activity.GetProperty("planetName").GetString());
            
            string title;
            string messageColor;
            Style panelStyle;
            
            if (isNewlyLinked)
            {
                title = "Activity Linked to Planet";
                messageColor = "green";
                panelStyle = Style.Parse("green");
            }
            else 
            {
                title = "Activity Already Linked";
                messageColor = "yellow";
                panelStyle = Style.Parse("yellow");
                table.AddRow("Status", "Activity was already linked to this planet");
            }

            // Output the message above the panel
            AnsiConsole.MarkupLine($"[{messageColor}]{title}[/]");

            
            var panel = new Panel(table)
                .Header("Link Details", Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderStyle(panelStyle);

            AnsiConsole.Write(panel);

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Request failed:[/] {ex.Message}");
            return 1;
        }
    }
} 