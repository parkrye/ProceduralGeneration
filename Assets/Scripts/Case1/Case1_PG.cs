using System.Collections.Generic;
using UnityEngine;

namespace Case1
{
    public class Case1_PG : MonoBehaviour
    {
        [SerializeField] GameObject PlayerPrefab, RoomPrefab, RoadPrefab;
        [SerializeField] int width, height, depth;
        [SerializeField] int[,,] map;
        [SerializeField] List<(int, int, int)> roomList;

        int roomCount;

        void Awake()
        {
            if(width < 10) width = 10;
            if(height < 10) height = 10;
            if(depth < 1) depth = 1;

            roomCount = (width * height) << 3;

            map = new int[width, height, depth];
            roomList = new List<(int, int, int)> ();
        }

        void Start()
        {
            // Room Setting
            for(int i = 0; i < depth; i++)
            {
                RoomSetting(i, 0, roomCount);
            }

            // Road Setting
            RoadSetting();

            // Instantiate Room, Road and Player
            InstantiateObjects();
        }

        void RoomSetting(int depth, int count, int limit)
        {
            Stack<(int, int, int, int)> roomStack = new Stack<(int, int, int, int)>();
            roomStack.Push((0, width, 0, height));

            // �� ���ѷ��� �߻�! ���� �ذ� ���
            while (roomStack.Count > 0 && count < limit)
            {
                (int xLow, int xHigh, int yLow, int yHigh) = roomStack.Pop();

                // ���� ���� �� ��ġ
                int nextX = Random.Range(xLow, xHigh);
                int nextY = Random.Range(yLow, yHigh);

                // x, y ������ �����Ѵٸ� ����
                if (xHigh - xLow > 1 || yHigh - yLow > 1)
                {
                    roomStack.Push((xLow,          nextX, yLow,          nextY));
                    roomStack.Push((xLow,          nextX, yHigh - nextY, yHigh));
                    roomStack.Push((xHigh - nextX, xHigh, yLow,          nextY));
                    roomStack.Push((xHigh - nextX, xHigh, yHigh - nextY, yHigh));
                }

                // ���� ��ġ�� ����ִٸ� �� ����
                if (map[nextX, nextY, depth] == 0)
                {
                    map[nextX, nextY, depth] = 2;
                    roomList.Add((nextX, nextY, depth));
                    count++;
                }
            }
        }

        void RoadSetting()
        {

        }

        void InstantiateObjects()
        {

        }
    }

}