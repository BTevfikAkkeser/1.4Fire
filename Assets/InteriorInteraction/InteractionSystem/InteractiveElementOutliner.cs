using UnityEngine;

/// <summary>
/// Etkileşimli nesneler için outline (dış çizgi) efekti sağlayan bileşen
/// </summary>
[RequireComponent(typeof(Renderer))]
public class InteractiveElementOutliner : MonoBehaviour
{
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 0.05f;
    public bool alwaysVisible = false;
    public bool showOnHighlight = true;

    private Material outlineMaterial;
    private Renderer rend;
    private Material[] originalMaterials;
    private Material[] outlineMaterials;

    void Awake()
    {
        // Renderer bileşenini al
        rend = GetComponent<Renderer>();
        
        // Orijinal materyalleri kaydet
        originalMaterials = rend.sharedMaterials;
        
        // Outline materyalini oluştur
        CreateOutlineMaterial();
        
        // Outline materyallerini hazırla
        PrepareOutlineMaterials();
        
        // Başlangıçta outline'ı gizle
        if (!alwaysVisible)
        {
            SetOutlineVisibility(false);
        }
    }

    void CreateOutlineMaterial()
    {
        // Outline shader'ını bul
        Shader outlineShader = Shader.Find("Standard");
        if (outlineShader == null)
        {
            Debug.LogError("Standard shader not found!");
            return;
        }

        // Outline materyalini oluştur
        outlineMaterial = new Material(outlineShader);
        outlineMaterial.SetColor("_Color", outlineColor);
        outlineMaterial.SetFloat("_Glossiness", 0);
        outlineMaterial.SetFloat("_Mode", 3); // Transparent
        outlineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        outlineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        outlineMaterial.SetInt("_ZWrite", 0);
        outlineMaterial.DisableKeyword("_ALPHATEST_ON");
        outlineMaterial.EnableKeyword("_ALPHABLEND_ON");
        outlineMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        outlineMaterial.renderQueue = 3000;
    }

    void PrepareOutlineMaterials()
    {
        // Outline materyalleri dizisini oluştur
        outlineMaterials = new Material[originalMaterials.Length + 1];
        
        // Orijinal materyalleri kopyala
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            outlineMaterials[i] = originalMaterials[i];
        }
        
        // Outline materyalini ekle
        outlineMaterials[outlineMaterials.Length - 1] = outlineMaterial;
    }

    /// <summary>
    /// Outline'ı göster/gizle
    /// </summary>
    public void SetOutlineVisibility(bool visible)
    {
        if (!enabled) return;

        if (visible)
        {
            // Outline materyallerini uygula
            rend.materials = outlineMaterials;
            
            // Outline genişliğini ayarla
            transform.localScale = Vector3.one * (1 + outlineWidth);
        }
        else
        {
            // Orijinal materyallere geri dön
            rend.materials = originalMaterials;
            
            // Normal ölçeğe geri dön
            transform.localScale = Vector3.one;
        }
    }

    void OnDestroy()
    {
        // Outline materyalini temizle
        if (outlineMaterial != null)
        {
            Destroy(outlineMaterial);
        }
    }

    void OnValidate()
    {
        // Outline rengini güncelle
        if (outlineMaterial != null)
        {
            outlineMaterial.SetColor("_Color", outlineColor);
        }
    }
} 