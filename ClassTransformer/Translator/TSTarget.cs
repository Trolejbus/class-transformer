using ClassTransformer.Models;
using System.Collections.Generic;

namespace ClassTransformer.Translator
{
    public class TSTarget : ILanguageTarget
    {
        public string Label
        {
            get { return "Typescript"; }
        }

        public string Stringify(IEnumerable<CodeClass> codeClasses)
        {
            var result = "";
            foreach(var codeClass in codeClasses)
            {
                result += StringifyClass(codeClass);
                result += "\n";
            }

            return result;
        }

        private string StringifyClass(CodeClass codeClass)
        {
            var result = "";
            result += $"export class {codeClass.Name} {{\n";
            result = StringifyParameters(result, codeClass.Properties);
            result += "}\n";
            return result;
        }

        private string StringifyParameters(string result, List<CodeProperty> properties)
        {
            foreach (var property in properties)
            {
                result += $"\tpublic {property.Name}: {TranslateMethod(property)};\n";
            }

            return result;
        }

        private string TranslateMethod(CodeProperty property)
        {
            switch (property.Type)
            {
                case "int":
                case "decimal":
                    return "number";
                case "bool":
                    return "boolean";
                default:
                    return property.Type;
            };
        }
    }
}
