using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ChessPiece : MonoBehaviour
{
    /* Piece atributes */
    protected bool eaten = false; // true = can't move anymore
    protected int boardPos; // Current board position

    /* Board limits */
    protected int minPos = 0;
    protected int maxPos = 63;

    /* Move event */
    public delegate bool MoveCallback (int origin, int target); // Signature of move event (2 args)
    protected event MoveCallback onUserMove; // Move callback


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

    // Get move callback
    public MoveCallback GetMoveCallback()
    {
        return onUserMove;
    }


    // Returns squares where the piece can move in the current position
    public abstract List<int> CurrentMovements();
}
