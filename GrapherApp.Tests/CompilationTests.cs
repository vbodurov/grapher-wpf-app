using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using NUnit.Framework;

namespace GrapherApp.Tests
{
    public static class f
    {
        public static double cube(double x)
        {
            return x * x * x;
        }
    }

    public class BaseFuncRunner
    {
        public virtual double RunFunc(double x)
        {
            return x;
        }
    }

    [TestFixture]
    public class CompilationTests
    {
        [Test]
        public void CanCompileAndRun()
        {
            var defaultReferences =
                new[] {"mscorlib.dll", "System.dll", "System.Core.dll"}
                    .Select(e => @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\" + e)
                    .ToArray();

            var script =
                @"using System;
                public class FuncRunner : GrapherApp.Tests.BaseFuncRunner
                {
                    public override double RunFunc(double x)
                    {
                        return x*x;
                    }
                }";


            var syntaxTree = CSharpSyntaxTree.ParseText(script);

            // Compile the SyntaxTree to a CSharpCompilation
            var compilation = CSharpCompilation.Create("Script",
                new[] {syntaxTree},
                defaultReferences
                    .Select(x => MetadataReference.CreateFromFile(x))
                    .Union(new[] { MetadataReference.CreateFromFile(GetType().Assembly.Location) }),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                Assembly assembly = Assembly.Load(ms.ToArray());
                var type = assembly.GetType("FuncRunner");
                var bfr = Activator.CreateInstance(type) as GrapherApp.Tests.BaseFuncRunner;

                Console.WriteLine(bfr.RunFunc(2));
            }
        }
    }



}