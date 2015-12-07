using System;
using GrapherApp.UI.Services;
using NUnit.Framework;

namespace GrapherApp.Tests
{
    [TestFixture]
    public class RangeSetTests
    {
        [Test]
        public void CanAddAndFind()
        {
            var rangeSet = new RangeSet<int>(0);
            for (var i = 10; i <= 1000; i+=10)
            {
                rangeSet.Add(i, i);
            }

            for (var i = 0; i < 1000; i+=3)
            {
                int n;
                var r  = rangeSet.TryFind(i, out n);

                Assert.That(r, Is.True);
                Assert.That(n, Is.EqualTo(i + 10 - i % 10.0).Or.EqualTo(i - i % 10.0));
            }
        }
    }
}