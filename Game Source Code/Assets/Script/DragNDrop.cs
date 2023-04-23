using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DragNDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;


    private RectTransform some;
    private Collider2D _collider;
    private Vector2 OriginalPos;
    private bool coll = false;

    private void Awake()
    {
        some = GetComponent<RectTransform>();
        _collider = GetComponent<Collider2D>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OriginalPos = some.anchoredPosition;
        coll = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        some.anchoredPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log(some.anchoredPosition);
        float resultingX = (float)Math.Round(some.anchoredPosition.x);
        float resultingY = (float)Math.Round(some.anchoredPosition.y);
        some.anchoredPosition = new Vector2(resultingX, resultingY);
        if (coll)
        {
            some.anchoredPosition = OriginalPos;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        coll = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        coll = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
