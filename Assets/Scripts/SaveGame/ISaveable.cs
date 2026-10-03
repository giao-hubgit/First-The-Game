public interface ISaveable
{
    // Đưa dữ liệu hiện tại trong Game vào GameData để chuẩn bị Save
    void PopulateSaveData(GameData data);

    // Đọc dữ liệu từ GameData ra để cập nhật trạng thái Game
    void LoadFromSaveData(GameData data);
}