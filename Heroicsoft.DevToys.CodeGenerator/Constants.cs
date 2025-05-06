namespace Heroicsoft.DevToys.CodeGenerator;

public static class Constants
{
    public const string InitialTemplateCode =
@"{% for item in Model %}
{{ item.Foo }}
{% endfor %}";

}