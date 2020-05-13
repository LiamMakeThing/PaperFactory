using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColorByPosition : MonoBehaviour
{
    Color newColor;
    [SerializeField] Color baseColor;
    public float speed = 1;
    public float offset;
    [SerializeField, Range(0,256)] float colorMin;
    [SerializeField, Range(0, 256)] float colorMax;
    [SerializeField, Range(-1, 1)] float colorVariantMin;
    [SerializeField, Range(-1, 1)] float colorVariantMax;
    float colorVariant;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    private void Awake()
    {
        colorVariant = Random.Range(colorVariantMin, colorVariantMax);
        newColor = new Color(colorVariant, colorVariant, colorVariant, 1.0f);




    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {

        
        
        if (_propBlock == null)
        {
            _propBlock = new MaterialPropertyBlock();
        }
        _renderer = GetComponent<Renderer>();
        _propBlock.SetColor("_Color",newColor);
        _renderer.SetPropertyBlock(_propBlock);
    }
    // Update is called once per frame
  
}
