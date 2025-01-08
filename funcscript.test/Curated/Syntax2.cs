﻿using FuncScript.Error;
using FuncScript.Model;
using Microsoft.VisualBasic;
using NuGet.Frameworks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FuncScript.Test.Curated
{
    public class Syntax2
    {
        [Test]
        public void TestMapSquare()
        {
            var exp="map([1,2,4],(x)=>x*x)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list ,Is.EquivalentTo(new []{1,4,16}));
        }

        [Test]
        public void StringInterpolationBasic()
        {
            var p = new KvcProvider(new ObjectKvc(new { x = 100 }),
                new DefaultFsDataProvider());
            var res = FuncScript.Evaluate(p, @"f'y={x+2}'");
            Assert.AreEqual("y=102", res);
        }

        [Test]
        public void StringInterpolationEscape()
        {
            var p = new KvcProvider(new ObjectKvc(new { x = 100 }),
                new DefaultFsDataProvider());
            var res = FuncScript.Evaluate(p, @"f'y=\{x+2}'");
            Assert.AreEqual(@"y={x+2}", res);
        }

        [Test]
        public void StringDoubleEscapeBug()
        {
            var exp = @"'test\'\''";
            var p = new KvcProvider(new ObjectKvc(new { x = 100 }),
                new DefaultFsDataProvider());
            var res = FuncScript.Evaluate(p, exp);
            Assert.That(res, Is.EqualTo("test''"));
        }

        [Test]
        public void ParseUnicodeString()
        {
            var exp = @"'test\u0020'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("test "));
        }
        
        [Test]
        public void NullSafeGetMemberNullValue()
        {
            var p = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate(p, @"x?.y");
            Assert.AreEqual(null, res);
        }

        [Test]
        public void NullSafeGetMemberNoneNullValue()
        {
            var p = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate(p, @"{ x:{y:5}; return x?.y}");
            Assert.AreEqual(5, res);
        }

        [Test]
        public void NullSafeExpressionNullValue()
        {
            var p = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate(p, @"x?!(x*200)");
            Assert.AreEqual(null, res);
        }

        [Test]
        public void NullSafeExpressionNoneNullValue()
        {
            var p = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate(p, @"{ x:5; return x?!(x*200)}");
            Assert.AreEqual(1000, res);
        }

        [Test]
        public void SquareBraceIndexLiteral()
        {
            var p = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate(p, @"[4,5,6][1]");
            Assert.AreEqual(5, res);
        }

        [Test]
        public void EmptyParameterList()
        {
            var exp = @"{y:()=>5;return y()}";
            var p = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate(p, exp);
            Assert.AreEqual(5, res);
        }

        [Test]
        public void SquareBraceIndexVariable()
        {
            var p = new DefaultFsDataProvider();
            var res = FuncScript.Evaluate(p, @"{x:[4,5,6];return x[1]}");
            Assert.AreEqual(5, res);
        }

        [Test]
        public void TestFSTemplate1()
        {
            var template = "abc";
            var expected = "abc";
            var res = FuncScript.Evaluate(template, new DefaultFsDataProvider(), null, FuncScript.ParseMode.FsTemplate);
            Assert.That(res, Is.EqualTo(expected));
        }

        [Test]
        public void TestFSTemplate2()
        {
            var template = "abc${'1'}";
            var expected = "abc1";
            var res = FuncScript.Evaluate(template, new DefaultFsDataProvider(), null, FuncScript.ParseMode.FsTemplate);
            Assert.That(res, Is.EqualTo(expected));
        }

        [Test]
        public void TestFSTemplate3()
        {
            var template = "abc${['d',1,['e',2]]}f";
            var expected = "abcd1e2f";
            var res = FuncScript.Evaluate(template, new DefaultFsDataProvider(), null, FuncScript.ParseMode.FsTemplate);
            Assert.That(res, Is.EqualTo(expected));
        }

        [Test]
        public void TestFSTemplate4()
        {
            var template = "abc${['d',1] map (x)=>'>'+x}f";
            var expected = "abc>d>1f";
            var res = FuncScript.Evaluate(template, new DefaultFsDataProvider(), null, FuncScript.ParseMode.FsTemplate);
            Assert.That(res, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("case 30", 30)]
        [TestCase("case 1>2:1, 2>3:2, 10", 10)]
        [TestCase("case 1>2:1, 2>1:2, 10", 2)]
        [TestCase("case 1>2:1, 10", 10)]
        [TestCase("(case 1>2:[1], [10])[0]", 10)]
        public void CaseExpression(string exp, object expected)
        {
            Assert.AreEqual(expected, FuncScript.Evaluate(exp));
        }

        [Test]
        [TestCase("switch 30", null)]
        [TestCase("switch 4, 1:'a', 2:'b', 4:'c'", "c")]
        [TestCase("switch 4, 1:'a', 2:'b', 3:'c'", null)]
        [TestCase("switch 4, 1:'a', 2:'b', 3:'c','that'", "that")]
        public void SwitchExpression(string exp, object expected)
        {
            Assert.AreEqual(expected, FuncScript.Evaluate(exp));
        }

        [Test]
        [TestCase("If(true,30,2)", 30)]
        public void IfFunction(string exp, object expected)
        {
            Assert.AreEqual(expected, FuncScript.Evaluate(exp));
        }
    }
}
