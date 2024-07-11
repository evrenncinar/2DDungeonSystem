using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] float cameraPosDuration = 0.2f; // Duration for the camera to move to the position
    [SerializeField] Ease cameraPosEase = Ease.OutCubic; // Effect for the camera to move to the position
    [SerializeField] int RoomCount = 10; // Number of rooms to be created

    [Header("Prefabs")]
    [SerializeField] GameObject roomPrefab; // Room prefab
    [SerializeField] GameObject NextDoorPrefab; // Next room door
    [SerializeField] GameObject PreviusDoorPrefab; // Previous room door

    [Header("Room Settings")]
    [SerializeField] float roomWidth = 10f; // Room width
    [SerializeField] float roomHeight = 5.7f; // Room height
    [SerializeField] float roomSpacing = 2f; // Spacing between rooms
    [SerializeField] float doorSpacingY = 2.3f; // Spacing between doors (Y-axis)
    [SerializeField] float doorSpacingX  = 4.55f; // Spacing between doors (X-axis)

    private GameObject lastRoom; // Reference to the last created room
    int lastDoorIndex; // Door number of the last created room
    Vector3 lastRoomPosition; // Position of the last created room

    void Start()
    {
        CreateRoom(Vector3.zero); // Creates a room at the center when the game starts.
        StartCoroutine(CreateRoomTest()); // Creates a room every second for testing purposes.
    }

    private IEnumerator CreateRoomTest() // Creates a room every second for testing purposes.
    {
        for (int i = 0; i < RoomCount; i++)
        {
            yield return new WaitForSeconds(1f);
            CreateNextRoom();
        }
    }

    public void CreateNextRoom()
    {
        Vector3 newPosition = GetNewRoomDirection(lastDoorIndex, lastRoomPosition); // Gets the position for the new room based on the last room's transform.
        Destroy(lastRoom,1f); // Destroys the last created room.
        CreateRoom(newPosition); // Creates a new room at the specified position.
    }

    void CreateRoom(Vector3 position)
    {
        GameObject newRoom = Instantiate(roomPrefab, position, Quaternion.identity); // Creates a room at the specified position.
        Vector3 newRoomPos = new Vector3(newRoom.transform.position.x, newRoom.transform.position.y, newRoom.transform.position.z); // Gets the position of the room.
        Vector3 CameraRoomPosition = new Vector3(newRoom.transform.position.x, newRoom.transform.position.y, -10); // Gets the camera position for the room.
        int doorNumber = GetDoorPos(); // Gets the door number for the room.
        PlaceDoor(doorNumber, newRoomPos, newRoom, NextDoorPrefab); // Creates the door for the room.
        if (lastDoorIndex > 0) { PlaceDoor(PreviusDoorIndex(lastDoorIndex), newRoomPos, newRoom, PreviusDoorPrefab); } // If there is a previous room, creates the previous room door.
        lastRoom = newRoom; lastDoorIndex = doorNumber; lastRoomPosition = newRoomPos; // Saves the last created room and door number.
        Camera.main.transform.DOLocalMove(CameraRoomPosition, cameraPosDuration).SetEase(cameraPosEase); // Moves the camera to the position of the last created room.
    }

    void PlaceDoor(int doorNumberIndex, Vector3 doorPosition, GameObject Room, GameObject DoorPrefab)
    {
        Vector3 newPosition = Vector3.zero; // Resets the door position.
        Quaternion newQueternion = Quaternion.identity; // Resets the door rotation.
        switch (doorNumberIndex) // Determines the door position based on the door number.
        {
            case 1:
                newPosition = new Vector3(doorPosition.x, doorPosition.y + doorSpacingY, doorPosition.z);
                break;
            case 2:
                newPosition = new Vector3(doorPosition.x, doorPosition.y - doorSpacingY, doorPosition.z);
                break;
            case 3:
                newQueternion = Quaternion.Euler(0, 0, 90); // Rotates the 3rd door 90 degrees to the right.
                newPosition = new Vector3(doorPosition.x - doorSpacingX, doorPosition.y, doorPosition.z);
                break;
            case 4:
                newQueternion = Quaternion.Euler(0, 0, 90); // Rotates the 4th door 90 degrees to the left.
                newPosition = new Vector3(doorPosition.x + doorSpacingX, doorPosition.y, doorPosition.z);
                break;
            default:
                break;
        }
        GameObject door = Instantiate(DoorPrefab, newPosition, newQueternion, Room.transform); // Creates the door.
    }

    private Vector3 GetNewRoomDirection(int DoorIndex, Vector3 RoomPosition)
    {
        Vector3 direction = Vector3.zero; // Resets the room position.
        float RoomSpace = 0f; // Resets the room spacing.
        switch (DoorIndex) // Determines the room position based on the door number.
        {
            case 1:
                RoomSpace = roomHeight + roomSpacing;
                direction = new Vector3(direction.x, direction.y + RoomSpace, direction.z);
                break;
            case 2:
                RoomSpace = roomHeight + roomSpacing;
                direction = new Vector3(direction.x, direction.y - RoomSpace, direction.z);
                break;
            case 3:
                RoomSpace = roomWidth + roomSpacing;
                direction = new Vector3(direction.x - RoomSpace, direction.y, direction.z);
                break;
            case 4:
                RoomSpace = roomWidth + roomSpacing;
                direction = new Vector3(direction.x + RoomSpace, direction.y, direction.z);
                break;
        }
        Vector3 newPosition = RoomPosition + direction;
        return newPosition; // Returns the new position.
    }

    private int GetDoorPos()
    {
        int doorNumber = 0;
        bool isplace = false;
        while (!isplace) // Checks to ensure the door does not lead to the previous room.
        {
            doorNumber = Random.Range(1, 5);
            isplace = isPlaceDoor(doorNumber, lastDoorIndex); // Ensures the door does not lead to the previous room.
        }
        return doorNumber; // Returns the door number.
    }

    private bool isPlaceDoor(int newDoorIndex, int LastDoorIndex) // Ensures the door does not lead to the previous room.
    {
        if(newDoorIndex == 1 && LastDoorIndex == 2) { return false; }
        if(newDoorIndex == 2 && LastDoorIndex == 1) { return false; }
        if(newDoorIndex == 3 && LastDoorIndex == 4) { return false; }
        if(newDoorIndex == 4 && LastDoorIndex == 3) { return false; }
        return true;
    }

    private int PreviusDoorIndex(int DoorIndex) // Returns the door number of the previous room.
    {
        int PreviusDoor = 0;
        switch (DoorIndex)
        {
            case 1:
                PreviusDoor = 2;
                break;
            case 2:
                PreviusDoor = 1;
                break;
            case 3:
                PreviusDoor = 4;
                break;
            case 4:
                PreviusDoor = 3;
                break;
        }
        return PreviusDoor;
    }
}
