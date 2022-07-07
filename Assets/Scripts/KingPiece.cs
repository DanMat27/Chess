using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPiece : ChessPiece
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
            if (GetShowMovesCallback() != null) GetShowMovesCallback()(moves, eatMoves, boardPos);
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
                    int curPiece = (this.tag == Constants.WHITE) ? Constants.WHITE_KING : Constants.BLACK_KING;
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
    // King --> Can move horizontally, vertically and diagonally without steps restrictions
    // Moves = [(x+1), (x-1), (y+1), (y-1)] & [(x+1, y+1), (x-1, y-1), (x+1, y-1), (x-1, y+1)]
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

        // Normal move up
        int moveUp = boardPos - 8;
        if (!GameState.Instance.squaresTop.Contains(boardPos)) curMoves.Add(moveUp);

        // Normal move down
        int moveDown = boardPos + 8;
        if (!GameState.Instance.squaresBottom.Contains(boardPos)) curMoves.Add(moveDown);

        // Normal move right
        int moveRight = boardPos + 1;
        if (!GameState.Instance.squaresRight.Contains(boardPos)) curMoves.Add(moveRight);

        // Normal move left
        int moveLeft = boardPos - 1;
        if (!GameState.Instance.squaresLeft.Contains(boardPos)) curMoves.Add(moveLeft);

        // Normal move diagonally up-right
        int moveUpRight = boardPos - 7;
        if (!GameState.Instance.squaresTop.Contains(boardPos) && !GameState.Instance.squaresRight.Contains(boardPos)) curMoves.Add(moveUpRight);

        // Normal move diagonally up-left
        int moveUpLeft = boardPos - (7 + 2);
        if (!GameState.Instance.squaresTop.Contains(boardPos) && !GameState.Instance.squaresLeft.Contains(boardPos)) curMoves.Add(moveUpLeft);

        // Normal move diagonally down-right
        int moveDownRight = boardPos + (7 + 2);
        if (!GameState.Instance.squaresBottom.Contains(boardPos) && !GameState.Instance.squaresRight.Contains(boardPos)) curMoves.Add(moveDownRight);

        // Normal move diagonally down-left
        int moveDownLeft = boardPos + 7;
        if (!GameState.Instance.squaresBottom.Contains(boardPos) && !GameState.Instance.squaresLeft.Contains(boardPos)) curMoves.Add(moveDownLeft);

        // Remove the invalid moves because of squares occupied by pieces of the same color
        List<int> possibleMoves = removeFriendMoves(new List<int>(curMoves), curPieceColor);

        // Remove the moves that are possible eat moves for this piece and save them in eat moves list
        List<int> finalMoves = new List<int>();
        foreach (int move in possibleMoves) {
            bool isEatMove = canBeEaten(move, curPieceColor);
            if (isEatMove) eatMoves.Add(move);
            else finalMoves.Add(move);
        }
        
        return finalMoves;
    }

    // Check if the square to move has an enemy piece that can be eaten
    protected override bool canBeEaten(int move, bool curPieceColor)
    {
        List<int> curBoard = GameState.Instance.GetCurState();

        if (curBoard[move] == Constants.EMPTY) return false;

        bool moveColor = curBoard[move] % 2 == 0 ? false : true;
        if (moveColor != curPieceColor) return true;

        return false;
    }
}
