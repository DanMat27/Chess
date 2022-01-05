using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    /* Representation of the board */
    protected bool userColor = true; // true = white, false = black
    protected bool colorTurn = true; // true = white, false = black
    protected List<int> squares_w_b = new List<int>(){ 7, 3, 5, 11, 9, 5, 3, 7,
                                                       1, 1, 1, 1, 1, 1, 1, 1,
                                                       0, 0, 0, 0, 0, 0, 0, 0,
                                                       0, 0, 0, 0, 0, 0, 0, 0,
                                                       0, 0, 0, 0, 0, 0, 0, 0,
                                                       0, 0, 0, 0, 0, 0, 0, 0,
                                                       2, 2, 2, 2, 2, 2, 2, 2,
                                                       8, 4, 6, 10, 12, 6, 4, 8 };
    protected List<int> squares_b_w = new List<int>(){ 8, 4, 6, 12, 10, 6, 4, 8,
                                                       2, 2, 2, 2, 2, 2, 2, 2,
                                                       0, 0, 0, 0, 0, 0, 0, 0,
                                                       0, 0, 0, 0, 0, 0, 0, 0,
                                                       0, 0, 0, 0, 0, 0, 0, 0,
                                                       0, 0, 0, 0, 0, 0, 0, 0,
                                                       1, 1, 1, 1, 1, 1, 1, 1,
                                                       7, 3, 5, 9, 11, 5, 3, 7 };
    /* Game variables */
    protected bool gameEnded = false;
    protected int winner = 0; // 0 = nobody, 1 = user, 2 = rival

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
        return userColor;
    }

    // Sets chosen color by user (black or white)
    public void SetUserColor(bool uc)
    {
        userColor = uc;
    }

    // Returns current color turn
    public bool GetColorTurn()
    {
        return colorTurn;
    }

    // Sets current color turn
    public void SetColorTurn(bool ct)
    {
        colorTurn = ct;
    }

    // Returns if game has ended
    public bool GetGameEnded()
    {
        return gameEnded;
    }

    // Sets end of game
    public void SetGameEnded(bool ge)
    {
        gameEnded = ge;
    }

    // Returns winner of the game
    public int GetWinner()
    {
        return winner;
    }
    
    // Sets winner of the game
    public void SetWinner(int w)
    {
        winner = w;
    }

    // Gets list of squares where user is black
    public List<int> GetBlackBoard()
    {
        return squares_w_b;
    }

    // Gets list of squares where user is white
    public List<int> GetWhiteBoard()
    {
        return squares_b_w;
    }
}
