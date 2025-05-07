using System.ComponentModel.Composition;
using System.Text;
using DevToys.Api;
using Fluid;
using Heroicsoft.DevToys.CodeGenerator;
using Heroicsoft.DevToys.CodeGenerator.Extensions;
using Newtonsoft.Json.Linq;
using static DevToys.Api.GUI;

namespace Heroicsoft.DevToys.JsonInputCodeGenerator;

[Export(typeof(IGuiTool))]
[Name("JsonInputCodeGenerator")]
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",
    IconGlyph = '\uEE3A',  // An icon that represents a wand
    GroupName = PredefinedCommonToolGroupNames.Generators,
    ResourceManagerAssemblyIdentifier = nameof(JsonInputCodeGeneratorResourceAssemblyIdentifier),
    ResourceManagerBaseName = "Heroicsoft.DevToys.JsonInputCodeGenerator.JsonInputCodeGenerator",
    ShortDisplayTitleResourceName = nameof(JsonInputCodeGenerator.ShortDisplayTitle),
    LongDisplayTitleResourceName = nameof(JsonInputCodeGenerator.LongDisplayTitle),
    DescriptionResourceName = nameof(JsonInputCodeGenerator.Description),
    AccessibleNameResourceName = nameof(JsonInputCodeGenerator.AccessibleName))]
internal sealed class JsonInputCodeGeneratorGui : IGuiTool
{
    private readonly FluidParser parser;
    private readonly TemplateOptions templateOptions;
    private object model = null;

    private IUIDropDownButton ddbLanguage = DropDownButton();

    private IUIInfoBar infoBar = InfoBar();
    private IUIMultiLineTextInput txtInput = MultiLineTextInput();
    private IUIMultiLineTextInput txtModel = MultiLineTextInput();
    private IUIMultiLineTextInput txtOutput = MultiLineTextInput();
    private IUIMultiLineTextInput txtTemplate = MultiLineTextInput();

    public JsonInputCodeGeneratorGui()
    {
        parser = new FluidParser();
        templateOptions = new TemplateOptions
        {
            MemberAccessStrategy = new UnsafeMemberAccessStrategy()
        };
        templateOptions.Filters.AddCustomFilters();

        ddbLanguage
            .AlignHorizontally(UIHorizontalAlignment.Left)
            .Icon("FluentSystemIcons", '\uEE93')
            .WithMenuItems(
                UIHelper.GetMenuItems(txtTemplate, txtOutput));
    }

    private enum ColumnId : byte
    {
        Left = 0,
        Right = 1
    }

    //Row identifiers
    private enum RowId : byte
    {
        Toolbar = 0,
        Content = 1,
        Output = 2
    }

    public UIToolView View => new
    (
        isScrollable: true,  // Recommended since you have multiple text inputs
        Grid()
            .ColumnMediumSpacing()
            .RowLargeSpacing()

            // Define rows: toolbar, middle content, bottom content
            .Rows(
                (RowId.Toolbar, UIGridLength.Auto),                     // Toolbar row
                (RowId.Content, new UIGridLength(1, UIGridUnitType.Fraction)), // Main content
                (RowId.Output, new UIGridLength(1, UIGridUnitType.Fraction)))  // Output area

            // Two equal columns
            .Columns(
                (ColumnId.Left, new UIGridLength(1, UIGridUnitType.Fraction)),
                (ColumnId.Right, new UIGridLength(1, UIGridUnitType.Fraction)))

            .Cells(
                // Toolbar spanning both columns
                Cell(
                    RowId.Toolbar, RowId.Toolbar,
                    ColumnId.Left, ColumnId.Right,
                    Stack()
                        .Vertical()
                        .WithChildren(
                            infoBar
                                .Title("Error")
                                .Description("Something went wrong.")
                                .Error()
                                .Close())
                ),

                // Input + Import button (left column)
                Cell(
                    RowId.Content, RowId.Content,
                    ColumnId.Left, ColumnId.Left,
                    txtInput
                        .Title("JSON Input")
                        .Text(
@"{
  ""company"": ""Sample JSON"",
  ""departments"": [
    {
      ""name"": ""Engineering"",
      ""teams"": [
        {
          ""teamName"": ""Backend"",
          ""members"": [
            { ""name"": ""Alice"", ""role"": ""Developer"" },
            { ""name"": ""Bob"", ""role"": ""DevOps"" }
          ]
        },
        {
          ""teamName"": ""Frontend"",
          ""members"": [
            { ""name"": ""Charlie"", ""role"": ""UI Developer"" },
            { ""name"": ""Dana"", ""role"": ""UX Designer"" }
          ]
        }
      ]
    },
    {
      ""name"": ""Marketing"",
      ""teams"": [
        {
          ""teamName"": ""Content"",
          ""members"": [
            { ""name"": ""Eve"", ""role"": ""Content Writer"" },
            { ""name"": ""Frank"", ""role"": ""SEO Specialist"" }
          ]
        }
      ]
    }
  ]
}")
                        .Extendable()
                        .Language("json")
                        .CommandBarExtraContent(
                             Button()
                                .Icon("FluentSystemIcons", '\uF15A')
                                .Text("Parse")
                                .AccentAppearance()
                                .OnClick(Import_Click))),

                // Model (right column)
                Cell(
                    RowId.Content, RowId.Content,
                    ColumnId.Right, ColumnId.Right,
                    txtModel
                        .Title("Model")
                        .Language("liquid")
                        .ReadOnly()
                        .AlwaysWrap()),

                // Template (left column, bottom row)
                Cell(
                    RowId.Output, RowId.Output,
                    ColumnId.Left, ColumnId.Left,
                    txtTemplate
                        .Title("Template")
                        .Text(
@"{% comment %}
Use the liquid templating language to iterate over the items in `Model`.
Documentation: https://shopify.github.io/liquid/basics/introduction/
Example below:
{% endcomment %}

{% for comp in Model %}
    <h2>{{ comp.company }}</h2>
    {% for department in comp.departments %}
    <h3>{{ department.name }}</h3>
    <table border=""1"" cellpadding=""5"" cellspacing=""0"">
        <thead>
        <tr>
            <th>Team</th>
            <th>Member Name</th>
            <th>Role</th>
        </tr>
        </thead>
        <tbody>
        {% for team in department.teams %}
            {% for member in team.members %}
            <tr>
                <td>{{ team.teamName }}</td>
                <td>{{ member.name }}</td>
                <td>{{ member.role }}</td>
            </tr>
            {% endfor %}
        {% endfor %}
        </tbody>
    </table>
    {% endfor %}
{% endfor %}")
                        .Extendable()
                        .CommandBarExtraContent(
                            Stack()
                                .Horizontal()
                                .WithChildren(
                                    ddbLanguage,
                                    Button()
                                    .Icon("FluentSystemIcons", '\uEE3A')
                                    .Text("Generate")
                                    .AccentAppearance()
                                    .OnClick(Generate_Click)
                                )
                        )
                ),

                // Output (right column, bottom row)
                Cell(
                    RowId.Output, RowId.Output,
                    ColumnId.Right, ColumnId.Right,
                    txtOutput
                        .Title("Output")
                        .Extendable()
                        .ReadOnly())
            ));

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        // Handle Smart Detection.
    }

    private void Generate_Click()
    {
        try
        {
            infoBar.Close();

            if (model == null)
            {
                infoBar.Title("No data!");
                infoBar.Description("Add some data first!");
                infoBar.Open();
                return;
            }

            if (parser.TryParse(txtTemplate.Text, out var template, out string error))
            {
                var context = new TemplateContext(model, templateOptions);
                txtOutput.Text(template.Render(context));
            }
            else
            {
                infoBar.Title("Error");
                infoBar.Description("Unable to parse the template");
                infoBar.Open();
            }
        }
        catch (Exception ex)
        {
            infoBar.Title("Error");
            infoBar.Description(ex.GetBaseException().Message);
            infoBar.Open();
        }
    }

    private void Import_Click()
    {
        try
        {
            infoBar.Close();

            txtModel.Text(string.Empty);

            var sb = new StringBuilder();
            sb.AppendLine("'Model' is ALWAYS an array, even when the provided data is a single object. Nested arrays are suffixed with '[]', but should be accessed without that suffix.");
            sb.AppendLine();
            sb.AppendLine("The following JSON properties can be accessed via the objects in the 'Model' array:");
            sb.AppendLine();

            var structure = JsonHelper.GetStructure(txtInput.Text);
            foreach (string item in structure)
            {
                sb.AppendLine(item);
            }

            txtModel.Text(sb.ToString());

            model = new
            {
                Model = txtInput.Text.StartsWith('[') ? JArray.Parse(txtInput.Text) : JArray.Parse($"[{txtInput.Text}]")
            };
        }
        catch (Exception ex)
        {
            infoBar.Title("Error");
            infoBar.Description(ex.GetBaseException().Message);
            infoBar.Open();
        }
    }
}