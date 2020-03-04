using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopUp : MonoBehaviour
{
    public float lifetime;
    public Vector3 position;
    public Vector3 velocity;
    public string text;
    public Color color;
    private float timeElapsed = 0;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        TextMesh temp = gameObject.GetComponent<TextMesh>();
        temp.text = text;
        temp.color = color;
        gameObject.transform.position = position + (velocity * timeElapsed);
        timeElapsed += Time.deltaTime;
    }
}
