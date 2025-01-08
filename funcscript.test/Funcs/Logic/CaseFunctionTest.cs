using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestCaseFunction
    {
        [Test]
        public void TestCaseWithTrueCondition()
        {
            var exp = "Case(true, 'Matched', false, 'Not Matched')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Matched"));
        }

        [Test]
        public void TestCaseWithFalseCondition()
        {
            var exp = "Case(false, 'Matched', false, 'Not Matched')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Not Matched"));
        }

        [Test]
        public void TestCaseWithNoConditions()
        {
            var exp = "Case()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestCaseWithInvalidConditionType()
        {
            var exp = "Case(1, 'Matched')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestCaseWithDefaultCase()
        {
            var exp = "Case(false, 'Matched', false, 'Not Matched', 'Default')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Default"));
        }

        [Test]
        public void TestCaseWithMultipleConditions()
        {
            var exp = "Case(false, 'First', true, 'Second', false, 'Third')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Second"));
        }

        [Test]
        public void TestCaseNoTrueConditionsNoDefault()
        {
            var exp = "Case(false, 'First', false, 'Second')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
