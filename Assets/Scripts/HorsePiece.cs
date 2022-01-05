using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorsePiece : ChessPiece
{
    // Start is called before the first frame update
    void Start()
    {
        moves = new List<int>();
        objectCollider = GetComponent<Collider2D>();
        isDraggable = false;
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        DragAndDrop();
    }

    // Controls drag and drop movement of the piece
    protected override void DragAndDrop()
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
            if (GetMoveCallback() != null) {
                float origin = (float)Math.Round(curX) + (float)Math.Round(curY)*(-1)*8;
                float target = (float)Math.Round(mousePosition.x) + (float)Math.Round(mousePosition.y)*(-1)*8;
                doMove = GetMoveCallback()((int)origin, (int)target);
            }
            
            // Apply movement or return to origin
            if (doMove) ApplyAproxPiecePosition(mousePosition.x, mousePosition.y);
            else comeBack();
        }
    }

    // Returns valid moves of this piece in the current state
    public override List<int> CurrentMovements() 
    {
        return moves;
    }
}
