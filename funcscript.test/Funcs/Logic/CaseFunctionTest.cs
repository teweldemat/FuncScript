using FuncScript.Core;
using FuncScript.Error;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestCaseFunction
    {
        [Test]
        public void TestCaseWithTrueCondition()
        {
            // Call the Case function with correct syntax
            var exp = "case true: 'Matched'; false: 'Not Matched';";
            var res = Helpers.Evaluate(exp); // Evaluate the expression
            Assert.That(res, Is.EqualTo("Matched")); // Expect "Matched"
        }

        [Test]
        public void TestCaseWithFalseCondition()
        {
            var exp = "case false: 'Matched'; false: 'Not Matched';";
            var res = Helpers.Evaluate(exp);
            Assert.IsNull(res);
        }

        [Test]
        public void TestCaseWithNoConditions()
        {
            var exp = "case ;";
            Assert.Throws<SyntaxError>(() =>
            {
                var res = Helpers.Evaluate(exp);
            });
        }

        [Test]
        public void TestCaseWithInvalidConditionType()
        {
            var exp = "case 1: 'Matched';"; // Invalid condition type (not a boolean)
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError); // Expecting an error
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER)); // Error type check
        }

        [Test]
        public void TestCaseWithDefaultCase()
        {
            var exp = "case false: 'Matched'; false: 'Not Matched'; 'Default';";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Default")); // Expect "Default" when no conditions match
        }

        [Test]
        public void TestCaseWithMultipleConditions()
        {
            // Multiple conditions to evaluate
            var exp = "case false: 'First'; true: 'Second'; false: 'Third';";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Second")); // Expect "Second" since the second condition is true
        }

        [Test]
        public void TestCaseNoTrueConditionsNoDefault()
        {
            var exp = "case false: 'First'; false: 'Second';";
            var res = Helpers.Evaluate(exp);
            Assert.IsNull(res);
        }
    }
}
