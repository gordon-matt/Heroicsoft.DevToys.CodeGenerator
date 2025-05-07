using DevToys.Api;

namespace Heroicsoft.DevToys.CodeGenerator;

public static class UIHelper
{
    public static void ChangeLanguage(string lang, params IUIMultiLineTextInput[] uIMultiLineTextInputs)
    {
        foreach (var uIMultiLineTextInput in uIMultiLineTextInputs)
        {
            uIMultiLineTextInput.Language(lang);
        }
    }

    public static IUIDropDownMenuItem[] GetMenuItems(params IUIMultiLineTextInput[] uIMultiLineTextInputs) =>
    [
        GUI.DropDownMenuItem()
            .Text("None")
            .OnClick(() => ChangeLanguage(string.Empty, uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("C++")
            .OnClick(() => ChangeLanguage("cpp", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("C#")
            .OnClick(() => ChangeLanguage("csharp", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("CSS")
            .OnClick(() => ChangeLanguage("css", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("Go")
            .OnClick(() => ChangeLanguage("go", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("HTML")
            .OnClick(() => ChangeLanguage("html", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("Java")
            .OnClick(() => ChangeLanguage("java", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("JavaScript")
            .OnClick(() => ChangeLanguage("javascript", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("Lua")
            .OnClick(() => ChangeLanguage("lua", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("Markdown")
            .OnClick(() => ChangeLanguage("markdown", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("Razor")
            .OnClick(() => ChangeLanguage("razor", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("Rust")
            .OnClick(() => ChangeLanguage("rust", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("SQL")
            .OnClick(() => ChangeLanguage("sql", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("TypeScript")
            .OnClick(() => ChangeLanguage("typescript", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("Visual Basic")
            .OnClick(() => ChangeLanguage("vb", uIMultiLineTextInputs)),
        GUI.DropDownMenuItem()
            .Text("XML")
            .OnClick(() => ChangeLanguage("xml", uIMultiLineTextInputs))
    ];
}