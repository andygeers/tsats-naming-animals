using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class FadingWordSuggestion : WordSuggestion
{
    public float wobbleFactor = 1.0f;
    public float wobbleOffset = 0.0f;

    public float minScreenY = 0.2f;
    public float maxScreenY = 0.8f;

    private float age = 0.0f;
    private bool startedFade;

    private float offset = 0.0f;
    private float offsetSpeed = 0.0f;

    Mesh mesh;

    Vector3[] vertices;

    // Use this for initialization
    void Start()
    {
        
    }

    protected new void OnEnable()
    {
        base.OnEnable();

        age = 0;
        startedFade = false;
        wobbleOffset = Random.Range(0.0f, 2 * Mathf.PI);

        // Fade in
        textMesh.DOFade(0.0f, 1.0f).From();

        StartCoroutine(PickPositionSoon());
    }

    private IEnumerator PickPositionSoon()
    {
        // Wait one frame for the size etc to be calculated
        yield return 0;

        RecalculateBounds();

        Camera cam = Camera.main;
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector3 worldPos = transform.position;
        box.enabled = false;    // Disable box so it doesn't collider with itself

        float screenRangeY = maxScreenY - minScreenY;

        for (int attempts = 0; attempts < 30; attempts ++)
        {
            
            // Pick a random screen position
            float screenX = Random.Range(0.2f, 0.8f) * cam.pixelWidth;
            float screenY = (Random.Range(0.3f, screenRangeY) + minScreenY) * cam.pixelHeight;
            worldPos = cam.ScreenToWorldPoint(new Vector3(screenX, screenY, transform.position.z));
            worldPos.z = 0.0f;
            //Debug.Log("Screen pos " + screenX + ", " + screenY + " transformed to " + worldPos);

            // Look for collisions with other words
            const float boxScaleFactor = 1.5f; // I don't know where this comes from but it works in Tree of Testing
            Collider2D[] collisions = Physics2D.OverlapBoxAll(worldPos, box.size / boxScaleFactor, 0.0f);

            // Exclude collisions which don't have text on them
            int collisionCount = 0;
            foreach (Collider2D collision in collisions)
            {
                TextMeshPro text = collision.GetComponent<TextMeshPro>();
                if (text != null) collisionCount++;
            }

            if (collisionCount == 0)
            {
                Debug.Log("Succeeded finding a spot with no collisions for " + textMesh.text + " on attempt " + attempts);
                break;
            }

            if ((attempts == 9) || (attempts == 19))
            {
                // Wait one more frame for the size etc to be calculated
                yield return 0;                
            }
            else if (attempts >= 29)
            {
                Debug.Log("Gave up finding a spot with no collisions for " + textMesh.text);
                foreach (Collider2D collision in collisions)
                {
                    WordSuggestion suggestion = collision.GetComponent<WordSuggestion>();
                    if (suggestion != null)
                    {
                        // Since we can't position ourselves without colliding, just make the other words disappear instead
                        suggestion.Clear();
                    }
                }
            }
        }

        box.enabled = true;
        transform.position = worldPos;
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime * speed * 50.0f / 4.0f;

        offset += offsetSpeed * Time.deltaTime;

        float alpha = 1.0f;
        if (age < 10.0f)
        {
            alpha = age / 10.0f;
        }
        else if (age > 100.0f)
        {
            if (onComplete != null) onComplete.Invoke(this);

            Destroy(gameObject);
        }
        else if (age > 90.0f)
        {
            alpha = (100.0f - age) / 10.0f;
        }        

        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        Color[] colors = mesh.colors;


        for (int wordIndex = 0; wordIndex < textMesh.textInfo.wordCount; wordIndex ++)
        {
            TMP_WordInfo w = textMesh.textInfo.wordInfo[wordIndex];
            Vector3 offset = Wobble(Time.time + wordIndex);

            for (int i = 0; i < w.characterCount; i++)
            {
                TMP_CharacterInfo c = textMesh.textInfo.characterInfo[w.firstCharacterIndex + i];

                int index = c.vertexIndex;

                Color col = colors[index];
                col.a = alpha;

                colors[index] = col;
                colors[index + 1] = col;
                colors[index + 2] = col;
                colors[index + 3] = col;

                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;
            }
        }

        mesh.vertices = vertices;
        mesh.colors = colors;
        if (textMesh.canvasRenderer != null) textMesh.canvasRenderer.SetMesh(mesh);
    }

    Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f + wobbleOffset), Mathf.Cos(time * 2.5f + wobbleOffset)) * wobbleFactor + Vector2.one * offset;
    }

    // Lies
    public override void PullToCentre()
    {
        offsetSpeed = 1.0f;
        age = 90.0f;
    }

    // Truth
    public override void WriggleAway()
    {
        offsetSpeed = -1.0f;
        age = 90.0f;
    }

    public override void Clear()
    {
        if (age < 90.0f)
        {
            age = 90.0f;
            speed *= 2.0f;
        }
    }

    public override void ProgressBy(float amount)
    {
        if (age < 90.0f)
        {
            age = Mathf.Min(age + amount * 100.0f, 90.0f);
        }
        
    }
}
