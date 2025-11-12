using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class WordSuggestion : MonoBehaviour
{
    public float speed = 1.0f;
    public float radius = 1.0f;

    protected TextMeshPro textMesh;
    protected UnityAction<WordSuggestion> onComplete;

    // Use this for initialization
    void Start()
    {

    }

    protected void OnEnable()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public virtual void PullToCentre()
    {

    }

    public virtual void WriggleAway()
    {

    }

    public void SetCompletionListener(UnityAction<WordSuggestion> onComplete)
    {
        this.onComplete = onComplete;
    }

    public void SetRadiusFactor(float factor)
    {
        Debug.Log("Adjusting word radius " + radius + " by factor " + factor);
        radius *= factor;
    }

    public virtual void Clear()
    {

    }

    public virtual void ProgressBy(float amount)
    {

    }

    public virtual void RecalculateBounds()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.size = textMesh.textBounds.size;
    }
}
