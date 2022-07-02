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

            // Calculate the possible moves when dragged
            if (!movesCalculated) {
                moves = CurrentMovements();
                movesCalculated = true;
            }

            // Show the possible moves of this piece on the board when dragging it
            float curPos = (float)Math.Round(curX) + (float)Math.Round(curY)*(-1)*8;
            GetShowMovesCallback()(moves, (int)curPos);
        }

        // When user drops the piece
        if (Input.GetMouseButtonUp(0) && isDragging) {
            isDraggable = false;
            isDragging = false;
            
            // If piece outside of board limits, apply position before movement
            if (outOfBounds(mousePosition.x, mousePosition.y)) {
                comeBack();
                cleanMoves();
                return;
            }

            // Check if piece can move to the new position
            bool doMove = false;
            float origin = (float)Math.Round(curX) + (float)Math.Round(curY)*(-1)*8;
            float target = (float)Math.Round(mousePosition.x) + (float)Math.Round(mousePosition.y)*(-1)*8;
            if (moves.Contains((int)target)) doMove = true;
            moves.ForEach(p => print(p));
            
            // Apply movement or return to origin
            if (doMove) { 
                // Send move to game manager to save it
                if (GetMoveCallback() != null) {
                    int curPiece = (this.tag == Constants.WHITE) ? Constants.WHITE_PAWN : Constants.BLACK_PAWN;
                    GetMoveCallback()((int)origin, (int)target, curPiece);
                }

                ApplyAproxPiecePosition(mousePosition.x, mousePosition.y);
                isFirstMove = false;
            }
            else comeBack();

            // Reset moves
            cleanMoves();
        }
    }

    // Calculates valid moves of this piece in the current state
    // Pawn --> Can move 1 up (normal move) or 1 diagonally up if another piece is there (eat piece)
    //          If can move 2 up too (first move)
    // ** up means down depending on user turn
    // Moves = [(x-9), (x-8), (x-16), (x-7)] or [(x+9), (x+8), (x+16), (x+7)]
    public override List<int> CurrentMovements() 
    {
        bool userColor = GameState.Instance.GetUserColor(); // Color user is playing
        bool turn = GameState.Instance.GetColorTurn(); // Current turn
        bool curPieceColor = (this.tag == Constants.WHITE) ? true : false; // Color of the current piece
        print(turn ? "TURN WHITE" : "TURN BLACK");

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

    // Reset calculated moves list and board
    private void cleanMoves() {
        // Reset moves list
        GetCleanMovesCallback()();
        moves = new List<int>(); 
        movesCalculated = false;
    }
}
