using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using GrapherApp.UI;
using NUnit.Framework;

namespace GrapherApp.Tests
{
    [TestFixture]
    public class MiscTests
    {
        [Test]
        public void Test1()
        {
            var s = @"
                return bezier(x, 123456,345)/**/;
            ";

            Console.WriteLine(Regex.Replace(s,@"([ \t\n\r;,/*]|return)",""));
            
        }
    }
}
