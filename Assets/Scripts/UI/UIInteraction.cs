using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInteraction : MonoBehaviour
{
    [SerializeField] Button addVerticeButton;
    public EventHandler onVerticeButtonClicked;

    private void Start()
    {
        addVerticeButton.onClick.AddListener(OnVerticeButtonClick);
    }

    void OnVerticeButtonClick()
    {
        onVerticeButtonClicked?.Invoke(this, EventArgs.Empty);
    }
}
