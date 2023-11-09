using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ECSSourceGenerator
{
    [Generator]
    public class PvoSourceGenerator : ISourceGenerator
    {
        private readonly Dictionary<string, string> typeLookup = new Dictionary<string, string>()
        {
            { "int", "System.Int32" },
            { "float", "System.Single" },
            { "double", "System.Double" },
        };

        public void Execute(GeneratorExecutionContext context)
        {
            var names = context.Compilation.SyntaxTrees.SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                .Where(x => x is ClassDeclarationSyntax)
                .Cast<ClassDeclarationSyntax>()
                .Where(c => c.BaseList?.ToString().Contains("IPvo") ?? false)
                .Select(c =>
                {
                    var t = c.Identifier.Text;
                    return new
                    {
                        Identifier =t.Substring(0, t.Length - 1),
                        Type = c.BaseList.ToString().Split('<')[1].TrimEnd('>')
                    };
                })
                .Distinct();
            ;


            foreach (var name in names)
            {
                var builder = new IndentBuilder();
                builder.AppendLine("using System;");
                var tostring = GetToString(name.Identifier);

                var hashcode = name.Type == "int" ? "Value" : "Value.GetHashCode()";
                builder.AppendLine("#if DEBUG");
                builder.AppendLine($"public readonly partial struct {name.Identifier} : IEquatable<{name.Identifier}>");
                builder.Braces(() =>
                {
                    builder.AppendLine(
                        $@"
    public readonly {name.Type} Value;

    public {name.Identifier}(in {name.Type} value)
    {{
        Value = value;
    }}

    public override int GetHashCode() => {hashcode};

    public static bool operator ==({name.Identifier} left, {name.Identifier} right) => left.Value == right.Value;

    public static bool operator !=({name.Identifier} left, {name.Identifier} right) => left.Value != right.Value;

    public bool Equals({name.Identifier} other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is {name.Identifier} other && other.Value == Value;

    //public int CompareTo({name.Identifier} other)
    //{{
    //    if (Value < other.Value) return -1;
    //    return Value > other.Value ? 1 : 0;
    //}}

    {tostring}

    public static implicit operator {name.Identifier}({name.Type} value) => new(value);
    public static implicit operator {name.Type}({name.Identifier} value) => value.Value;
");
                });
                builder.AppendLine("#endif");

                context.AddSource($"{name.Identifier}PVO.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            }


            var b = new IndentBuilder();
            b.AppendLine("#if !DEBUG");
            foreach (var name in names)
            {
                var actualType = typeLookup[name.Type];
                b.AppendLine($"global using {name.Identifier} = {actualType};");
            }

            b.AppendLine("#endif");

            context.AddSource("GlobalUsings.g.cs", SourceText.From(b.ToString(), Encoding.UTF8));
        }

        private string GetToString(string identifier)
        {
            if (identifier == "GlobalId")
            {
                //return @"public override string ToString()=> this.GetStringId() ?? ""Unknown id "" + Value.ToString();";
                return @"public override string ToString()=> Value.ToString();";
            }
            else
            {
                return @"public override string ToString()=> this.Value.ToString();";
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}