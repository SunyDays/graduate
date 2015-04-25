using NUnit.Framework;
using System;
using Types;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Tests
{
    [TestFixture]
    public class VectorExtensionsTest
    {
        [Test]
        public void Add1()
        {
            var vector = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var value = 10;

            Assert.AreNotEqual(vector, vector.Add(value));
        }

        [Test]
        public void Add2()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var vector2 = new Vector<double>(new[] {0.6, 7.0, 8.0, 9.0, 10.0, 11.0});

            Assert.AreNotEqual(vector1, vector1.AddElementWise(vector2));
        }

        [Test]
        public void Substract1()
        {
            var vector = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var value = 10;

            Assert.AreNotEqual(vector, vector.Substract(value));
        }

        [Test]
        public void Substract2()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var vector2 = new Vector<double>(new[] {0.6, 7.0, 8.0, 9.0, 10.0, 11.0});

            Assert.AreNotEqual(vector1, vector1.SubstractElementWise(vector2));
        }

        [Test]
        public void Multiply1()
        {
            var vector = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var value = 10;

            Assert.AreNotEqual(vector, vector.Multiply(value));
        }

        [Test]
        public void Multiply2()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var vector2 = new Vector<double>(new[] {0.6, 7.0, 8.0, 9.0, 10.0, 11.0});

            Assert.AreNotEqual(vector1, vector1.MultiplyElementWise(vector2));
        }

        [Test]
        public void Divide1()
        {
            var vector = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var value = 10;

            Assert.AreNotEqual(vector, vector.Divide(value));
        }

        [Test]
        public void Divide2()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var vector2 = new Vector<double>(new[] {0.6, 7.0, 8.0, 9.0, 10.0, 11.0});

            Assert.AreNotEqual(vector1, vector1.DivideElementWise(vector2));
        }

        [Test]
        public void Negate()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});

            Assert.AreNotEqual(vector1, vector1.Negate());
        }

        [Test]
        public void Pow()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var power = 2;

            Assert.AreNotEqual(vector1, vector1.Pow(power));
        }
    }
}