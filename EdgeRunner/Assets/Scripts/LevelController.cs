using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Diagnostics;
using Enums;

public class LevelState
{
    public int NowLevel;
    public int StartNextIdxCnt;
    public int Thorn;
    public int Crack;
    public int MinInterval;
    public int ExcludedVerticesCnt;
    public int NowLevelGroup;
    public int NextStartIdx;
    public int NextStartArea;
    public int NextGoalIdx;
}

class StageState
{
    public int Width;
    public int Height;
    public int LevelCntTotal;
    public int LevelGroupCntTotal;
}

public class LevelController : MonoBehaviour
{
    StageState StaticStageState;
    LevelState NowLevelState;
    List<int> AllIdx;
    List<List<Tuple<int, PlatformType>>> Cases;
    List<int> Vertices;
    double ElapsedTime;
    List<PlatformType> Result;
    List<int> StartNextIdx;
    int MAXCASES = 10000; // 10000:0.008s 100000:0.06s

    void Start()
    {
        StaticStageState = new StageState
        {
            Width = LoadManager.StaticState.Width,
            Height = LoadManager.StaticState.Height,
            LevelCntTotal = LoadManager.Instance.LevelCntTotal,
            LevelGroupCntTotal = LoadManager.Instance.LevelGroupCntTotal
        };

        List<int> PriorGoalList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 27, 28, 29, 30, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };
        List<PlatformType> test;

       /* for (int i = 0; i < PriorGoalList.Count; i++)
        {
            test = RandomFunction(i-1, PriorGoalList[i]);
        }*/
    }

    public List<PlatformType> RandomFunction(int PriorLevel, int PriorGoal)
    {
        UnityEngine.Debug.Log($"Now PriorLevel : {PriorLevel}, PriorGoal : {PriorGoal}");
        NowLevelState = new();

        setNewStageState(PriorLevel, PriorGoal);
        logNewStageState();

        setStartNextIdx();
        //logStartNextIdx();

        setAllIdx();
        //logAllIdx();

        removeIdx();
        //logRemoveIdx();

        findAllCases();
        logFindAllCases();
        //logAllCases();

        if (Cases.Count > 0)
        {
            selectCase();
            logResult();
        }
        else
        {
            if (PriorLevel == -1)
            {
                RandomFunction(-1, PriorGoal);

            }
            else
            {
                RandomFunction(0, PriorGoal);
            }
        }
        return Result;

    }

    /*
     * 현재 게임 상태 : Prior 데이터 기반 현재 Level 정보 세팅
     */
    private void setNewStageState(int PriorLevel, int PriorGoal)
    {
        NowLevelState.NowLevel = PriorLevel + 1;

        if (NowLevelState.NowLevel >= StaticStageState.LevelCntTotal)
        {
            NowLevelState.NowLevelGroup = LoadManager.Instance.LevelGroupStates.Length - 1;
        }
        else
        {
            NowLevelState.NowLevelGroup = LoadManager.Instance.LevelGroup[NowLevelState.NowLevel];
        }

        NowLevelState.StartNextIdxCnt = LoadManager.Instance.LevelGroupStates[NowLevelState.NowLevelGroup].StartNextIdxCnt;

        NowLevelState.Thorn = LoadManager.Instance.LevelGroupStates[NowLevelState.NowLevelGroup].Thorn;
        NowLevelState.Crack = LoadManager.Instance.LevelGroupStates[NowLevelState.NowLevelGroup].Crack;

        NowLevelState.MinInterval = LoadManager.Instance.LevelGroupStates[NowLevelState.NowLevelGroup].MinInterval;
        NowLevelState.ExcludedVerticesCnt = LoadManager.Instance.LevelGroupStates[NowLevelState.NowLevelGroup].ExcludedVerticesCnt;

        if (NowLevelState.NowLevel == 0)
        {
            NowLevelState.NextStartIdx = LoadManager.StaticState.StartIdx;
            //NowLevelState.NextStartIdx = 6;
        }
        else
        {
            NowLevelState.NextStartIdx = PriorGoal;
        }
        NowLevelState.NextStartArea = getArea(NowLevelState.NextStartIdx);
        NowLevelState.NextGoalIdx = getExit(NowLevelState.NextStartArea);
    }

    /*
    * 해당 인덱스가 속해 있는 변의 위치 : (0-상, 1-우, 2-하, 3-좌)
    */
    private int getArea(int NowIdx)
    {
        if (NowIdx < StaticStageState.Width)
        {
            return 0;
        }
        else if (NowIdx < StaticStageState.Width + StaticStageState.Height)
        {
            return 1;
        }
        else if (NowIdx < StaticStageState.Width + StaticStageState.Height + StaticStageState.Width)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    /*
     * 시작점 오른쪽 인덱스 구하기 (개수 : StartNextIdxCnt 개)
     */
    private void setStartNextIdx()
    {
        StartNextIdx = new();

        int Idx = NowLevelState.NextStartIdx;
        int LimitIdx = (StaticStageState.Width + StaticStageState.Height) * 2;
        int PriorIdx = Idx == 0 ? LimitIdx-1 : Idx-1;

        while (StartNextIdx.Count < NowLevelState.StartNextIdxCnt)
        {
            Idx++;

            if (Idx >= LimitIdx)
            {
                Idx = Idx - LimitIdx -1;
            }
            else
            {
                StartNextIdx.Add(Idx);
            }
        }

        StartNextIdx.Add(PriorIdx);
    }

    /*
    * 출구 인덱스 : 시작 인덱스를 기준으로 왼쪽 영역에 해당하는 인덱스로 설정
    */
    private int getExit(int StartArea)
    {
        if (StartArea == 1)
        {
            return UnityEngine.Random.Range(1, StaticStageState.Width - 1);
        }
        else if (StartArea == 2)
        {
            return UnityEngine.Random.Range(StaticStageState.Width + 1, StaticStageState.Width + StaticStageState.Height - 1);
        }
        else if (StartArea == 3)
        {
            return UnityEngine.Random.Range(StaticStageState.Width + StaticStageState.Height + 1, StaticStageState.Width + StaticStageState.Height + StaticStageState.Width - 1);
        }

        return UnityEngine.Random.Range(StaticStageState.Width + StaticStageState.Height + StaticStageState.Width + 1, (StaticStageState.Width + StaticStageState.Height) * 2 - 1);
    }

    /*
     * 현재 Level의 Platform List 선택 : 여러 경우의 수 중 하나 선택
     */
    private List<PlatformType> selectCase()
    {
        int size = (StaticStageState.Width + StaticStageState.Height) * 2;

        Result = Enumerable.Repeat(PlatformType.Normal, size).ToList();

        int SelectedIdx = UnityEngine.Random.Range(0, Cases.Count);
        List<Tuple<int, PlatformType>> selectedCase = Cases[SelectedIdx];

        foreach (var item in selectedCase)
        {
            Result[item.Item1] = item.Item2;
        }

        Result[NowLevelState.NextStartIdx] = PlatformType.Start;
        Result[NowLevelState.NextGoalIdx] = PlatformType.Goal;

        return Result;
    }

    /*
     * 전체 인덱스 리스트 반환
     */
    private List<int> setAllIdx()
    {
        AllIdx = new();
        int maxLimit = (StaticStageState.Width + StaticStageState.Height) * 2;

        for (int i = 0; i < maxLimit; i++)
        {
            AllIdx.Add(i);
        }

        return AllIdx;
    }

    /*
     * 인덱스 제외 : 모서리, 출발지, 출발지 근처, 도착지 
     */
    private void removeIdx()
    {
        // 모서리 제외
        Vertices = new();
        for (int i = 0; i < NowLevelState.ExcludedVerticesCnt; i++)
        {
            Vertices.Add(0 + i);
            Vertices.Add(StaticStageState.Width - 1 - i);
            Vertices.Add(StaticStageState.Width + i);
            Vertices.Add(StaticStageState.Width + StaticStageState.Height - 1 - i);
            Vertices.Add(StaticStageState.Width + StaticStageState.Height + i);
            Vertices.Add(StaticStageState.Width + StaticStageState.Height + StaticStageState.Width - 1 - i);
            Vertices.Add(StaticStageState.Width + StaticStageState.Height + StaticStageState.Width + i);
            Vertices.Add((StaticStageState.Width + StaticStageState.Height) * 2 - 1 - i);
        }
        AllIdx.RemoveAll(idx => Vertices.Contains(idx));

        // 출발지 제외
        AllIdx.Remove(NowLevelState.NextStartIdx);

        // 출발지 근처 제외
        AllIdx.RemoveAll(idx => StartNextIdx.Contains(idx));

        // 도착지 제외
        AllIdx.Remove(NowLevelState.NextGoalIdx);
    }

    /*
     * 배치 데이터 생성
     */
    private void findAllCases()
    {
        Cases = new();
        List<Tuple<int, PlatformType>> CurrentPlacement = new();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        //int startIdx = UnityEngine.Random.Range(0, AllIdx.Count);
        findPlacements(0, NowLevelState.Thorn, NowLevelState.Crack, CurrentPlacement);

        stopwatch.Stop();
        long elapsedTicks = stopwatch.ElapsedTicks;
        ElapsedTime = (double)elapsedTicks / Stopwatch.Frequency;
    }

    /*
     * 조건을 만족하는 모든 오브젝트 배치 데이터 생성
     */
    private void findPlacements(int index, int remainingThorn, int remainigCrack, List<Tuple<int, PlatformType>> CurrentPlacement)
    {
        if (remainingThorn == 0 && remainigCrack == 0)
        {
            // 모든 Thorn과 Crack이 배치되었으면, 현재 배치를 결과에 추가
            if (Cases.Count < MAXCASES)
            {
                Cases.Add(new List<Tuple<int, PlatformType>>(CurrentPlacement));
            }
            return;
        }

        for (int i = index; i < AllIdx.Count; i++)
        {
            int pos = AllIdx[i];

            if (IsValidPlacement(pos, CurrentPlacement))
            {
                if (NowLevelState.NowLevelGroup < 3 && !isBetweenStartAndGoal(pos))
                {
                    continue;
                }

                if (remainingThorn > 0)
                {
                    // Thorn 배치
                    CurrentPlacement.Add(Tuple.Create(pos, PlatformType.Thorn));
                    findPlacements(i + 1, remainingThorn - 1, remainigCrack, CurrentPlacement);
                    CurrentPlacement.RemoveAt(CurrentPlacement.Count - 1);

                }
                else if (remainigCrack > 0)
                {
                    if (AllIdx.Contains(pos + 1))
                    {
                        // Crack 배치
                        CurrentPlacement.Add(Tuple.Create(pos, PlatformType.Crack));
                        CurrentPlacement.Add(Tuple.Create(pos + 1, PlatformType.Crack));
                        findPlacements(i + 2, remainingThorn, remainigCrack - 1, CurrentPlacement);
                        CurrentPlacement.RemoveRange(CurrentPlacement.Count - 2, 2);
                    }
                }
            }

            if (Cases.Count >= MAXCASES)
            {
                return;
            }
        }
    }

    private bool isBetweenStartAndGoal(int pos)
    {
        int maxValue = (StaticStageState.Width + StaticStageState.Height) * 2 - 1;

        // 0 <= startIdx < pos < endIdx <= maxValue
        if (NowLevelState.NextStartIdx < NowLevelState.NextGoalIdx && NowLevelState.NextStartIdx <= pos && pos < NowLevelState.NextGoalIdx)
        {
            return true;
        }

        // 0 <= endIdx < startIdx < pos <= maxValue
        if (NowLevelState.NextGoalIdx < NowLevelState.NextStartIdx && NowLevelState.NextStartIdx <= pos && pos <= maxValue)
        {
            return true;
        }

        // 0 <= pos < endIdx < startIdx <= maxValue
        if (NowLevelState.NextGoalIdx < NowLevelState.NextStartIdx && 0 <= pos && pos < NowLevelState.NextGoalIdx)
        {
            return true;
        }

        return false;
    }

    /*
     * 현재 배치된 위치들과 최소 간격 조건을 비교
     */
    private bool IsValidPlacement(int pos, List<Tuple<int, PlatformType>> CurrentPlacement)
    {
        foreach (var placement in CurrentPlacement)
        {
            int placePos = placement.Item1;
            if (Mathf.Abs(pos - placePos) <= NowLevelState.MinInterval)
            {
                return false;
            }
        }
        return true;
    }

    private void logNewStageState()
    {
        UnityEngine.Debug.Log($"[SetNewStageState] NowLevel : {NowLevelState.NowLevel}, StartNextIdxCnt : {NowLevelState.StartNextIdxCnt}, " +
            $"NowLevelGroup : {NowLevelState.NowLevelGroup}, Thorn : {NowLevelState.Thorn}, Crack : {NowLevelState.Crack}, " +
            $"MinInterval : {NowLevelState.MinInterval}, ExcludedVerticesCnt : {NowLevelState.ExcludedVerticesCnt}, " +
            $"NextStartIdx : {NowLevelState.NextStartIdx} NextStartArea : {NowLevelState.NextStartArea}, NextGoalIdx : {NowLevelState.NextGoalIdx}");
    }

    private void logAllIdx()
    {
        UnityEngine.Debug.Log($"[SetAllIdx] AllIdx : {string.Join(", ", AllIdx)}");
    }

    private void logRemoveIdx()
    {
        Vertices.Sort();
        UnityEngine.Debug.Log($"[RemoveIdx] Vertices : {string.Join(", ", Vertices)} (모서리의 총 개수 : {Vertices.Count}개)");
        UnityEngine.Debug.Log($"[RemoveIdx] NextStartIdx : {NowLevelState.NextStartIdx}, NextGoalIdx : {NowLevelState.NextGoalIdx}");
        UnityEngine.Debug.Log($"[RemoveIdx] AllIdx : {string.Join(", ", AllIdx)} (남은 칸의 개수 : {AllIdx.Count}개)");
    }

    private void logFindAllCases()
    {
        UnityEngine.Debug.Log($"경우의 수 : {Cases.Count} 개, 걸린 시간 : {ElapsedTime} s");
    }

    private void logAllCases()
    {
        foreach(List<Tuple<int, PlatformType>> Case in Cases)
        {
            UnityEngine.Debug.Log($"[AllCases]{string.Join(", ", Case)}");
        }
    }

    private void logResult()
    {
        StringBuilder sb = new StringBuilder();

        int index = 0;
        bool isWidthLine = true;

        while (index < Result.Count)
        {
            int count = isWidthLine ? StaticStageState.Width : StaticStageState.Height;

            for (int i = 0; i < count && index < Result.Count; i++)
            {
                sb.Append($"({index})");
                sb.Append(Result[index]);

                if (i < count - 1)
                {
                    sb.Append(", ");
                }

                index++;
            }

            sb.AppendLine();
            isWidthLine = !isWidthLine;
        }

        UnityEngine.Debug.Log(sb.ToString());
    }

    private void logStartNextIdx()
    {
        UnityEngine.Debug.Log($"[SetStartNextIdx] Indices Next to StartIdx : {string.Join(", ", StartNextIdx)} (총 개수 : {StartNextIdx.Count}개)");
    }
}