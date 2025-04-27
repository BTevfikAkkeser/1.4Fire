using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Oyuncu etkileşim kontrolcüsü
/// </summary>
public class InteractionPlayerController : MonoBehaviour
{
    [Header("Ray Settings")]
    public bool showRayDebug = true;
    public float rayDistance = 2.5f;
    public LayerMask interactableLayers = -1; // Varsayılan olarak tüm layerlar
    public Color rayColorNormal = Color.white;
    public Color rayColorHit = Color.green;
    public KeyCode interactionKey = KeyCode.E;
    public Transform rayOrigin; // Null ise kamera kullanılır
    
    // Ray değişkenleri
    private Vector3 rayStartPos;
    private Vector3 rayEndPos;
    private bool hasRayHit;
    private InteractableElementSO raycastTarget;
    
    void Start()
    {
        // Ray kaynağını kontrol et
        if (rayOrigin == null)
        {
            // Eğer kamera varsa onu kullan
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                rayOrigin = mainCamera.transform;
            }
            else
            {
                // Kamera yoksa kendisini kullan
                rayOrigin = transform;
            }
        }
    }
    
    void Update()
    {
        // Ray işlemini gerçekleştir
        CastInteractionRay();
        
        // Etkileşim tuşu kontrolü
        if (raycastTarget != null && Input.GetKeyDown(interactionKey))
        {
            raycastTarget.Interact();
        }
    }
    
    /// <summary>
    /// Etkileşim için ray gönder
    /// </summary>
    private void CastInteractionRay()
    {
        Ray ray;
        RaycastHit hit;
        
        // Önceki hedefi temizle
        if (raycastTarget != null)
        {
            raycastTarget.SetHighlighted(false);
            raycastTarget = null;
        }
        
        // Ray başlangıç noktasını kaydet (Gizmos için)
        rayStartPos = rayOrigin.position;
        
        // Ray oluştur
        ray = new Ray(rayOrigin.position, rayOrigin.forward);
        hasRayHit = Physics.Raycast(ray, out hit, rayDistance, interactableLayers);
        
        // Ray sonunu belirle (Gizmos için)
        rayEndPos = hasRayHit ? hit.point : rayStartPos + rayOrigin.forward * rayDistance;
        
        // Ray çizgisini görselleştir
        Debug.DrawLine(rayStartPos, rayEndPos, hasRayHit ? rayColorHit : rayColorNormal);
        
        // Eğer bir şeye çarptıysa etkileşim kontrolü yap
        if (hasRayHit)
        {
            // Hedefin InteractableElementSO bileşenini kontrol et
            raycastTarget = hit.collider.GetComponent<InteractableElementSO>();
            
            if (raycastTarget != null && raycastTarget.canBeInteracted)
            {
                // Hedefi vurgula
                raycastTarget.SetHighlighted(true);
                
                // Debug bilgileri
                if (showRayDebug)
                {
                    Debug.Log($"Ray hit: {raycastTarget.GetDisplayName()}");
                }
            }
        }
    }
    
    /// <summary>
    /// Editor modundayken ve oyun çalışırken ray'i görselleştir
    /// </summary>
    void OnDrawGizmos()
    {
        if (!showRayDebug)
            return;
            
        // Oyun çalışmıyorsa basit bir gizmo göster
        if (!Application.isPlaying)
        {
            Transform source = rayOrigin;
            if (source == null)
            {
                // Editor modunda rayOrigin atanmamış olabilir
                Camera editorCamera = Camera.current;
                if (editorCamera != null)
                {
                    source = editorCamera.transform;
                }
                else
                {
                    source = transform;
                }
            }
            
            Gizmos.color = rayColorNormal;
            Gizmos.DrawLine(source.position, source.position + source.forward * rayDistance);
            Gizmos.DrawWireSphere(source.position + source.forward * rayDistance, 0.05f);
            return;
        }
        
        // Oyun çalışıyorsa kaydedilen ray bilgilerini kullan
        Gizmos.color = hasRayHit ? rayColorHit : rayColorNormal;
        Gizmos.DrawLine(rayStartPos, rayEndPos);
        
        // Çarpışma noktasını göster
        if (hasRayHit)
        {
            Gizmos.DrawWireSphere(rayEndPos, 0.05f);
        }
    }
}