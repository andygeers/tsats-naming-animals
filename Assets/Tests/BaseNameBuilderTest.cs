using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public abstract class BaseNameBuilderTest
{
    protected AnimalNameBuilder.NameBuilder nameBuilder;
    protected NameAnimal namer;

    protected List<NameAnimal.NameSuggestion> allWords;

    private Locale _originalLocale;
    protected Locale _testLocale;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Ensure the system is ready
        LocalizationSettings.InitializationOperation.WaitForCompletion();

        // Find the French locale (make sure FR is in Project Settings > Localization)
        _testLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(GetLocaleName()));
        Assert.IsNotNull(_testLocale, "Test locale not found. Add it in Project Settings > Localization.");
    }

    protected virtual string GetLocaleName()
    {
        return "en-GB";
    }

    [SetUp]
    public void Setup()
    {
        _originalLocale = LocalizationSettings.SelectedLocale;
        LocalizationSettings.SelectedLocale = _testLocale;
        LocalizationSettings.Instance.ForceRefresh();

        GameObject go = new GameObject();
        namer = go.AddComponent<NameAnimal>();
        nameBuilder = AnimalNameBuilder.NameBuilder.LocalisedNameBuilder();        
    }
    
    [TearDown]
    public void TearDown()
    {
        LocalizationSettings.SelectedLocale = _originalLocale;
        LocalizationSettings.Instance.ForceRefresh();
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
            if (nameBuilder.LabelForSuggestion(suggestion).Equals(word))
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
                string firstSuggestion = nameBuilder.LabelForSuggestion(allWords[0]);
                throw new Exception($"No suggestion found for word {words[i]} - probably an issue with your test case. First suggestion is {firstSuggestion}");
            }
            selectedElements.Add(suggestion);
        }
        return selectedElements;
    }

    protected void TestNameCombo(string animalName, string[] inputWords, string expectedName)
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

        Assert.AreEqual(words.Count, wordIndices.Count);

        int wordCount = 0;
        for (int i = 0; i < words.Count; i ++)
        {
            if (wordIndices[i] != -1)
            {
                wordCount++;
            }
        }

        // Assert that the resulting name has the expected number of words in it
        Assert.AreEqual(wordCount, selectedElements.Count);        

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
