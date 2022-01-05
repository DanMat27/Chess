using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    private bool isFirstMove = true;

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
        if (moves.Count == 0) {
            moves = CurrentMovements();
        }
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
            if (doMove) { 
                ApplyAproxPiecePosition(mousePosition.x, mousePosition.y);
                moves = new List<int>(); // Reset moves list
                isFirstMove = false;
            }
            else comeBack();
        }
    }

    // Calculates valid moves of this piece in the current state
    // Pawn --> Can move 1 up (normal move) or 1 diagonally up if another piece is there (eat piece)
    //          If can move 2 up too (first move)
    // ** up means down depending on user turn
    // Moves = [(x-9), (x-8), (x-16), (x-7)] or [(x+9), (x+8), (x+16), (x+7)]
    public override List<int> CurrentMovements() 
    {
        bool userColor = boardState.GetComponent<GameState>().GetUserColor(); // Color user is playing
        bool turn = boardState.GetComponent<GameState>().GetColorTurn(); // Current turn
        bool curPieceColor = false; // Color of the current piece
        if (this.tag == "White") curPieceColor = true;

        List<int> curMoves = new List<int>();
        // If white turn and piece is white
        if (curPieceColor && turn) {
            return curMoves;
        }
        // If black turn and piece is black
        if (!curPieceColor && !turn) {
            return curMoves;
        }
        
        return moves;
    }
}
