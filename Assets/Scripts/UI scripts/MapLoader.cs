using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/* Treat this script more as an example as to how Large-Menu scripts should be designed.
 * Keep in mind this script is a component of the panel labelled "Expand"
 * 
 * Note: You MUST put everything in OnEnable. Unity throws unfixable errors when
 * manipulating objects that aren't enabled for some reason. Some sort of 
 * SUCCEEDED(hr) that crashes the script object. 
 * 
 * TileTypes: Moving objects instantiate to 0
 *  0-Empty
 *  1-Wall
 *  2-Enemy
 *  3-Player
 *  10-Gold Because Why not?
 *  15-Chest
 *  
 *  TODO: Make it efficient
 *  TODO: Fix weird inconsistencies (May be Generation)
 */

public class MapLoader : MonoBehaviour
{
    public Canvas canvas; //No idea how important this is. Probably only important for automated UI Movements
    RectTransform rtf;
    public GameObject gameManager;


    private Texture2D mapTex;
    private Texture2D miniTex;
    private Sprite mapSprite;
    private Sprite miniSprite;
    private BoardManager board;
    private int[,] boardData;
    private List<Vector2> prevEnemyPositions;
    private Vector2 prevPlayerPosition;

    private GameObject miniMap;
    private GameObject mainMap;

    private bool firstUpdate = true;

    private int mapRadius = 7;
    // Use this for initialization. Start or Awake will crash it. 
    void Start()
    {
        miniMap = gameObject.transform.FindChild("Contents").gameObject;
        mainMap = gameObject.transform.FindChild("Expand").gameObject;
        board = (BoardManager)gameManager.GetComponent(typeof(BoardManager));
        prevEnemyPositions = new List<Vector2>();
        boardData = createMapBoard();
        loadMap();
        constructMiniMap();
    }

    /// <summary>
    /// Construct the most basic map.
    /// </summary>
    private void loadMap()
    {
        mapTex = new Texture2D(boardData.GetLength(0)+2, boardData.GetLength(1)+2, TextureFormat.ARGB32, false);
        mapTex.filterMode = FilterMode.Point;

        for (int i = 0; i < mapTex.width ; i++)
        {
            for (int j = 0; j < mapTex.height; j++)
            {
                mapTex.SetPixel(j, i, Color.black);
            }
        }

        for (int i = 1; i < mapTex.width-1;i++)
        {
            for(int j = 1; j < mapTex.height-1;j++)
            {
                if (boardData[j-1, i-1] == 0)
                    mapTex.SetPixel(j, i, Color.white);
            }
        }
        mapTex.Apply();
        mapSprite = Sprite.Create(mapTex, new Rect(0, 0, mapTex.width, mapTex.height), new Vector2(0.5f, 0.5f),16.0f);
        var mapTransform = mainMap.transform.FindChild("MapStore").GetComponent<RectTransform>();
        Debug.Log(mapTex.width + ", " + mapTex.height);
        mapTransform.sizeDelta = new Vector2(mapTransform.sizeDelta.x * ((float)mapTex.height / (float)mapTex.width), mapTransform.sizeDelta.y); //Perfect Square Pixels
        mainMap.transform.FindChild("MapStore").gameObject.GetComponent<Image>().sprite = mapSprite;
    }
    
    /// <summary>
    /// Update map every so often to reflect changes.
    /// </summary>
    private void updateMap()
    { 
        GameObject[,] fog = createFogTile();
        Vector2 playerPos = getPlayerPosition();
        List<Vector2> enemies = GetEnemyPositions();

        //int sightRange = 20;

        //Prev cleanups
        boardData[(int)prevPlayerPosition.x, (int)prevPlayerPosition.y] = 0;
        foreach(Vector2 v in prevEnemyPositions)
        {
            boardData[(int)v.x, (int)v.y] = 0;
        }
        prevEnemyPositions.Clear(); //Empty prevPositions
        //Passes to adjust the map
        boardData[(int)playerPos.x, (int)playerPos.y] = 3; //Player Pass
        prevPlayerPosition = playerPos; //prev Save
        foreach(Vector2 v in enemies) //Enemy Pass
        {
            prevEnemyPositions.Add(v);
            boardData[(int)v.x, (int)v.y] = 2;
        }

        for (int i = 1; i < mapTex.width - 1; i++) //Raw Pass
        {
            for (int j = 1; j < mapTex.height - 1; j++)
            {
                if (boardData[j - 1, i - 1] == 3)
                    mapTex.SetPixel(j, i, Color.green);
                else if (boardData[j - 1, i - 1] == 2)
                    mapTex.SetPixel(j, i, Color.red);
                else if (boardData[j - 1, i - 1] == 0)
                    mapTex.SetPixel(j, i, Color.white);
                else if (boardData[j - 1, i - 1] == 10)
                    mapTex.SetPixel(j, i, Color.yellow);
                else if (boardData[j - 1, i - 1] == 15)
                    mapTex.SetPixel(j, i, Color.cyan);
                else if (boardData[i - 1, j - 1] == 1)
                    mapTex.SetPixel(j, i, Color.gray);

                //var curFog = fog[i, j].GetComponent<SpriteRenderer>(); //Fog Pass
                //if (curFog != null)
                //{
                //    if (curFog.color.a > 0.8f) //Hidden and unseen
                //    {
                //        mapTex.SetPixel(i, j, Color.black);
                //    }
                //    else if (curFog.color.a > 0.5f && curFog.color.a < 0.8f) //Seen, but currently hidden
                //    {
                //        if (boardData[i - 1, j - 1] != 1)
                //            mapTex.SetPixel(i, j, Color.white);

                //        mapTex.SetPixel(i, j, mapTex.GetPixel(i, j) - new Color(0.2f, 0.2f, 0.2f, 0));
                //    }
                //}
            }
        }
        if (firstUpdate)
            firstUpdate = false;
        mapTex.Apply();
    }

    /// <summary>
    /// Start construction of a minimap
    /// </summary>
    private void constructMiniMap()
    {
        miniTex = new Texture2D(2*mapRadius+1,2*mapRadius+1, TextureFormat.ARGB32, false);
        miniTex.filterMode = FilterMode.Point;


        miniSprite = Sprite.Create(miniTex, new Rect(0, 0, miniTex.width, miniTex.height), new Vector2(0.5f, 0.5f), 16.0f);
        miniMap.transform.FindChild("MapStore").gameObject.GetComponent<Image>().sprite = miniSprite;
    }  
     
    /// <summary>
    /// Update the minimap
    /// </summary>
    private void UpdateMiniMap()
    {
        Vector2 playerPos = getPlayerPosition();
        List<Vector2> enemies = GetEnemyPositions();
        GameObject[,] fog = createFogTile();

        boardData[(int)prevPlayerPosition.x, (int)prevPlayerPosition.y] = 0;
        foreach (Vector2 v in prevEnemyPositions)
        {
            boardData[(int)v.x, (int)v.y] = 0;
        }
        prevEnemyPositions.Clear(); //Empty prevPositions
        //Passes to adjust the map
        boardData[(int)playerPos.x, (int)playerPos.y] = 3; //Player Pass
        prevPlayerPosition = playerPos; //prev Save
        foreach (Vector2 v in enemies) //Enemy Pass
        {
            prevEnemyPositions.Add(v);
            boardData[(int)v.x, (int)v.y] = 2;
        }

        for (int i = -mapRadius; i <= mapRadius; i++) //Raw Pass
        {
            for (int j = -mapRadius; j <= mapRadius; j++)
            {
                if (playerPos.x + i < 0 || playerPos.x + i >= boardData.GetLength(0) || playerPos.y + j < 0 || playerPos.y + j >= boardData.GetLength(1))
                {
                    miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.black);
                }
                else
                {
                    if (boardData[(int)playerPos.x + i, (int)playerPos.y + j] == 3)
                        miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.green);
                    else if (boardData[(int)playerPos.x + i, (int)playerPos.y + j] == 2)
                        miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.red);
                    else if (boardData[(int)playerPos.x + i, (int)playerPos.y + j] == 0)
                        miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.white);
                    else if (boardData[(int)playerPos.x + i, (int)playerPos.y + j] == 10)
                        miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.yellow);
                    else if (boardData[(int)playerPos.x + i, (int)playerPos.y + j] == 15)
                        miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.cyan);
                    else
                        miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.gray);

                    var curFog = fog[(int)playerPos.x + i + 1, (int)playerPos.y + j + 1].GetComponent<SpriteRenderer>(); //Fog Pass
                    if (curFog != null)
                    {
                        if (curFog.color.a > 0.8f) //Hidden and unseen
                        {
                            miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.black);
                        }
                        else if (curFog.color.a > 0.5f && curFog.color.a < 0.8f) //Seen, but currently hidden
                        {
                            if (boardData[(int)playerPos.x + i, (int)playerPos.y + j] != 1)
                                miniTex.SetPixel(i + mapRadius, j + mapRadius, Color.white);

                            miniTex.SetPixel(i + mapRadius, j + mapRadius, miniTex.GetPixel(i + mapRadius, j + mapRadius) - new Color(0.2f, 0.2f, 0.2f, 0));
                        }
                    }
                }
            }
        }
        miniTex.Apply();
    }

    /// <summary>
    /// Creates a copy of the board from boardManager
    /// </summary>
    /// <returns></returns>
    private int[,] createMapBoard()
    {
        int[,] curBoard = board.getBoard(); //Code gymnastics is fun.
        return curBoard;
    }

    /// <summary>
    /// Creates a copy of the fog matrix from BoardManager
    /// </summary>
    /// <returns></returns>
    private GameObject[,] createFogTile()
    {
        GameObject[,] fogTiles = board.getFogTiles();
        return fogTiles;
    }

    /// <summary>
    /// Grabs the player position. Done here since the player object in BoardManager is weird
    /// </summary>
    /// <returns></returns>
    private Vector2 getPlayerPosition()
    {
        Vector2 position;
        Transform player = GameObject.Find("Player").transform;
        position = new Vector2(player.position.x, player.position.y);
        return position;
    }

    /// <summary>
    /// Grabs the positions of all enemies
    /// </summary>
    /// <returns></returns>
    public List<Vector2> GetEnemyPositions()
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        List<Vector2> returnList = new List<Vector2>();
       foreach (GameObject t in enemyList)
        {
            Vector2 pos = new Vector2(t.transform.position.x, t.transform.position.y);
            returnList.Add(pos);
        }
        return returnList;
    }

    void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            //If the UI element needs to repeatedly update: Do it here.
            //updateMap();
            UpdateMiniMap();
        }
    }

}
