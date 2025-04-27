using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Etkileşimli elemanlara dış çizgi (outliner) efekti ekleyen bileşen
/// </summary>
[RequireComponent(typeof(Renderer))]
public class InteractiveElementOutliner : MonoBehaviour
{
    [Header("Outliner Ayarları")]
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 0.05f;
    
    [Header("Görünürlük")]
    public bool alwaysVisible = false;
    public bool showOnHighlight = true;
    
    // Referanslar
    private Renderer elementRenderer;
    private GameObject outlineElement;
    private Renderer outlineRenderer;
    private InteractableElement interactableElement;
    
    // Materyaller
    private Material outlineMaterial;
    
    void Start()
    {
        // Gerekli bileşenleri al
        elementRenderer = GetComponent<Renderer>();
        interactableElement = GetComponent<InteractableElement>();
        
        // Outliner oluştur
        CreateOutliner();
        
        // Başlangıçta outline'ı ayarla
        SetOutlineVisibility(alwaysVisible);
    }
    
    /// <summary>
    /// Outliner elementini oluştur
    /// </summary>
    void CreateOutliner()
    {
        // Outline elementi zaten varsa silip yeniden oluşturalım
        Transform existingOutline = transform.Find("Outliner");
        if (existingOutline != null)
        {
            Destroy(existingOutline.gameObject);
        }
        
        // Outliner elementi oluştur
        outlineElement = new GameObject("Outliner");
        outlineElement.transform.SetParent(transform);
        outlineElement.transform.localPosition = Vector3.zero;
        outlineElement.transform.localRotation = Quaternion.identity;
        outlineElement.transform.localScale = Vector3.one * (1 + outlineWidth);
        
        // MeshFilter kopyala
        MeshFilter originalMeshFilter = GetComponent<MeshFilter>();
        if (originalMeshFilter != null && originalMeshFilter.sharedMesh != null)
        {
            MeshFilter outlineMeshFilter = outlineElement.AddComponent<MeshFilter>();
            outlineMeshFilter.sharedMesh = originalMeshFilter.sharedMesh;
            
            // Renderer ekle
            outlineRenderer = outlineElement.AddComponent<MeshRenderer>();
        }
        else
        {
            // SkinnedMeshRenderer kontrolü
            SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh != null && skinnedMesh.sharedMesh != null)
            {
                SkinnedMeshRenderer outlineSkinnedMesh = outlineElement.AddComponent<SkinnedMeshRenderer>();
                outlineSkinnedMesh.sharedMesh = skinnedMesh.sharedMesh;
                outlineSkinnedMesh.rootBone = skinnedMesh.rootBone;
                outlineSkinnedMesh.bones = skinnedMesh.bones;
                
                outlineRenderer = outlineSkinnedMesh;
            }
            else
            {
                // Karmaşık elemanlar için basit küp outline'ı oluştur
                MeshFilter outlineMeshFilter = outlineElement.AddComponent<MeshFilter>();
                outlineMeshFilter.sharedMesh = CreateCubeMesh(elementRenderer.bounds);
                
                outlineRenderer = outlineElement.AddComponent<MeshRenderer>();
                
                // Outline elementini doğru konumlandır
                outlineElement.transform.position = elementRenderer.bounds.center;
            }
        }
        
        // Outline materyali oluştur
        CreateOutlineMaterial();
        
        // Outline için düzgün görünüm ayarları
        if (outlineRenderer != null)
        {
            outlineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            outlineRenderer.receiveShadows = false;
            
            // Materyali uygula
            outlineRenderer.material = outlineMaterial;
        }
    }
    
    /// <summary>
    /// MeshFilter bulunamadığında basit bir küp mesh'i oluştur
    /// </summary>
    Mesh CreateCubeMesh(Bounds bounds)
    {
        Vector3 size = bounds.size;
        
        // Küp için mesh oluştur
        Mesh mesh = new Mesh();
        
        // Köşe noktaları
        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(-size.x/2, -size.y/2, -size.z/2);
        vertices[1] = new Vector3(size.x/2, -size.y/2, -size.z/2);
        vertices[2] = new Vector3(size.x/2, size.y/2, -size.z/2);
        vertices[3] = new Vector3(-size.x/2, size.y/2, -size.z/2);
        vertices[4] = new Vector3(-size.x/2, -size.y/2, size.z/2);
        vertices[5] = new Vector3(size.x/2, -size.y/2, size.z/2);
        vertices[6] = new Vector3(size.x/2, size.y/2, size.z/2);
        vertices[7] = new Vector3(-size.x/2, size.y/2, size.z/2);
        
        // Üçgenler
        int[] triangles = new int[] {
            // Ön
            0, 2, 1, 0, 3, 2,
            // Sağ
            1, 6, 5, 1, 2, 6,
            // Arka
            5, 7, 4, 5, 6, 7,
            // Sol
            4, 3, 0, 4, 7, 3,
            // Üst
            3, 6, 2, 3, 7, 6,
            // Alt
            0, 1, 5, 0, 5, 4
        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }
    
    /// <summary>
    /// Outline materyali oluştur
    /// </summary>
    void CreateOutlineMaterial()
    {
        outlineMaterial = new Material(Shader.Find("Standard"));
        outlineMaterial.SetFloat("_Mode", 3); // Transparent mode
        outlineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        outlineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        outlineMaterial.SetInt("_ZWrite", 0);
        outlineMaterial.DisableKeyword("_ALPHATEST_ON");
        outlineMaterial.EnableKeyword("_ALPHABLEND_ON");
        outlineMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        outlineMaterial.renderQueue = 3000;
        outlineMaterial.color = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.5f);
        outlineMaterial.SetFloat("_Glossiness", 0);
    }
    
    /// <summary>
    /// Outline görünürlüğünü ayarla
    /// </summary>
    public void SetOutlineVisibility(bool visible)
    {
        if (outlineElement != null)
        {
            outlineElement.SetActive(visible);
        }
    }
    
    /// <summary>
    /// Outline rengini değiştir
    /// </summary>
    public void SetOutlineColor(Color color)
    {
        outlineColor = color;
        if (outlineMaterial != null)
        {
            outlineMaterial.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }
    
    /// <summary>
    /// Outline genişliğini değiştir
    /// </summary>
    public void SetOutlineWidth(float width)
    {
        outlineWidth = width;
        if (outlineElement != null)
        {
            outlineElement.transform.localScale = Vector3.one * (1 + width);
        }
    }
    
    void OnDestroy()
    {
        // Outliner materyalini temizle
        if (outlineMaterial != null)
        {
            Destroy(outlineMaterial);
        }
    }
}