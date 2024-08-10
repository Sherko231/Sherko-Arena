using System.Collections;
using System.Collections.Generic;
using Sherko.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class GrappleGun : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private LineRenderer grappleRenderer;
    [Title("Settings")]
    [SerializeField] private float grappleResetRadius = 5f;
    [SerializeField] private float grappleSpeed = 50f;
    [SerializeField, SuffixLabel("Sec"), MinValue(0)] private float grappleMaxTime = 2f;

    private readonly List<Coroutine> r_grappleTimerCoroutines = new();
    
    private Vector3 _grappleHit = Vector3.zero;
    private Coroutine _previousPullCoroutine;
    private Player _player;
    private bool _canGrapple;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }
    
    public void Shoot()
    {
        bool isHit = Physics.Raycast(_player.CamPoint.position, _player.LookingVec, out RaycastHit hit, 120f);
        _grappleHit = isHit ? hit.point : Vector3.zero;
        if (!isHit) return;
        
        Coroutine timerCoroutine = _player.StartCoroutine(StartGrappleTimer());
        r_grappleTimerCoroutines.Add(timerCoroutine);
        
        if (_previousPullCoroutine != null) _player.StopCoroutine(_previousPullCoroutine);
        _previousPullCoroutine = _player.StartCoroutine(PullTowardsGrappleHit(hit.point));
    }

    private void LateUpdate()
    {
        if (_grappleHit != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, _grappleHit) <= grappleResetRadius) _grappleHit = Vector3.zero;
        }
        
        grappleRenderer.SetPosition(0, transform.position);
        grappleRenderer.SetPosition(1, _grappleHit == Vector3.zero ? transform.position : _grappleHit);
    }

    private IEnumerator PullTowardsGrappleHit(Vector3 hit)
    {
        _player.Controller.ControlsDisabled = true;
        
        while (SherkoUtils.SquaredDistance(hit, transform.position) > Mathf.Pow(grappleResetRadius, 2) && _canGrapple)
        {
            _player.Rb.AddForce((hit - transform.position).normalized * (grappleSpeed * 100 * Time.deltaTime), ForceMode.VelocityChange);
            yield return new WaitForEndOfFrame();
        }

        StopAllGrappleTimersCoroutines();
        _canGrapple = false;
        _player.Controller.ControlsDisabled = false;
    }

    private IEnumerator StartGrappleTimer()
    {
        _canGrapple = true;
        yield return new WaitForSeconds(grappleMaxTime);
        _canGrapple = false;
        _grappleHit = Vector3.zero;
    }

    private void StopAllGrappleTimersCoroutines()
    {
        foreach (Coroutine coroutine in r_grappleTimerCoroutines) _player.StopCoroutine(coroutine);
        r_grappleTimerCoroutines.Clear();
    }
}
