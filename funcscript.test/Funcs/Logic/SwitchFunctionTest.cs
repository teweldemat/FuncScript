using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestSwitchFunction
    {
        [Test]
        public void TestSwitchCaseMatch()
        {
            var exp = "switch 2; 1:'one'; 2:'two'; 'default';";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("two"));
        }

        [Test]
        public void TestSwitchNoMatchDefault()
        {
            var exp = "switch 3; 1:'one'; 2:'two'; 'default';";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("default"));
        }

        [Test]
        public void TestSwitchNoMatchWithoutDefault()
        {
            var exp = "switch 3; 1:'one'; 2:'two';";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestSwitchNullSelector()
        {
            var exp = "switch null; null:'matched'; 1:'not matched';";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("matched"));
        }

        [Test]
        public void TestSwitchErrorEvenParameters()
        {
            var exp = "switch 1; 'one':'two'; 'three';";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("three"));
        }

        [Test]
        public void TestSwitchErrorOddParameters()
        {
            var exp = "switch 1; 'one':'two'; 'three'; 'four';";
            Assert.Throws<SyntaxError>(() => { Helpers.Evaluate(exp); });
        }
    }
}
