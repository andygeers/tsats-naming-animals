using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace AnimalNameBuilder
{
    public class NameBuilder : MonoBehaviour
    {
        public static NameBuilder LocalisedNameBuilder()
        {
            string locale = LocalizationSettings.SelectedLocale.Identifier.Code;
            string[] bits = locale.Split("-", 2);
            string language = bits[0];
            Debug.Log($"Locale is {locale} (language: {language})");
            if (language.Equals("fr"))
            {
                return new FrenchNameBuilder();
            }
            else
            {
                return new NameBuilder();
            }
        }        

        public virtual void GenerateName(NameAnimal namer, List<NameAnimal.NameSuggestion> selectedElements, ref List<string> words, ref List<int> wordIndices)
        {
            // Sort our chosen words by type, because it tends to produce nicer results
            List<NameAnimal.NameSuggestion> sorted = new List<NameAnimal.NameSuggestion>(selectedElements);
            sorted.Sort((s, t) => CompareNameElements(s, t));

            // Pull out all 'name'-type words
            List<NameAnimal.NameSuggestion> sortedNames = ExtractNameTypeWords(namer, selectedElements, ref sorted);

            // Now use all of the remaining words,        
            // sorted into word type order, because it tends to produce nicer results
            ExtractAdjectives(namer, sorted, sortedNames, ref words, ref wordIndices);

            AppendNames(sortedNames, ref words, ref wordIndices);
        }

        public static string BuildString(ref List<string> words)
        {
            return string.Join(" ", words);
        }

        protected List<NameAnimal.NameSuggestion> ExtractNameTypeWords(NameAnimal namer, List<NameAnimal.NameSuggestion> selectedElements, ref List<NameAnimal.NameSuggestion> sorted)
        {
            List<NameAnimal.NameSuggestion> sortedNames = new List<NameAnimal.NameSuggestion>(selectedElements.Count);
            foreach (NameAnimal.NameSuggestion suggestion in selectedElements)
            {
                if (suggestion.wordType == 3)
                {
                    sortedNames.Add(suggestion);
                }
            }
            // We will sort all names by key - so they *always* go in a fixed order
            sortedNames.Sort((s, t) => s.suggestionIndex.CompareTo(t.suggestionIndex));

            // If there are no actual names then use a word
            if (sortedNames.Count == 0)
            {
                UseOtherWordAsName(namer, ref sortedNames, ref sorted);
            }

            return sortedNames;
        }

        protected virtual void UseOtherWordAsName(NameAnimal namer, ref List<NameAnimal.NameSuggestion> sortedNames, ref List<NameAnimal.NameSuggestion> sorted)
        {
            NameAnimal.NameSuggestion nameBit = sorted[sorted.Count - 1];
            sortedNames.Add(nameBit);
            Debug.Log("Reverting to '" + nameBit.word + "' as the name bit");            
        }

        protected virtual string GetJoiningWord()
        {
            return null;
        }

        protected void ExtractAdjectives(NameAnimal namer, List<NameAnimal.NameSuggestion> sorted, List<NameAnimal.NameSuggestion> sortedNames, ref List<string> words, ref List<int> wordIndices)
        {
            bool hasInsertedAdjective = false;
            for (int j = 0; j < sorted.Count; j++)
            {
                NameAnimal.NameSuggestion suggestion = sorted[j];
                if (sortedNames.Contains(suggestion)) continue;

                if (hasInsertedAdjective)
                {
                    string joiningWord = GetJoiningWord();
                    if (joiningWord != null) words.Add(joiningWord);
                }

                string word = AdjectiveFromSuggestion(namer, suggestion);
                if (words.Count > 0) word = word.ToLower();
                words.Add(word);
                wordIndices.Add(suggestion.selectedWordIndex);

                if (suggestion.wordType == 0) hasInsertedAdjective = true;
            }
        }

        public virtual string LabelForSuggestion(NameAnimal.NameSuggestion suggestion)
        {
            return suggestion.word;
        }

        protected void AppendNames(List<NameAnimal.NameSuggestion> sortedNames, ref List<string> words, ref List<int> wordIndices)
        {
            // Append all 'name'-type words at the end        
            foreach (NameAnimal.NameSuggestion nameSuggestion in sortedNames)
            {
                // Just use the name as-is
                string name = NameFromSuggestion(nameSuggestion);
                if (words.Count > 0) name = name.ToLower();
                words.Add(name);
                wordIndices.Add(nameSuggestion.selectedWordIndex);
            }
        }

        protected int CompareNameElements(NameAnimal.NameSuggestion s, NameAnimal.NameSuggestion t)
        {
            int comparison = s.wordType.CompareTo(t.wordType);
            if (comparison == 0)
            {
                // If words are of equal type, put whichever one was selected first first
                // (This is because you might change the literal order by tapping, but the word order should remain the same)
                return s.selectedAt.CompareTo(t.selectedAt);
            }
            else
            {
                return comparison;
            }
        }

        protected virtual string NameFromSuggestion(NameAnimal.NameSuggestion suggestion)
        {
            // Default form of any kind of word can be used as a name
            return suggestion.word;
        }

        protected virtual string AdjectiveFromSuggestion(NameAnimal namer, NameAnimal.NameSuggestion suggestion)
        {
            //{ "Adjective", "Noun", "Verb", "Name" }
            switch (suggestion.wordType)
            {
                case 0:
                    {
                        // Adjective: already an adjective!
                        return suggestion.word;
                    }

                case 1:
                    {
                        // Noun: use the 'noun as adjective' form
                        return namer.GenerateSuggestion("Noun_as_adj", suggestion.suggestionIndex);
                    }

                case 2:
                    {
                        // Verb: use the 'imperfect verb' form
                        return namer.GenerateSuggestion("Imperfect_Verb", suggestion.suggestionIndex);
                    }

                case 3:
                    {
                        // Name: should never be here
                        Debug.LogWarning("Why is name being used as adjective? " + suggestion.word);
                        return suggestion.word;
                    }

                default:
                    return suggestion.word;
            }
        }
    }
}
