namespace bt_api_tests
{
    public class Utilities_GenerateRandomString_Tests
    {
        [Test]
        [TestCase(12)]
        [TestCase(1)]
        [TestCase(99)]
        public void Test_ReturnsExpectedLength(int length)
        {
            var actual = Utilities.GenerateRandomString(length).Length;

            Assert.That(actual, Is.EqualTo(length));
        }
    }
}
