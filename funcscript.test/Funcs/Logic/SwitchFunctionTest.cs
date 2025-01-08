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
            var exp = "switch(2, 1, 'one', 2, 'two', 'default')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("two"));
        }

        [Test]
        public void TestSwitchNoMatchDefault()
        {
            var exp = "switch(3, 1, 'one', 2, 'two', 'default')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("default"));
        }

        [Test]
        public void TestSwitchNoMatchWithoutDefault()
        {
            var exp = "switch(3, 1, 'one', 2, 'two')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestSwitchNullSelector()
        {
            var exp = "switch(null, null, 'matched', 1, 'not matched')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("matched"));
        }

        [Test]
        public void TestSwitchErrorEvenParameters()
        {
            var exp = "switch(1, 'one', 'two', 'three')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSwitchErrorOddParameters()
        {
            var exp = "switch(1, 'one', 'two', 'three', 'four')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
