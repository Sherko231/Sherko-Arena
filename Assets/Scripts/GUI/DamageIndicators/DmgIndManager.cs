using System;
using UnityEngine;

public class DmgIndManager : MonoBehaviour
{
    public static DmgIndManager Instance { get; private set; }
    
    [SerializeField] private DamageIndicator bottomRightDI, bottomLeftDI, upperRightDI, upperLeftDI;

    public DamageIndicator BottomRightDI => bottomRightDI;
    public DamageIndicator BottomLeftDI => bottomLeftDI;
    public DamageIndicator UpperRightDI => upperRightDI;
    public DamageIndicator UpperLeftDI => upperLeftDI;

    private void Awake() => Instance = this;
}
