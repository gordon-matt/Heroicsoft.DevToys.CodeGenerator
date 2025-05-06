using System.ComponentModel.Composition;
using DevToys.Api;

namespace Heroicsoft.DevToys.CsvInputCodeGenerator;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(CsvInputCodeGeneratorResourceAssemblyIdentifier))]
internal sealed class CsvInputCodeGeneratorResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}