using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NameBuilderTest
{
    AnimalNameBuilder.NameBuilder nameBuilder;
    NameAnimal namer;

    private List<NameAnimal.NameSuggestion> allWords;

    [SetUp]
    public void Setup()
    {
        GameObject go = new GameObject();
        namer = go.AddComponent<NameAnimal>();        
        nameBuilder = go.AddComponent<AnimalNameBuilder.NameBuilder>();   
    }

    protected void PreloadOptions()
    {
        //{ "Adjective", "Noun", "Verb", "Name" }
        allWords = new List<NameAnimal.NameSuggestion>();
        for (int wordType = 0; wordType < 4; wordType++)
        {
            for (int idx = 0; idx < 3; idx++)
            {
                NameAnimal.NameSuggestion suggestion = GetWordOfType(wordType, idx);
                if (suggestion == null)
                {
                    // No suggestion available with this index
                    continue;
                }

                allWords.Add(suggestion);
            }
        }
    }
    
    protected NameAnimal.NameSuggestion FindSuggestionMatchingWord(string word)
    {
        foreach (NameAnimal.NameSuggestion suggestion in allWords)
        {
            if (suggestion.word.Equals(word))
            {
                return suggestion;
            }
        }
        return null;
    }

    protected List<NameAnimal.NameSuggestion> SelectWords(string[] words)
    {
        List<NameAnimal.NameSuggestion> selectedElements = new List<NameAnimal.NameSuggestion>();
        for (int i = 0; i < words.Length; i++)
        {
            NameAnimal.NameSuggestion suggestion = FindSuggestionMatchingWord(words[i]);
            if (suggestion == null)
            {
                throw new Exception($"No suggestion found for word {words[i]} - probably an issue with your test case");
            }
            selectedElements.Add(suggestion);
        }
        return selectedElements;
    }

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
        if (inputWords.Length > 3)
        {
            throw new Exception("Input should be no more than 3 words - invalid test case");
        }

        namer.animalName = animalName;
        PreloadOptions();     

        List<string> words = new List<string>();
        List<int> wordIndices = new List<int>();

        // These are the words selected by the user, in their 'base' form
        // (as they would appear when floating around in the UI in-game)
        List<NameAnimal.NameSuggestion> selectedElements = SelectWords(inputWords);
        
        nameBuilder.GenerateName(namer, selectedElements, ref words, ref wordIndices);

        // Assert that the resulting name has the expected number of words in it
        Assert.AreEqual(words.Count, selectedElements.Count);
        Assert.AreEqual(wordIndices.Count, selectedElements.Count);

        // Assert that the resulting name is what we would expect
        Assert.AreEqual(expectedName, AnimalNameBuilder.NameBuilder.BuildString(ref words));
    }


    NameAnimal.NameSuggestion GetAdjective(int idx)
    {
        //{ "Adjective", "Noun", "Verb", "Name" }
        return GetWordOfType(0, idx);
    }

    NameAnimal.NameSuggestion GetWordOfType(int wordType, int idx)
    {
        string[] typePrefixes = { "Adjective", "Noun", "Verb", "Name" };
        string suggestion = namer.GenerateSuggestion(typePrefixes[wordType], idx);

        if ((suggestion.Length == 0) || (suggestion.StartsWith("No translation found for")))
        {
            return null;
        }

        NameAnimal.NameSuggestion result = new NameAnimal.NameSuggestion();
        result.wordType = wordType;
        result.suggestionIndex = idx;
        result.word = suggestion;
        return result;
    }
}
