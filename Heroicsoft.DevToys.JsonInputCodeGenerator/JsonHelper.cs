using Newtonsoft.Json.Linq;

namespace Heroicsoft.DevToys.JsonInputCodeGenerator;

public class JsonHelper
{
    public static IEnumerable<string> GetStructure(string json)
    {
        var result = new List<string>();
        var token = JToken.Parse(json);
        ProcessToken(result, token, string.Empty);
        return result;
    }

    private static void ProcessToken(ICollection<string> result, JToken token, string path)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (var prop in token.Children<JProperty>())
                {
                    ProcessToken(result, prop.Value, Prefix(path, prop.Name));
                }
                break;

            case JTokenType.Array:
                int index = 0;
                foreach (var childToken in token.Children())
                {
                    ProcessToken(result, childToken, Prefix(path, "[]"));
                    index++;
                }
                break;

            default:
                if (!result.Contains(path))
                {
                    result.Add(path);
                }
                break;
        }
    }

    private static string Prefix(string path, string name) => string.IsNullOrEmpty(path) ? name : $"{path}.{name}";
}