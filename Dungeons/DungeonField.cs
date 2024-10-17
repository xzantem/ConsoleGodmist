using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Dungeons
{
    public abstract class DungeonField(DungeonFieldType fieldType)
    {
        public DungeonFieldType FieldType {get;private set;} = fieldType;
        public bool Revealed {get; private set;} = false;

        public void Reveal() {
            Revealed = true;
        }

        public void Clear()
        {
            FieldType = DungeonFieldType.Empty;
        }
    }
}