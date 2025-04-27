using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles an individual UI label for an interactive car interior element
/// </summary>
public class InteractableElementLabel : MonoBehaviour
{
    [Header("References")]
    public GameObject target;
    public Text labelText;
    public Image backgroundImage;
    
    [Header("Settings")]
    public Color standardColor = new Color(0.2f, 0.2f, 0.8f, 0.8f);
    public Color highlightedColor = new Color(0.2f, 0.6f, 1.0f, 0.9f);
    public float fadeDistance = 3.0f;
    
    private Camera mainCamera;
    private RectTransform rectTransform;
    private InteractableElement elementScript;
    
    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        
        if (target)
        {
            elementScript = target.GetComponent<InteractableElement>();
        }
    }
    
    void Update()
    {
        if (!target || !mainCamera)
        {
            Destroy(gameObject);
            return;
        }
        
        UpdatePosition();
        UpdateVisibility();
        UpdateContent();
    }
    
    /// <summary>
    /// Updates the screen position of the label based on the world position of the target
    /// </summary>
    void UpdatePosition()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.transform.position);
        
        // Check if element is behind camera
        if (screenPos.z < 0)
        {
            // Hide or clamp to screen edge
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        rectTransform.position = new Vector3(screenPos.x, screenPos.y, 0);
    }
    
    /// <summary>
    /// Updates the visibility and opacity based on distance
    /// </summary>
    void UpdateVisibility()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, target.transform.position);
        
        // Make labels fade out with distance
        if (distance > fadeDistance)
        {
            float alpha = 1 - ((distance - fadeDistance) / fadeDistance);
            Color textColor = labelText.color;
            textColor.a = Mathf.Clamp01(alpha);
            labelText.color = textColor;
            
            Color bgColor = backgroundImage.color;
            bgColor.a = Mathf.Clamp01(alpha * 0.8f);
            backgroundImage.color = bgColor;
        }
        else
        {
            Color textColor = labelText.color;
            textColor.a = 1.0f;
            labelText.color = textColor;
            
            Color bgColor = backgroundImage.color;
            bgColor.a = 0.8f;
            backgroundImage.color = bgColor;
        }
    }
    
    /// <summary>
    /// Updates the content and styling of the label
    /// </summary>
    void UpdateContent()
    {
        if (elementScript)
        {
            // Update text content
            labelText.text = elementScript.displayName;
            
        }
    }
}