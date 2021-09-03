using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FastEnumStringGenerator
{
    [Generator]
    internal class EnumToStringGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var builder = new StringBuilder();
            builder.Append(@"
using System;

namespace Generated
{
    public static class EnumExtensions
    {");

            foreach (var (space, name, members) in GetEnums(context.Compilation))
            {
                var full = $"{space}.{name}";
                builder.Append($@"
        public static string ToStringFast(this {full} val) => val switch
        {{");
                foreach (var member in members)
                {
                    builder.Append(@$"
            {full}.{member} => ""{member}"",");
                }
                builder.Append(@$"
            _ => throw new {nameof(NotSupportedException)}(""Unkonwn enum member"")
        }};");
            }

            builder.Append(@"
    }
}");

            context.AddSource("EnumGenerator", SourceText.From(builder.ToString(), Encoding.UTF8));
        }

        public IEnumerable<(string space, string name, IEnumerable<string> members)> GetEnums(Compilation compilation)
        {
            var nodes = compilation.SyntaxTrees.SelectMany(t => t.GetRoot().DescendantNodes());
            var enums = nodes.Where(t => t.IsKind(SyntaxKind.EnumDeclaration))
                .OfType<EnumDeclarationSyntax>();

            return enums.Select(e => (CollectNamespaces(e.Ancestors().ToList()), e.Identifier.Text, e.Members.Select(s => s.Identifier.Text)));
        }

        public string CollectNamespaces(IEnumerable<SyntaxNode> nodes)
        {
            var spaces = nodes.OfType<NamespaceDeclarationSyntax>()
                .Select(n => $"{n.Name}");

            return string.Join(".", spaces.Reverse());
        }

        public void Initialize(GeneratorInitializationContext context)
        {

        }
    }
}