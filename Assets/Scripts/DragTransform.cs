using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTransform : MonoBehaviour
{
    Collider2D objectCollider;
    private bool isDraggable; // true = piece can be dragged
    private bool isDragging; // true = piece is being dragged
    private float curX; // Current x position (before applying movement)
    private float curY; // Current y position (before applying movement)
    
    /* Board limits */
    public float boundX1 = (float)-0.5;
    public float boundX2 = (float)7.5;
    public float boundY1 = (float)-7.5;
    public float boundY2 = (float)0.5;

    /* Sounds */
    private AudioSource audioSource;
    public AudioClip soundClip;


    // Start is called before the first frame update
    void Start()
    {
        objectCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = soundClip;
        isDraggable = false;
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        DragAndDrop();
    }

    // Controls drag and drop movement of the piece
    private void DragAndDrop()
    {
        // Take mouse cursor position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // When user drags the piece
        if (Input.GetMouseButtonDown(0) && !isDragging) {
            if(objectCollider == Physics2D.OverlapPoint(mousePosition)) {
                isDraggable = true;
            }
            else {
                isDraggable = false;
            }

            if (isDraggable) {
                isDragging = true;
            }
        }

        // Apply live movement to the dragged piece
        if (isDragging) {
            this.transform.position = mousePosition;
        }

        // When user drops the piece
        if (Input.GetMouseButtonUp(0) && isDragging) {
            isDraggable = false;
            isDragging = false;

            // If piece outside of board limits, apply position before movement
            if (outOfBounds(mousePosition.x, mousePosition.y)) {
                comeBack();
                return;
            }

            // Send move to game manager and wait response
            bool doMove = false;
            if (GetComponent<ChessPiece>().GetMoveCallback() != null) {
                float origin = (float)Math.Round(curX) + (float)Math.Round(curY)*(-1)*8;
                float target = (float)Math.Round(mousePosition.x) + (float)Math.Round(mousePosition.y)*(-1)*8;
                doMove = GetComponent<ChessPiece>().GetMoveCallback()((int)origin, (int)target);
            }
            
            // Apply movement or return to origin
            if (doMove) ApplyAproxPiecePosition(mousePosition.x, mousePosition.y);
            else comeBack();
        }
    }

    // Returns piece to position before the move
    private void comeBack()
    {
        this.transform.position = new Vector2(curX, curY);
    }

    // Returns true if position is out of board bounds
    private bool outOfBounds(float x, float y)
    {
        if ((x < boundX1 || x > boundX2) || (y < boundY1 || y > boundY2)) {
            this.transform.position = new Vector2(curX, curY);
            return true;
        }
        return false;
    }

    // Applies new position coords doing an approximation to center the piece
    private void ApplyAproxPiecePosition(float x, float y)
    {
        curX = (float)Math.Round(x);
        curY = (float)Math.Round(y);
        this.transform.position = new Vector2(curX, curY);
        PlaySound(audioSource);
    }

    /* Cambia la pieza de sonido del AudioSource */
    private void PlaySound(AudioSource audioSource)
    {   
        audioSource.Stop();
        audioSource.Play();
    }

    // Changes current position of piece externally
    public void SetCurPosition(float x, float y)
    {
        if (outOfBounds(x, y)) return;
        curX = x;
        curY = y;
    }
}