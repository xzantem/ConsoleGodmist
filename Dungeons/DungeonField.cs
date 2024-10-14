using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Dungeons
{
    public class DungeonField {
        public DungeonFieldType FieldType {get;private set;}
        public bool Revealed {get; private set;}
        public DungeonField(DungeonFieldType fieldType) {
            FieldType = fieldType;
            Revealed = false;
        }
        public void Reveal() {
            Revealed = true;
        }

        public void Clear()
        {
            FieldType = DungeonFieldType.Empty;
        }
    }
}