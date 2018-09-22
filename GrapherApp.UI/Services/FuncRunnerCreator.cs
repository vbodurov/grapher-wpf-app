using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace GrapherApp.UI.Services
{
    public interface IFuncRunnerCreator
    {
        bool TryGetRunner(string code, out BaseFuncRunner runner, out IList<string> errors);
    }
    public sealed class FuncRunnerCreator : IFuncRunnerCreator
    {
        private readonly IDictionary<string, BaseFuncRunner> _runnerByCodeCache = 
            new Dictionary<string, BaseFuncRunner>();

        bool IFuncRunnerCreator.TryGetRunner(string code, out BaseFuncRunner runner, out IList<string> errors)
        {
            errors = new List<string>();
            runner = null;
            var defaultReferences =
                new[]
                {
                    typeof(Activator).Assembly.Location,// mscorlib
                    typeof(UriBuilder).Assembly.Location, // System
                    typeof(Queryable).Assembly.Location, // System.Core,
                    typeof(IFuncRunnerCreator).Assembly.Location // this assembly
                };

            if (code == null || code.Trim() == "")
            {
                errors.Add("Missing function code.");
                return false;
            }

            code = code.Trim();
            var lines = code.Split('\n');
            var lastLineIndex = lines.Length - 1;

            var firstLine = lines[0].Trim();
            var lastLine = lines[lastLineIndex].Trim();
            var lastLineIsBracket = (lastLine == ")" || lastLine == ");");
            var sourceHasNoReturnWord = code.IndexOf("return", StringComparison.InvariantCulture) < 0;
            if (sourceHasNoReturnWord && lastLineIsBracket && !firstLine.StartsWith("var "))
            {
                lines[0] = "return " + firstLine;
                sourceHasNoReturnWord = false;
            }
            if (sourceHasNoReturnWord && !lastLineIsBracket)
            {
                if (firstLine.StartsWith("bezier"))
                {
                    lines[0] = "return " + firstLine;
                }
                else
                {
                    lines[lastLineIndex] = "return " + lastLine;
                }
            }
            if (!lastLine.EndsWith(";"))
                lines[lastLineIndex] = lines[lastLineIndex] + ";";
            code = String.Join("\n", lines);

            sourceHasNoReturnWord = code.IndexOf("return", StringComparison.InvariantCulture) < 0;
            if(sourceHasNoReturnWord)
            {
                errors.Add("Missing word 'return' - cannot infer where to put the 'return' statement. Please add 'return' word manually.");
                return false;
            }

            if (_runnerByCodeCache.TryGetValue(code, out runner))
            {
                return true;
            }

            var name = "FuncRunner" + DateTime.UtcNow.Ticks;

            var script = @"
                using System;
                using GrapherApp.UI;

                public class " + name + @" : GrapherApp.UI.Services.BaseFuncRunner
                {
                    public override double Run(double x)
                    {
                        var xx = x;
                        var xxx = x;
                        var xxxx = x;
                        " + code + @";
                    }
                }";


            var syntaxTree = CSharpSyntaxTree.ParseText(script);

            // Compile the SyntaxTree to a CSharpCompilation
            var compilation = CSharpCompilation.Create("Script",
                new[] { syntaxTree },
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

                    errors.Add("Compilation errors");
                    foreach (Diagnostic diagnostic in failures)
                    {
                        errors.Add(diagnostic.GetMessage());
                    }

                    return false;
                }
                Assembly assembly = Assembly.Load(ms.ToArray());
                var type = assembly.GetType(name);
                runner = Activator.CreateInstance(type) as BaseFuncRunner;

                if (runner == null)
                {
                    errors.Add("Cannot instantiate BaseFuncRunner");
                    return false;
                }

                _runnerByCodeCache[code] = runner;
            }

            return true;
        }
    }
}