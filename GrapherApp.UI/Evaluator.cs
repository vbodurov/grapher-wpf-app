using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;


namespace GrapherApp.UI
{
    public class Evaluator
    {
        private const string _source = @"import System;

                class Eval
                {

                    public function ifNotNan(a : double, b : double) { return double.IsNaN(a) ? b : a; }
                    public function ifElse(condition : boolean, ifTrue : double, ifFalse : double) { return condition ? ifTrue : ifFalse; }
                    public function avg(a : double, b : double) { return (b - a) * 0.5 + a; }
                    public function pow(x : double, n : double) { return System.Math.Pow(x, n); }
                    public function abs(x : double) { return System.Math.Abs(x); }
                    public function acos(x : double) { return System.Math.Acos(x); }
                    public function asin(x : double) { return System.Math.Asin(x); }
                    public function atan(x : double) { return System.Math.Atan(x); }
                    public function atan2(x : double, y : double) { return System.Math.Atan2(x, y); }
                    public function sin(x : double) { return System.Math.Sin(x); }
                    public function sinh(x : double) { return System.Math.Sinh(x); }
                    public function cos(x : double) { return System.Math.Cos(x); }
                    public function cosh(x : double) { return System.Math.Cosh(x); }
                    public function tan(x : double) { return System.Math.Tan(x); }
                    public function tanh(x : double) { return System.Math.Tanh(x); }
                    public function sqrt(x : double) { return System.Math.Sqrt(x); }
                    public function sign(x : double) { return System.Math.Sign(x); }
                    public function max(x : double, y : double) { return System.Math.Max(x, y); }
                    public function min(x : double, y : double) { return System.Math.Min(x, y); }
                    public function exp(x : double) { return System.Math.Exp(x); }
                    public function floor(x : double) { return System.Math.Floor(x); }
                    public function ceiling(x : double) { return System.Math.Ceiling(x); }
                    public function round(x : double) { return System.Math.Round(x); }
                    public function log(x : double) { return System.Math.Log(x); }
                    public function log(x : double, y : double) { return System.Math.Log(x, y); }
                    public function log10(x : double) { return System.Math.Log10(x); }

                    public function IfNotNan(a : double, b : double) { return ifNotNan(a, b); }
                    public function IfElse(condition : boolean, ifTrue : double, ifFalse : double) { return ifElse(condition, ifTrue, ifFalse); }
                    public function Avg(a : double, b : double) { return avg(a,b); }
                    public function Pow(x : double, n : double) { return pow(x, n); }
                    public function Abs(x : double) { return abs(x); }
                    public function Acos(x : double) { return acos(x); }
                    public function Asin(x : double) { return asin(x); }
                    public function Atan(x : double) { return atan(x); }
                    public function Atan2(x : double, y : double) { return atan2(x, y); }
                    public function Sin(x : double) { return sin(x); }
                    public function Sinh(x : double) { return sinh(x); }
                    public function Cos(x : double) { return cos(x); }
                    public function Cosh(x : double) { return cosh(x); }
                    public function Tan(x : double) { return tan(x); }
                    public function Tanh(x : double) { return tanh(x); }
                    public function Sqrt(x : double) { return sqrt(x); }
                    public function Sign(x : double) { return sign(x); }
                    public function Max(x : double, y : double) { return max(x, y); }
                    public function Min(x : double, y : double) { return min(x, y); }
                    public function Exp(x : double) { return exp(x); }
                    public function Floor(x : double) { return floor(x); }
                    public function Ceiling(x : double) { return ceiling(x); }
                    public function Round(x : double) { return round(x); }
                    public function Log(x : double) { return log(x); }
                    public function Log(x : double, y : double) { return log(x, y); }
                    public function Log10(x : double) { return log10(x); }

                    public function EvaluateCode(code : String, x : double) : double
                    {
                        var E : double = Math.E;
                        var e : double = Math.E;
                        var PI : double = Math.PI;
                        var pi : double = Math.PI;
                        return eval(code);
                    }
                }";

        private delegate double EvaluatorFunc(string code, double x);
        private static EvaluatorFunc _evaluatorFunc;

        public Evaluator()
        {
            SetUp();
        }

        private void SetUp()
        {
            var cp = new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false };
            foreach (var assembly in AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly =>
                        !assembly.IsDynamic &&
                        System.IO.File.Exists(assembly.Location)))
            {
                cp.ReferencedAssemblies.Add(assembly.Location);
            }

            var compilerResult = 
                new Microsoft.JScript.JScriptCodeProvider()
                .CompileAssemblyFromSource(cp, _source);

            if(compilerResult.Errors.Count > 0)
            {
                throw new InvalidOperationException("JavaScript evaluator error: "+String.Join("|", compilerResult.Errors.Cast<CompilerError>().Select(err => err.FileName + ":" + err.ErrorText).ToArray()));
            }
            
            var evalType =
                compilerResult
                    .CompiledAssembly
                    .GetType("Eval");

            _evaluatorFunc =
                Delegate.CreateDelegate(
                        typeof(EvaluatorFunc),
                        Activator.CreateInstance(evalType), "EvaluateCode") as EvaluatorFunc;
        }

        public double Evaluate(string code, double x, out string error)
        {
            try
            {
                code = code.Trim();

                code =
                    new StringBuilder(code)
                        .Replace("Math.", "")
                        .Replace("0f", "0")
                        .Replace("1f", "1")
                        .Replace("2f", "2")
                        .Replace("3f", "3")
                        .Replace("4f", "4")
                        .Replace("5f", "5")
                        .Replace("6f", "6")
                        .Replace("7f", "7")
                        .Replace("8f", "8")
                        .Replace("9f", "9")
                        .Replace("return ", "")
                        .Replace("const ", "")
                        .Replace("double ", "var ")
                        .Replace("float ", "var ")
                        .Replace("(float)", "")
                        .ToString();

                error = null;
                return _evaluatorFunc(code, x);
            }
            catch(Exception ex)
            {
                error = ex.Message;

                return Double.NaN;
            }
        }

    }
}