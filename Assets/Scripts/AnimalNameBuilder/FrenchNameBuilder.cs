using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AnimalNameBuilder
{
    public class FrenchNameBuilder : NameBuilder
    {
        public enum EnumGender
        {
            kMasculineSingular = 0,
            kFeminineSingular,
            kMasculinePlural,
            kFemininePlural
        };

        protected EnumGender genderIndex;       
        
        public override void GenerateName(NameAnimal namer, List<NameAnimal.NameSuggestion> selectedElements, ref List<string> words, ref List<int> wordIndices)
        {
            // Sort our chosen words by type, because it tends to produce nicer results
            List<NameAnimal.NameSuggestion> sorted = new List<NameAnimal.NameSuggestion>(selectedElements);
            sorted.Sort((s, t) => CompareNameElementsFR(s, t));            

            // Pull out all 'name'-type words
            List<NameAnimal.NameSuggestion> sortedNames = ExtractNameTypeWords(namer, selectedElements, ref sorted);

            genderIndex = 0; // Masculine singular            

            // Use the last of these names to decide which gender to make it
            if (sortedNames.Count > 0)
            {
                NameAnimal.NameSuggestion lastName = sortedNames[sortedNames.Count - 1];
                genderIndex = DetermineGenderFromName(lastName);
            }

            // In French the name needs to go BEFORE the adjectives
            AppendNames(sortedNames, ref words, ref wordIndices);            

            // Now use all of the remaining words,        
            // sorted into word type order, because it tends to produce nicer results
            ExtractAdjectives(namer, sorted, sortedNames, ref words, ref wordIndices);            
        }

        protected override void UseOtherWordAsName(NameAnimal namer, ref List<NameAnimal.NameSuggestion> sortedNames, ref List<NameAnimal.NameSuggestion> sorted)
        {            
            NameAnimal.NameSuggestion nameBit = sorted[0];

            // See if it's an adjective
            if (nameBit.wordType == 0)
            {
                // Look up a special translation to help us turn it into name form
                nameBit.word = namer.GenerateSuggestion("Adj_as_noun", nameBit.suggestionIndex);
            }

            sortedNames.Add(nameBit);
            Debug.Log("Reverting to '" + nameBit.word + "' as the name bit");
        }

        protected override string GetJoiningWord()
        {
            return "et";
        }

        protected int CompareNameElementsFR(NameAnimal.NameSuggestion s, NameAnimal.NameSuggestion t)
        {
            //{ "Adjective", "Noun", "Verb", "Name" }
            // Sort nouns to go before adjectives
            int t1 = s.wordType;
            int t2 = t.wordType;
            if (t1 == 1)
            {
                t1 = 0;
            }
            else if (t1 == 0)
            {
                t1 = 1;
            }
            if (t2 == 1)
            {
                t2 = 0;
            }
            else if (t2 == 0)
            {
                t2 = 1;
            }

            int comparison = t1.CompareTo(t2);
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

        public override string LabelForSuggestion(NameAnimal.NameSuggestion suggestion)
        {
            // If word contains gender information, leave it out
            string[] bits = suggestion.word.Split(":", 2);
            Debug.Log("Split suggestion: " + bits);
            return bits[0];
        }

        protected EnumGender DetermineGenderFromName(NameAnimal.NameSuggestion name)
        {
            if (name.word.EndsWith(":FF"))
            {
                return EnumGender.kFemininePlural;
            }
            else if (name.word.EndsWith(":MM"))
            {
                return EnumGender.kMasculinePlural;
            }
            else if (name.word.EndsWith(":F"))
            {
                return EnumGender.kFeminineSingular;
            }
            else
            {
                return EnumGender.kMasculineSingular;
            }
        }

        protected override string NameFromSuggestion(NameAnimal.NameSuggestion suggestion)
        {
            // Start with the default form
            string name = suggestion.word;                        

            // A : might be used to indicate word gender
            string[] bits = name.Split(":", 2);
            if (bits.Length < 2) return name;
            return bits[0];
        }

        protected override string AdjectiveFromSuggestion(NameAnimal namer, NameAnimal.NameSuggestion suggestion)
        {
            if (suggestion.wordType == 1)
            {
                // If it's a noun, we will actually use THIS to determine the gender,
                // not the original name
                genderIndex = DetermineGenderFromName(suggestion);
            }

            string adj = base.AdjectiveFromSuggestion(namer, suggestion);
            string[] bits = adj.Split(":", 4);
            if (bits.Length < 2) return adj;            
            
            // Return the correct form
            return bits[(int)genderIndex];            
        }
    }
}