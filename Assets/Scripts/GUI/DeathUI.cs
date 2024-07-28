using Sherko.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DeathUI : MonoBehaviour
{
    public static DeathUI Instance;

    private VisualElement _deathUI;
    private Button _respawnBtn;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _deathUI = root.Q<VisualElement>("deathUI");
        _respawnBtn = root.Q<Button>("respawnBtn");
        _respawnBtn.clicked += OnRespawnBtnClicked;
        Instance = this;
    }

    private void OnRespawnBtnClicked()
    {
        Player.OwnerSingleton.Respawn();
        Hide();
    }

    [ContextMenu("Show")]
    public void Show()
    {
        _deathUI.style.display = DisplayStyle.Flex;
        Player.OwnerSingleton.Controller.ControlsDisabled = true;
        SherkoUtils.ToggleCursor(true);
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        _deathUI.style.display = DisplayStyle.None;
        Player.OwnerSingleton.Controller.ControlsDisabled = false;
        SherkoUtils.ToggleCursor(false);
    }
}
