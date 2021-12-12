using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    /* Chess board */
    public GameObject board;
    /* Chess pieces */
    public GameObject pawn_w;
    public GameObject pawn_b;
    public GameObject horse_w;
    public GameObject horse_b;
    public GameObject bishop_w;
    public GameObject bishop_b;
    public GameObject tower_w;
    public GameObject tower_b;
    public GameObject queen_w;
    public GameObject queen_b;
    public GameObject king_w;
    public GameObject king_b;
    /* Representation of the board */
    public bool userColor; // true = white, false = black
    private int[] squares_w_b = { 7, 3, 5, 11, 9, 5, 3, 7,
                                  1, 1, 1, 1, 1, 1, 1, 1,
                                  0, 0, 0, 0, 0, 0, 0, 0,
                                  0, 0, 0, 0, 0, 0, 0, 0,
                                  0, 0, 0, 0, 0, 0, 0, 0,
                                  0, 0, 0, 0, 0, 0, 0, 0,
                                  2, 2, 2, 2, 2, 2, 2, 2,
                                  8, 4, 6, 10, 12, 6, 4, 8 };
    private int[] squares_b_w = { 8, 4, 6, 12, 10, 6, 4, 8,
                                  2, 2, 2, 2, 2, 2, 2, 2,
                                  0, 0, 0, 0, 0, 0, 0, 0,
                                  0, 0, 0, 0, 0, 0, 0, 0,
                                  0, 0, 0, 0, 0, 0, 0, 0,
                                  0, 0, 0, 0, 0, 0, 0, 0,
                                  1, 1, 1, 1, 1, 1, 1, 1,
                                  7, 3, 5, 9, 11, 5, 3, 7 };
    /* Game variables */
    private bool gameEnded = false;
    private int winner = 0; // 0 = nobody, 1 = user, 2 = rival

    /* Start is called before the first frame update */
    void Start()
    {
        // First, create chess board
        double x = 3.5;
        double y = -0.5;
        Instantiate(board, new Vector3((float)x, (float)y, 0), Quaternion.identity);

        // Then, put all the pieces on the board
        x = 0;
        y = 0;
        if (userColor) {
            print("I am white");
            foreach (int square in squares_b_w) {
                drawPieces(square, x, y);
            }
        }
        else {
            print("I am black");
            foreach (int square in squares_w_b) {
                drawPieces(square, x, y);
            }
        }
    }

    /* Update is called once per frame */
    void Update()
    {
        
    }

    /* Draws all the alive pieces of the board
       White = Even, Black = Odd */
    void drawPieces(int num, double x, double y)
    {
        if (num == 1) {
            Instantiate(pawn_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 2) {
            Instantiate(pawn_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 3) {
            Instantiate(horse_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 4) {
            Instantiate(horse_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 5) {
            Instantiate(bishop_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 6) {
            Instantiate(bishop_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 7) {
            Instantiate(tower_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 8) {
            Instantiate(tower_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 9) {
            Instantiate(queen_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 10) {
            Instantiate(queen_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 11) {
            Instantiate(king_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 12) {
            Instantiate(king_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
    }
}
