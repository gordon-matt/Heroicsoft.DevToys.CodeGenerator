using System.ComponentModel.Composition;
using System.Data;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DevToys.Api;
using Extenso;
using Fluid;
using Heroicsoft.DevToys.CodeGenerator;
using Heroicsoft.DevToys.CodeGenerator.Extensions;
using Newtonsoft.Json.Linq;
using static DevToys.Api.GUI;

namespace Heroicsoft.DevToys.CsvInputCodeGenerator;

[Export(typeof(IGuiTool))]
[Name("CsvInputCodeGenerator")]
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",
    IconGlyph = '\uEE3A',  // An icon that represents a wand
    GroupName = PredefinedCommonToolGroupNames.Generators,
    ResourceManagerAssemblyIdentifier = nameof(CsvInputCodeGeneratorResourceAssemblyIdentifier),
    ResourceManagerBaseName = "Heroicsoft.DevToys.CsvInputCodeGenerator.CsvInputCodeGenerator",
    ShortDisplayTitleResourceName = nameof(CsvInputCodeGenerator.ShortDisplayTitle),
    LongDisplayTitleResourceName = nameof(CsvInputCodeGenerator.LongDisplayTitle),
    DescriptionResourceName = nameof(CsvInputCodeGenerator.Description),
    AccessibleNameResourceName = nameof(CsvInputCodeGenerator.AccessibleName))]
internal sealed class CsvInputCodeGeneratorGui : IGuiTool
{
    private readonly FluidParser parser;
    private readonly TemplateOptions templateOptions;
    private object model = null;

    private bool hasHeaderRow = true;

    private IUISwitch switchHasHeaderRow = Switch();
    private IUISingleLineTextInput txtDelimiter = SingleLineTextInput();
    private IUIDropDownButton ddbLanguage = DropDownButton();

    private IUIInfoBar infoBar = InfoBar();
    private IUIFileSelector fileSelector = FileSelector();
    private IUIMultiLineTextInput txtModel = MultiLineTextInput();
    private IUIMultiLineTextInput txtOutput = MultiLineTextInput();
    private IUIMultiLineTextInput txtTemplate = MultiLineTextInput();

    public CsvInputCodeGeneratorGui()
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
                                .Close()
                        )
                ),

                // Input + Import button (left column)
                Cell(
                    RowId.Content, RowId.Content,
                    ColumnId.Left, ColumnId.Left,

                    Stack()
                        .Vertical()
                        .WithChildren(
                            fileSelector
                                .CanSelectOneFile()
                                .LimitFileTypesTo(".csv", ".txt", ".tsv", ".tab")
                                .OnFilesSelected(async x => await OnFileSelected(x)),

                            Setting()
                                .Title("Has Header Row")
                                .Description("Are there column headers in the file?")
                                .InteractiveElement(
                                    switchHasHeaderRow
                                        .On()
                                        .OnText("Yes")
                                        .OffText("No")
                                        .OnToggle((isOn) =>
                                        {
                                            hasHeaderRow = isOn;
                                            return ValueTask.CompletedTask;
                                        })
                                ),

                            Setting()
                                .Title("Delimiter")
                                .Description("Specify the delimiter used in the file.")
                                .InteractiveElement(
                                    txtDelimiter
                                        .Text(",")
                                        .HideCommandBar()
                                )

                        )
                ),

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
                        .Text(Constants.InitialTemplateCode)
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

    private async Task OnFileSelected(SandboxedFileReader[] files)
    {
        try
        {
            infoBar.Close();
            using var memoryStream = new MemoryStream();
            await files[0].CopyFileContentToAsync(memoryStream, CancellationToken.None);

            // Reset the stream position to the beginning
            memoryStream.Position = 0;

            var data = ReadCsv(memoryStream);

            // Handle the selected files.
            // [...]
            for (int i = 0; i < files.Length; i++)
            {
                files[i].Dispose();
            }

            var sb = new StringBuilder();
            sb.AppendLine("Properties for each item in 'Model' are as follows: ");

            foreach (DataColumn column in data.Columns)
            {
                column.ColumnName = column.ColumnName.Trim().SplitPascal().ToPascalCase();
                sb.AppendLine($"- {column.ColumnName}");
            }

            txtModel.Text(sb.ToString());

            model = new
            {
                Model = JArray.Parse(data.JsonSerialize())
            };
        }
        catch (Exception ex)
        {
            infoBar.Title("Error");
            infoBar.Description(ex.GetBaseException().Message);
            infoBar.Open();
        }
    }

    private DataTable ReadCsv(Stream fileStream)
    {
        using var streamReader = new StreamReader(fileStream);
        using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
            HasHeaderRecord = hasHeaderRow,
            Delimiter = txtDelimiter.Text
        });
        csvReader.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add(string.Empty);
        using var csvDataReader = new CsvDataReader(csvReader);

        var table = new DataTable();
        table.Load(csvDataReader);
        return table;
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
}