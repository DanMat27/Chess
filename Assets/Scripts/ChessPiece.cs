using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract public class ChessPiece : MonoBehaviour
{
    /* Piece atributes */
    protected bool eaten = false; // true = can't move anymore
    protected int boardPos; // Current board position

    /* Move event */
    public delegate void MoveCallback (int origin, int target, int piece, List<int> pos_moves, List<int> eat_moves); // Signature of move event (5 args)
    public delegate void ShowMovesCallback (List<int> pos_moves, List<int> eat_moves, int piecePos); // Signature of show board moves event (3 args)
    public delegate void CleanMovesCallback (); // Signature of show board moves event (no args)
    protected event MoveCallback onUserMove; // Move callback
    protected event ShowMovesCallback onShowMoves; // Show board moves callback
    protected event CleanMovesCallback onCleanMoves; // Show board moves callback
    protected List<int> moves; // List of valid moves
    protected List<int> eatMoves = new List<int>(); // List of eat moves
    protected bool movesCalculated = false; // Flag to know if the moves have been calculated
    protected bool moving = false; // Flag to know if the piece is moving
    protected Vector2 curPosition; // Speed to the movement of the piece

    /* Board limits */
    protected int minPos = 0;
    protected int maxPos = 63;
    protected float boundX1 = (float)-0.5;
    protected float boundX2 = (float)7.5;
    protected float boundY1 = (float)-7.5;
    protected float boundY2 = (float)0.5;

    /* Drag&Drop atributes */
    protected Collider2D objectCollider;
    protected bool isDraggable; // true = piece can be dragged
    protected bool isDragging; // true = piece is being dragged
    protected float curX; // Current x position (before applying movement)
    protected float curY; // Current y position (before applying movement)
    protected float toX; // Target x position (after applying movement)
    protected float toY; // Target y position (after applying movement)
    protected float speed = 15f; // Speed to the movement of the piece
    
    /* Sounds */
    protected AudioSource audioSource;
    protected AudioClip soundClip;


    // Changes eaten state externally
    public void SetEaten(bool e, bool color)
    {
        eaten = e;
        if (color) this.transform.position = new Vector2(-1, -8);
        else this.transform.position = new Vector2(-1, 1);
        curPosition = this.transform.position;
    }

    // Returns eaten value
    public bool GetEaten()
    {
        return eaten;
    }

    // Changes current position of piece externally
    public void SetBoardPos(int pos)
    {
        if (pos < minPos && pos > maxPos) return;
        boardPos = pos;
    }

    // Returns current board position
    public int GetBoardPos()
    {
        return boardPos;
    }

    // Sets move callback
    public void SetMoveCallback(MoveCallback callback)
    {
        onUserMove += callback;
    }

    // Gets move callback
    public MoveCallback GetMoveCallback()
    {
        return onUserMove;
    }

    // Sets show board moves callback
    public void SetShowMovesCallback(ShowMovesCallback callback)
    {
        onShowMoves += callback;
    }

    // Gets show board moves callback
    public ShowMovesCallback GetShowMovesCallback()
    {
        return onShowMoves;
    }

    // Sets clean board moves callback
    public void SetCleanMovesCallback(CleanMovesCallback callback)
    {
        onCleanMoves += callback;
    }

    // Gets clean board moves callback
    public CleanMovesCallback GetCleanMovesCallback()
    {
        return onCleanMoves;
    }

    // Sets audio clip (move sound)
    public void SetAudioClip(AudioClip a)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = a;
        soundClip = a;
    }

    // Gets audio clip (move sound)
    public AudioClip GetAudioClip()
    {
        return soundClip;
    }

    // Changes current position of piece externally
    public void SetCurPosition(float x, float y)
    {
        if (outOfBounds(x, y)) return;
        curPosition.x = x;
        curPosition.y = y;
    }


    // Returns piece to position before the move
    protected void comeBack()
    {
        this.transform.position = new Vector2(curPosition.x, curPosition.y);
    }

    // Returns true if position is out of board bounds
    protected bool outOfBounds(float x, float y)
    {
        if ((x < boundX1 || x > boundX2) || (y < boundY1 || y > boundY2)) {
            this.transform.position = new Vector2(curPosition.x, curPosition.y);
            return true;
        }
        return false;
    }

    // Applies new position coords doing an approximation to center the piece
    // This is call in the update frames of the piece, only when moving
    protected void ApplyAproxPiecePosition(float x, float y)
    {
        if (!moving) return;
        Vector2 target = new Vector2(x, y);

        float distance = distanceBetweenPositions(curPosition.x, curPosition.y, target.x, target.y);
        distance = (distance > 1f) ? distance : 2f;
        float step = speed * Time.deltaTime * (distance/2f);

        // this.transform.position = new Vector2(curX, curY);
        this.transform.position = Vector2.MoveTowards(curPosition, target, step);
        curPosition = this.transform.position;
        if (curPosition.x == target.x && curPosition.y == target.y) {
            moving = false;
            GameState.Instance.SetPieceMoving(false);
            PlaySound(audioSource);
        }
    }

    /* Cambia la pieza de sonido del AudioSource */
    protected void PlaySound(AudioSource audioSource)
    {   
        audioSource.Stop();
        audioSource.Play();
    }

    // Reset calculated moves list and board
    public void cleanMoves()
    {
        // Reset moves list
        GetCleanMovesCallback()();
        moves = new List<int>(); 
        eatMoves = new List<int>(); 
        movesCalculated = false;
    }

    // Returns a list with all the positions of pieces with the same color 
    // in the current state of the board
    protected List<int> getFriendPiecesPositions(bool color) 
    {
        List<int> curBoard = GameState.Instance.GetCurState();
        List<int> friends = new List<int>();

        // Pieces alive of that color
        int piecesAlive = 16;
        if (color) piecesAlive = GameState.Instance.GetWhiteAlive();
        else piecesAlive = GameState.Instance.GetBlackAlive();

        int cont = 0;
        int pos = -1;
        foreach (int piece in curBoard) {
            pos++;
            if (piece == Constants.EMPTY) continue;
            // White or Black
            if ((color && piece % 2 != 0) || (!color && piece % 2 == 0)) { 
                friends.Add(pos); 
                cont++;
            }
            if (cont == piecesAlive) break;
        } 

        return friends;
    }

    // Returns a list with all the positions of pieces with the opposite color 
    // in the current state of the board
    protected List<int> getEnemyPiecesPositions(bool color) 
    {
        List<int> curBoard = GameState.Instance.GetCurState();
        List<int> enemies = new List<int>();

        // Pieces alive of enemy color
        int piecesAlive = 16;
        if (color) piecesAlive = GameState.Instance.GetBlackAlive();
        else piecesAlive = GameState.Instance.GetWhiteAlive();

        int cont = 0;
        int pos = -1;
        foreach (int piece in curBoard) {
            pos++;
            if (piece == Constants.EMPTY) continue;
            // Black or White
            if ((color && piece % 2 == 0) || (!color && piece % 2 != 0)) { 
                enemies.Add(pos); 
                cont++;
            }
            if (cont == piecesAlive) break;
        } 

        return enemies;
    }

    // Removes the squares from the moves list where there is a piece of the same color
    protected List<int> removeFriendMoves(List<int> piece_moves, bool color)
    {
        return piece_moves.Except(getFriendPiecesPositions(color)).ToList();
    }

    // Calculates the distance between 2 positions
    protected float distanceBetweenPositions(float x1, float y1, float x2, float y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }
    
    // Controls drag and drop movement of the piece
    // Contains the logic for the moves control and action
    protected abstract void DragAndDrop();

    // Returns squares where the piece can move to in the current position
    public abstract List<int> CurrentMovements();

    // Check if the square to move has an enemy piece that can be eaten
    protected abstract bool canBeEaten(int move, bool curPieceColor);
}
