using System;
using System.Diagnostics;
using System.Linq;
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
            foreach (var i in Enumerable.Range(1,10))
            {
                var midIndex = (int)(i / 2.0);
                Console.WriteLine(i+"="+midIndex);
            }
        }
    }
}
