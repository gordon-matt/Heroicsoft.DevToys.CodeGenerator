using System.ComponentModel.Composition;
using System.Text;
using System.Xml.Linq;
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
    }

    //public UIToolView View
    //=> new UIToolView(
    //    isScrollable: true,
    //    Grid()
    //        .ColumnSmallSpacing()  // Reduced from MediumSpacing
    //        .RowLargeSpacing()

    //        .Rows(
    //            (RowId.Toolbar, UIGridLength.Auto),
    //            (RowId.Content, new UIGridLength(1, UIGridUnitType.Fraction)),
    //            (RowId.Output, new UIGridLength(1, UIGridUnitType.Fraction)))

    //        .Columns(
    //            (ColumnId.Left, new UIGridLength(1, UIGridUnitType.Fraction)),
    //            (ColumnId.Right, new UIGridLength(1, UIGridUnitType.Fraction))))

    //        .Cells(
    //            // Toolbar row (unchanged)
    //            Cell(RowId.Toolbar, RowId.Toolbar, ColumnId.Left, ColumnId.Right,
    //                Stack().Horizontal().WithChildren(/* buttons */)),

    //            // Input section - now with properly sized button
    //            Cell(RowId.Content, ColumnId.Left,
    //                Stack()
    //                    .Horizontal()
    //                    .WithChildren(
    //                        txtInput,
    //                        Button()
    //                            .Icon("FluentSystemIcons", '\uF15A')
    //                            .OnClick(Import_Click) // Set fixed height
    //                    )),

    //            // Model section (unchanged)
    //            Cell(RowId.Content, RowId.Content, ColumnId.Right, ColumnId.Right, txtModel),

    //            // Bottom row (unchanged)
    //            Cell(RowId.Output, RowId.Output, ColumnId.Left, ColumnId.Left, txtTemplate),
    //            Cell(RowId.Output, RowId.Output, ColumnId.Right, ColumnId.Right, txtOutput)
    //        ));
    //// Add this nested enum for the inner grid
    //private enum ColumnId
    //{
    //    Input,
    //    Button,
    //    Left,
    //    Right
    //}

    // Column identifiers
    private enum ColumnId
    {
        Left,
        Right
    }

    //Row identifiers
    private enum RowId
    {
        Toolbar,
        Content,
        Output
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
                        .Horizontal()
                        .WithChildren(
                            //Button().Icon("FluentSystemIcons", '\uF42F').OnClick(OpenTemplate_Click),
                            //Button().Icon("FluentSystemIcons", '\uE4DE').OnClick(NewTemplate_Click),
                            //Button().Icon("FluentSystemIcons", '\uF680').OnClick(Save_Click),
                            //Button().Icon("FluentSystemIcons", '\uEE3A').OnClick(Generate_Click),
                            DropDownButton()
                                .Icon("FluentSystemIcons", '\uEE93')
                                .Text("Lang")
                                .WithMenuItems(/* your items */)
                        )),

                // Input + Import button (left column)
                Cell(
                    RowId.Content, RowId.Content,
                    ColumnId.Left, ColumnId.Left,
                    txtInput
                        .Title("Input")
                        .Extendable()
                        .Language("json")
                        .CommandBarExtraContent(
                             Button()
                                .Icon("FluentSystemIcons", '\uF15A')
                                .Text("Import")
                                .AccentAppearance()
                                .OnClick(Import_Click))),

                // Model (right column)
                Cell(
                    RowId.Content, RowId.Content,
                    ColumnId.Right, ColumnId.Right,
                    txtModel
                        .Title("Model")
                        .ReadOnly()),

                // Template (left column, bottom row)
                Cell(
                    RowId.Output, RowId.Output,
                    ColumnId.Left, ColumnId.Left,
                    txtTemplate
                        .Title("Template")
                        .Text(Constants.InitialTemplateCode)
                        .Extendable()
                        .CommandBarExtraContent(
                             Button()
                                .Icon("FluentSystemIcons", '\uEE3A')
                                .Text("Generate")
                                .AccentAppearance()
                                .OnClick(Generate_Click))),

                // Output (right column, bottom row)
                Cell(
                    RowId.Output, RowId.Output,
                    ColumnId.Right, ColumnId.Right,
                    txtOutput
                        .Title("Output")
                        .Extendable()
                        .ReadOnly())
            ));

    //public UIToolView View
    //    => new
    //    (
    //        Stack()
    //            .Vertical()
    //            .WithChildren
    //            (
    //                Stack()
    //                    .Horizontal()
    //                    .WithChildren
    //                    (
    //                        Button()
    //                            .Icon("FluentSystemIcons", '\uF42F')
    //                            .OnClick(OpenTemplate_Click), // Open

    //                        Button()
    //                            .Icon("FluentSystemIcons", '\uE4DE')
    //                            .OnClick(NewTemplate_Click), // Add

    //                        Button()
    //                            .Icon("FluentSystemIcons", '\uF680')
    //                            .OnClick(Save_Click), // Save

    //                        Button()
    //                            .Icon("FluentSystemIcons", '\uEE3A')
    //                            .OnClick(Generate_Click), // Generate

    //                        DropDownButton()
    //                            .AlignHorizontally(UIHorizontalAlignment.Left)
    //                            .Icon("FluentSystemIcons", '\uEE93') // Language
    //                            .Text("Lang")
    //                            .WithMenuItems(
    //                                DropDownMenuItem()
    //                                    .Text("C#")
    //                                    .OnClick(ChangeLanguage_Click),
    //                                DropDownMenuItem()
    //                                    .Text("C#")
    //                                    .OnClick(ChangeLanguage_Click),
    //                                DropDownMenuItem()
    //                                    .Text("C#")
    //                                    .OnClick(ChangeLanguage_Click),
    //                                DropDownMenuItem("Item 4"))
    //                    ),
    //                Stack()
    //                    .Horizontal()
    //                    .WithChildren
    //                    (
    //                        txtInput,

    //                        Button()
    //                            .Icon("FluentSystemIcons", '\uF15A')
    //                            .OnClick(Import_Click), // Import

    //                        txtModel
    //                    ),
    //                Stack()
    //                    .Horizontal()
    //                    .WithChildren
    //                    (
    //                        txtTemplate,
    //                        txtOutput
    //                    )
    //            )
    //    );

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        // Handle Smart Detection.
    }

    private void ChangeLanguage_Click()
    {
    }

    private void Generate_Click()
    {
        if (model == null)
        {
            //MessageBox.Show("Add some data first!", "No data!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return;
        }

        if (parser.TryParse(txtTemplate.Text, out var template, out string error))
        {
            var context = new TemplateContext(model, templateOptions);
            txtOutput.Text(template.Render(context));
        }
        else
        {
            //MessageBox.Show(
            //    $"Unable to parse the template. Please check and fix any errors. {error}.",
            //    "Error",
            //    MessageBoxButtons.OK,
            //    MessageBoxIcon.Error);
        }
    }

    private void Import_Click()
    {
        //listModel.Items.Clear();

        //var structure = JsonHelper.GetStructure(txtInput.Text);

        //foreach (string item in structure)
        //{
        //    listModel.Items.Add(Item(Label().Text(item), value: item));
        //}

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

        model = txtInput.Text.StartsWith('[') ? JArray.Parse(txtInput.Text) : JArray.Parse($"[{txtInput.Text}]");
    }

    private void NewTemplate_Click()
    {
    }

    private void OpenTemplate_Click()
    {
    }

    private void Save_Click()
    {
    }
}