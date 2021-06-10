using ClassTransformer.Models;
using System.Collections.Generic;

namespace ClassTransformer.Translator
{
    public interface ILanguageTarget
    {
        string Label { get; }
        string Stringify(IEnumerable<CodeClass> codeClasses);
    }
}
