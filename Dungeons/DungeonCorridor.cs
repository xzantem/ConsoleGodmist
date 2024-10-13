using System.Drawing;
using System.Dynamic;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Dungeons
{
    public class DungeonCorridor : DungeonRoom {
        public DungeonCorridor(int cornerX, int cornerY, int index, int length, Direction entranceDirection) : base(cornerX, cornerY, index, 3, 3, entranceDirection) {
            CornerX = cornerX;
            CornerY = cornerY;
            Index = index;
            switch (entranceDirection) {
                case Direction.North:
                case Direction.South:
                SizeX = 3;
                SizeY = length;
                break;
                case Direction.East:
                case Direction.West:
                SizeX = length;
                SizeY = 3;
                break;
            }
            Contents = new DungeonField[SizeX, SizeY];
            for (int i = 0; i < SizeX; i++) {
                for (int j = 0; j < SizeY; j++) {
                    Contents[i,j] = new(DungeonFieldType.Empty);
                }
            }
        }
    }
}