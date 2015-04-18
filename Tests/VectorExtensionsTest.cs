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

            var result = vector.Add(value);

            Assert.AreNotEqual(vector, result);
        }

        [Test]
        public void Add2()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var vector2 = new Vector<double>(new[] {0.6, 7.0, 8.0, 9.0, 10.0, 11.0});

            var result = vector1.Add(vector2);

            Assert.AreNotEqual(vector1, result);
        }

        [Test]
        public void Substract1()
        {
            var vector = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var value = 10;

            var result = vector.Substract(value);

            Assert.AreNotEqual(vector, result);
        }

        [Test]
        public void Substract2()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var vector2 = new Vector<double>(new[] {0.6, 7.0, 8.0, 9.0, 10.0, 11.0});

            var result = vector1.Substract(vector2);

            Assert.AreNotEqual(vector1, result);
        }

        [Test]
        public void Multiply1()
        {
            var vector = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var value = 10;

            var result = vector.Multiply(value);

            Assert.AreNotEqual(vector, result);
        }

        [Test]
        public void Multiply2()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var vector2 = new Vector<double>(new[] {0.6, 7.0, 8.0, 9.0, 10.0, 11.0});

            var result = vector1.Multiply(vector2);

            Assert.AreNotEqual(vector1, result);
        }

        [Test]
        public void Divide1()
        {
            var vector = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var value = 10;

            var result = vector.Divide(value);

            Assert.AreNotEqual(vector, result);
        }

        [Test]
        public void Divide2()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var vector2 = new Vector<double>(new[] {0.6, 7.0, 8.0, 9.0, 10.0, 11.0});

            var result = vector1.Divide(vector2);

            Assert.AreNotEqual(vector1, result);
        }

        [Test]
        public void Negate()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});

            var result = vector1.Negate();

            Assert.AreNotEqual(vector1, result);
        }

        [Test]
        public void Pow()
        {
            var vector1 = new Vector<double>(new[] {0.0, 1.0, 2.0, 3.0, 4.0, 5.0});
            var power = 2;

            var result = vector1.Pow(power);

            Assert.AreNotEqual(vector1, result);
        }
    }
}