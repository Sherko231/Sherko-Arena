using System.Collections;
using Sherko.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [field: SerializeField, EnumToggleButtons] 
    public DmgIndPosition DmgIndPosition { get; private set; } = DmgIndPosition.UpperRight;
    [SerializeField] private float targetOpacity = 0.8f;
    [SerializeField] [Range(1, 20f)] private float speed = 0.1f;

    private ValueTransferer _opacityValueTransferer;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _opacityValueTransferer = new ValueTransferer(0, speed);
    }

    [Button]
    public void Trigger()
    {
        StopAllCoroutines();
        StartCoroutine(ShowImageSmoothly());
    }

    private IEnumerator ShowImageSmoothly()
    {
        
        while (Mathf.Abs(_image.color.a - targetOpacity) > 0.1f)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b,
                _opacityValueTransferer.SmoothTransfer(targetOpacity));
            yield return new WaitForEndOfFrame();
        }
        
        StartCoroutine(HideImageSmoothly());
    }

    private IEnumerator HideImageSmoothly()
    {
        while (_image.color.a > 0.05)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b,
                _opacityValueTransferer.SmoothReset());
            yield return new WaitForEndOfFrame();
        }
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
    }
    
    
}
