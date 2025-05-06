using System.ComponentModel.Composition;
using DevToys.Api;
using Heroicsoft.DevToys.CodeGenerator;
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
    public UIToolView View
        => new
        (
            Stack()
                .Vertical()
                .WithChildren
                (
                    Stack()
                        .Horizontal()
                        .WithChildren
                        (
                            Button()
                                .Icon("FluentSystemIcons", '\uF42F')
                                .OnClick(OpenTemplate_Click), // Open

                            Button()
                                .Icon("FluentSystemIcons", '\uE4DE')
                                .OnClick(NewTemplate_Click), // Add

                            Button()
                                .Icon("FluentSystemIcons", '\uF680')
                                .OnClick(Save_Click), // Save

                            Button()
                                .Icon("FluentSystemIcons", '\uF15A')
                                .OnClick(Import_Click), // Import

                            Button()
                                .Icon("FluentSystemIcons", '\uEE3A')
                                .OnClick(Generate_Click), // Generate

                            DropDownButton()
                                .AlignHorizontally(UIHorizontalAlignment.Left)
                                .Icon("FluentSystemIcons", '\uEE93') // Language
                                .Text("Lang")
                                .WithMenuItems(
                                    DropDownMenuItem()
                                        .Text("C#")
                                        .OnClick(ChangeLanguage_Click),
                                    DropDownMenuItem()
                                        .Text("C#")
                                        .OnClick(ChangeLanguage_Click),
                                    DropDownMenuItem()
                                        .Text("C#")
                                        .OnClick(ChangeLanguage_Click),
                                    DropDownMenuItem("Item 4"))
                        ),
                    Stack()
                        .Vertical()
                        .WithChildren
                        (
                            MultiLineTextInput()
                                .Title("Template")
                                .Text(Constants.InitialTemplateCode)
                                .AlwaysWrap(),

                            MultiLineTextInput()
                                .Title("Model")
                                .Text("Help text will appear here to describe the data model after you load some data")
                                .AlwaysWrap()
                        )
                )
        );

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        // Handle Smart Detection.
    }

    private void OpenTemplate_Click()
    {
    }

    private void NewTemplate_Click()
    {
    }

    private void Save_Click()
    {
    }

    private void Generate_Click()
    {
    }

    private void ChangeLanguage_Click()
    {
    }

    private void Import_Click()
    {
    }
}