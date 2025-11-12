using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnglishNameBuilderTest : BaseNameBuilderTest
{
    // Test individual words, selected on their own
    [Test]
    [TestCase("Fish", new string[] { "Gold" }, "Gold")]
    [TestCase("Fish", new string[] { "Spotted" }, "Spotted")]
    [TestCase("Fish", new string[] { "White" }, "White")]
    [TestCase("Fish", new string[] { "Fin" }, "Fin")]
    [TestCase("Fish", new string[] { "Back" }, "Back")]
    [TestCase("Fish", new string[] { "Scales" }, "Scales")]
    [TestCase("Fish", new string[] { "Water dweller" }, "Water dweller")]
    [TestCase("Fish", new string[] { "Teemer" }, "Teemer")]
    [TestCase("Fish", new string[] { "Dreamer" }, "Dreamer")]
    [TestCase("Fish", new string[] { "Fish" }, "Fish")]
    [TestCase("Fish", new string[] { "Carp" }, "Carp")]
    [TestCase("Fish", new string[] { "Coley" }, "Coley")]
    [TestCase("Fish", new string[] { "Gold", "Carp" }, "Gold carp")]
    [TestCase("Fish", new string[] { "Carp", "Gold" }, "Gold carp")] // Name should always go at the end
    [TestCase("Fish", new string[] { "Spotted", "Gold" }, "Spotted gold")] // Two adjectives
    [TestCase("Fish", new string[] { "Spotted", "Fish" }, "Spotted fish")]
    [TestCase("Fish", new string[] { "White", "Coley" }, "White coley")]
    [TestCase("Fish", new string[] { "Gold", "Fin", "Carp" }, "Gold finned carp")] // Noun changes to adjective
    [TestCase("Fish", new string[] { "Fin", "Gold", "Carp" }, "Gold finned carp")] // Words are always in a fixed order
    [TestCase("Fish", new string[] { "Spotted", "Water dweller" }, "Spotted water dweller")]
    [TestCase("Fish", new string[] { "Spotted", "Water dweller", "Carp" }, "Spotted water dwelling carp")] // Verb is changed to an adjective
    [TestCase("Yak", new string[] { "Horns", "Wool", "Cattle" }, "Horned woolly cattle")]
    public void NameAnimalTestSimplePasses(string animalName, string[] inputWords, string expectedName)
    {
        TestNameCombo(animalName, inputWords, expectedName);
    }
}
