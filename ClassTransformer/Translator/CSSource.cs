using ClassTransformer.Extensions;
using ClassTransformer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClassTransformer.Translator
{
    public class CSSource : ILanguageSource
    {
        public string Label
        {
            get { return "C#"; }
        }

        public CodeFile GetClasses(string text)
        {
            var lines = text.Split('\n');
            lines = RemoveEmptyLines(lines);
            lines = RemoveUsings(lines);
            lines = RemoveComments(lines);
            lines = RemoveAttributes(lines);
            lines = RemoveNamespace(lines);

            var classes = GetFile(lines);
            return classes;
        }

        private string[] RemoveEmptyLines(string[] lines)
        {
            return lines
                .Where(l => !string.IsNullOrEmpty(l))
                .Select(l => l.Trim()).ToArray();
        }

        private string[] RemoveUsings(string[] lines)
        {
            return lines.Where(l => !l.StartsWith("using ")).ToArray();
        }

        private string[] RemoveComments(string[] lines)
        {
            return lines.Where(l => !l.StartsWith("//")).ToArray();
        }

        private string[] RemoveAttributes(string[] lines)
        {
            return lines.Where(l => !l.StartsWith("[")).ToArray();
        }

        private string[] RemoveNamespace(string[] lines)
        {
            var hasNamespace = lines.Any(l => l.StartsWith("namespace "));
            if (!hasNamespace)
            {
                return lines;
            }

            return lines.Skip(2).SkipLast(1).ToArray();
        }

        private CodeFile GetFile(string[] lines)
        {

            List<ParseObject> classesLines = GetClassObjects(lines);
            return new CodeFile()
            {
                Classes = classesLines.Where(c => c.Type == "class").Select(c => GetClasses(c)).ToArray(),
                Enums = classesLines.Where(c => c.Type == "enum").Select(c => GetEnums(c)).ToArray(),
            };
        }

        private static List<ParseObject> GetClassObjects(string[] lines)
        {
            var classesLines = new List<ParseObject>();
            foreach (var line in lines)
            {
                if (line.Contains(" class "))
                {
                    classesLines.Add(new ParseObject()
                    {
                        Type = "class",
                        Lines = new List<string>(),
                    });
                }
                else if (line.Contains(" enum "))
                {
                    classesLines.Add(new ParseObject()
                    {
                        Type = "enum",
                        Lines = new List<string>(),
                    });
                }

                classesLines.Last().Lines.Add(line);
            }

            return classesLines;
        }

        private CodeClass GetClasses(ParseObject parseObject)
        {
            var lines = parseObject.Lines;
            var result = new CodeClass();
            (result.Name, lines) = GetObjectName(lines, "class");
            (result.Properties, lines) = GetClassProperties(lines); 
            return result;
        }

        private CodeEnum GetEnums(ParseObject parseObject)
        {
            var lines = parseObject.Lines;
            var result = new CodeEnum();
            (result.Name, lines) = GetObjectName(lines, "enum");
            (result.Entries, lines) = GetEnumEntries(lines);
            return result;
        }

        private (string, List<string>) GetObjectName(List<string> lines, string objectKeyword)
        {
            var line = lines.First();
            line = line.TrimStart("public ").TrimStart("private ").TrimStart("protected ").TrimStart("internal ");
            line = line.TrimStart($"{objectKeyword} ");
            line = Regex.Replace(line.Split()[0], @"[^0-9a-zA-Z\ ]+", "");
            return (line, lines.Skip(1).ToList());
        }

        private (List<CodeProperty>, List<string>) GetClassProperties(List<string> lines)
        {
            var resultLines = new List<string>();
            var resultProperties = new List<CodeProperty>();

            foreach (var line in lines)
            {
                var withoutDefaultValue = line.Split("=").First().Trim();
                var trimmedSpaces = withoutDefaultValue.Replace(" ", "");
                var isProperty = trimmedSpaces.EndsWith("{get;set;}") ||
                    trimmedSpaces.EndsWith("{get;}") ||
                    trimmedSpaces.EndsWith("{set;}") ||
                    trimmedSpaces.EndsWith("{set;get;}");
                if (!isProperty)
                {
                    continue;
                }

                var modifiers = new string[] { "private", "protected", "public", "internal" };
                var lineSplitted = withoutDefaultValue.Split(" ")
                    .Where(l => !modifiers.Any(m => m == l))
                    .ToList();
                resultProperties.Add(new CodeProperty()
                {
                    Name = lineSplitted[1],
                    Type = lineSplitted[0],
                });
            }

            return (resultProperties, resultLines);
        }

        private (List<string>, List<string>) GetEnumEntries(List<string> lines)
        {
            var resultEntries = new List<string>();
            foreach (var line in lines)
            {
                var formatted = line.Trim(',');
                if (formatted == "{" || formatted == "}")
                {
                    continue;
                }

                resultEntries.Add(formatted);
            }

            return (resultEntries, lines);
        }
    }
}

class ParseObject
{
    public string Type { get; set; }
    public List<string> Lines { get; set; }
}