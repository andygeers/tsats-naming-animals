using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class FrenchNameBuilderTest : BaseNameBuilderTest
{
    protected override string GetLocaleName()
    {
        return "fr";
    }

    // A Test behaves as an ordinary method
    [Test]
    [TestCase("Fish", new string[] { "Nageoire" }, "Nageoire")]
    [TestCase("Fish", new string[] { "Blanc", "Poisson" }, "Poisson blanc")] // Let's try genders
    [TestCase("Fish", new string[] { "Blanc", "Carpe" }, "Carpe blanche")]   // ...
    [TestCase("Fish", new string[] { "Nageoire", "Doré" }, "Nageoire dorée")]   // ...
    [TestCase("Fish", new string[] { "Écailles", "Doré" }, "Écailles dorées")]   // Plurals
    [TestCase("Fish", new string[] { "Écailles", "Tacheté" }, "Écailles tachetées")]   // ...
    [TestCase("Fish", new string[] { "Nageoire", "Blanc", "Poisson" }, "Poisson à la nageoires blanche")]   // A name and two adjectives
    [TestCase("Fish", new string[] { "Poisson", "Doré", "Dos" }, "Poisson à dos doré")]
    [TestCase("Fish", new string[] { "Poisson", "Écailles", "Tacheté" }, "Poisson aux écailles tachetées")]
    [TestCase("Fish", new string[] { "Tacheté", "Doré", "Blanc" }, "Taches dorées et blanches")] // Three adjectives
    [TestCase("Fish", new string[] { "Doré", "Tacheté", "Blanc" }, "Or tacheté et blanc")] // Three adjectives
    [TestCase("Fish", new string[] { "Blanc", "Doré", "Tacheté" }, "Truc blanc doré et tacheté")] // Three adjectives
    public void FrenchNameBuilderFish(string animalName, string[] inputWords, string expectedName)
    {
        TestNameCombo(animalName, inputWords, expectedName);
    }
}
