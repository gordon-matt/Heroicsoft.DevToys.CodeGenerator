using System.ComponentModel.Composition;
using DevToys.Api;

namespace Heroicsoft.DevToys.JsonInputCodeGenerator;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(JsonInputCodeGeneratorResourceAssemblyIdentifier))]
internal sealed class JsonInputCodeGeneratorResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}