using System;
using System.Diagnostics;
using NUnit.Framework;
using Roslyn.Scripting.CSharp;

namespace GrapherApp.Tests
{
    public static class f
    {
        public static double cube(double x)
        {
            return x*x*x;
        }
    }

    [TestFixture]
    public class RoslynTests
    {
        [Test]
        public void CanCompileAndRun()
        {
            var sw = Stopwatch.StartNew();
            var scriptEngine = new ScriptEngine();
            var session = scriptEngine.CreateSession();
            session.AddReference("System");
            session.AddReference("System.Core");
            session.AddReference(this.GetType().Assembly);

            session.ImportNamespace("System");
            session.ImportNamespace("GrapherApp.Tests");

            sw.Stop();

            

            Console.WriteLine("time 1: "+sw.ElapsedMilliseconds);

            session.Execute(@"
                const double PI = Math.PI;
                double pow(double a, double b) { return Math.Pow(a, b); }
                double sin(double n) { return Math.Sin(n); }
                ");


        
            sw = Stopwatch.StartNew();
            var f = session.Execute<Func<double, double>>(@"
                Func<double, double> factory() { 
                    return x =>{ 
                                return pow(x,4); 
                            }; 
                 } 
                 factory();");

            

            sw.Stop();
            Console.WriteLine("time 2: " + sw.ElapsedMilliseconds);

            Console.WriteLine(f);
            Console.WriteLine(f(0));
            Console.WriteLine(f(0.5));
            Console.WriteLine(f(1));

            sw = Stopwatch.StartNew();
            f = session.Execute<Func<double, double>>(@"
                Func<double, double> factory() { 
                    return x =>{ 
                                return pow(x,4)*sin(x); 
                            }; 
                 } 
                 factory();");



            sw.Stop();
            Console.WriteLine("time 2: " + sw.ElapsedMilliseconds);

            Console.WriteLine(f);
            Console.WriteLine(f(0));
            Console.WriteLine(f(0.5));
            Console.WriteLine(f(1));

            sw = Stopwatch.StartNew();
            f = session.Execute<Func<double, double>>(@"
                Func<double, double> factory() { 
                    return x =>{
                                Func<double, double> zzz = n => n * 3;
                                return sin(x * PI)*zzz(2); 
                            }; 
                 } 
                 factory();");



            sw.Stop();
            Console.WriteLine("time 2: " + sw.ElapsedMilliseconds);

            Console.WriteLine(f);
            Console.WriteLine(f(0));
            Console.WriteLine(f(0.5));
            Console.WriteLine(f(1));
            
            


        }
    }
}