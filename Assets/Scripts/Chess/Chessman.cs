﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum PieceType{
    Queen,
    King,
    Rook,
    Knight,
    Bishop,
    Pawn
}

public enum PlayerType{
    Black,
    White,
    None
}


public interface IHealthBar
{
    public void UpdateHealth(float _health);
}

public interface ISaveHighLowLeftRightMedle
{
    public void SaveHighLowLeftRightMedle(float high,float low,float left, float right,float medle);
}

public class Chessman : MonoBehaviour,IPunInstantiateMagicCallback,IHealthBar,ISaveHighLowLeftRightMedle
{
    public GameObject controller;
    public GameObject movePlate;

    public PieceType type;
    public PlayerType playerType;
    private PhotonView photonView;

    public int xBoard = -1;
    public int yBoard = -1;

    private string player;
    public static List<Chessman> pieces = new List<Chessman>();

    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public CharacterData character;
    public float high, low, left, right, medle;

    public class CharacterRuntimeData{
        public int 
        health;
        public int stamina;
        public float speed;

        
    }
    public CharacterRuntimeData pData;

    public int PieceIndex;

    private Vector3 localScale;
    [SerializeField] private GameObject healthBar;


    //public bool isFristMovePawn=true;
    public PieceType myPiece, opponentpiece;

    public bool AlreadyPlayedPvP =false;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        //Debug.Log($"<color=yellow> {name} health {character.health} </color>  high {high}" +
            //$"low {low} left {left} right {right} medle {medle}");
        localScale = healthBar.transform.localScale;
        AlreadyPlayedPvP = false;
        //SaveHighLowLeftRightMedle(-1, -1, -1, -1, -1);
    }

 
    public void UpdateHealth(float _health)
    {
        //Debug.Log($"<color=yellow> UPDATE ->  {name} health {character.health} _health is{_health} </color>");
        float x = _health / 100;
        
        localScale.x = x;
        //Debug.Log($" x is {x} and localscale is {localScale.x}  xcal is {_health/100} with brakets {(_health/100)}  {70/100}" );
        healthBar.transform.localScale = localScale;
    }

    public void SaveHighLowLeftRightMedle(float _high, float _low, float _left, float _right, float _medle)
    {
        //Debug.Log($"<color=yellow> {name} health {character.health} </color>  high {high}" +
        //  $"low {low} left {left} right {right} medle {medle}");

        high = _high;
        left = _left;
        low = _low;
        left = _left;
        right = _right;
        medle = _medle;

        //Debug.Log("====================================================================================================================");

        //Debug.Log($"<color=yellow> {name} health {character.health} </color>  high {high}" +
        //  $"low {low} left {left} right {right} medle {medle}");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info){

        Debug.Log("OnPhotonInstantiate");
        object[] data = info.photonView.InstantiationData;
        transform.SetParent(Game.Get().Board.transform);
        name = data[0].ToString();
        type = (PieceType)data[1];
        playerType = (PlayerType)data[2];
        SetXBoard((int)data[3]);
        SetYBoard((int)data[4]);
        character = GameData.Get().GetCharacter(data[6].ToString());
        pData = new CharacterRuntimeData();
        pData.health = character.health;
        pData.stamina = character.stamina;
        //characterHealth = character.health;
        Activate();
        pieces.Add(this);
        PieceIndex = (int)data[5];
    }

    public void OnInstantiate(object[] data){

        Debug.Log("OnInstantiate");

        transform.SetParent(Game.Get().Board.transform);
        name = data[0].ToString();
        type = (PieceType)data[1];
        playerType = (PlayerType)data[2];
        SetXBoard((int)data[3]);
        SetYBoard((int)data[4]);
        character = GameData.Get().GetCharacter(data[6].ToString());
        pData = new CharacterRuntimeData();
        pData.health = character.health;
        pData.stamina = character.stamina;
        //characterHealth = character.health;
        Activate();
        pieces.Add(this);
        PieceIndex = (int)data[5];
        
    }


    private void OnDestroy()
    {
        pieces.Remove(this);
    }
    public void Activate()
    {
        //Debug.Log("Activate");

        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        player = playerType == PlayerType.Black ? "black" : "white";

        switch(type)
        {
            case PieceType.Queen: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_queen : white_queen; break;
            case PieceType.Knight: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_knight : white_knight;  break;
            case PieceType.Bishop: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_bishop : white_bishop;  break;
            case PieceType.King: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_king : white_king;  break;
            case PieceType.Rook: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_rook : white_rook;  break;
            case PieceType.Pawn: this.GetComponent<SpriteRenderer>().sprite = playerType == PlayerType.Black ? black_pawn : white_pawn;  break;
        }
    }

    public Sprite GetSprite(){
        return this.GetComponent<SpriteRenderer>().sprite;
    }

    public void Init(){

    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXboard()
    {
        return xBoard;
    }

    public int GetYboard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp");

        if (!Game.Get().IsGameOver() && Game.Get().GetCurrentPlayer() == player && Game.Get().isLocalPlayerTurn)
        {
            photonView.RPC("DestroyMovePlatesRPC",RpcTarget.AllViaServer);
            photonView.RPC("InitiateMovePlatesRPC",RpcTarget.AllViaServer,type);
            if(PVPManager.manager.moveChoiceConfirmation.gameObject.activeSelf) 
            {
                PVPManager.manager.moveChoiceConfirmation.gameObject.SetActive(false);
                PVPManager.manager.selectedMove = null;
            }
        }
    }

    public void DestroyMovePlates()
    {
         photonView.RPC("DestroyMovePlatesRPC",RpcTarget.AllViaServer);
        
    }
    [PunRPC]
    public void SetPieceType(PieceType type) 
    {
      PVPManager.manager.opponentpiece = type;
    }
    bool isPlateInstantiated = false;
    public void InitiateMovePlates(PieceType type)
    {
        Debug.Log("InitiateMovePlates");
       
        switch(type)
        {
            case PieceType.Queen:
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(1,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                LineMovePlate(-1,-1);
                LineMovePlate(-1,1);
                LineMovePlate(1,-1);

                break;
            case PieceType.Knight:
                LMovePlate();
                break;
            case PieceType.Bishop:
                LineMovePlate(1,1);
                LineMovePlate(1,-1);
                LineMovePlate(-1,1);
                LineMovePlate(-1,-1);
                break;
            case PieceType.King:
                SurroundMovePlate();
                break;
            case PieceType.Rook:
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                break;
            case PieceType.Pawn:
                Vector2 piecePosition = new Vector2(xBoard,yBoard);
                Game sc = controller.GetComponent<Game>();
                if(playerType == PlayerType.Black)
                {

                    if(GameManager.instace.isFristMovePawn)
                    {
                        Debug.LogError("*****Black Two by Two X :" + xBoard + " Y :" + yBoard);
                        Debug.LogError("*****FIRST PAWN " + GameManager.instace.isFristMovePawn);
                        if((sc.GetPosition(xBoard,yBoard-2) == null || sc.GetPosition(xBoard,yBoard-2).GetComponent<Chessman>().player != player) && sc.GetPosition(xBoard,yBoard-1) == null)
                            PawnMovePlateBlack(xBoard,yBoard - 2,piecePosition);
                        if(sc.GetPosition(xBoard,yBoard-1) == null || sc.GetPosition(xBoard,yBoard-1).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard,yBoard - 1,piecePosition);
                        if((sc.GetPosition(xBoard-2,yBoard) == null || sc.GetPosition(xBoard-2,yBoard).GetComponent<Chessman>().player != player) && sc.GetPosition(xBoard-1,yBoard) == null)
                            PawnMovePlateBlack(xBoard - 2,yBoard,piecePosition);
                        if(sc.GetPosition(xBoard-1,yBoard) == null || sc.GetPosition(xBoard-1,yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard - 1,yBoard,piecePosition);
                    }
                    else
                    {

                        Debug.LogError("*****Black One by One X :" + xBoard + " Y :" + yBoard);
                        Debug.LogError("*****Black One to One");
                        if(sc.GetPosition(xBoard,yBoard-1) == null || sc.GetPosition(xBoard,yBoard-1).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard,yBoard - 1,piecePosition,true);
                        if(sc.GetPosition(xBoard-1,yBoard) == null || sc.GetPosition(xBoard-1,yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlateBlack(xBoard - 1,yBoard,piecePosition);
                    }

                }
                else
                {


                    if(GameManager.instace.isFristMovePawn)
                    {

                        Debug.LogError("*****White Two by Two X :" + xBoard + " Y :" + yBoard);
                        Debug.LogError("*****FIRST PAWN " + GameManager.instace.isFristMovePawn);
                        if((sc.GetPosition(xBoard,yBoard+2) == null || sc.GetPosition(xBoard,yBoard+2).GetComponent<Chessman>().player != player) && sc.GetPosition(xBoard,yBoard+1) == null)
                            PawnMovePlate(xBoard,yBoard + 2,piecePosition);
                        if(sc.GetPosition(xBoard,yBoard+1) == null || sc.GetPosition(xBoard,yBoard+1).GetComponent<Chessman>().player != player)
                            PawnMovePlate(xBoard,yBoard + 1,piecePosition);
                        if((sc.GetPosition(xBoard+2,yBoard) == null || sc.GetPosition(xBoard+2,yBoard).GetComponent<Chessman>().player != player) && sc.GetPosition(xBoard+1,yBoard) == null)
                            PawnMovePlate(xBoard + 2,yBoard,piecePosition);
                        if(sc.GetPosition(xBoard+1,yBoard) == null || sc.GetPosition(xBoard+1,yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlate(xBoard + 1,yBoard,piecePosition);
                    }
                    else
                    {

                        Debug.LogError("*****White One by One X :" + xBoard + " Y :" + yBoard);
                        Debug.LogError("*****White One to One");
                        if(sc.GetPosition(xBoard,yBoard+1) == null || sc.GetPosition(xBoard,yBoard+1).GetComponent<Chessman>().player != player)
                            PawnMovePlate(xBoard,yBoard + 1,piecePosition,true);
                        if(sc.GetPosition(xBoard+1,yBoard) == null || sc.GetPosition(xBoard+1,yBoard).GetComponent<Chessman>().player != player)
                            PawnMovePlate(xBoard + 1,yBoard,piecePosition);
                    }
                }

                break;
        }

        if(isPlateInstantiated) 
        {
            SetLastPieceInfo(type);
            isPlateInstantiated = false;
        }
    }

    private void SetLastPieceInfo(PieceType type)
    {
        if(PhotonNetwork.LocalPlayer == Game.Get()._currnetTurnPlayer)
        {
           // Debug.Log("LAST PIECE "+)
            PVPManager.manager.tempPiece = type;
           // photonView.RPC("SetPieceType",RpcTarget.Others,type);
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Debug.Log("LineMovePlate");

        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while(sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
            isPlateInstantiated = true;
        }

        if(sc.PositionOnBoard(x,y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            PieceType pieceType = sc.GetPosition(x,y).GetComponent<Chessman>().type;
            PVPManager.manager.SetOpponentAttackPieceInfo(pieceType);
            MovePlateAttackSpawn(x, y);
            isPlateInstantiated = true;
        }
    }

    public void LMovePlate()
    {
        Debug.Log("LMovePlate");

        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        Debug.Log("LMovePlate");

        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 0);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 0);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Debug.Log($"PointMovePlate x is {x} y is {y}");

        Game sc = controller.GetComponent<Game>();
        if(sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if(cp == null)
            {
                MovePlateSpawn(x, y);
                isPlateInstantiated = true;
            }
            else if(cp.GetComponent<Chessman>().player != player)
            {
                PieceType pieceType = sc.GetPosition(x,y).GetComponent<Chessman>().type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType);
                MovePlateAttackSpawn(x, y);
                isPlateInstantiated = true;
            }
        }
    }

    public void PawnMovePlate(int x, int y,Vector2 pieceVector,bool isAttackSibling=false)
    {
        //Debug.Log($"PawnMovePlate x {x} and y {y}");

        Game sc = controller.GetComponent<Game>();
        if(sc.PositionOnBoard(x, y))
        {
            //OLD Working
            if(sc.GetPosition(x,y) == null)
            {
                MovePlateSpawn(x,y);
                isPlateInstantiated = true;
                Debug.Log("Simple Move Player Generate from here X " + x + " Y " + y);
            }
            //if(sc.PositionOnBoard(x + 1,y) && sc.GetPosition(x + 1,y) != null &&
            //   sc.GetPosition(x + 1,y).GetComponent<Chessman>().player != player )//&& isAttackSibling )
            //{
            //    if((x + 1) == (pieceVector.x + 1) && y == (pieceVector.y + 1))
            //    {
            //        MovePlateAttackSpawn(x + 1,y);
            //    }
            //    Debug.Log($"<color=yellow> if1 MovePlateAttackSpawn(x + 1, y) {x} { y}   </color>");
            //}
            if(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y) != null &&
               sc.GetPosition(x,y).GetComponent<Chessman>().player != player)//&& isAttackSibling )
            {
                //if((x) == (pieceVector.x) && y == (pieceVector.y))
                // {
                PieceType pieceType = sc.GetPosition(x,y).GetComponent<Chessman>().type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType);
                MovePlateAttackSpawn(x,y);
                isPlateInstantiated = true;
                // }
                Debug.Log($"<color=yellow> if1 MovePlateAttackSpawn(x + 1, y) {x} { y}   </color>");
            }

            //New

            //if(sc.GetPosition(x,y) == null)
            //{
            //    MovePlateSpawn(x,y);
            //    Debug.Log("Simple Move Player Generate from here X " + x + " Y " + y);
            //}
            // if(sc.PositionOnBoard(x+1,y+1) && sc.GetPosition(x+1 ,y+1) != null &&
            //   sc.GetPosition(x+1 ,y+1).GetComponent<Chessman>().player != player)
            //{
            //    MovePlateAttackSpawn(x+1,y+1);
            //    Debug.Log($"<color=yellow> if1 MovePlateAttackSpawn(x + 1, y) {x} { y}   </color>");
            //}
            //


            //if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
            //    sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            //{
            //    MovePlateAttackSpawn(x - 1, y);
            //    Debug.Log($"<color=yellow> if2 MovePlateAttackSpawn(x + 1, y) {x} { y}   </color>");
            //}
        }
    }
    public void PawnMovePlateBlack(int x,int y,Vector2 pieceVector,bool isAttackSibling = false)
    {
        //Debug.Log($"PawnMovePlate x {x} and y {y}");

        Game sc = controller.GetComponent<Game>();
        if(sc.PositionOnBoard(x,y))
        {
            //OLD Working
            if(sc.GetPosition(x,y) == null)
            {
                MovePlateSpawn(x,y);
                isPlateInstantiated = true;
                Debug.Log("Simple Move Player Generate from here X " + x + " Y " + y);
            }
            //if(sc.PositionOnBoard(x - 1,y) && sc.GetPosition(x - 1,y) != null &&
            //   sc.GetPosition(x - 1,y).GetComponent<Chessman>().player != player) //&& !isAttackSibling )
            //{
            //    if((x - 1) == (pieceVector.x - 1) && y == (pieceVector.y - 1))
            //    {
            //        MovePlateAttackSpawn(x - 1,y);
            //        Debug.Log($"<color=yellow> if1 MovePlateAttackSpawn(x + 1, y) {x} { y}   </color>");
            //    }

            //}
            if(sc.PositionOnBoard(x,y) && sc.GetPosition(x,y) != null &&
               sc.GetPosition(x,y).GetComponent<Chessman>().player != player) //&& !isAttackSibling )
            {
                //if(x == (pieceVector.x) && y == (pieceVector.y))
                //{
                PieceType pieceType = sc.GetPosition(x,y).GetComponent<Chessman>().type;
                PVPManager.manager.SetOpponentAttackPieceInfo(pieceType);
                MovePlateAttackSpawn(x,y);
                isPlateInstantiated = true;
                Debug.Log($"<color=yellow> if1 MovePlateAttackSpawn(x + 1, y) {x} { y}   </color>");
               // }

            }

            //New

            //if(sc.GetPosition(x,y) == null)
            //{
            //    MovePlateSpawn(x,y);
            //    Debug.Log("Simple Move Player Generate from here X " + x + " Y " + y);
            //}
            // if(sc.PositionOnBoard(x+1,y+1) && sc.GetPosition(x+1 ,y+1) != null &&
            //   sc.GetPosition(x+1 ,y+1).GetComponent<Chessman>().player != player)
            //{
            //    MovePlateAttackSpawn(x+1,y+1);
            //    Debug.Log($"<color=yellow> if1 MovePlateAttackSpawn(x + 1, y) {x} { y}   </color>");
            //}
            //


            //if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
            //    sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            //{
            //    MovePlateAttackSpawn(x - 1, y);
            //    Debug.Log($"<color=yellow> if2 MovePlateAttackSpawn(x + 1, y) {x} { y}   </color>");
            //}
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        //Debug.Log($"MovePlateSpawn x {matrixX} y {matrixY}");

        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        object[] data = new object[]{playerType,type,matrixX,matrixY,false,PieceIndex};
        GameObject mp = PhotonNetwork.Instantiate("movePlate", new Vector3(x, y, -3.0f), Quaternion.identity,0,data);
      
        // MovePlate mpScript = mp.GetComponent<MovePlate>();
        // mpScript.SetReference(this);
        // mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        Debug.Log($"MovePlateAttackSpawn x {matrixX} y {matrixY}");

        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        object[] data = new object[]{playerType,type,matrixX,matrixY,true,PieceIndex};
        GameObject mp = PhotonNetwork.Instantiate("movePlate", new Vector3(x, y, -3.0f), Quaternion.identity,0,data);

        // MovePlate mpScript = mp.GetComponent<MovePlate>();
        // mpScript.attack = true;
        // mpScript.SetReference(this);
        // mpScript.SetCoords(matrixX, matrixY);
    }

    public static Chessman GetPiece(PlayerType ptype,PieceType type,int ind){
        foreach (Chessman item in pieces)
        {
            if(item.playerType == ptype && item.type == type && item.PieceIndex == ind)
                return item;
        }
        return null;
    }

    public static Chessman GetPiece(int ind){
        foreach (Chessman item in pieces)
        {
            if(item.PieceIndex == ind)
                return item;
        }
        return null;
    }

    public static List<Chessman> GetPiecesOfPlayer(PlayerType ptype){
        List<Chessman> res = new List<Chessman>();
        foreach (Chessman item in pieces)
        {
            if(item.playerType == ptype)
                res.Add(item);
        }
        return res;
    }

    #region Pun calls
    [PunRPC]
    public void InitiateMovePlatesRPC(PieceType type){
        InitiateMovePlates(type);
    }

    [PunRPC]
    public void DestroyMovePlatesRPC(){
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for(int i =0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

   



    #endregion
}
