using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenPiece : ChessPiece
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
                    int curPiece = (this.tag == Constants.WHITE) ? Constants.WHITE_QUEEN : Constants.BLACK_QUEEN;
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
    // Queen --> Can move horizontally, vertically and diagonally without steps restrictions
    // Moves = [(x+n), (x-n), (y+n), (y-n)] & [(x+n, y+n), (x-n, y-n), (x+n, y-n), (x-n, y+n)]
    public override List<int> CurrentMovements() 
    {
        bool userColor = GameState.Instance.GetUserColor(); // Color user is playing
        bool turn = GameState.Instance.GetColorTurn(); // Current turn
        bool curPieceColor = (this.tag == Constants.WHITE) ? true : false; // Color of the current piece
        List<int> friends = getFriendPiecesPositions(curPieceColor); // Positions of the other same color pieces
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
        int moveUp = boardPos;
        for (int i = 1; i <= 7; i++) {
            if (GameState.Instance.squaresTop.Contains(moveUp)) break;
            moveUp = boardPos - (8 * i);

            // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
            if (friends.Contains(moveUp)) break;

            // Save the moves that are possible eat moves for this piece in eat moves list
            bool isEatMove = canBeEaten(moveUp, curPieceColor);
            if (isEatMove){ eatMoves.Add(moveUp); break; }
            else curMoves.Add(moveUp);
        }

        // Normal move down
        int moveDown = boardPos;
        for (int i = 1; i <= 7; i++) {
            if (GameState.Instance.squaresBottom.Contains(moveDown)) break;
            moveDown = boardPos + (8 * i);

            // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
            if (friends.Contains(moveDown)) break;

            // Save the moves that are possible eat moves for this piece in eat moves list
            bool isEatMove = canBeEaten(moveDown, curPieceColor);
            if (isEatMove){ eatMoves.Add(moveDown); break; }
            else curMoves.Add(moveDown);
        }

        // Normal move right
        int moveRight = boardPos;
        for (int i = 1; i <= 7; i++) {
            if (GameState.Instance.squaresRight.Contains(moveRight)) break;
            moveRight = boardPos + i;

            // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
            if (friends.Contains(moveRight)) break;

            // Save the moves that are possible eat moves for this piece in eat moves list
            bool isEatMove = canBeEaten(moveRight, curPieceColor);
            if (isEatMove){ eatMoves.Add(moveRight); break; }
            else curMoves.Add(moveRight);
        }

        // Normal move left
        int moveLeft = boardPos;
        for (int i = 1; i <= 7; i++) {
            if (GameState.Instance.squaresLeft.Contains(moveLeft)) break;
            moveLeft = boardPos - i;

            // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
            if (friends.Contains(moveLeft)) break;

            // Save the moves that are possible eat moves for this piece in eat moves list
            bool isEatMove = canBeEaten(moveLeft, curPieceColor);
            if (isEatMove){ eatMoves.Add(moveLeft); break; }
            else curMoves.Add(moveLeft);
        }

        // Normal move diagonally up-right
        int moveUpRight = boardPos;
        for (int i = 1; i <= 7; i++) {
            if (GameState.Instance.squaresTop.Contains(moveUpRight)) break;
            if (GameState.Instance.squaresRight.Contains(moveUpRight)) break;
            moveUpRight = moveUpRight - 7;

            // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
            if (friends.Contains(moveUpRight)) break;

            // Save the moves that are possible eat moves for this piece in eat moves list
            bool isEatMove = canBeEaten(moveUpRight, curPieceColor);
            if (isEatMove){ eatMoves.Add(moveUpRight); break; }
            else curMoves.Add(moveUpRight);
        }

        // Normal move diagonally up-left
        int moveUpLeft = boardPos;
        for (int i = 1; i <= 7; i++) {
            if (GameState.Instance.squaresTop.Contains(moveUpLeft)) break;
            if (GameState.Instance.squaresLeft.Contains(moveUpLeft)) break;
            moveUpLeft = moveUpLeft - (7 + 2);

            // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
            if (friends.Contains(moveUpLeft)) break;

            // Save the moves that are possible eat moves for this piece in eat moves list
            bool isEatMove = canBeEaten(moveUpLeft, curPieceColor);
            if (isEatMove){ eatMoves.Add(moveUpLeft); break; }
            else curMoves.Add(moveUpLeft);
        }

        // Normal move diagonally down-right
        int moveDownRight = boardPos;
        for (int i = 1; i <= 7; i++) {
            if (GameState.Instance.squaresBottom.Contains(moveDownRight)) break;
            if (GameState.Instance.squaresRight.Contains(moveDownRight)) break;
            moveDownRight = moveDownRight + (7 + 2);

            // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
            if (friends.Contains(moveDownRight)) break;

            // Save the moves that are possible eat moves for this piece in eat moves list
            bool isEatMove = canBeEaten(moveDownRight, curPieceColor);
            if (isEatMove){ eatMoves.Add(moveDownRight); break; }
            else curMoves.Add(moveDownRight);
        }

        // Normal move diagonally down-left
        int moveDownLeft = boardPos;
        for (int i = 1; i <= 7; i++) {
            if (GameState.Instance.squaresBottom.Contains(moveDownLeft)) break;
            if (GameState.Instance.squaresLeft.Contains(moveDownLeft)) break;
            moveDownLeft = moveDownLeft + 7;

            // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
            if (friends.Contains(moveDownLeft)) break;

            // Save the moves that are possible eat moves for this piece in eat moves list
            bool isEatMove = canBeEaten(moveDownLeft, curPieceColor);
            if (isEatMove){ eatMoves.Add(moveDownLeft); break; }
            else curMoves.Add(moveDownLeft);
        }

        return new List<int>(curMoves);
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
