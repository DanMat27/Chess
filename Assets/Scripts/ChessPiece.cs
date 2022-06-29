using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ChessPiece : MonoBehaviour
{
    /* Constants */
    protected const string WHITE = "White";
    protected const string BLACK = "Black";

    /* Piece atributes */
    protected bool eaten = false; // true = can't move anymore
    protected int boardPos; // Current board position

    /* Move event */
    public delegate void MoveCallback (int origin, int target, bool color); // Signature of move event (3 args)
    protected event MoveCallback onUserMove; // Move callback
    protected List<int> moves; // List of valid moves
    protected GameObject boardState; // Current state of board and game

    /* Board limits */
    protected int minPos = 0;
    protected int maxPos = 63;
    protected float boundX1 = (float)-0.5;
    protected float boundX2 = (float)7.5;
    protected float boundY1 = (float)-7.5;
    protected float boundY2 = (float)0.5;
    protected List<int> squaresTop = new List<int>(){0, 1, 2, 3, 4, 5, 6, 7};
    protected List<int> squaresBottom = new List<int>(){56, 57, 58, 59, 60, 61, 62, 63};
    protected List<int> squaresLeft = new List<int>(){0, 8, 16, 24, 32, 40, 48, 56};
    protected List<int> squaresRight = new List<int>(){7, 15, 23, 31, 39, 47, 55, 63};

    /* Drag&Drop atributes */
    protected Collider2D objectCollider;
    protected bool isDraggable; // true = piece can be dragged
    protected bool isDragging; // true = piece is being dragged
    protected float curX; // Current x position (before applying movement)
    protected float curY; // Current y position (before applying movement)
    
    /* Sounds */
    protected AudioSource audioSource;
    protected AudioClip soundClip;


    // Changes eaten state externally
    public void SetEaten(bool e)
    {
        eaten = e;
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

    // Sets board and game state externally
    public void SetBoardState(GameObject g)
    {
        boardState = g;
    }

    // Gets board and game state
    public GameObject GetBoardState()
    {
        return boardState;
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
        curX = x;
        curY = y;
    }


    // Returns piece to position before the move
    protected void comeBack()
    {
        this.transform.position = new Vector2(curX, curY);
    }

    // Returns true if position is out of board bounds
    protected bool outOfBounds(float x, float y)
    {
        if ((x < boundX1 || x > boundX2) || (y < boundY1 || y > boundY2)) {
            this.transform.position = new Vector2(curX, curY);
            return true;
        }
        return false;
    }

    // Applies new position coords doing an approximation to center the piece
    protected void ApplyAproxPiecePosition(float x, float y)
    {
        curX = (float)Math.Round(x);
        curY = (float)Math.Round(y);
        this.transform.position = new Vector2(curX, curY);
        PlaySound(audioSource);
    }

    /* Cambia la pieza de sonido del AudioSource */
    protected void PlaySound(AudioSource audioSource)
    {   
        audioSource.Stop();
        audioSource.Play();
    }


    // Controls drag and drop movement of the piece
    protected abstract void DragAndDrop();

    // Returns squares where the piece can move to in the current position
    public abstract List<int> CurrentMovements();
}
