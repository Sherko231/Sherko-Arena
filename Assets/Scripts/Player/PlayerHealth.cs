using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text playerHealthTmp;
    [SerializeField] private ParticleSystem deathVFX;
    private Player _player;

    [Header("Properties")]
    [SerializeField] private List<GameObject> disabledObjectsOnDeath;
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth = 100f;

    public event Action OnHealthUpdate;

    public List<GameObject> DisabledObjectsOnDeath => disabledObjectsOnDeath;
    public float MaxHealth => maxHealth;
    public bool IsAlive { get; private set; } = true;
    public bool Damagable { get; set; }

    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            UpdateSlider();
            OnHealthUpdate?.Invoke();
        }
    }

    private void Awake()
    {
        _player = GetComponent<Player>();
    }
    
    public void Die()
    {
        IsAlive = false;
        if (_player.Network.IsOwner) DeathUI.Instance.Show();
        _player.PlayerAbilities.TerminateAll();
        deathVFX.Play();
        ToggleAppearance(false);
    }

    public void Heal(float heal)
    {
        CurrentHealth += heal;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
            _player.RpcManager.SendDeathConfirmClientRpc(_player.Network.OwnerClientId);
        }
    }
    
    public void TakeDamage(float damage, Vector3 bulletDir)
    {
        if (!IsAlive) return;
        CurrentHealth -= damage;
        float dot = CalculateIncomingShootDot(bulletDir);
        float cross = CalculateIncomingShootCross(bulletDir);
        if (_player.Network.IsOwner) TriggerIndicator(dot, cross);
        if (CurrentHealth <= 0)
        {
            Die();
            _player.RpcManager.SendDeathConfirmClientRpc(_player.Network.OwnerClientId);
        }
    }

    private float CalculateIncomingShootDot(Vector3 bulletDir)
    {
        Vector3 meshFwd = _player.Mesh.transform.forward; //because LookingVec is not synchronized in network
        Vector2 lookVec2D = new(meshFwd.x, meshFwd.z);
        Vector2 bulletDir2D = new(bulletDir.x, bulletDir.z);
        
        float dotProduct = Vector2.Dot(lookVec2D.normalized, bulletDir2D.normalized);
        return dotProduct;
    }
    
    private float CalculateIncomingShootCross(Vector3 bulletDir)
    {
        Vector3 meshFwd = _player.Mesh.transform.forward; //because LookingVec is not synchronized in network
        Vector2 lookVec2D = new(meshFwd.x, meshFwd.z);
        Vector2 bulletDir2D = new(bulletDir.x, bulletDir.z);
        
        float crossProduct = lookVec2D.x * bulletDir2D.y - lookVec2D.y * bulletDir2D.x;
        return crossProduct;
    }

    private void TriggerIndicator(float dotProduct, float crossProduct)
    {
        if (dotProduct < 0) //fwd
        {
            if (crossProduct > 0) DmgIndManager.Instance.UpperRightDI.Trigger();
            else DmgIndManager.Instance.UpperLeftDI.Trigger();
        }
        else
        {
            if (crossProduct > 0) DmgIndManager.Instance.BottomRightDI.Trigger();
            else DmgIndManager.Instance.BottomLeftDI.Trigger();
        }
    }
    
    public void Fill()
    {
        CurrentHealth = maxHealth;
        ToggleAppearance(true);
        IsAlive = true;
    }

    private void UpdateSlider()
    {
        playerHealthSlider.value = _currentHealth / MaxHealth;
        playerHealthTmp.text = _currentHealth + "";
    }

    private void ToggleAppearance(bool appeared)
    {
        foreach (GameObject obj in disabledObjectsOnDeath) obj.SetActive(appeared);
    }
}
