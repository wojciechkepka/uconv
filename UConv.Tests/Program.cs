using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UConv.Tests
{
    [TestClass]
    public class ExprParserTests
    {
        [TestMethod]
        public void LexesIdent()
        {
            var inp = "x";
            var l = new ExprLexer(inp);

            var want = new IdentToken(inp);
            var got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(want.Identifier, ((IdentToken) got).Identifier);
        }

        [TestMethod]
        public void LexesNum()
        {
            var inp = "12";
            var l = new ExprLexer(inp);

            var want = new NumToken(12.0);
            var got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(want.Number, ((NumToken) got).Number);
        }

        [TestMethod]
        public void LexesLongIdent()
        {
            var inp = "some_name";
            var l = new ExprLexer(inp);

            var want = new IdentToken(inp);
            var got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(want.Identifier, ((IdentToken) got).Identifier);
        }

        [TestMethod]
        public void LexesIdentAndNum()
        {
            var inp = "some_var = 42.0";
            var l = new ExprLexer(inp);

            var got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual("some_var", ((IdentToken) got).Identifier);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.IsTrue(got.Type == TokenType.Eq);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(42.0, ((NumToken) got).Number);
        }

        [TestMethod]
        public void LexesNumOperations()
        {
            var inp = "x = 42.0 + 15.9;";
            var l = new ExprLexer(inp);

            var got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual("x", ((IdentToken) got).Identifier);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.IsTrue(got.Type == TokenType.Eq);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(42.0, ((NumToken) got).Number);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.IsTrue(got.Type == TokenType.Plus);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.AreEqual(15.9, ((NumToken) got).Number);

            got = l.NextToken();
            Assert.IsNotNull(got);
            Assert.IsTrue(got.Type == TokenType.Semi);
        }
    }
}