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
        namer.animalName = "Fish";
        nameBuilder = go.AddComponent<AnimalNameBuilder.NameBuilder>();   

        PreloadOptions();     
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
    [TestCase("Gold", "Gold")]
    [TestCase("Spotted", "Spotted")]
    [TestCase("White", "White")]
    [TestCase("Fin", "Fin")]
    [TestCase("Back", "Back")]
    [TestCase("Scales", "Scales")]
    [TestCase("Water dweller", "Water dweller")]
    [TestCase("Teemer", "Teemer")]
    [TestCase("Dreamer", "Dreamer")]
    [TestCase("Fish", "Fish")]
    [TestCase("Carp", "Carp")]
    [TestCase("Coley", "Coley")]
    public void NameAnimalTestSimplePasses(string inputWord, string expectedName)
    {
        List<string> words = new List<string>();
        List<int> wordIndices = new List<int>();

        // These are the words selected by the user, in their 'base' form
        // (as they would appear when floating around in the UI in-game)
        List<NameAnimal.NameSuggestion> selectedElements = SelectWords(new string[] { inputWord });
        
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
