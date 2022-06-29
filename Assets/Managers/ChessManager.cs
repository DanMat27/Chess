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
    private List<GameObject> chessPieces;
    /* Game State */
    public GameObject state;
    /* Sound variables */
    public AudioClip moveSound;

    /* Move management */
    public struct Move {
        public int origin; // Origin board position
        public int target; // Target board position
        public bool color; // Color that moved: true = white, false = black
    }
    private List<Move> moves; // Contains all match movements


    /* Start is called before the first frame update */
    void Start()
    {
        moves = new List<Move>();
        chessPieces = new List<GameObject>();

        // User is WHITE
        state.GetComponent<GameState>().SetUserColor(true); 

        // First, create chess board
        double x = 3.5;
        double y = -0.5;
        Instantiate(board, new Vector3((float)x, (float)y, 1), Quaternion.identity);

        // Then, put all the pieces on the board
        x = 0;
        y = 0;
        int cont = 0;
        if (state.GetComponent<GameState>().GetUserColor()) {
            print("I am white");
            foreach (int square in state.GetComponent<GameState>().GetBlackBoard()) {
                drawPieces(square, x, y, cont);
                x = x + 1;
                if (x % 8 == 0){ 
                    x = 0;
                    y = y - 1;
                }
                cont++;
            }
        }
        else {
            print("I am black");
            foreach (int square in state.GetComponent<GameState>().GetWhiteBoard()) {
                drawPieces(square, x, y, cont);
                x = x + 1;
                if (x % 8 == 0){ 
                    x = 0;
                    y = y - 1;
                }
                cont++;
            }
        }
    }

    /* Update is called once per frame */
    /* Always white player starts the game */
    void Update()
    {
        
    }

    /* Stores the applied move in the saved list of moves.
       Changes the turn of the players. */
    private void DoMove(int origin, int target, bool color)
    {
        print("DO MOVE? " + origin + " --> " + target + " ## Color: " + (color ? "WHITE" : "BLACK"));

        // Store move
        Move newMove = new Move(){ origin = origin, target = target, color = color };
        moves.Add(newMove);

        // Change color turn
        state.GetComponent<GameState>().SetUserColor(!color);
        print(!color ? "NEW WHITE" : "NEW BLACK");
    }

    /* Draws all the alive pieces of the board
       White = Even, Black = Odd */
    void drawPieces(int num, double x, double y, int pos)
    {
        GameObject o = null;
        // Instantiates pieces
        if (num == 1) {
            o = Instantiate(pawn_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 2) {
            o = Instantiate(pawn_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 3) {
            o = Instantiate(horse_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 4) {
            o = Instantiate(horse_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 5) {
            o = Instantiate(bishop_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 6) {
            o = Instantiate(bishop_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 7) {
            o = Instantiate(tower_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 8) {
            o = Instantiate(tower_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 9) {
            o = Instantiate(queen_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 10) {
            o = Instantiate(queen_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 11) {
            o = Instantiate(king_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == 12) {
            o = Instantiate(king_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        // Saves piece position
        if (o) { 
            o.GetComponent<ChessPiece>().SetCurPosition((float)x, (float)y);
            o.GetComponent<ChessPiece>().SetBoardPos(pos);
            o.GetComponent<ChessPiece>().SetMoveCallback(DoMove);
            o.GetComponent<ChessPiece>().SetAudioClip(moveSound);
            o.GetComponent<ChessPiece>().SetBoardState(state); // Pieces have reference of current game state
            chessPieces.Add(o);
        }
    }
}
