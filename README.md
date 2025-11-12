# The Serpent & The Seed
## Naming the Animals

### Intro
[The Serpent & The Seed](https://discipleship.tech/serpentseedgame) is a mobile narrative adventure game about the Bible, by UK charity [Discipleship Tech](https://discipleship.tech).

One of the early sequences in Act 1 revolves around helping Adam naming the animals. A sequence of words floats around, which are of various types, for example:

  * 'names' like "Fish" or "Carp"
  * 'adjectives' like "Gold" or "White"
  * 'nouns' like "Fins" or "Scales"
  * 'verb' based names like "Water-dweller" or "Dreamer")
  
The player can choose up to three of these words at a time, and the game cleverly combines them in a sensible way to make a coherent sounding name (e.g. "Gold water dweller", "Finned fish", "White gold dreamer", etc).

This repository just contains the code related to this aspect of the game, and a series of unit tests in different languages, to help us get the game translated in to as many languages as we can. Each language is going to need its own logic implemented for how to turn that series of words into a coherent name (plus translating the words themselves).

A video about this project can be found here: [https://youtu.be/v4rjmOWTZno?si=wjhi9dBEtqELC71Z&t=132](https://youtu.be/v4rjmOWTZno?si=wjhi9dBEtqELC71Z&t=132)

### How it works

In English, we have a "NameBuilder" class (found under Assets/Scripts/AnimalNameBuilder/NameBuilder.cs).
There is also a string table (look in Unity menu under Window > Asset Management > Localization Tables, and find the "AnimalNames" table). For each animal, there are up to three options for the four main types of words, for examples:

  * "Fish_Adjective_X"
  * "Fish_Noun_X"
  * "Fish_Verb_X"
  * "Fish_Name_X"
  
Then there are a couple of alternatives used when combining names:

  * "Imperfect_Verb" (which is really a present participle I think, sorry my grammar knowledge is a bit basic) - e.g. "Dreamer" becomes "Dreaming", "Water dweller" becomes "Water-dwelling.
  * "Noun_as_adj" (a noun but in adjective form) - e.g. "Scales" becomes "Scaled", "Fin" becomes "Finned", "Back" becomes "Backed" - so you can have a "Gold backed fish" or a "White scaled carp".

When combining them together to form a name, we always try to make sure there is one key word which as picked as the "name" bit - if a name is picked explicitly by the player (e.g. "Carp", "Fish" etc) then that's easy. Otherwise we try to use the first noun that they pick (e.g. "Scales") or if none of those are available, we fall back to using an adjective they have picked (e.g. "Gold") or a verb ("Water dweller"). Then we render all remaining words as an adjective and pile them all together.

### Translation principles 

When translating to other languages, in general we would ideally want to stick as closely as possible to the English - i.e. do a straight translation (keep the same basic terms, "White", "Gold", etc) but if this makes it super hard then it is acceptable to pick suitable alternatives that are easier. Sometimes it might also make sense to change them: e.g. in French a "goldfish" is actually a "red fish" (Poisson rouge) so it may make more sense to change "Gold" to "Red".

For now we are going to see if we can keep the way that the player selects words consistent between languages, and put all of the variation into the "how do you convert this into a name" side of the equation. But in theory we could make certain languages behave differently when picking words in the first place - e.g. if you have already picked one verb then in theory it could just not show you any more verbs until you remove the one you've picked already. Or it could start by ONLY showing you nouns, so that there is always at least one noun picked. But let's try and avoid that if we can.

### Using the Unit Tests

There is no actual "scene" in this project, it is handled entirely by the test runner (chose the menu option "Window > General > Test Runner"). If you press "Run all" then it should run them all. Most should pass (especially in English) but in some languages (e.g. French) there are some cases that haven't been catered for in the code yet so those tests are still failing.

If you look at the test code (e.g. Assets/Tests/EnglishNameBuilderTest.cs) you will see the various test cases. They each specify three argument: the animal name, an array of words chosen, and the expected name that results. The array of words is in the "base" format - i.e. how they would appear in-game for the player to choose (in English, that just means the noun, adjective, verb or name form - so no "noun as adjective" or "present participle").

### Other languages

Look at how the French name builder works for an example of how this could be customised per language. In French we have to handle genders and singular/plural forms, so the string table contains a bit more metadata (which gender each noun is, what the various forms of each adjective would be, separated by ":"). There is also an extra variation in the string table: "Adj_as_noun" (adjective as noun). For example, the "golden" adjective ("Doré") becomes "gold" ("Or") when used as a noun.

### Adding your own language

There would typically be five steps involved in adding your own language:

  1. Add a new locale to the project itself ("Edit > Project settings..." > "Localization")
  2. Create a custom "NameBuilder" subclass for your language (in "Assets/Scripts/AnimalNameBuilder")
  3. Edit the NameBuilder base class's `LocalisedNameBuilder()` method so that when using your language it returns your subclass
  4. Create a new unit test in your language (under "Assets/Tests") and add a suitable range of test cases.
  5. Populate the relevant column for your new locale in each row of the "AnimalNames" string table