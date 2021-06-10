using ClassTransformer.Models;
using System.Collections.Generic;

namespace ClassTransformer.Translator
{
    public interface ILanguageSource
    {
        string Label { get; }
        CodeFile GetClasses(string text);    
    }
}
