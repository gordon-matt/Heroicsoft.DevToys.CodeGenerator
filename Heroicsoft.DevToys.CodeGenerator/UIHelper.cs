using DevToys.Api;

namespace Heroicsoft.DevToys.CodeGenerator;

public static class UIHelper
{
    public static void ChangeLanguage(IUIMultiLineTextInput uIMultiLineTextInput, string lang) => uIMultiLineTextInput.Language(lang);

    public static IUIDropDownMenuItem[] GetMenuItems(IUIMultiLineTextInput uIMultiLineTextInput) =>
    [
        GUI.DropDownMenuItem()
            .Text("None")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, string.Empty)),
        GUI.DropDownMenuItem()
            .Text("C++")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "cpp")),
        GUI.DropDownMenuItem()
            .Text("C#")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "csharp")),
        GUI.DropDownMenuItem()
            .Text("CSS")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "css")),
        GUI.DropDownMenuItem()
            .Text("Go")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "go")),
        GUI.DropDownMenuItem()
            .Text("HTML")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "html")),
        GUI.DropDownMenuItem()
            .Text("Java")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "java")),
        GUI.DropDownMenuItem()
            .Text("JavaScript")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "javascript")),
        GUI.DropDownMenuItem()
            .Text("Lua")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "lua")),
        GUI.DropDownMenuItem()
            .Text("Markdown")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "markdown")),
        GUI.DropDownMenuItem()
            .Text("Razor")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "razor")),
        GUI.DropDownMenuItem()
            .Text("Rust")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "rust")),
        GUI.DropDownMenuItem()
            .Text("SQL")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "sql")),
        GUI.DropDownMenuItem()
            .Text("TypeScript")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "typescript")),
        GUI.DropDownMenuItem()
            .Text("Visual Basic")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "vb")),
        GUI.DropDownMenuItem()
            .Text("XML")
            .OnClick(() => ChangeLanguage(uIMultiLineTextInput, "xml"))
    ];
}