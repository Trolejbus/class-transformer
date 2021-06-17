using ClassTransformer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassTransformer.Translator
{
    public class TSTarget : ILanguageTarget
    {
        public string Label
        {
            get { return "Typescript"; }
        }

        public string Stringify(CodeFile file, StringifyConfig stringifyConfig)
        {
            var result = "";
            foreach (var codeEnum in file.Enums)
            {
                result += StringifyEnum(codeEnum);
                result += "\n";
            }


            foreach (var codeClass in file.Classes)
            {
                result += StringifyClass(codeClass, stringifyConfig);
                result += "\n";
            }

            return result;
        }

        private string StringifyEnum(CodeEnum codeEnum)
        {
            var result = "";
            result += $"export enum {codeEnum.Name} {{\n";
            result = StringifyEnumEntries(result, codeEnum.Entries);
            result += "}\n";
            return result;
        }

        private string StringifyEnumEntries(string result, List<string> entries)
        {
            foreach (var entry in entries)
            {
                result += $"\t{entry} = \"{entry}\",\n";
            }

            return result;
        }

        private string StringifyClass(CodeClass codeClass, StringifyConfig stringifyConfig)
        {
            var result = "";
            if (stringifyConfig.ConvertClassToInteface)
            {
                result += $"export interface {codeClass.Name} {{\n";
            }
            else
            {
                result += $"export class {codeClass.Name} {{\n";
            }

            result = StringifyParameters(result, codeClass.Properties, stringifyConfig);
            result += "}\n";
            return result;
        }

        private string StringifyParameters(string result, List<CodeProperty> properties, StringifyConfig stringifyConfig)
        {
            foreach (var property in properties)
            {
                if (stringifyConfig.ConvertClassToInteface)
                {
                    result += $"\t{ToLowerCammelcase(property)}: {TranslateMethod(property)};\n";
                }
                else
                {
                    result += $"\tpublic {ToLowerCammelcase(property)}: {TranslateMethod(property)};\n";
                }
            }

            return result;
        }

        private static string ToLowerCammelcase(CodeProperty property)
        {
            var name = property.Name;
            var firstLetter = property.Name.TakeWhile(t => Char.IsUpper(t)).ToArray();
            var firstLetterFormatted = new string(firstLetter).ToLower();
            return firstLetterFormatted + name.Substring(firstLetterFormatted.Length);
        }

        private string TranslateMethod(CodeProperty property)
        {
            switch (property.Type)
            {
                case "int[]":
                case "decimal[]":
                    return "number[]";
                case "int":
                case "decimal":
                    return "number";
                case "bool":
                    return "boolean";
                case "DateTime":
                    return "Date";
                default:
                    return property.Type;
            };
        }
    }
}
