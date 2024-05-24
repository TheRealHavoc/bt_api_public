namespace bt_api_tests
{
    public class Utilities_ConvertAbilityScoreToModifier_Tests
    {
        [Test]
        [TestCase(12, 1)]
        [TestCase(13, 1)]
        [TestCase(14, 2)]
        [TestCase(9, -1)]
        [TestCase(1, -5)]
        public void Test_ReturnsExpectedModifier(int abilityScore, int expectedModifier)
        {
            var actual = Utilities.ConvertAbilityScoreToModifier(abilityScore);

            Assert.That(actual, Is.EqualTo(expectedModifier));
        }

        [Test]
        [TestCase(12, 99)]
        [TestCase(1, 99)]
        [TestCase(99, 1)]
        public void Test_DoesNotReturnExpectedModifier(int abilityScore, int expectedModifier)
        {
            var actual = Utilities.ConvertAbilityScoreToModifier(abilityScore);

            Assert.That(expectedModifier, Is.Not.EqualTo(actual));
        }
    }
}
