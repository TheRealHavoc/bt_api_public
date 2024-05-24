namespace bt_api_tests
{
    public class Utilities_GetCharacterAttributeScore_Tests
    {
        private readonly CharacterModelBuilder characterModelBuilder = new CharacterModelBuilder();

        private CharacterModel characterModel;

        [SetUp]
        public void SetUp()
        {
            this.characterModel = this.characterModelBuilder.GetFirstCharacter();
        }

        [Test]
        [TestCase(AttributeEnum.STR, 12)]
        [TestCase(AttributeEnum.DEX, 16)]
        [TestCase(AttributeEnum.CON, 12)]
        public void Test_ReturnsExpectedScore(AttributeEnum attribute, int expectedScore)
        {
            var actual = Utilities.GetCharacterAttributeScore(this.characterModel, attribute);

            Assert.That(actual, Is.EqualTo(expectedScore));
        }
    }
}
