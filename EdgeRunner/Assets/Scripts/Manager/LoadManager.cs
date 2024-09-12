using System.Collections.Generic;
using UnityEngine;

public class StageStaticState
{
    public int Width;
    public int Height;
    public int StartIdx;
}

public class LevelGroupState
{
    public int LevelCnt;
    public int Speed;
    public int Thorn;
    public int Wall;
    public int Crack;
    public int MinInterval;
    public int ExcludedVerticesCnt;
    public int StartNextIdxCnt;
}

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance;

    public static StageStaticState StaticState;
    public LevelGroupState[] LevelGroupStates;
    public List<int> LevelGroup;
    public int LevelGroupCntTotal;
    public int LevelCntTotal;

    private void Awake()
    {
        Instance = this;
        loadStaticDataset();
        loadLevelGroupDataset();
    }

    void Start()
    {
        //logStaticDataset();
        //logLevelGroupDataset();
        //logLevelInLevelGroup();
    }
    private void loadStaticDataset()
    {
        TextAsset csvData = Resources.Load("StaticDataset") as TextAsset;

        if (csvData != null)
        {
            string[] data = csvData.text.Split(new char[] { '\n' });
            string[] elements = data[1].Split(new char[] { ',' });

            StaticState = new StageStaticState
            {
                Width = int.Parse(elements[0]),
                Height = int.Parse(elements[1]),
                StartIdx = int.Parse(elements[2])
            };

            //Debug.Log($"Success Load StaticDataset!");
        }
        else
        {
            //Debug.LogError("Failed to load dataset.");
        }
    }

    private void loadLevelGroupDataset()
    {
        TextAsset csvData = Resources.Load("LevelGroupDataset") as TextAsset;

        if (csvData != null)
        {
            string[] data = csvData.text.Split(new char[] { '\n' });
            LevelGroupCntTotal = data.Length;
            LevelGroupStates = new LevelGroupState[LevelGroupCntTotal - 1];
            LevelGroup = new();
            for (int i = 1; i < LevelGroupCntTotal; i++)
            {
                string[] elements = data[i].Split(new char[] { ',' });
                if (elements[0] != "")
                {
                    int NowLevelGroup = int.Parse(elements[0]);
                    LevelGroupState StageGroupInfo = new LevelGroupState
                    {
                        LevelCnt = int.Parse(elements[1]),
                        Speed = int.Parse(elements[2]),
                        Thorn = int.Parse(elements[3]),
                        Wall = int.Parse(elements[4]),
                        Crack = int.Parse(elements[5]),
                        MinInterval = int.Parse(elements[6]),
                        ExcludedVerticesCnt = int.Parse(elements[7]),
                        StartNextIdxCnt = int.Parse(elements[8])
                    };

                    for (int j = 0; j < StageGroupInfo.LevelCnt; j++)
                    {
                        LevelGroup.Add(NowLevelGroup);
                    }
                    LevelCntTotal += StageGroupInfo.LevelCnt;
                    LevelGroupStates[NowLevelGroup] = StageGroupInfo;
                }
            }

            //Debug.Log($"Success Load StageGroupDataset!");
        }
        else
        {
            //Debug.LogError("Failed to load StageGroupDataset.");
        }
    }

    private void logStaticDataset()
    {
        Debug.Log($"[Stage Static State] Width : {StaticState.Width}, Height : {StaticState.Height}, StartIdx : {StaticState.StartIdx}");
    }

    private void logLevelGroupDataset()
    {
        for (int i = 0; i < LevelGroupCntTotal - 1; i++)
        {
            Debug.Log($"[Level Group State : {i}] Stage Count : {LevelGroupStates[i].LevelCnt}, Speed : {LevelGroupStates[i].Speed}, Thorn : {LevelGroupStates[i].Thorn}, Wall : {LevelGroupStates[i].Wall}, Crack : {LevelGroupStates[i].Crack}, MinInterval : {LevelGroupStates[i].MinInterval}, ExcludedVerticesCnt : {LevelGroupStates[i].ExcludedVerticesCnt}, StartNextIdxCnt : {LevelGroupStates[i].StartNextIdxCnt}");
        }
    }

    private void logLevelInLevelGroup()
    {
        int start = 0;
        int nowLevelGroup = 0;

        for (int i = 0; i < LevelCntTotal; i++)
        {
            if ((i == LevelCntTotal - 1) && (LevelGroup[i] == nowLevelGroup))
            {
                Debug.Log($"Level Group {LevelGroup[i]} : {start} ~ {i} ({i - start + 1}개)");
            }
            else if ((i == LevelCntTotal - 1) && (LevelGroup[i] != nowLevelGroup))
            {
                Debug.Log($"Level Group {nowLevelGroup} : {start} ~ {i - 1} ({i - start}개)");
                Debug.Log($"Level Group {LevelGroup[i]} : {i} ~ {i} (1개)");
            }
            else if (LevelGroup[i] != nowLevelGroup)
            {
                Debug.Log($"Level Group {nowLevelGroup} : {start} ~ {i - 1} ({i - start}개)");
                nowLevelGroup = LevelGroup[i];
                start = i;
            }
        }
    }
}