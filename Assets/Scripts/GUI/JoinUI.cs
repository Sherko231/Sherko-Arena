using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class JoinUI : MonoBehaviour
{
    public static JoinUI Instance { get; private set; }
    
    private VisualElement _mainUI;
    private TextField _joinCodeField;
    private Button _joinBtn;
    private Button _HostBtn;
    private DropdownField _characterSelect;

    private string _joinCode;

    private void Awake()
    {
        _mainUI = GetComponent<UIDocument>().rootVisualElement;
        _joinCodeField = _mainUI.Q<TextField>("join-code");
        _joinBtn = _mainUI.Q<Button>("join-btn");
        _HostBtn = _mainUI.Q<Button>("host-btn");
        _characterSelect = _mainUI.Q<DropdownField>("character-select");
        PopulateDropdown();
        Instance = this;
        _joinBtn.clicked += Join;
        _HostBtn.clicked += Host;
    }

    private void OnDisable()
    {
        _joinBtn.clicked -= Join;
        _HostBtn.clicked -= Host;
    }

    private void ConfirmSelection()
    {
        CharacterSelector.Instance.Selection = Enum.Parse<CharacterType>(_characterSelect.value);
    }

    private void PopulateDropdown()
    {
        _characterSelect.value = _characterSelect.choices[0];
        foreach (string charType in Enum.GetNames(typeof(CharacterType)))
        {
            _characterSelect.choices.Add(charType);
        }
    }
    
    private void Join()
    {
        ConfirmSelection();
        _joinCode = _joinCodeField.text;
        if (_joinCode == "XXXXXX" || _joinCode.Length != 6) return;
        GameRelay.Instance.JoinRelay(_joinCode);
        _mainUI.style.display = DisplayStyle.None;
    }

    private void Host()
    {
        ConfirmSelection();
        GameRelay.Instance.CreateRelay();
        _mainUI.style.display = DisplayStyle.None;
    }
}
