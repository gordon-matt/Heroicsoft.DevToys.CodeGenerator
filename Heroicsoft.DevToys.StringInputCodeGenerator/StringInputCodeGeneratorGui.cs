using System.ComponentModel.Composition;
using DevToys.Api;
using Fluid;
using Heroicsoft.DevToys.CodeGenerator;
using Heroicsoft.DevToys.CodeGenerator.Extensions;
using static DevToys.Api.GUI;

namespace Heroicsoft.DevToys.StringInputCodeGenerator;

[Export(typeof(IGuiTool))]
[Name("StringInputCodeGenerator")]
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",
    IconGlyph = '\uEE3A',  // An icon that represents a wand
    GroupName = PredefinedCommonToolGroupNames.Generators,
    ResourceManagerAssemblyIdentifier = nameof(StringInputCodeGeneratorResourceAssemblyIdentifier),
    ResourceManagerBaseName = "Heroicsoft.DevToys.StringInputCodeGenerator.StringInputCodeGenerator",
    ShortDisplayTitleResourceName = nameof(StringInputCodeGenerator.ShortDisplayTitle),
    LongDisplayTitleResourceName = nameof(StringInputCodeGenerator.LongDisplayTitle),
    DescriptionResourceName = nameof(StringInputCodeGenerator.Description),
    AccessibleNameResourceName = nameof(StringInputCodeGenerator.AccessibleName))]
internal sealed class StringInputCodeGeneratorGui : IGuiTool
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

    public StringInputCodeGeneratorGui()
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
                        .Title("Text Input")
                        .Text(
@"Input values per line and/or separated by a comma. Example:
one,two,three

OR:

one
two
three

OR:

one,two
three,etc")
                        .Extendable()
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

{% for item in Model %}
public string {{ item }} { get; set; }
{% endfor %}")
                        .Extendable()
                        .Language("csharp")
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
                        .Language("csharp")
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

            txtModel.Text("Just iterate over each item in 'Model'");

            model = new
            {
                Model = txtInput.Text
                    .Trim()
                    .Split([Environment.NewLine, ","], StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
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