using System;
using System.Reflection;

namespace Chronozoom.AdlibImporter.Backend.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}