using ClassTransformer.Models;
using System.Collections.Generic;

namespace ClassTransformer.Translator
{
    public interface ILanguageSource
    {
        string Label { get; }
        IEnumerable<CodeClass> GetClasses(string text);    
    }
}
