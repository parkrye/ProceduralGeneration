using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Case2
{
    public enum StructureType { None, Room, Road, Stair }

    public class Structure
    {
        int depth;
        (int x, int y) position;
        StructureType type;

        public int Depth { get { return depth; } }
        public (int x, int y) Position { get { return position; } }
        public StructureType Type { get { return type; } set { type = value; } }

        public Structure(int _depth, (int, int) _position, StructureType _type = StructureType.None)
        {
            depth = _depth;
            position = _position;
            type = _type;
        }
    }

    class PGNode
    {
        PGNode parent;
        PGNode[] children;
        List<Structure> rooms;

        public PGNode Parent { get { return parent; } }
        public PGNode[] Children { get { return children; } set { children = value; } }
        public List<Structure> Rooms { get { return rooms; } set { rooms = value; } }
        public bool IsRoot { get { return parent == null; } }
        public bool IsLeaf { get { return children[0] == null; } }

        public PGNode(PGNode _parent = null)
        {
            parent = _parent;
            children = new PGNode[2];
            rooms = new List<Structure>();
        }

        public int GetRoomCount()
        {
            if(IsLeaf)
                return rooms.Count;

            return children[0].GetRoomCount() + children[1].GetRoomCount();
        }
    }

    public class Case2_PG : MonoBehaviour
    {
        [SerializeField] GameObject RoomPrefab, RoadPrefab, StairPrefab;

        Structure[,,] map;
        PGNode[] roots;
        public Structure[,,] Map { get { return map; } }

        [SerializeField][Range(1, 10)] int depth;
        [SerializeField][Range(100, 200)] int size;
        [SerializeField][Range(1, 10)] int quantity;
        [SerializeField][Range(100, 200)] int maxRoomSize;
        float calcQuantity;

        void Awake()
        {
            map = new Structure[depth, size, size];
            roots = new PGNode[depth];

            calcQuantity = maxRoomSize - maxRoomSize * quantity * 0.1f;
        }

        void Start()
        {
            Generate();
        }

        void Generate()
        {
            for(int deep = 0; deep < depth; deep++)
            {
                roots[deep] = new PGNode();
                RoomGenerate(deep, 0, size, 0, size, roots[deep]);
                RoadGenerate(roots[deep]);
            }

            StairGenerate();

            CreateMap();
        }

        void RoomGenerate(int deep, int x1, int x2, int y1, int y2, PGNode parent)
        {
            int area = GetArea(x1, x2, y1, y2);

            if (area < calcQuantity || x1 >= x2 - 1 || y1 >= y2 - 1)
                return;

            if (area <= maxRoomSize)
            {
                for (int xi = x1; xi < x2; xi++)
                {
                    for(int  yi = y1; yi < y2; yi++)
                    {
                        map[deep, xi, yi] = new Structure(deep, (xi, yi), StructureType.Room);
                        parent.Rooms.Add(map[deep, xi, yi]);
                    }
                }
                return;
            }

            PGNode child1 = new(parent), child2 = new(parent);
            parent.Children[0] = child1; parent.Children[1] = child2;

            if (x2 - x1 >= y2 - y1)
            {
                int nextX = Random.Range(x1, x2);
                RoomGenerate(deep, nextX + 1, x2, y1, y2, child1);
                RoomGenerate(deep, x1, nextX - 1, y1, y2, child2);
            }
            else
            {
                int nextY = Random.Range(y1, y2);
                RoomGenerate(deep, x1, x2, nextY + 1, y2, child1);
                RoomGenerate(deep, x1, x2, y1, nextY - 1, child2);
            }
        }

        void RoadGenerate(PGNode parent)
        {
            if (parent.IsLeaf)
                return;

            if (parent.Children[0].IsLeaf)
            {
                Structure roomA = parent.Children[0].Rooms[0];
                Structure roomB = parent.Children[1].Rooms[0];

                // x ���̰� 0�϶�
                // y ���̰� 0�϶�
                // �� �� 0�� �ƴ� ��

                int xDiff = roomA.Position.x - roomB.Position.x;
                int yDiff = roomA.Position.y - roomB.Position.y;
                float gradient = yDiff / xDiff;

                // ��ȭ�� ���� 1 �ö�����
                // �ٸ��� ���� n.5���
                // n, n+1�� room
                
                return;
            }

            RoadGenerate(parent.Children[0]);
            RoadGenerate(parent.Children[1]);
        }

        void StairGenerate()
        {

        }

        void CreateMap()
        {
            for(int deep = 0; deep < depth; deep++)
            {
                for (int xi = 0; xi < size; xi++)
                {
                    for (int yi = size; yi < size; yi++)
                    {
                        switch (map[deep, xi, yi].Type)
                        {
                            default:
                            case StructureType.None:
                                break;
                            case StructureType.Room:
                                Instantiate(RoomPrefab, new Vector2(xi, yi), Quaternion.identity, transform);
                                break;
                            case StructureType.Road:
                                Instantiate(RoadPrefab, new Vector2(xi, yi), Quaternion.identity, transform);
                                break;
                            case StructureType.Stair:
                                Instantiate(StairPrefab, new Vector2(xi, yi), Quaternion.identity, transform);
                                break;
                        }
                    }
                }
            }
        }

        int GetArea(int x1, int x2, int y1, int y2)
        {
            int xDiff = x1 <= x2 ? x2 - x1 : x1 - x2;
            int yDiff = y1 <= y2 ? y2 - y1 : y1 - y2;

            return xDiff * yDiff;
        }
    }

}