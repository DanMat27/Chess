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
    // Contains the logic for the moves control and action
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
                    int curPiece = (this.tag == Constants.WHITE) ? Constants.WHITE_PAWN : Constants.BLACK_PAWN;
                    GetMoveCallback()((int)origin, (int)target, curPiece);
                }

                // If moves two steps, can be eaten en passant by enemy pawn next turn
                if (isDoubleMove((int)target)) GameState.Instance.SetPassant((int)target, (this.tag == Constants.WHITE));
                else GameState.Instance.SetPassant(-1);

                ApplyAproxPiecePosition(mousePosition.x, mousePosition.y);
                boardPos = (int)target;
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
    // Moves = [(y-1), (y-2), (x+1, y-1), (x-1, y-1)] or [(y+1), (y+2), (x+1, y+1), (x-1, y+1)]
    public override List<int> CurrentMovements() 
    {
        bool userColor = GameState.Instance.GetUserColor(); // Color user is playing
        bool turn = GameState.Instance.GetColorTurn(); // Current turn
        bool curPieceColor = (this.tag == Constants.WHITE) ? true : false; // Color of the current piece
        List<int> friends = getFriendPiecesPositions(curPieceColor); // Positions of the other same color pieces
        List<int> enemies = getEnemyPiecesPositions(curPieceColor); // Positions of the opposite color pieces
        List<int> curMoves = new List<int>();
        bool isLeft = false; // Current pos in the left limit
        bool isRight = false; // Current pos in the right limit
        bool notPassantLeft = false; // Can not eat en passant left
        bool notPassantRight = false; // Can not eat en passant right

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
            if (!GameState.Instance.squaresBottom.Contains(boardPos)) {
                // Normal move
                int move1Down = boardPos + 8;

                // Eat moves
                int move1LeftDown = boardPos + 7;
                int move1RightDown = boardPos + 9;
                if (!GameState.Instance.squaresLeft.Contains(boardPos)) {
                    // If eat moves or invalid moves
                    if (!friends.Contains(move1LeftDown)) {
                        bool isEatMove = canBeEaten(move1LeftDown, curPieceColor);
                        if (isEatMove) { eatMoves.Add(move1LeftDown); notPassantLeft = true; }
                    }
                }
                else isLeft = true;
                if (!GameState.Instance.squaresRight.Contains(boardPos)) { 
                    if (!friends.Contains(move1RightDown)) {
                        bool isEatMove = canBeEaten(move1RightDown, curPieceColor);
                        if (isEatMove) { eatMoves.Add(move1RightDown); notPassantRight = true; }
                    }
                }
                else isRight = true;
                
                // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
                if (friends.Contains(move1Down)) return curMoves;
                // If invalid move (square occupied by a piece of the opposite color), stop to cut the lane of movement of this piece
                if (enemies.Contains(move1Down)) return curMoves;

                curMoves.Add(move1Down);

                // If first move, can move 2 up
                if (isFirstMove) {
                    int move2Down = boardPos + 16;
                    if (!friends.Contains(move2Down) && !enemies.Contains(move2Down)) curMoves.Add(move2Down);
                }

                // En passant moves
                int lastPassant = GameState.Instance.GetPassantPosition();
                bool lastPassantColor = GameState.Instance.GetPassantColor();
                if (lastPassant != -1 && lastPassantColor != curPieceColor) {
                    if (!isLeft) {
                        int enPassantLeft = boardPos - 1;
                        if (enemies.Contains(enPassantLeft)) {
                            if (!notPassantLeft) eatMoves.Add(move1LeftDown);
                        }
                    }
                    if (!isRight) {
                        int enPassantRight = boardPos + 1;
                        if (enemies.Contains(enPassantRight)) {
                            if (!notPassantRight) eatMoves.Add(move1RightDown);
                        }
                    }
                }
            }
        }
        else { // Black
            if (!GameState.Instance.squaresTop.Contains(boardPos)) {
                // Normal move
                int move1Up = boardPos - 8;

                // Eat moves
                int move1LeftUp = boardPos - 9;
                int move1RightUp = boardPos - 7;
                if (!GameState.Instance.squaresLeft.Contains(boardPos)) {
                    // If eat moves or invalid moves
                    if (!friends.Contains(move1LeftUp)) {
                        bool isEatMove = canBeEaten(move1LeftUp, curPieceColor);
                        if (isEatMove) { eatMoves.Add(move1LeftUp); notPassantLeft = true; }
                    }
                }
                else isLeft = true;
                if (!GameState.Instance.squaresRight.Contains(boardPos)) { 
                    if (!friends.Contains(move1RightUp)) {
                        bool isEatMove = canBeEaten(move1RightUp, curPieceColor);
                        if (isEatMove) { eatMoves.Add(move1RightUp); notPassantRight = true; }
                    }
                }
                else isRight = true;

                // If invalid move (square occupied by a piece of the same color), stop to cut the lane of movement of this piece
                if (friends.Contains(move1Up)) return curMoves;
                // If invalid move (square occupied by a piece of the opposite color), stop to cut the lane of movement of this piece
                if (enemies.Contains(move1Up)) return curMoves;

                curMoves.Add(move1Up);

                // If first move, can move 2 up
                if (isFirstMove) {
                    int move2Up = boardPos - 16;
                    if (!friends.Contains(move2Up) && !enemies.Contains(move2Up)) curMoves.Add(move2Up);
                }

                // En passant moves
                int lastPassant = GameState.Instance.GetPassantPosition();
                bool lastPassantColor = GameState.Instance.GetPassantColor();
                if (lastPassant != -1 && lastPassantColor != curPieceColor) {
                    if (!isLeft) {
                        int enPassantLeft = boardPos - 1;
                        if (enemies.Contains(enPassantLeft)) {
                            if (!notPassantLeft) eatMoves.Add(move1LeftUp);
                        }
                    }
                    if (!isRight) {
                        int enPassantRight = boardPos + 1;
                        if (enemies.Contains(enPassantRight)) {
                            if (!notPassantRight) eatMoves.Add(move1RightUp);
                        }
                    }
                }
            }
        }
        
        return curMoves;
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

    // Tells if a pawn move is an initial double step move
    private bool isDoubleMove(int move)
    {
        if (move == boardPos + 16 || move == boardPos - 16) return true;
        return false;
    }
}
