using System;
using Cinemachine;
using UnityEngine;

public class OverlayCamFovSyncer : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera target;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        _camera.fieldOfView = target.m_Lens.FieldOfView;
    }
}
