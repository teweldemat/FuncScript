using funcscript.core;
using funcscript.model;
using funcscript.error;
using NUnit.Framework;
using System;

namespace funcscript.test
{

    public class TestSet2
    {
        
        [Test]
        public void CallingNoneFunction()
        {
            
            Assert.Throws(typeof(EvaluationException), () =>
            {
                var g = new DefaultFsDataProvider();
                FuncScript.Evaluate("3(4,5)",g);
            });
        }
        [Test]
        public void DuplicateKeyInCollection()
        {

            Assert.Throws(typeof(SyntaxError), () =>
            {
                FuncScript.Evaluate("{a:5;a:6;return 5;}");
            });
        }


    }
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        public static object AssertSingleResult(string expStr)
        {
            var p = new DefaultFsDataProvider();
            var ret=FuncScript.Evaluate(expStr,p);
            Assert.AreEqual(ret, FuncScript.NormalizeDataType(ret));
            return ret;
        }
        public static object AssertSingleResultType(string expStr, System.Type type)
        {
            var p = new DefaultFsDataProvider();
            var res=FuncScript.Evaluate(expStr,p);
            Assert.AreEqual(res, FuncScript.NormalizeDataType(res));

            if (type == null)
                Assert.IsTrue(res == null, $"Expected null resul");
            else
            {
                Assert.IsNotNull(res, "Null result");
                Assert.IsTrue(res != null && res.GetType().IsAssignableTo(type), $"Invalid data result type. Expected:{type} found {(res == null ? "null" : res.GetType())}");
            }
            return res;
        }
        void ShouldReturnEmpty(string expStr)
        {
            var p = new DefaultFsDataProvider();
            var res=FuncScript.Evaluate(expStr,p);
            Assert.IsTrue(res == null , $"Must returl empty result, got {res}");
        }

        [Test]
        public void TestConstant()
        {
            int val = 1;
            var res=AssertSingleResultType($"{val}", typeof(int));
            Assert.IsTrue(res!=null && (int)res== val,"Incorrect result");
        }
        [Test]
        [TestCase("12",12)]
        [TestCase("12.", 12.0)]
        [TestCase("12.0", 12.0)]
        [TestCase("12e1", 120)]
        [TestCase("12.0e1", 120.0)]
        
        [TestCase("-12", -12)]
        [TestCase("-12.", -12.0)]
        [TestCase("-12.0", -12.0)]
        [TestCase("-12e1", -120)]
        [TestCase("-12.0e1", -120.0)]

        [TestCase("-12l", -12L)]
        [TestCase("-12e1l", -120L)]
        [TestCase("-1", -1)]

        [TestCase("12+12l", 24L)]
        [TestCase("\"12\"", "12")]
        [TestCase("3+\"12\"", "312")]
        [TestCase("\"12\"+3", "123")]
        [TestCase("1.0/2", 0.5)]
        [TestCase("5/2", 2)]
        [TestCase("1.0+5", 6.0)]
        [TestCase("5+1/2", 5)]
        [TestCase("1+2+3",6)]
        [TestCase("(1+2+3)+5", 11)]
        [TestCase("1+2*3", 7)]
        [TestCase("1+2*3+4", 11)]
        [TestCase("\"a\"+\"b\"+3+\"c\"", "ab3c")]
        [TestCase("If(1=0,1,\n3)", 3)] //ignores line feed
        [TestCase("{r:2; return R;}",2)] //ignore cases
        [TestCase("{r:(a)=>A*A; return R(2);}", 4)] //ignore cases
        [TestCase("{R:(a)=>A*A; return r(2);}", 4)] //ignore cases
        public void TestNumberAndStringParsing(string exp,object val)
        {
            var res = AssertSingleResultType(exp, val.GetType());
            Assert.AreEqual(val,res);
        }
        
        [Test]
        [TestCase("\"12\"+\"34\"", "1234")]
        [TestCase("3+\"12\"", "312")]
        [TestCase("\"12\"+3", "123")]
        [TestCase("\"a\\\"b\"", "a\"b")]

        [TestCase("{x:5; return \"a{x}b\"; }", "a5b")]
        [TestCase("{x:5; return \"a\\{x}b\"; }", "a{x}b")]

        [TestCase("{return 3}",3)] //single white space
        [TestCase("{return\t3}", 3)] //tab white space
        [TestCase("{return\n3}", 3)] //line break
        [TestCase("{return\r3}", 3)] //line fee
        [TestCase("{a:(x)=>x,return a( 5)}", 5)] //param list white space
        public void TestStringConcatenation(string exp, object val)
        {
            var res = AssertSingleResultType(exp, val.GetType());
            Assert.AreEqual(val,res );
        }

        [Test]
        [TestCase("null", null)]
        [TestCase("true", true)]
        [TestCase("TRue", true)]
        [TestCase("false", false)]
        [TestCase("FAlse", false)]
        public void KeyWordLiteralLiterals(string exp, object val)
        {
            var res = AssertSingleResultType(exp, val==null?null:val.GetType());
            if(val==null)
                Assert.IsNull(res , "Incorrect result");
            else
                Assert.IsTrue(res != null && res.Equals(val), "Incorrect result");
        }

        [Test]
        public void TestOverflow()
        {
            Assert.Throws<error.SyntaxError>( ()=>
            {
                FuncScript.Evaluate($"{long.MaxValue}0");
            });
        }
        [Test]
        [TestCase("12e-")]
        public void TestInvalid(string exp)
        {
            Assert.Throws<error.SyntaxError>(() =>
            {
                FuncScript.Evaluate(exp);
            });
        }
        [Test]
        public void TestJsonParser()
        {
            var res = AssertSingleResultType("{\"a\":23}", typeof(KeyValueCollection));
        }

        [Test]
        public void TestMapList()
        {

            var res = AssertSingleResultType("Map([1,2,4],(x)=>x*x)", typeof(FsList));
            Assert.IsTrue(((FsList)res).Data[2].Equals(16), "Result  not correct");
        }

        [Test]
        public void SumListWthNoInitialInt()
        {

            var res = AssertSingleResultType("Reduce(Map([1,2,4],(x)=>x*x),(c,p)=>p+c)", typeof(int));
            Assert.AreEqual(1*1+2*2+4*4 , res);
        }
        [Test]
        public void SumListWthInitial()
        {

            var res = AssertSingleResultType("Reduce(Map([1,2,4],(x)=>x*x),(c,t)=>t+c,0)", typeof(int));
            Assert.AreEqual(1 * 1 + 2 * 2 + 4 * 4, res);
        }
        [Test]
        public void TestListParser()
        {

            var res = AssertSingleResultType("[1,2,4]", typeof(FsList));
            Assert.IsTrue(res is FsList, ".net data type not ListData");
        }
        
        [Test]
        [TestCase("12.3")]
        [TestCase("-12.3")]
        [TestCase("-12.")]
        [TestCase("-12.3e-12")]
        [TestCase("-12.3E-12")]
        [TestCase("12.3E-12")]
        [TestCase("12.E-12")]
        public void TesDoubleParser(string exp)
        {

            var res = AssertSingleResultType(exp, typeof(double));
            Assert.IsTrue(res is double, ".net data type not double");
            Assert.AreEqual((double)res,double.Parse(exp));
        }
        
        [Test]
        [TestCase("12.E2+2", "1202")]
        [TestCase("12.0E2+2", "1202")]
        [TestCase("12.0E-2+2", "2.12")]
        public void TesDoubleParser(string exp,double resVal)
        {

            var res = AssertSingleResultType(exp, typeof(double));
            Assert.IsTrue(res is double, ".net data type not double");
            Assert.AreEqual((double)res, resVal);
        }

        [Test]
        public void TestJsonMemberAccess()
        {

            var res = AssertSingleResultType("{x:{\"a\":23}; return x.a;}", typeof(int));
            Assert.IsTrue(res!=null && (int)res== 23, "Incorrect result");
        }

        [Test]
        public void TestLambda()
        {
            int val_a = 3;
            var res = AssertSingleResultType($"((a)=>a*a+a)({val_a})", typeof(int));
            Assert.IsTrue((int)res== val_a*val_a+val_a,"Incorrect result");
        }

        [Test]
        public void TestExpCollection()
        {
            int val_a = 3;
            var res = AssertSingleResultType($"{{x:{val_a};return x*x;}}", typeof(int));
            Assert.IsTrue(res!=null && (int)res== val_a * val_a , "Incorrect result");
        }
        [Test]
        public void TestNullFuncCall()
        {
            var res = AssertSingleResultType("(()=>())()", null);
        }
        [Test]
        [TestCase("(()=>())()+1", 1)] //null value+1
        [TestCase("(()=>5)()", 5)] //empty paramter function call
        [TestCase("-1", -1)] //parse negative number
        [TestCase("2--1", 3)] //parse negative number
        [TestCase("2-1", 1)] //simple subtraction
        [TestCase("1/2", 0)]
        [TestCase("If(1=0,10,5-1)", 4)]
        [TestCase("((a)=>a*a)(3)",9)]
        
    [TestCase(
@"{
    x:3;
    return x*x+{return 2;};
}",3*3+2)]
        [TestCase(
@"{
    x:(a)=>a*a;
    return x(3);
}", 3 * 3 )]
        [TestCase(
@"{j:{
		""age"":30,

        ""name"":20,
        ""parts"":{""x"":23, ""y"":10}
};
return j.parts.x;
}", 23)]
        [TestCase(
@"{j:(a)=>a*a;
return j;
}(4)", 16)]
[TestCase(
@"{b:{""x"":(a)=>a*a};
	return (b.x)(2);
}", 4)]
        [TestCase(
@"{return (x)=>3;}(3)", 3)]
        public void IntResultTest(string expStr,int expectedRes)
        {
            var res = AssertSingleResultType(expStr, typeof(int));
            Assert.AreEqual(expectedRes,res);
        }

        [Test]
        public void TestSpread()
        {
            var res = AssertSingleResultType("Select({'a':1,'b':2},{'b':5,'c':8})", typeof(KeyValueCollection));
            var expected= FuncScript.Evaluate("{'a':1,'b':5,'c':8}",null);
            Assert.AreEqual(expected,res);
        }

        [Test]
        public void TestDotnetObject()
        {
            var res = new ObjectKvc(new { a = 1,b=5,c=8});
            var expected = FuncScript.Evaluate("{'a':1,'b':5,'c':8}",null);
            Assert.AreEqual(expected, res);
        }
        [Test]
        public void TestSpread2()
        {
            var g = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate("Select({'a':1,'b':5,'c':8},{'a':null,'b':null})",g);
            var expected = new ObjectKvc(new { a = 1, b = 5});
            Assert.AreEqual(expected, res);
        }
        [Test]
        public void TestIdenKey()
        {
            var g = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate("Select({a:3,b:4},{a,c:5})",g);
            var expected = new ObjectKvc(new { a = 3, c = 5 });
            Assert.AreEqual(expected, res);
        }
        [Test]
        public void MapNull()
        {
            
            Assert.IsNull(FuncScript.Evaluate("null map (x)=>x"));
            Assert.IsNull(FuncScript.Evaluate("y map (x)=>x"));
        }
        
    }
}