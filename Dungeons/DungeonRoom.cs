using System.Drawing;
using System.Dynamic;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Dungeons
{
    public class DungeonRoom {
        public int SizeX {get; protected init;}
        public int SizeY {get; protected init;}
        public int Index {get; protected init;}
        public DungeonField[,] Contents {get; set;}
        public Direction EntranceDirection {get; protected set;}
        
        public Dictionary<int[,], int[]> Exits { get; set; } // Key: Local Coordinates, Value: Entered Room Index
        public int CornerX {get; protected init;}
        public int CornerY  {get; protected init;}
        public DungeonRoom(int cornerX, int cornerY, int index, int sizeX, int sizeY, Direction entranceDirection) {
            CornerX = cornerX;
            CornerY = cornerY;
            SizeX = sizeX;
            SizeY = sizeY;
            Index = index;
            EntranceDirection = entranceDirection;
            Contents = new DungeonField[SizeX, SizeY];
            Exits = new Dictionary<int[,], int[]>();
            for (int i = 0; i < SizeX; i++) {
                for (int j = 0; j < SizeY; j++) {
                    Contents[i,j] = new DungeonField(DungeonFieldType.Empty);
                }
            }
            if (index == 0) 
                Contents[(SizeX / 2) - 1, SizeY - 1] = new DungeonField(DungeonFieldType.Exit);
        }
        public string ShowRectangle() {
            return ("(" + CornerX + ", " + CornerY + ") (" + (CornerX + SizeX) + ", " + CornerY + ")\n(" + CornerX + ", " + (CornerY - SizeY) + ") (" + (CornerX + SizeX) + ", " + (CornerY - SizeY) + ")");
        }
    }
}