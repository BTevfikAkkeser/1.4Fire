using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Araba iç mekanı için UI kontrolcüsü
/// ScriptableObject tabanlı etkileşim sistemi ile çalışır
/// </summary>
public class CarInteriorUISO : MonoBehaviour
{
    [Header("UI References")]
    public Text centerScreenText; // Ekranın ortasında görünen yazı
    public GameObject interactionPanel; // Etkileşim paneli
    public Text elementNameText; // Eleman adı yazısı
    public Text elementDescriptionText; // Eleman açıklama yazısı
    public Slider interactionSlider; // Değer kontrolü slider'ı
    public Button rotateLeftButton; // Sola döndürme butonu
    public Button rotateRightButton; // Sağa döndürme butonu
    public Button toggleButton; // Açma/kapama butonu
    public Button closeButton; // Paneli kapatma butonu
    public Image elementIcon; // Eleman ikonu (opsiyonel)
    
    
    [Header("Interaction Settings")]
    public float interactionDistance = 2.5f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode uiToggleKey = KeyCode.Tab; // UI panelini açma/kapama tuşu
    public LayerMask interactionLayer = -1; // Etkileşime girecek katmanlar
    
    // Etkileşim durumu
    private Camera playerCamera;
    private InteractableElementSO currentTarget;
    private bool isControlPanelOpen = false;
    
    // Dönme kontrolü hızı
    private float rotationSpeed = 15f;
    
    void Start()
    {
        // Ana kamera referansı
        playerCamera = Camera.main;
        
        // UI bileşenlerini kontrol et
        if (!centerScreenText)
        {
            Debug.LogWarning("Center screen text not assigned in CarInteriorUISO!");
        }
        
        // UI panel başlangıçta kapalı
        if (interactionPanel)
        {
            interactionPanel.SetActive(false);
        }
        
        // Button olaylarını ayarla
        SetupButtonEvents();
    }
    
    void Update()
    {
        // Ana kamera referansını kontrol et
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null) return;
        }

        // Bakılan elemanı bul
        if (!isControlPanelOpen)
        {
            CheckForInteractiveElement();
        }
        
        // Etkileşim tuşuna basıldığında
        if (Input.GetKeyDown(interactionKey) && currentTarget != null)
        {
            if (!isControlPanelOpen)
            {
                currentTarget.Interact();
            }
        }
        
        // UI panel açma/kapama tuşuna basıldığında
        if (Input.GetKeyDown(uiToggleKey) && currentTarget != null)
        {
            ToggleControlPanel();
        }
        
        // UI'ı güncelle
        UpdateUI();
    }
    
    /// <summary>
    /// Buton olaylarını ayarla
    /// </summary>
    void SetupButtonEvents()
    {
        if (rotateLeftButton) rotateLeftButton.onClick.AddListener(OnRotateLeftClick);
        if (rotateRightButton) rotateRightButton.onClick.AddListener(OnRotateRightClick);
        if (toggleButton) toggleButton.onClick.AddListener(OnToggleClick);
        if (closeButton) closeButton.onClick.AddListener(CloseControlPanel);
        if (interactionSlider) interactionSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    
    /// <summary>
    /// Kameradan bir ışın göndererek etkileşimli eleman ara
    /// </summary>
    void CheckForInteractiveElement()
    {
        if (playerCamera == null) return;

        // Önceki hedefi temizle
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
            currentTarget = null;
        }
        
        // Merkez yazıyı temizle
        if (centerScreenText != null)
        {
            centerScreenText.text = "";
        }
        
        // Ekranın ortasından ışın gönder
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Etkileşim mesafesi içinde bir şeye çarparsa
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            // Çarpılan nesne InteractableElementSO bileşenine sahip mi?
            InteractableElementSO element = hit.collider.GetComponent<InteractableElementSO>();
            if (element != null && element.canBeInteracted)
            {
                // Mevcut hedef olarak ayarla
                currentTarget = element;
                currentTarget.SetHighlighted(true);
                
                // UI yazısını güncelle
                if (centerScreenText != null)
                {
                    centerScreenText.text = element.GetDisplayName();
                }
            }
        }
    }
    
    /// <summary>
    /// Kontrol panelini açıp kapatma
    /// </summary>
    void ToggleControlPanel()
    {
        if (isControlPanelOpen)
        {
            CloseControlPanel();
        }
        else
        {
            OpenControlPanel();
        }
    }
    
    /// <summary>
    /// Kontrol panelini aç
    /// </summary>
    void OpenControlPanel()
    {
        if (currentTarget == null || interactionPanel == null)
            return;
            
        interactionPanel.SetActive(true);
        isControlPanelOpen = true;
        
        // Panel başlık ve açıklamasını ayarla
        if (elementNameText) elementNameText.text = currentTarget.GetDisplayName();
        if (elementDescriptionText) elementDescriptionText.text = currentTarget.GetInteractionMessage();
        
        // Kontrolleri eleman türüne göre yapılandır
        ConfigureUIForElement();
        
        // Merkez yazıyı gizle
        if (centerScreenText) centerScreenText.text = "";
        
        // İmleci göster ve serbest bırak
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    /// <summary>
    /// Kontrol panelini kapat
    /// </summary>
    void CloseControlPanel()
    {
        if (interactionPanel == null)
            return;
            
        interactionPanel.SetActive(false);
        isControlPanelOpen = false;
        
        // İmleci kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    /// <summary>
    /// UI'ı eleman tipine göre yapılandır
    /// </summary>
    void ConfigureUIForElement()
    {
        if (currentTarget == null || currentTarget.interactionEvent == null)
            return;
            
        // Varsayılan olarak tüm kontrolleri gizle
        if (rotateLeftButton) rotateLeftButton.gameObject.SetActive(false);
        if (rotateRightButton) rotateRightButton.gameObject.SetActive(false);
        if (toggleButton) toggleButton.gameObject.SetActive(false);
        if (interactionSlider) interactionSlider.gameObject.SetActive(false);
        
        // Transformasyon tipine göre kontrolleri göster
        var transformType = currentTarget.interactionEvent.transformationType;
        
        switch (transformType)
        {
            case CarInteractionEventSO.TransformationType.Rotate:
                if (rotateLeftButton) rotateLeftButton.gameObject.SetActive(true);
                if (rotateRightButton) rotateRightButton.gameObject.SetActive(true);
                break;
                
            case CarInteractionEventSO.TransformationType.Slide:
            case CarInteractionEventSO.TransformationType.Toggle:
                if (toggleButton)
                {
                    toggleButton.gameObject.SetActive(true);
                    toggleButton.GetComponentInChildren<Text>().text = currentTarget.isActivated ? "Kapat" : "Aç";
                }
                break;
                
            case CarInteractionEventSO.TransformationType.VolumeControl:
                if (interactionSlider)
                {
                    interactionSlider.gameObject.SetActive(true);
                    interactionSlider.minValue = currentTarget.interactionEvent.minValue;
                    interactionSlider.maxValue = currentTarget.interactionEvent.maxValue;
                    interactionSlider.value = currentTarget.currentValue;
                }
                break;
        }
        
        // Eleman ikonu (varsa)
        if (elementIcon && currentTarget.interactionEvent.displayIcon != null)
        {
            elementIcon.gameObject.SetActive(true);
            elementIcon.sprite = currentTarget.interactionEvent.displayIcon;
        }
        else if (elementIcon)
        {
            elementIcon.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// UI'ı güncel duruma göre güncelle
    /// </summary>
    void UpdateUI()
    {
        if (!isControlPanelOpen || currentTarget == null)
            return;
            
        // Slider değerini güncelle
        if (interactionSlider && interactionSlider.gameObject.activeSelf)
        {
            if (interactionSlider.value != currentTarget.currentValue)
            {
                interactionSlider.value = currentTarget.currentValue;
            }
        }
        
        // Toggle butonunu güncelle
        if (toggleButton && toggleButton.gameObject.activeSelf)
        {
            toggleButton.GetComponentInChildren<Text>().text = currentTarget.isActivated ? "Kapat" : "Aç";
        }
    }
    
    #region Button Events
    
    /// <summary>
    /// Sola döndürme butonuna tıklandığında
    /// </summary>
    void OnRotateLeftClick()
    {
        if (currentTarget == null || currentTarget.interactionEvent == null)
            return;
            
        // Elemanı sola döndür
        Vector3 rotationAxis = currentTarget.interactionEvent.rotationAxis;
        currentTarget.transform.Rotate(-rotationAxis, rotationSpeed);
    }
    
    /// <summary>
    /// Sağa döndürme butonuna tıklandığında
    /// </summary>
    void OnRotateRightClick()
    {
        if (currentTarget == null || currentTarget.interactionEvent == null)
            return;
            
        // Elemanı sağa döndür
        Vector3 rotationAxis = currentTarget.interactionEvent.rotationAxis;
        currentTarget.transform.Rotate(rotationAxis, rotationSpeed);
    }
    
    /// <summary>
    /// Toggle butonuna tıklandığında
    /// </summary>
    void OnToggleClick()
    {
        if (currentTarget == null)
            return;
            
        // Elemanın etkileşimini tetikle
        currentTarget.Interact();
    }
    
    /// <summary>
    /// Slider değeri değiştiğinde
    /// </summary>
    void OnSliderValueChanged(float value)
    {
        if (currentTarget == null)
            return;
            
        // Değeri doğrudan ayarla
        currentTarget.SetValue(value);
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Belirli bir isimle etkileşimli eleman bul
    /// </summary>
    public InteractableElementSO FindElementByName(string elementName)
    {
        // Tüm etkileşimli elemanları bul
        InteractableElementSO[] allElements = FindObjectsOfType<InteractableElementSO>();
        
        // İsme göre ara
        foreach (var element in allElements)
        {
            if (element.GetDisplayName() == elementName)
            {
                return element;
            }
        }
        
        Debug.LogWarning($"Element with name '{elementName}' not found!");
        return null;
    }
    
    /// <summary>
    /// Belirli bir elemanı seç ve UI panelini aç
    /// </summary>
    public void SelectElement(InteractableElementSO element)
    {
        if (element == null)
        {
            Debug.LogWarning("Tried to select a null element!");
            return;
        }
        
        // Önceki hedefi temizle
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
        }
        
        // Yeni hedefi ayarla
        currentTarget = element;
        currentTarget.SetHighlighted(true);
        OpenControlPanel();
    }
    
    /// <summary>
    /// İsme göre eleman seç ve UI panelini aç
    /// </summary>
    public void SelectElementByName(string elementName)
    {
        InteractableElementSO element = FindElementByName(elementName);
        if (element != null)
        {
            SelectElement(element);
        }
    }
    
    /// <summary>
    /// Elemana odaklanma (UI olmadan)
    /// </summary>
    public void FocusOnElement(InteractableElementSO element)
    {
        if (element == null)
            return;
            
        // Önceki hedefi temizle
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
        }
        
        // Yeni hedefi ayarla
        currentTarget = element;
        currentTarget.SetHighlighted(true);
        
        // UI metnini güncelle
        if (centerScreenText)
        {
            centerScreenText.text = element.GetDisplayName();
        }
    }
    
    /// <summary>
    /// İsme göre elemana odaklan (UI olmadan)
    /// </summary>
    public void FocusOnElementByName(string elementName)
    {
        InteractableElementSO element = FindElementByName(elementName);
        if (element != null)
        {
            FocusOnElement(element);
        }
    }
    
    /// <summary>
    /// Mevcut hedefi al
    /// </summary>
    public InteractableElementSO GetCurrentTarget()
    {
        return currentTarget;
    }
    
    /// <summary>
    /// Tüm etkileşimli elemanları bul
    /// </summary>
    public InteractableElementSO[] GetAllInteractableElements()
    {
        return FindObjectsOfType<InteractableElementSO>();
    }
    
    /// <summary>
    /// Tüm etkileşimli elemanları belirli bir tipte bul
    /// </summary>
    public List<InteractableElementSO> GetElementsByType(CarInteractionEventSO.TransformationType type)
    {
        List<InteractableElementSO> elements = new List<InteractableElementSO>();
        InteractableElementSO[] allElements = GetAllInteractableElements();
        
        foreach (var element in allElements)
        {
            if (element.interactionEvent != null && element.interactionEvent.transformationType == type)
            {
                elements.Add(element);
            }
        }
        
        return elements;
    }
    
    /// <summary>
    /// UI panelinin açık olup olmadığını kontrol et
    /// </summary>
    public bool IsControlPanelOpen()
    {
        return isControlPanelOpen;
    }
    
    /// <summary>
    /// Tüm etkileşimi bir anda durdur
    /// </summary>
    public void StopAllInteractions()
    {
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
            currentTarget = null;
        }
        
        CloseControlPanel();
        
        if (centerScreenText)
        {
            centerScreenText.text = "";
        }
    }
    
    #endregion
    
    void OnDrawGizmos()
    {
        if (playerCamera == null) return;
        
        // Ray'in başlangıç noktasını ve yönünü al
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        // Ray'in rengini ayarla (Yeşil = Etkileşimde, Kırmızı = Etkileşim yok)
        Gizmos.color = currentTarget != null ? Color.green : Color.red;
        
        // Ray'i çiz
        Gizmos.DrawRay(ray.origin, ray.direction * interactionDistance);
        
        // Etkileşim mesafesini gösteren küre
        if (currentTarget != null)
        {
            Gizmos.DrawWireSphere(ray.origin + ray.direction * interactionDistance, 0.1f);
        }
    }
}