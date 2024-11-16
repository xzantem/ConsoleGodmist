namespace ConsoleGodmist.Utilities;

public interface IDataPersistent
{
    void LoadData(SaveData data);
    void SaveData(SaveData data);
    void AddToPersistanceManager();
}