﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurrentDungeonData : MonoBehaviour
{
    //Variables||

        //Main village id number
        public int mainVillageID = 1;

        //Dungeons List
        public GameObject dungeonsData;

        //Starting room
        private GameObject startingRoom;

        //The current room coordinates
        private Vector2Int currentRoom;

        //The next room teleport point
        private string nextRoomSide;

        //The player
        Rigidbody2D playerRB;

        //Current played dungeon
        Dungeon currentDungeon;
               
        private List<roomPos> map = new List<roomPos>();
    //_________||

    //Class to handle rooms position
    class roomPos
    {
        public Vector2Int position = new Vector2Int();
        public GameObject room;

        public roomPos(int x, int y, GameObject room)
        {
            position.x = x;
            position.y = y;
        }

    }

    private GameObject getRoomViaCords(Vector2Int cords)
    {
        GameObject foundRoom = new GameObject();

        //Loop through all the dungeon rooms that exist
        foreach (roomPos room in map)
        {
            if (room.position == cords)
            {
                foundRoom = room.room;
            }
        }

        return foundRoom;
    }

    public void changeNextRoom(string roomSide)
    {

        //Calculate on which side from the next room should the player be spawned in
        switch (roomSide)
        {
            case "right":
                nextRoomSide = "left";
                currentRoom.x++;
                break;

            case "left":
                nextRoomSide = "right";
                currentRoom.x--;
                break;

            case "top":
                nextRoomSide = "bottom";
                currentRoom.y++;
                break;

            case "bottom":
                nextRoomSide = "top";
                currentRoom.y--;
                break;

            case "exit":
                StartCoroutine("LoadMainMap", mainVillageID);
                break;

            default:
                nextRoomSide = null;
                break;
        }


        //Add some sort of transition

        //Destroy all the current existing dungeon rooms
        foreach (GameObject room in GameObject.FindGameObjectsWithTag("dungeonRoom"))
        {
            Destroy(room);
        }

        GameObject roomToCreate = getRoomViaCords(currentRoom);

        Debug.Log(roomToCreate);

        //Check if it needs to create new rooms
        Instantiate(roomToCreate, Vector2.zero, Quaternion.identity);

        //Teleport the player to the correct side
    }

    private void createNewRoom(Vector2Int roomLocation)
    {
        //TODO
        int randomNum = 1;
        int a = Random.Range(0, 10);

        GameObject chosenRoom = currentDungeon.rooms[randomNum].roomPrefab;

        map.Add(new roomPos(roomLocation.x, roomLocation.y, chosenRoom));

    }
    private void getCorrectDungeon(List<Dungeon> theList, string wantedResult)
    {
        //Check if the list is empty
        if (theList.Count > 0)
        {
            //Loop through all the dungeons
            foreach(Dungeon dungeon in theList)
            {
                if (dungeon.DungeonName == wantedResult)
                {
                    currentDungeon = dungeon;

                    //Spawn the starting room
                    startingRoom = Instantiate(dungeon.startingRoom, Vector2.zero, Quaternion.identity);


                    //ADD STARTING ROOM TO MAP||

                        //Add the starting room
                        map.Add(new roomPos(0, 0, startingRoom));

                        List<Dungeon.room> acceptedRooms = new List<Dungeon.room>();

                        //Add the following room
                        foreach (Dungeon.room room in currentDungeon.rooms)
                        {
                            //Check if the room has a bottom dooor
                            if (room.roomSides.bottom)
                            {
                                acceptedRooms.Add(room);
                            }
                        }

                        int chooseRandom = Random.Range(0, acceptedRooms.Count);
                        map.Add(new roomPos(0, 1, acceptedRooms[chooseRandom].roomPrefab));
                    //________________________||


                    //Set the current coordinates
                    currentRoom = new Vector2Int(0, 0);

                    return;
                }
            }
        }
        else
        {
            Debug.Log("No dungeons found");
        }

    }


    private void SpawnPlayer()
    {
        foreach (Transform child in startingRoom.transform)
        {
            if(child.gameObject.name == "TeleportLocations")
            {
                foreach (Transform location in child)
                {
                    if (location.gameObject.name == "exit")
                    {
                        playerRB.position = location.position;
                    }
                }
            }
        }

    }

    private void Start()
    {
        //Get the list of dungeons
        List<Dungeon> dungeonsList = dungeonsData.GetComponent<DungeonsAPI>().dungeons;

        //Get the player body
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        //Change the second argument when needed
        getCorrectDungeon(dungeonsList, "Deadmines");
        SpawnPlayer();
    }







    //Variables for the loading screen||

        public GameObject background;
        public GameObject loadingScreen;
        public GameObject loadingBar;
    //________________________________||

    IEnumerator LoadMainMap(int scene)
    {
        //Get the loading screen transparency
        CanvasGroup backAlpha = background.GetComponent<CanvasGroup>();
        backAlpha.alpha = 0;

        //Turn the loading screen on
        loadingScreen.SetActive(true);

        //Add some sort of transition
        while (backAlpha.alpha < 1)
        {
            backAlpha.alpha += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }



        loadingBar.SetActive(true);

        //Just wait
        yield return new WaitForSeconds(1);


        //Start actually loading the new scene
        AsyncOperation opereration = SceneManager.LoadSceneAsync(scene);

        //Track the new scene progress
        while (!opereration.isDone)
        {
            //Calculate current progress
            float CurrentProgress = Mathf.Clamp(opereration.progress / 0.9f, 1, 2);

            //Get the bar fill object
            GameObject barFill = loadingBar.GetComponent<Transform>().GetChild(0).gameObject;

            //Change the bar size
            barFill.GetComponent<Image>().fillAmount = CurrentProgress;

            //Restart the loop
            yield return null;
        }

    }

}
