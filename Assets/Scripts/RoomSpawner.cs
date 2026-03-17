using Unity.Mathematics;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int openingDir;
    private int rand;
    private bool spawned = false;

    private RoomTemplate template;

    void Awake()
    {
        template = GameObject.FindGameObjectWithTag("Room").GetComponent<RoomTemplate>();
    }

    void Start()
    {
        // Mẹo nhỏ: Cộng thêm một chút thời gian ngẫu nhiên để tránh các spawner chạy cùng 1 frame
        float randomDelay = 0.1f + UnityEngine.Random.Range(-0.02f, 0.02f);
        Invoke("Spawn", randomDelay);
    }

    void Spawn()
    {
        if (spawned == false)
        {
            if (!spawned && template.currentRooms < template.maxRooms)
            {
                if (openingDir == 1)
                {
                    rand = UnityEngine.Random.Range(0, template.botRoom.Length);
                    Instantiate(template.botRoom[rand], transform.position, template.botRoom[rand].transform.rotation);
                }
                else if (openingDir == 2)
                {
                    rand = UnityEngine.Random.Range(0, template.topRoom.Length);
                    Instantiate(template.topRoom[rand], transform.position, template.topRoom[rand].transform.rotation);
                }
                else if (openingDir == 3)
                {
                    rand = UnityEngine.Random.Range(0, template.leftRoom.Length);
                    Instantiate(template.leftRoom[rand], transform.position, template.leftRoom[rand].transform.rotation);
                }
                else if (openingDir == 4)
                {
                    rand = UnityEngine.Random.Range(0, template.rightRoom.Length);
                    Instantiate(template.rightRoom[rand], transform.position, template.rightRoom[rand].transform.rotation);
                }

                template.currentRooms++;
                spawned = true;
            }
            else if (!spawned && template.currentRooms >= template.maxRooms)
            {
                Instantiate(template.closedRoom, transform.position, Quaternion.identity);
            }
            spawned = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Kiểm tra xem Object chạm vào có tag SpawnPoint không
        if (collision.CompareTag("SpawnPoint"))
        {
            // 2. Lấy script RoomSpawner từ Object chạm vào
            RoomSpawner otherSpawner = collision.GetComponent<RoomSpawner>();

            // 3. Kiểm tra AN TOÀN: Đảm bảo script thực sự tồn tại (không bị Null)
            if (otherSpawner != null)
            {
                // Nếu cả 2 đều chưa sinh phòng
                if (otherSpawner.spawned == false && spawned == false)
                {
                    // Sinh ra phòng đóng (cụt) và tiêu hủy bản thân
                    Instantiate(template.closedRoom, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }

                // Đánh dấu là đã xử lý xong để không sinh thêm phòng đè lên
                spawned = true;
            }
            else
            {
                // Cảnh báo trên Console nếu bạn vô tình gắn nhầm tag
                Debug.LogWarning("Có một Object mang tag 'SpawnPoint' nhưng không có script RoomSpawner: " + collision.gameObject.name);
            }
        }
    }
}