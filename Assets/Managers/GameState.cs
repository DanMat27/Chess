using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    /* Singleton instance */
    public static GameState Instance { get; private set; }

    /* Representation of the board */
    protected bool userColor = true; // true = white, false = black
    protected bool colorTurn = true; // true = white, false = black
    protected List<int> squares_w_b = new List<int>(){ 
        Constants.WHITE_TOWER, Constants.WHITE_HORSE, Constants.WHITE_BISHOP, Constants.WHITE_KING, Constants.WHITE_QUEEN, Constants.WHITE_BISHOP, Constants.WHITE_HORSE, Constants.WHITE_TOWER,
        Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN,
        Constants.BLACK_TOWER, Constants.BLACK_HORSE, Constants.BLACK_BISHOP, Constants.BLACK_QUEEN, Constants.BLACK_KING, Constants.BLACK_BISHOP, Constants.BLACK_HORSE, Constants.BLACK_TOWER
    };
    protected List<int> squares_b_w = new List<int>(){ 
        Constants.BLACK_TOWER, Constants.BLACK_HORSE, Constants.BLACK_BISHOP, Constants.BLACK_KING, Constants.BLACK_QUEEN, Constants.BLACK_BISHOP, Constants.BLACK_HORSE, Constants.BLACK_TOWER,
        Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN, Constants.BLACK_PAWN,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN, Constants.WHITE_PAWN,
        Constants.WHITE_TOWER, Constants.WHITE_HORSE, Constants.WHITE_BISHOP, Constants.WHITE_QUEEN, Constants.WHITE_KING, Constants.WHITE_BISHOP, Constants.WHITE_HORSE, Constants.WHITE_TOWER
    };
    public List<int> squaresTop = new List<int>(){0, 1, 2, 3, 4, 5, 6, 7};
    public List<int> squaresBottom = new List<int>(){56, 57, 58, 59, 60, 61, 62, 63};
    public List<int> squaresLeft = new List<int>(){0, 8, 16, 24, 32, 40, 48, 56};
    public List<int> squaresRight = new List<int>(){7, 15, 23, 31, 39, 47, 55, 63};
    /* Game variables */
    protected bool gameEnded = false;
    protected int winner = 0; // 0 = nobody, 1 = user, 2 = rival
    protected List<int> curState = new List<int>(){
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY,
        Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY, Constants.EMPTY
    };
    protected int blackAlive = 16;
    protected int whiteAlive = 16;

    // Create game state instance first
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Returns chosen color by user (black or white)
    public bool GetUserColor()
    {
        return Instance.userColor;
    }

    // Sets chosen color by user (black or white)
    public void SetUserColor(bool uc)
    {
        Instance.userColor = uc;
    }

    // Returns current color turn
    public bool GetColorTurn()
    {
        return Instance.colorTurn;
    }

    // Sets current color turn
    public void SetColorTurn(bool ct)
    {
        Instance.colorTurn = ct;
    }

    // Returns if game has ended
    public bool GetGameEnded()
    {
        return Instance.gameEnded;
    }

    // Sets end of game
    public void SetGameEnded(bool ge)
    {
        Instance.gameEnded = ge;
    }

    // Returns winner of the game
    public int GetWinner()
    {
        return Instance.winner;
    }
    
    // Sets winner of the game
    public void SetWinner(int w)
    {
        Instance.winner = w;
    }

    // Gets list of squares where user is black
    public List<int> GetBlackBoard()
    {
        return Instance.squares_w_b;
    }

    // Gets list of squares where user is white
    public List<int> GetWhiteBoard()
    {
        return Instance.squares_b_w;
    }

    // Gets the current state of the board
    public List<int> GetCurState()
    {
        return Instance.curState;
    }

    // Sets a piece or move in the current state of the board
    public void SetCurStateWithNewMove(int origin, int target, int piece)
    {
        if (origin != -1) Instance.curState[origin] = Constants.EMPTY;
        Instance.curState[target] = piece;
    }

    // Gets the number of black pieces alive right now
    public int GetBlackAlive()
    {
        return Instance.blackAlive;
    }
    
    // Sets the number of black pieces alive right now
    public void SetBlackAlive(int ba)
    {
        Instance.blackAlive = ba;
    }

    // Gets the number of white pieces alive right now
    public int GetWhiteAlive()
    {
        return Instance.whiteAlive;
    }
    
    // Sets the number of white pieces alive right now
    public void SetWhiteAlive(int wa)
    {
        Instance.whiteAlive = wa;
    }
}
