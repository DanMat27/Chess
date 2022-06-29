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

            // Check if piece can move to the new position
            if (moves.Count == 0) moves = CurrentMovements();
            bool doMove = false;
            float origin = (float)Math.Round(curX) + (float)Math.Round(curY)*(-1)*8;
            float target = (float)Math.Round(mousePosition.x) + (float)Math.Round(mousePosition.y)*(-1)*8;
            if (moves.Contains((int)target)) doMove = true;
            moves.ForEach(p => print(p));
            
            // Apply movement or return to origin
            if (doMove) { 
                // Send move to game manager to save it
                if (GetMoveCallback() != null) {
                    bool curPieceColor = this.tag == WHITE;
                    GetMoveCallback()((int)origin, (int)target, curPieceColor);
                }

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
        if (this.tag == WHITE) curPieceColor = true;
        print(turn ? "WHITE" : "BLACK");
        List<int> curMoves = new List<int>();
        // If white turn and piece is black
        if (!curPieceColor && turn) {
            return curMoves;
        }
        // If black turn and piece is white
        if (curPieceColor && !turn) {
            return curMoves;
        }

        // Calculate the current valid moves
        if (curPieceColor) { // White
            // Normal move
            if (!squaresBottom.Contains(boardPos)) {
                int move1Down = boardPos + 8;
                curMoves.Add(move1Down);

                // If first move, can move 2 up
                if (isFirstMove) {
                    int move2Down = boardPos + 16;
                    curMoves.Add(move2Down);
                }
            }
        }
        else { // Black
            // Normal move
            if (!squaresTop.Contains(boardPos)) {
                int move1Up = boardPos - 8;
                curMoves.Add(move1Up);

                // If first move, can move 2 up
                if (isFirstMove) {
                    int move2Up = boardPos - 16;
                    curMoves.Add(move2Up);
                }
            }
        }
        
        return curMoves;
    }
}
