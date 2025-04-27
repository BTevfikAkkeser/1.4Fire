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
    public Text elementNameText; // Eleman adı yazısı
    public GameObject interactionPanel; // Etkileşim paneli
    public Text centerScreenText; // Ekranın ortasında görünen yazı
    public Text elementDescriptionText; // Eleman açıklama yazısı
    public Slider interactionSlider; // Değer kontrolü slider'ı
    public Button rotateLeftButton; // Sola döndürme butonu
    public Button rotateRightButton; // Sağa döndürme butonu
    public Button toggleButton; // Açma/kapama butonu
    public Button closeButton; // Paneli kapatma butonu
    public Image elementIcon; // Eleman ikonu (opsiyonel)
    public Image crosshairImage; // Crosshair görseli
    
    [Header("Interaction Settings")]
    public float interactionDistance = 2.5f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode uiToggleKey = KeyCode.Tab; // UI panelini açma/kapama tuşu
    public LayerMask interactionLayer = -1; // Etkileşime girecek katmanlar
    
    [Header("Ray Debug Settings")]
    public bool showRayDebug = true;
    public Color rayColorNormal = Color.white;
    public Color rayColorHit = Color.green;
    public float rayWidth = 2f;
    public bool showHitPoint = true;
    public float hitPointSize = 0.05f;
    
    [Header("Crosshair Settings")]
    public Color crosshairNormalColor = Color.white;
    public Color crosshairHighlightColor = Color.green;
    public float crosshairSize = 20f;
    
    // Etkileşim durumu
    private Camera playerCamera;
    private InteractableElementSO currentTarget;
    private bool isControlPanelOpen = false;
    private Vector3 rayStartPos;
    private Vector3 rayEndPos;
    private bool hasRayHit;
    
    // Dönme kontrolü hızı
    private float rotationSpeed = 15f;
    
    void Start()
    {
        // Ana kamera referansı
        playerCamera = Camera.main;
        
        // UI panel başlangıçta kapalı
        if (interactionPanel)
        {
            interactionPanel.SetActive(false);
        }
        
        // Crosshair ayarlarını yap
        if (crosshairImage)
        {
            crosshairImage.rectTransform.sizeDelta = new Vector2(crosshairSize, crosshairSize);
            crosshairImage.color = crosshairNormalColor;
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
        
        // Ray'i oluştur
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        // Ray başlangıç pozisyonunu kaydet
        rayStartPos = ray.origin;
        
        // Etkileşim mesafesi içinde bir şeye çarparsa
        hasRayHit = Physics.Raycast(ray, out hit, interactionDistance, interactionLayer);
        rayEndPos = hasRayHit ? hit.point : ray.origin + ray.direction * interactionDistance;
        
        // Crosshair'i güncelle
        UpdateCrosshair(hasRayHit && hit.collider.GetComponent<InteractableElementSO>() != null);
        
        if (hasRayHit)
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
        else if (centerScreenText != null)
        {
            centerScreenText.text = "";
        }
    }
    
    /// <summary>
    /// Crosshair'i güncelle
    /// </summary>
    private void UpdateCrosshair(bool isTargeting)
    {
        if (crosshairImage == null) return;
        
        // Rengi güncelle
        crosshairImage.color = isTargeting ? crosshairHighlightColor : crosshairNormalColor;
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
        if (elementIcon)
        {
            if (currentTarget.interactionEvent.displayIcon != null)
            {
                elementIcon.gameObject.SetActive(true);
                elementIcon.sprite = currentTarget.interactionEvent.displayIcon;
            }
            else
            {
                elementIcon.gameObject.SetActive(false);
            }
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
        
        return null;
    }
    
    /// <summary>
    /// Belirli bir elemanı seç ve UI panelini aç
    /// </summary>
    public void SelectElement(InteractableElementSO element)
    {
        // Önceki hedefi temizle
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
        }
        
        // Yeni hedefi ayarla
        currentTarget = element;
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(true);
            OpenControlPanel();
        }
    }
    
    /// <summary>
    /// Elemana odaklanma (UI olmadan)
    /// </summary>
    public void FocusOnElement(InteractableElementSO element)
    {
        // Önceki hedefi temizle
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(false);
        }
        
        // Yeni hedefi ayarla
        currentTarget = element;
        if (currentTarget != null)
        {
            currentTarget.SetHighlighted(true);
            
            // UI metnini güncelle
            if (centerScreenText)
            {
                centerScreenText.text = element.GetDisplayName();
            }
        }
    }
    
    /// <summary>
    /// Mevcut hedefi al
    /// </summary>
    public InteractableElementSO GetCurrentTarget()
    {
        return currentTarget;
    }
    
    void OnDrawGizmos()
    {
        if (!showRayDebug) return;
        
        if (!Application.isPlaying)
        {
            DrawEditorRay();
        }
        else
        {
            DrawGameRay();
        }
    }
    
    void DrawEditorRay()
    {
        Camera camera = Camera.current;
        if (camera == null) return;
        
        Vector3 startPos = camera.transform.position;
        Vector3 endPos = startPos + camera.transform.forward * interactionDistance;
        
        // Ana ışını çiz
        Gizmos.color = rayColorNormal;
        Gizmos.DrawRay(startPos, camera.transform.forward * interactionDistance);
        
        if (showHitPoint)
        {
            // Başlangıç noktasında artı işareti
            float crossSize = hitPointSize;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPos + Vector3.up * crossSize, startPos - Vector3.up * crossSize);
            Gizmos.DrawLine(startPos + Vector3.right * crossSize, startPos - Vector3.right * crossSize);
            
            // Bitiş noktasında küre
            Gizmos.color = rayColorNormal;
            Gizmos.DrawWireSphere(endPos, hitPointSize);
        }
    }
    
    void DrawGameRay()
    {
        if (rayStartPos == Vector3.zero) return;
        
        // Ana ışını çiz
        Gizmos.color = hasRayHit ? rayColorHit : rayColorNormal;
        Gizmos.DrawRay(rayStartPos, (rayEndPos - rayStartPos).normalized * interactionDistance);
        
        if (showHitPoint)
        {
            // Başlangıç noktasında artı işareti
            float crossSize = hitPointSize;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rayStartPos + Vector3.up * crossSize, rayStartPos - Vector3.up * crossSize);
            Gizmos.DrawLine(rayStartPos + Vector3.right * crossSize, rayStartPos - Vector3.right * crossSize);
            
            // Çarpışma noktasında küre
            if (hasRayHit)
            {
                Gizmos.color = rayColorHit;
                Gizmos.DrawWireSphere(rayEndPos, hitPointSize);
                
                // Eğer hedef varsa, hedef nesneye bir çizgi çiz
                if (currentTarget != null)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(rayEndPos, currentTarget.transform.position);
                }
            }
        }
    }

    void OnDisable()
    {
        // Component devre dışı kaldığında crosshair'i normal renge döndür
        if (crosshairImage)
        {
            crosshairImage.color = crosshairNormalColor;
        }
    }
}