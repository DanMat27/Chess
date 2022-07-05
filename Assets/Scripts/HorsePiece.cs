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

            // Calculate the possible moves when dragged
            if (!movesCalculated) {
                moves = CurrentMovements();
                movesCalculated = true;
            }

            // Show the possible moves of this piece on the board when dragging it
            float curPos = (float)Math.Round(curX) + (float)Math.Round(curY)*(-1)*8;
            if (GetShowMovesCallback() != null) GetShowMovesCallback()(moves, (int)curPos);
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
            
            // Apply movement or return to origin
            if (doMove) { 
                // Send move to game manager to save it
                if (GetMoveCallback() != null) {
                    int curPiece = (this.tag == Constants.WHITE) ? Constants.WHITE_HORSE : Constants.BLACK_HORSE;
                    GetMoveCallback()((int)origin, (int)target, curPiece);
                }

                ApplyAproxPiecePosition(mousePosition.x, mousePosition.y);
                boardPos = (int)target;
            }
            else comeBack();

            // Reset moves
            cleanMoves();
        }
    }

    // Returns valid moves of this piece in the current state
    // Horse --> Can move in L shape any direction just 3 steps away
    // Moves = [(x+1, y+2), (x-1, y+2), (x+2, y+1), (x+2, y-1), (x-2, y+1), (x-2, y-1), (x+1, y-2), (x-1, y-2)]
    public override List<int> CurrentMovements() 
    {
        bool userColor = GameState.Instance.GetUserColor(); // Color user is playing
        bool turn = GameState.Instance.GetColorTurn(); // Current turn
        bool curPieceColor = (this.tag == Constants.WHITE) ? true : false; // Color of the current piece
        HashSet<int> curMoves = new HashSet<int>();

        // If white turn and piece is black
        if (!curPieceColor && turn) {
            return new List<int>(curMoves);
        }
        // If black turn and piece is white
        if (curPieceColor && !turn) {
            return new List<int>(curMoves);
        }

        // Normal moves L up-left
        int moveUpLeft = boardPos - (8 * 2 + 1);
        if (moveUpLeft > minPos && Math.Abs(boardPos % 8 - moveUpLeft % 8) <= 1) curMoves.Add(moveUpLeft);
        moveUpLeft = boardPos - (2 + 8);
        if (moveUpLeft > minPos && Math.Abs(boardPos % 8 - moveUpLeft % 8) <= 2) curMoves.Add(moveUpLeft);

        // Normal moves L up-right
        int moveUpRight = boardPos - (8 * 2 - 1);
        if (moveUpRight > minPos && Math.Abs(boardPos % 8 - moveUpRight % 8) <= 1) curMoves.Add(moveUpRight);
        moveUpRight = boardPos - (8 - 2);
        if (moveUpRight > minPos && Math.Abs(boardPos % 8 - moveUpRight % 8) <= 2) curMoves.Add(moveUpRight);

        // Normal moves L down-left
        int moveDownLeft = boardPos + (8 * 2 - 1);
        if (moveDownLeft < maxPos && Math.Abs(boardPos % 8 - moveDownLeft % 8) <= 1) curMoves.Add(moveDownLeft);
        moveDownLeft = boardPos + (8 - 2);
        if (moveDownLeft < maxPos && Math.Abs(boardPos % 8 - moveDownLeft % 8) <= 2) curMoves.Add(moveDownLeft);
        
        // Normal moves L down-right
        int moveDownRight = boardPos + (8 * 2 + 1);
        if (moveDownRight < maxPos && Math.Abs(boardPos % 8 - moveDownRight % 8) <= 1) curMoves.Add(moveDownRight);
        moveDownRight = boardPos + (2 + 8);
        if (moveDownRight < maxPos && Math.Abs(boardPos % 8 - moveDownRight % 8) <= 2) curMoves.Add(moveDownRight);

        // Remove the invalid moves because of squares occupied by pieces of the same color
        List<int> possibleMoves = removeFriendMoves(new List<int>(curMoves), curPieceColor);
        
        return possibleMoves;
    }
}
