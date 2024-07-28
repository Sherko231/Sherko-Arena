using UnityEngine;

public class GunLinker : MonoBehaviour
{
    [SerializeField] private float lerpTimeMultiplier = 40f;
    private Transform _parentTrans;
    
    public Transform Head { get; set; }

    public void Init()
    {
        _parentTrans = transform.parent;
        transform.parent = null;
    }
    
    private void LateUpdate()
    {
        if (_parentTrans == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.Lerp(transform.position, _parentTrans.position, Time.deltaTime * lerpTimeMultiplier);
        transform.rotation = Quaternion.Lerp(transform.rotation, Head.transform.rotation, Time.deltaTime * lerpTimeMultiplier);
    }
}
