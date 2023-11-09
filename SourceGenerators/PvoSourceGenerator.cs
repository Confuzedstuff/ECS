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
        public void Execute(GeneratorExecutionContext context)
        {
            var names = context.Compilation.SyntaxTrees.SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                .Where(x => x is StructDeclarationSyntax)
                .Cast<StructDeclarationSyntax>()
                .Where(c => c.BaseList?.ToString().Contains("IPvo") ?? false)
                .Select(c => new
                {
                    Identifier = c.Identifier.Text,
                    Type = c.BaseList.ToString().Split('<')[1].TrimEnd('>')
                })
                .Distinct();
            ;


            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("using System;");
            foreach (var name in names)
            {
                var tostring = GetToString(name.Identifier);

                var hashcode = name.Type == "int" ? "Value" : "Value.GetHashCode()";
                sourceBuilder.Append(
                    $@"
public readonly partial struct {name.Identifier} : IEquatable<{name.Identifier}>
{{
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

    // TODO public static implicit operator {name.Identifier}({name.Type} value) => new(value);
}}
");
            }


            context.AddSource("pvoSourceGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
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