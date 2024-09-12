using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlatformInfo
{
    public PlatformType platformType;
    public GameObject platformPrefab;
}

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public int MapWidth;
    public int MapHeight;

    public event Action<Color> OnColorChanged;

    [SerializeField] private List<PlatformInfo> platformPrefabList;
    private Dictionary<PlatformType, GameObject> platformPairs = new Dictionary<PlatformType, GameObject>();
    private List<List<Platform>> currentPlatforms = new List<List<Platform>>();

    [SerializeField] private List<Transform> changeGravityTransforms;

    [SerializeField] private List<Color> colorList;

    public int CurrentColor { get; private set; }

    [SerializeField] private LevelController levelController;
    List<PlatformType> platformList;

    public int CurrentGoalIndex {  get; private set; }  


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach(PlatformInfo info in platformPrefabList)
        {
            platformPairs.Add(info.platformType, info.platformPrefab);
        }
    }

    public void GetPlatformListFromController(int priorLevel, int priorGoal)
    {
        platformList = levelController.RandomFunction(priorLevel, priorGoal);
    }

    public void GetNewGoalColor()
    {
        int randomNum = UnityEngine.Random.Range(0, colorList.Count);
        if(randomNum == CurrentColor)
        {
            randomNum = (randomNum + 1) % colorList.Count;
        }
        CurrentColor = randomNum;
    }

    private void Start()
    {
        GameManager.Instance.OnGameRestart += MapManager_OnGameRestart;

        GenerateInitialMap();
    }

    private void MapManager_OnGameRestart()
    {
        GenerateInitialMap();
    }

    public void GenerateNewLevel(int priorLevel, int priorGoal)
    {
        GetPlatformListFromController(priorLevel, priorGoal);
        GetNewGoalColor();
        GenerateMap();
    }

    public void GenerateMap()
    {
        foreach(List<Platform> list in currentPlatforms)
        {
            foreach (Platform platform in list)
            {
                if(platform != null && platform.gameObject != null)
                    Destroy(platform.gameObject);
            }
        }

        currentPlatforms.Clear();

        for(int i = 0; i < 4; i ++)
        {
            currentPlatforms.Add(new List<Platform>());
        }

        GameObject platformPrefab = null;
        int index = 0;
        foreach (PlatformType platformType in platformList)
        {
            switch(platformType)
            {
                case PlatformType.Start:
                    platformPrefab = GetPrefabWithType(PlatformType.Start);
                    break;
                case PlatformType.Normal:
                    platformPrefab = GetPrefabWithType(PlatformType.Normal);
                    break;
                case PlatformType.Thorn:
                    platformPrefab = GetPrefabWithType(PlatformType.Thorn);
                    break;
                case PlatformType.Crack:
                    platformPrefab = GetPrefabWithType(PlatformType.Crack);
                    break;
                case PlatformType.Goal:
                    CurrentGoalIndex = index;
                    platformPrefab = GetPrefabWithType(PlatformType.Goal);
                    break;
            }

            if(index <= MapWidth)
            {
                Vector2 newPlatformPosition = new Vector3(-MapWidth / 2 + index, MapHeight / 2);

                Platform newAbovePlatform = Instantiate(platformPrefab, newPlatformPosition, Quaternion.Euler(new Vector3(0, 0, 0))).GetComponent<Platform>();
                currentPlatforms[0].Add(newAbovePlatform);
                if (newAbovePlatform is GoalPlatform)
                {
                    ((GoalPlatform)newAbovePlatform).ChangeColor(colorList[CurrentColor]);
                }
            }
            else if(index <= MapWidth + MapHeight)
            {
                Vector2 newPlatformPosition = new Vector3(MapWidth / 2, MapHeight / 2 - (index - MapWidth) + 1);

                Platform newAbovePlatform = Instantiate(platformPrefab, newPlatformPosition, Quaternion.Euler(new Vector3(0, 0, -90))).GetComponent<Platform>();
                currentPlatforms[1].Add(newAbovePlatform);
                if (newAbovePlatform is GoalPlatform)
                {
                    ((GoalPlatform)newAbovePlatform).ChangeColor(colorList[CurrentColor]);
                }
            }
            else if(index <= MapWidth * 2 + MapHeight + 1)
            {
                Vector2 newPlatformPosition = new Vector3(MapWidth / 2 - (index - (MapWidth + MapHeight + 2)) - 1, -MapHeight / 2);

                Platform newAbovePlatform = Instantiate(platformPrefab, newPlatformPosition, Quaternion.Euler(new Vector3(0, 0, -180))).GetComponent<Platform>();
                currentPlatforms[2].Add(newAbovePlatform);
                if (newAbovePlatform is GoalPlatform)
                {
                    ((GoalPlatform)newAbovePlatform).ChangeColor(colorList[CurrentColor]);
                }
            }
            else
            {
                Vector2 newPlatformPosition = new Vector3(-MapWidth / 2, -MapHeight / 2 + (index - (MapWidth * 2 + MapHeight + 1)) - 1);

                Platform newAbovePlatform = Instantiate(platformPrefab, newPlatformPosition, Quaternion.Euler(new Vector3(0, 0, -270))).GetComponent<Platform>();
                currentPlatforms[3].Add(newAbovePlatform);
                if (newAbovePlatform is GoalPlatform)
                {
                    ((GoalPlatform)newAbovePlatform).ChangeColor(colorList[CurrentColor]);
                }
            }

            index++;
            //Debug.Log($"Index: {index}");
        }

        
        //for(int i = -MapWidth / 2; i <= MapWidth / 2; i++)
        //{
        //    // Test Code
        //    float value = UnityEngine.Random.Range(0f, 100f);
        //    GameObject prefab;
        //    if (value < 10)
        //    {
        //        prefab = GetPrefabWithType(PlatformType.Normal);
        //    }
        //    else if (value < 15)
        //    {
        //        prefab = GetPrefabWithType(PlatformType.Thorn);
        //    }
        //    else if (value < 20)
        //    {
        //        prefab = GetPrefabWithType(PlatformType.Goal);
        //    }
        //    else
        //    {
        //        prefab = GetPrefabWithType(PlatformType.Normal);
        //    }

        //    Platform newAbovePlatform = Instantiate(prefab, new Vector3(i, MapHeight / 2), Quaternion.Euler(new Vector3(0, 0, 0))).GetComponent<Platform>();
        //    currentPlatforms[0].Add(newAbovePlatform);
        //    if(newAbovePlatform is GoalPlatform)
        //    {
        //        ((GoalPlatform)newAbovePlatform).ChangeColor(colorList[CurrentColor]);
        //    }

        //    Platform newBelowPlatform = Instantiate(prefab, new Vector3(-i, -MapHeight / 2), Quaternion.Euler(new Vector3(0, 0, -180))).GetComponent<Platform>();
        //    currentPlatforms[2].Add(newBelowPlatform);
        //    if (newBelowPlatform is GoalPlatform)
        //    {
        //        ((GoalPlatform)newBelowPlatform).ChangeColor(colorList[CurrentColor]);
        //    }
        //}

        //for (int i = -MapHeight / 2; i <= MapHeight / 2; i++)
        //{
        //    float value = UnityEngine.Random.Range(0f, 100f);
        //    GameObject prefab;
        //    if (value < 10)
        //    {
        //        prefab = GetPrefabWithType(PlatformType.Normal);
        //    }
        //    else if (value < 15)
        //    {
        //        prefab = GetPrefabWithType(PlatformType.Thorn);
        //    }
        //    else if (value < 20)
        //    {
        //        prefab = GetPrefabWithType(PlatformType.Goal);
        //    }
        //    else
        //    {
        //        prefab = GetPrefabWithType(PlatformType.Normal);
        //    }

        //    Platform newRightPlatform = Instantiate(prefab, new Vector3(MapWidth / 2, -i), Quaternion.Euler(new Vector3(0, 0, -90))).GetComponent<Platform>();
        //    currentPlatforms[1].Add(newRightPlatform);
        //    if (newRightPlatform is GoalPlatform)
        //    {
        //        ((GoalPlatform)newRightPlatform).ChangeColor(colorList[CurrentColor]);
        //    }

        //    Platform newLeftPlatform = Instantiate(prefab, new Vector3(-MapWidth / 2, i), Quaternion.Euler(new Vector3(0, 0, -270))).GetComponent<Platform>();
        //    currentPlatforms[3].Add(newLeftPlatform);
        //    if (newLeftPlatform is GoalPlatform)
        //    {
        //        ((GoalPlatform)newLeftPlatform).ChangeColor(colorList[CurrentColor]);
        //    }
        //}

        //changeGravityTransforms[0].position = new Vector3(MapWidth / 2 + 2.6f, MapHeight / 2 + 3);
        //changeGravityTransforms[1].position = new Vector3(MapWidth / 2 + 3, -MapHeight / 2 - 2.8f);
        //changeGravityTransforms[2].position = new Vector3(-MapWidth / 2 - 2.6f, -MapHeight / 2 - 3);
        //changeGravityTransforms[3].position = new Vector3(-MapWidth / 2 - 3, MapHeight / 2 + 2.8f);
    }

    public void GenerateInitialMap()
    {
        foreach (List<Platform> list in currentPlatforms)
        {
            foreach (Platform platform in list)
            {
                if (platform != null && platform.gameObject != null)
                    Destroy(platform.gameObject);
            }
        }

        currentPlatforms.Clear();

        for (int i = 0; i < 4; i++)
        {
            currentPlatforms.Add(new List<Platform>());
        }

        for (int i = -MapWidth / 2; i <= MapWidth / 2; i++)
        {
            // Test Code
            float value = UnityEngine.Random.Range(0f, 100f);
            GameObject prefab;
            if (value < 10)
            {
                prefab = GetPrefabWithType(PlatformType.Normal);
            }
            else if (value < 15)
            {
                prefab = GetPrefabWithType(PlatformType.Normal);
            }
            else if (value < 20)
            {
                prefab = GetPrefabWithType(PlatformType.Normal);
            }
            else
            {
                prefab = GetPrefabWithType(PlatformType.Normal);
            }

            Platform newAbovePlatform = Instantiate(prefab, new Vector3(i, MapHeight / 2), Quaternion.Euler(new Vector3(0, 0, 0))).GetComponent<Platform>();
            currentPlatforms[0].Add(newAbovePlatform);
            if (newAbovePlatform is GoalPlatform)
            {
                ((GoalPlatform)newAbovePlatform).ChangeColor(colorList[CurrentColor]);
            }

            Platform newBelowPlatform = Instantiate(prefab, new Vector3(-i, -MapHeight / 2), Quaternion.Euler(new Vector3(0, 0, -180))).GetComponent<Platform>();
            currentPlatforms[2].Add(newBelowPlatform);
            if (newBelowPlatform is GoalPlatform)
            {
                ((GoalPlatform)newBelowPlatform).ChangeColor(colorList[CurrentColor]);
            }
        }

        for (int i = -MapHeight / 2; i <= MapHeight / 2; i++)
        {
            float value = UnityEngine.Random.Range(0f, 100f);
            GameObject prefab;
            if (value < 10)
            {
                prefab = GetPrefabWithType(PlatformType.Normal);
            }
            else if (value < 15)
            {
                prefab = GetPrefabWithType(PlatformType.Normal);
            }
            else if (value < 20)
            {
                prefab = GetPrefabWithType(PlatformType.Normal);
            }
            else
            {
                prefab = GetPrefabWithType(PlatformType.Normal);
            }

            Platform newRightPlatform = Instantiate(prefab, new Vector3(MapWidth / 2, -i), Quaternion.Euler(new Vector3(0, 0, -90))).GetComponent<Platform>();
            currentPlatforms[1].Add(newRightPlatform);
            if (newRightPlatform is GoalPlatform)
            {
                ((GoalPlatform)newRightPlatform).ChangeColor(colorList[CurrentColor]);
            }

            Platform newLeftPlatform = Instantiate(prefab, new Vector3(-MapWidth / 2, i), Quaternion.Euler(new Vector3(0, 0, -270))).GetComponent<Platform>();
            currentPlatforms[3].Add(newLeftPlatform);
            if (newLeftPlatform is GoalPlatform)
            {
                ((GoalPlatform)newLeftPlatform).ChangeColor(colorList[CurrentColor]);
            }
        }

    }




    public GameObject GetPrefabWithType(PlatformType type)
    {
        return platformPairs[type];
    }

    public Color GetColorFromIndex(int index)
    {
        return colorList[index]; 
    }

}
