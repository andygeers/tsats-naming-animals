using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using FMODUnity;
using UnityEngine.Localization;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using Lean.Touch;
//using TMPro;
//using DG.Tweening;
//using Ilumisoft.VisualStateMachine;

public class NameAnimal : MonoBehaviour
{
    /*
    public enum ANIMALPARAMS
    {
        None         ,
        JustLooking  ,
        Otters       ,
        Mandrill     ,
        Fish         ,
        Elephant     ,
        Yak          ,
        Bird         ,
        Bird2        ,
    }

    public ANIMALPARAMS fmodAnimalParam;
    */
    public class NameSuggestion
    {
        public int wordType;
        public int suggestionIndex;
        public int selectedWordIndex;
        public float selectedAt;
        public string word;
    }
    
    public string animalName;

    /*
    public WordSuggestion wordSuggestionPrefab;
    public StudioGlobalParameterTrigger FmodAnimalTrigger;    

    private List<WordSuggestion> words;    
    private int suggestionIndex;
    private string nameSoFar;
    private Coroutine suggestingCoroutine;

    public int concurrentSuggestions = 4;
    public int minimumRequiredWords = 2;
    public TextMeshProUGUI chosenNameElementPrefab;
    public GameObject nameSoFarContainer;
    public UnityEngine.UI.Button confirmButton;
    public UnityEngine.UI.Button resetButton;
    public TextMeshProUGUI populateWithSelection;
    public Actor animalActor;    
    public StateMachine stateMachine;

    public Color normalButtonColor = Color.white;

    public UnityEvent onComplete;
    public UnityEvent onInsufficientWords;

    private List<NameSuggestion> allSuggestions;
    private List<NameSuggestion> selectedElements;

    private AnimalNameBuilder.NameBuilder nameBuilder;

    // Use this for initialization
    void Start()
    {
        
    }

    private void OnEnable()
    {
        // Instantiate based on which language
        nameBuilder = AnimalNameBuilder.NameBuilder.LocalisedNameBuilder();

        words = new List<WordSuggestion>();
        selectedElements = new List<NameSuggestion>();
        FmodAnimalTrigger.Value = (int)fmodAnimalParam;
        FmodAnimalTrigger.TriggerParameters();
        FmodAnimalTrigger.Value = (int)ANIMALPARAMS.None;   // Put it back afterwards
        SetConfirmButtonEnabled(false);
        EventSystem.current.SetSelectedGameObject(null);        
        if (resetButton != null) resetButton.gameObject.SetActive(false);

        BuildSuggestionsList();
        suggestionIndex = Random.Range(0, allSuggestions.Count - 1);

        ResetChoice();

        suggestingCoroutine = StartCoroutine(SuggestNames(0.0f));
    }

    private void SetConfirmButtonEnabled(bool enabled)
    {
        if (confirmButton == null) return;

        UnityEngine.UI.ColorBlock colors = confirmButton.colors;
        if (enabled)
        {
            colors.normalColor = normalButtonColor;
        }
        else
        {
            colors.normalColor = colors.disabledColor;
        }
        confirmButton.colors = colors;
    }

    private void BuildSuggestionsList()
    {
        allSuggestions = new List<NameSuggestion>();

        string[] typePrefixes = { "Adjective", "Noun", "Verb", "Name" };

        for (int index = 0; index < 3; index ++)
        {
            for (int typeIndex = 0; typeIndex < 4; typeIndex ++)
            {
                string suggestion = GenerateSuggestion(typePrefixes[typeIndex], index);

                if ((suggestion.Length == 0) || (suggestion.StartsWith("No translation found for")))
                {
                    // No suggestion available with this index
                    continue;
                }

                NameSuggestion result = new NameSuggestion();
                result.wordType = typeIndex;
                result.suggestionIndex = index;
                result.word = suggestion;
                allSuggestions.Add(result);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SuggestNames(float initialDelay)
    {        
        if (initialDelay > 0.0f) yield return new WaitForSeconds(initialDelay);

        while (enabled)
        {
            NextWord();

            // Use a shorter delay if there's not enough words yet
            Debug.Log("Word count is " + words.Count);
            float delay = 2.0f;
            if (words.Count < concurrentSuggestions) delay = 0.5f;
            yield return new WaitForSeconds(delay);
        }
    }
    */
    public string GenerateSuggestion(string typePrefix, int index)
    {
        LocalizedString localized = new LocalizedString();
        localized.TableReference = "AnimalNames";
        localized.TableEntryReference = animalName + "_" + typePrefix + "_" + index;
        Debug.Log("Picking suggestion with key " + localized.TableEntryReference);            
        return localized.GetLocalizedString();            
    }    
    /*
    private bool UsedAlready(NameSuggestion suggestion)
    {
        if (suggestion == null) return false;

        foreach (NameSuggestion existing in selectedElements)
        {
            if (existing.Equals(suggestion))
            {
                return true;
            }
        }

        return false;
    }

    void NextWord()
    {
        if (wordSuggestionPrefab == null) return;

        NameSuggestion suggestion = null;
        while ((suggestion == null) || UsedAlready(suggestion))
        {
            suggestionIndex = (suggestionIndex + 1) % allSuggestions.Count;
            suggestion = allSuggestions[suggestionIndex];
        }

        WordSuggestion word = Instantiate(wordSuggestionPrefab, transform, false);
        word.GetComponent<TMPro.TextMeshPro>().text = nameBuilder.LabelForSuggestion(suggestion);
        word.SetCompletionListener((WordSuggestion word) => RemoveWord(word));
        word.name = "Suggestion " + suggestion.word;

        LeanSelectableByFinger selectable = word.GetComponent<LeanSelectableByFinger>();
        selectable.OnSelected.AddListener(() => SelectWord(word, suggestion));

        words.Add(word);

        if (words.Count > concurrentSuggestions)
        {
            // Age some of the earlier words
            int toClear = words.Count - concurrentSuggestions;
            for (int i = 0; i < toClear; i ++)
            {
                words[i].Clear();
            }
        }
    }    

    private void SelectWord(WordSuggestion word, NameSuggestion suggestion)
    {
        // See if name is already selected
        if (selectedElements.Contains(suggestion)) return;

        // Keep track of when this suggestion was selected
        suggestion.selectedAt = Time.time;

        Debug.Log("Select word " + suggestion.word);
        int removedIndex = -1;        
        if (selectedElements.Count >= 3)
        {
            // FIFO remove one element to restrict the total number
            removedIndex = selectedElements.Count - 1;
            selectedElements.RemoveAt(removedIndex);
        }

        selectedElements.Insert(0, suggestion);

        // Use the new word to build a complete name
        Transform wordToReplace = BuildNameFromSelected(removedIndex);
        if (wordToReplace == null)
        {
            wordToReplace = nameSoFarContainer.transform;
        }
        Debug.Log("Word to replace is " + wordToReplace.name);
        // Convert the UI element's screen position to world position
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(nameSoFarContainer.GetComponent<RectTransform>());
        Vector3 uiPosition = Camera.main.ScreenToWorldPoint(wordToReplace.position);

        word.transform.DOMove(uiPosition, 0.5f);
        word.Clear();          
    }

    private void FloatAwayOldSelection(GameObject oldWord)
    {
        Debug.Log("Removed old word " + oldWord.name);

        // Find our canvas
        Canvas canvas = oldWord.transform.GetComponentInParent<Canvas>();
        if (canvas == null) return;        

        // Let's detach from its parent then float away
        oldWord.transform.SetParent(canvas.transform);
        TextMeshProUGUI text = oldWord.GetComponent<TextMeshProUGUI>();
        if (text == null) return;
        
        text.rectTransform.DOMoveY(-50.0f, 1.0f).SetRelative(true).        
            OnComplete(() => Destroy(oldWord));

        // Fade out the text at the same time        
        text.DOFade(0.0f, 1.0f);        
    }

    private Transform BuildNameFromSelected(int removedIndex = -1)
    {
        SetConfirmButtonEnabled(selectedElements.Count >= minimumRequiredWords);
        if (resetButton != null) resetButton.gameObject.SetActive(selectedElements.Count > 0);

        if (selectedElements.Count == 0)
        {
            foreach (Transform child in nameSoFarContainer.transform)
            {
                Destroy(child.gameObject);
            }
            return null;
        }        

        for (int idx = 0; idx < selectedElements.Count; idx ++)
        {
            // Keep track of which word ends up where once sorted
            selectedElements[idx].selectedWordIndex = idx;
        }


        // Generate name
        List<string> words = new List<string>();
        List<int> wordIndices = new List<int>();
        nameBuilder.GenerateName(this, selectedElements, ref words, ref wordIndices);

        int i = 0;
        foreach (Transform child in nameSoFarContainer.transform)
        {
            AnimalNameChosenWord choice = child.GetComponent<AnimalNameChosenWord>();
            if ((choice != null) && (choice.choiceIndex == removedIndex))
            {
                FloatAwayOldSelection(child.gameObject);
                break;
            }
            i++;
        }
        foreach (Transform child in nameSoFarContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Keep track of which position our newest word was used in
        Transform targetInsertPosition = null;
        i = 0;
        foreach (string word in words)
        {            
            TextMeshProUGUI presenter = Instantiate(chosenNameElementPrefab, nameSoFarContainer.transform);
            presenter.text = word;
            presenter.name = "Choice " + word;

            EventTrigger trigger = presenter.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(GetWordTappedHandler(wordIndices[i]));
            trigger.triggers.Add(entry);

            EventTrigger.Entry onStartDrag = new EventTrigger.Entry();
            onStartDrag.eventID = EventTriggerType.BeginDrag;
            onStartDrag.callback.AddListener(GetWordDraggedHandler(wordIndices[i]));
            trigger.triggers.Add(onStartDrag);

            // Let's adjust the text colour for each word according to how old it is - so the ones that are about to drop off get paler
            Color textColor = Color.black;
            textColor.r = wordIndices[i] * 0.15f;
            textColor.g = textColor.r;
            textColor.b = textColor.r;
            presenter.color = textColor;

            AnimalNameChosenWord choice = presenter.gameObject.AddComponent<AnimalNameChosenWord>();
            choice.choiceIndex = wordIndices[i];

            if ((wordIndices[i] == 0) && (targetInsertPosition == null))
            {
                targetInsertPosition = presenter.transform;
            }

            i ++;
        }
        nameSoFar = string.Join(" ", words);

        return targetInsertPosition;
    }    

    private UnityAction<BaseEventData> GetWordTappedHandler(int index)
    {
        return (data) => TapSelectedWord((PointerEventData)data, index);
    }

    private UnityAction<BaseEventData> GetWordDraggedHandler(int index)
    {
        return (data) => RemoveSelectedWord(index);
    }

    private void RemoveSelectedWord(int index)
    {
        Debug.Log("Remove selected word " + index);
        if (index >= selectedElements.Count) return;

        // Remove this word from the queue
        selectedElements.RemoveAt(index);        

        // Refresh name
        BuildNameFromSelected();
    }

    private void TapSelectedWord(PointerEventData data, int index)
    {
        Debug.Log("Tapped selected word " + index);
        if (index >= 2) return;

        // Remove this word from the queue and add it at the end
        NameSuggestion word = selectedElements[index];
        selectedElements.RemoveAt(index);
        selectedElements.Add(word);

        // Refresh name
        BuildNameFromSelected();
    }    

    public void RemoveWord(WordSuggestion word)
    {
        words.Remove(word);

        if (words.Count < concurrentSuggestions)
        {
            // Immediately suggest a new word
            if (suggestingCoroutine != null)
            {
                StopCoroutine(suggestingCoroutine);
                suggestingCoroutine = StartCoroutine(SuggestNames(0.1f));
            }
        }
    }

    public void ResetChoice()
    {
        // Remove all words
        selectedElements.Clear();
        BuildNameFromSelected();
    }

    public void ConfirmChoice()
    {
        if (selectedElements.Count < minimumRequiredWords)
        {
            onInsufficientWords?.Invoke();
            resetButton.Select();
            return;
        }

        if (onComplete != null)
            onComplete.Invoke();

        if (stateMachine != null)
        {
            stateMachine.TryTriggerByLabel("Named");
            Firebase.Analytics.FirebaseAnalytics.LogEvent("animal_named", "name", nameSoFar);
        }   

        if (animalActor != null) animalActor.TriggerAnimation("Celebrate");

        UnityAction openBook = () =>
        {
            JournalBook book = FindObjectOfType<JournalBook>(true);
            if (book != null) book.FillInAnimalName(animalName, nameSoFar, null);

            StatusMonitor.Instance["Animal_Name_" + animalName] = nameSoFar;
        };

        if (populateWithSelection != null)
        {
            populateWithSelection.text = nameSoFar;
            JournalEntry journal = populateWithSelection.GetComponentInParent<JournalEntry>(true);
            if (journal != null)
            {
                // Display this entry
                JournalEntry journalEntry = Instantiate(journal);
                UnityEvent onComplete = new UnityEvent();
                onComplete.AddListener(openBook);
                journalEntry.onComplete = onComplete;
                
                journalEntry.gameObject.SetActive(true);                
            }
            else
            {
                openBook.Invoke();
            }
        }
        else
        {
            openBook.Invoke();
        }

        // Look for parent stage controller
        StageController stageController = GetComponentInParent<StageController>();
        Debug.Log("About to write name " + nameSoFar + " for " + animalName);
        stageController.UpdateName(animalName, nameSoFar);
        if ((stageController != null) && (stageController.parentPsm != null))
        {
            // Persist the chosen name
            stageController.parentPsm.PersistValueForKey("A1_Name_" + animalName, nameSoFar);
        }

        gameObject.SetActive(false);
    }
    */
}
