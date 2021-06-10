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

        public IEnumerable<CodeClass> GetClasses(string text)
        {
            var lines = text.Split('\n');
            lines = RemoveEmptyLines(lines);
            lines = RemoveUsings(lines);
            lines = RemoveComments(lines);
            lines = RemoveAttributes(lines);
            lines = RemoveNamespace(lines);

            var classes = GetClasses(lines);
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

        private IEnumerable<CodeClass> GetClasses(string[] lines)
        {
            List<List<string>> classesLines = GetClassLines(lines);
            return classesLines.Select(c => GetClasses(c)).ToArray();
        }

        private static List<List<string>> GetClassLines(string[] lines)
        {
            var classesLines = new List<List<string>>();
            foreach (var line in lines)
            {
                if (line.Contains(" class "))
                {
                    classesLines.Add(new List<string>());
                }

                classesLines.Last().Add(line);
            }

            return classesLines;
        }

        private CodeClass GetClasses(List<string> lines)
        {
            var result = new CodeClass();
            (result.Name, lines) = GetClassName(lines);
            (result.Properties, lines) = GetClassProperties(lines); 

            return result;
        }

        private (string, List<string>) GetClassName(List<string> lines)
        {
            var line = lines.First();
            line = line.TrimStart("public ").TrimStart("private ").TrimStart("protected ").TrimStart("internal ");
            line = line.TrimStart("class ");
            line = Regex.Replace(line.Split()[0], @"[^0-9a-zA-Z\ ]+", "");
            return (line, lines.Skip(1).ToList());
        }

        private (List<CodeProperty>, List<string>) GetClassProperties(List<string> lines)
        {
            var resultLines = new List<string>();
            var resultProperties = new List<CodeProperty>();

            foreach (var line in lines)
            {
                var isProperty = line.Replace(" ", "").EndsWith("{get;set;}");
                if (!isProperty)
                {
                    continue;
                }

                var modifiers = new string[] { "private", "protected", "public", "internal" };
                var lineSplitted = line.Split(" ")
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
    }
}
