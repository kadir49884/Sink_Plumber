using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class StarterEvent : MonoBehaviour, IPointerDownHandler
{

    public UnityEvent swipe;

    private GameManager gameManager;
    
    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public bool isPressed;

    public void OnPointerDown(PointerEventData data)
    {
        isPressed = true;
        if (isPressed && !gameManager.RunGame)
        {
            gameManager.Initialize();
            gameManager.GameStart();
            transform.gameObject.SetActive(false);
        }
    }

}

