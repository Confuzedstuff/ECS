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
                        Identifier = t.Substring(0, t.Length - 1),
                        Type = c.BaseList.ToString().Split('<')[1].Split('>')[0]
                    };
                })
                .Distinct();
            ;


            foreach (var name in names)
            {
                var builder = new IndentBuilder();
                builder.AppendLine("using System;");
                builder.AppendLine("using System.Runtime.CompilerServices;");
                

                var hashcode = name.Type == "int" ? "Value" : "Value.GetHashCode()";
                builder.AppendLine($"public readonly partial struct {name.Identifier} : IEquatable<{name.Identifier}>");
                builder.AppendLine($", IComparable<{name.Identifier}>");
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

    public int CompareTo({name.Identifier} other)
    {{
        if (Value < other.Value) return -1;
        return Value > other.Value ? 1 : 0;
    }}

   public static bool operator <({name.Identifier} left, {name.Identifier} right) => left.CompareTo(right) < 0;

    public static bool operator >({name.Identifier} left, {name.Identifier} right) => left.CompareTo(right) > 0;

    public static bool operator <=({name.Identifier} left, {name.Identifier} right) => left.CompareTo(right) <= 0;

    public static bool operator >=({name.Identifier} left, {name.Identifier} right) => left.CompareTo(right) >= 0;

    public override string ToString() => Value.ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator {name.Identifier}({name.Type} value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]    
    public static implicit operator {name.Type}({name.Identifier} value) => value.Value;
");
                });

                context.AddSource($"{name.Identifier}PVO.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}