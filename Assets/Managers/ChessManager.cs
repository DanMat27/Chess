using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    /* Chess board */
    public GameObject board;
    public GameObject canMoveTile;
    public GameObject eatEnemyTile;
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
    /* Sound variables */
    public AudioClip moveSound;

    /* Move management */
    public struct Move {
        public int origin; // Origin board position
        public int target; // Target board position
        public int piece; // Piece that moved
    }
    private List<Move> moves; // Contains all match movements
    private List<GameObject> paintMoves = new List<GameObject>(); // Contains all painted moves
    private int prevPaintedPiece = -1; // Previous painted piece


    /* Start is called before the first frame update */
    void Start()
    {
        moves = new List<Move>();
        chessPieces = new List<GameObject>();

        // User is WHITE
        // state.GetComponent<GameState>().SetUserColor(true);
        GameState.Instance.SetUserColor(true);

        // First, create chess board
        double x = 3.5;
        double y = -0.5;
        Instantiate(board, new Vector3((float)x, (float)y, 1), Quaternion.identity);

        // Then, put all the pieces on the board
        x = 0;
        y = 0;
        int cont = 0;
        // if (state.GetComponent<GameState>().GetUserColor()) {
        if (GameState.Instance.GetUserColor()) {
            print("I am white");
            // foreach (int square in state.GetComponent<GameState>().GetBlackBoard()) {
            foreach (int square in GameState.Instance.GetBlackBoard()) {
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
            // foreach (int square in state.GetComponent<GameState>().GetWhiteBoard()) {
            foreach (int square in GameState.Instance.GetWhiteBoard()) {
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
    private void DoMove(int origin, int target, int piece)
    {
        print("DO MOVE? " + origin + " --> " + target + " ## Piece: " + piece);

        // Store move
        Move newMove = new Move(){ origin = origin, target = target, piece = piece };
        moves.Add(newMove);
        GameState.Instance.SetCurStateWithNewMove(origin, target, piece); // Save in the current state of the game

        // Change color turn
        GameState.Instance.SetColorTurn(!(piece % 2 != 0));
        print(!(piece % 2 != 0) ? "NEW WHITE" : "NEW BLACK");
    }

    /* Show in the current board the possible moves of a given piece */
    private void ShowBoardMoves(List<int> pos_moves, List<int> eat_moves, int piecePos)
    {
        if (prevPaintedPiece != piecePos) {
            prevPaintedPiece = piecePos;

            // Destroy all painted moves if they were not yet
            CleanBoardMoves();

            // Paint the possible moves
            double x = 0;
            double y = 0;
            foreach (int move in pos_moves) { 
                x = move % 8;
                y = Math.Floor((double)(move / (-8)));
                GameObject o = Instantiate(canMoveTile, new Vector3((float)x, (float)y, 1), Quaternion.identity);
                paintMoves.Add(o);
            }

            // Paint the possible eats
            foreach (int move in eat_moves) {
                x = move % 8;
                y = Math.Floor((double)(move / (-8)));
                GameObject o = Instantiate(eatEnemyTile, new Vector3((float)x, (float)y, 1), Quaternion.identity);
                paintMoves.Add(o);
            }
        }
    }

    /* Cleans the current board possible moves of the last piece */
    private void CleanBoardMoves()
    {
        foreach (GameObject move in paintMoves) {
            Destroy(move);
        }
        prevPaintedPiece = -1;
    }

    /* Draws all the alive pieces of the board
       White = Even, Black = Odd */
    void drawPieces(int num, double x, double y, int pos)
    {
        GameObject o = null;
        // Instantiates pieces
        if (num == Constants.WHITE_PAWN) {
            o = Instantiate(pawn_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.BLACK_PAWN) {
            o = Instantiate(pawn_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.WHITE_HORSE) {
            o = Instantiate(horse_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.BLACK_HORSE) {
            o = Instantiate(horse_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.WHITE_BISHOP) {
            o = Instantiate(bishop_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.BLACK_BISHOP) {
            o = Instantiate(bishop_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.WHITE_TOWER) {
            o = Instantiate(tower_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.BLACK_TOWER) {
            o = Instantiate(tower_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.WHITE_QUEEN) {
            o = Instantiate(queen_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.BLACK_QUEEN) {
            o = Instantiate(queen_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.WHITE_KING) {
            o = Instantiate(king_w, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        if (num == Constants.BLACK_KING) {
            o = Instantiate(king_b, new Vector3((float)x, (float)y, 0), Quaternion.identity);
        }
        // Saves piece position
        if (o) { 
            o.GetComponent<ChessPiece>().SetCurPosition((float)x, (float)y);
            o.GetComponent<ChessPiece>().SetBoardPos(pos);
            o.GetComponent<ChessPiece>().SetMoveCallback(DoMove);
            o.GetComponent<ChessPiece>().SetShowMovesCallback(ShowBoardMoves);
            o.GetComponent<ChessPiece>().SetCleanMovesCallback(CleanBoardMoves);
            o.GetComponent<ChessPiece>().SetAudioClip(moveSound);
            GameState.Instance.SetCurStateWithNewMove(-1, pos, num); // Save in the initial state of the game
            chessPieces.Add(o);
        }
    }
}
