using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UConv.Core;

namespace UConv.Tests
{

    [TestClass]
    public class ExprParserTests
    {
        [TestMethod]
        public void LexesIdent()
        {
            string inp = "x";
            ExprLexer l = new ExprLexer(inp);

            IdentToken want = new IdentToken(inp);
            Token got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(want.Identifier, ((IdentToken)got).Identifier);
        }
        [TestMethod]
        public void LexesNum()
        {
            string inp = "12";
            ExprLexer l = new ExprLexer(inp);

            NumToken want = new NumToken(12.0);
            Token got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(want.Number, ((NumToken)got).Number);
        }
        [TestMethod]
        public void LexesLongIdent()
        {
            string inp = "some_name";
            ExprLexer l = new ExprLexer(inp);

            IdentToken want = new IdentToken(inp);
            Token got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(want.Identifier, ((IdentToken)got).Identifier);
        }
        [TestMethod]
        public void LexesIdentAndNum()
        {
            string inp = "some_var = 42.0";
            ExprLexer l = new ExprLexer(inp);

            Token got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual("some_var", ((IdentToken)got).Identifier);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.IsTrue(got.Type == TokenType.Eq);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(42.0, ((NumToken)got).Number);
        }

        [TestMethod]
        public void LexesNumOperations()
        {
            string inp = "x = 42.0 + 15.9";
            ExprLexer l = new ExprLexer(inp);

            Token got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual("x", ((IdentToken)got).Identifier);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.IsTrue(got.Type == TokenType.Eq);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(42.0, ((NumToken)got).Number);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.IsTrue(got.Type == TokenType.Plus);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(15.9, ((NumToken)got).Number);
        }
    }
}
