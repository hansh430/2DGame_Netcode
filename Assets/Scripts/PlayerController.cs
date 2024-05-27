using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxX;
    private float clickedScreenX;
    private float clickedPlayerX;
    private void Update()
    {
        ManageInput();
    }
    private void ManageInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clickedScreenX = Input.mousePosition.x;
            clickedPlayerX = transform.position.x;
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = Input.mousePosition.x - clickedScreenX;
            deltaX/=Screen.width;
            deltaX *= moveSpeed;
            float newXPosition = clickedPlayerX+ deltaX;
            newXPosition = Mathf.Clamp(newXPosition, -maxX, maxX);
            transform.position=new Vector2(newXPosition,transform.position.y);
        }
    }
}
