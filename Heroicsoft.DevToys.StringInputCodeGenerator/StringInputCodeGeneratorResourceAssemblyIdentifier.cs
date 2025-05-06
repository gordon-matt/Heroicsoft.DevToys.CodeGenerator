using System.ComponentModel.Composition;
using DevToys.Api;

namespace Heroicsoft.DevToys.StringInputCodeGenerator;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(StringInputCodeGeneratorResourceAssemblyIdentifier))]
internal sealed class StringInputCodeGeneratorResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}