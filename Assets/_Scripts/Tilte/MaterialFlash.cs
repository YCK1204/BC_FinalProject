using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFlash : MonoBehaviour
{
    [SerializeField] private List<Renderer> _takgerMat;
    [SerializeField] private Material _material;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    void Awake()
    {
        foreach (var rend in _takgerMat)
        {
            originalMaterials[rend] = rend.materials;
        }
    }

    public void Flash(float duration)
    {
        StartCoroutine(FlashCoroutine(duration));
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        foreach (var rend in _takgerMat)
        {
            Material[] mats = new Material[rend.materials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = _material;
            rend.materials = mats;
        }

        yield return new WaitForSeconds(duration);

        foreach (var rend in _takgerMat)
        {
            rend.materials = originalMaterials[rend];
        }
    }
}
