# STEVE RUNNER 

> **Bài tập cá nhân:** Xây dựng Game 2D Endless Runner trên Unity  
> **Chủ đề:** Chrome Dino Runner × Minecraft Theme (4 Biome)  
> **Engine:** Unity 2022.3 LTS (2D Physics & URP/Built-in)  
> **Ngôn ngữ:** C#  

---

##  I. TỔNG QUAN DỰ ÁN

**Minecraft Dino Runner 2D** là tựa game chạy vô tận (Endless Runner) lấy cảm hứng từ trò chơi Chrome Dino kinh điển trên trình duyệt, kết hợp với phong cách đồ họa, âm thanh và thế giới khối vuông của **Minecraft**.

Người chơi sẽ điều khiển nhân vật **Steve** chạy liên tục qua các vùng đất (Biome) huyền thoại, nhảy hoặc trượt cúi để vượt qua hàng loạt bẫy đất, hố sập và quái bay. Game tự động tăng tốc độ và đổi Biome theo điểm số.

---

##  II. HƯỚNG DẪN ĐIỀU KHIỂN (GAMEPLAY CONTROLS)

| Thao tác | Phím thực hiện | Tác dụng |
| :--- | :--- | :--- |
| **Nhảy (Jump)** | `Space` / `W` / `Mũi tên lên (↑)` | Nhảy qua bẫy mặt đất, hố sập & chim bay thấp |
| **Cúi / Trượt (Slide)** | `Left Ctrl` / `S` / `Mũi tên xuống (↓)` | Thu nhỏ Hitbox né chim bay tầm trung |
| **Bắt đầu / Tương tác** | `Mouse Click` / Phím UI | Chọn Play, Restart, Return Menu trên UI |

---

## 🗺️ III. HỆ THỐNG BIOME & ĐỘ KHÓ (MAP PROGRESSION)

Game tự động chuyển đổi giữa **4 Biome Minecraft** cùng hiệu ứng đổi nhạc nền (BGM) và mượt màu bầu trời (Camera Color Lerp):

| Giai đoạn | Mốc điểm | Biome Minecraft | Tốc độ trôi | Âm thanh BGM | Đặc điểm bẫy |
| :---: | :---: | :---: | :---: | :---: | :---: |
| **1** | `0 – 1499` |  **Overworld** | `6.0 – 8.5` | Theme Xanh lá | Xương rồng đơn/đôi, bẫy bay thưa |
| **2** | `1500 – 2999` |  **Nether** | `8.5 – 10.5` | Theme Đỏ địa ngục | Hố sập, bẫy bay xuất hiện dày hơn |
| **3** | `3000 – 4499` |  **Sculk / Deep Dark** | `10.5 – 12.5` | Theme Xanh đen | Bẫy liên hoàn, tốc độ nhanh |
| **4** | `4500+` |  **The End** | `12.5 – 14.0` *(Max)* | Theme Tím | Tốc độ tối đa, tần suất spawn dồn dập |

---

## IV. TÍNH NĂNG NỔI BẬT (KEY FEATURES)

### 1. Gameplay & Vật lý Player (Steve)
- **Grounded Check chính xác:** Dùng `Physics2D.OverlapBox` kiểm tra chân chạm đất, không bị nhảy đúp vô lý.
- **Dynamic Hitbox Crouch:** Co giãn `BoxCollider2D` kích thước `crouchSize` & `crouchOffSet` khi trượt cúi, nếp gập Sprite tự động khớp với Hitbox.
- **Continuous Collision:** Khóa không xuyên đất, chống giật khựng với Vật liệu `NoFriction`.

### 2. Hệ thống Bẫy & Spawner thông minh (`SpawnManager`)
- Bộ bẫy riêng biệt cho từng Map (`MapObstacleSet`).
- Thả bẫy Đất (1m/2m), Hố sập (Pit) và Bẫy Bay ở **3 độ cao** (Thấp - Nhảy, Vừa - Cúi, Cao - Đứng yên).
- **Gia tốc sinh bẫy:** Tốc độ game càng nhanh, thời gian thả bẫy càng giảm (`minDelay/speedMultiplier`).

### 3. Hệ thống Băng tải Tilemap vô tận (`TilemapManager`)
- Kỹ thuật **2-Chunk Conveyor**: Luân phiên 2 segment Tilemap nối đuôi nhau giúp game chạy dài vô hạn mà không tốn dung lượng RAM.

### 4. Điểm số & Kỷ lục (`ScoreManager`)
- Định dạng chuẩn Retro **8 chữ số** (`00000150`).
- Tự động lưu High Score xuống bộ nhớ bằng `PlayerPrefs.Save()` ngay khi va chạm.
- **Milestone Sound:** Tự động phát tiếng `Ting EXP` mỗi khi chạy đạt thêm **100 điểm**.

### 5. UI & Visual Polish
- **Fake Freeze Frame & Screen Masking:** Dừng thế giới trôi lập tức khi chết, hoãn 0.5s cho Animation Die diễn trọn vẹn rồi mới phủ bảng Game Over.
- **Adaptive Game Over:** Tự động đổi tên Map/Biome vừa chết trên bảng Game Over.
- **Camera Color Changer:** Chuyển màu nền bầu trời mượt mà (Color Lerp) theo màu chuẩn Hex của từng Biome.

### 6. Âm thanh sống động (`AudioManager`)
- **BGM:** Tự đổi bài nhạc nền chủ đề riêng khi chuyển Biome.
- **SFX:** Đầy đủ tiếng Nhảy, Chết ("Oof!"), Click UI, Tiếng thưởng mốc điểm 100.

---

##  V. KIẾN TRÚC CODE & CẤU TRÚC DỰ ÁN
Assets/
├── Scripts/
│ ├── Managers/
│ │ ├── GameManager.cs // State Machine (Menu/Playing/GameOver), quản lý tốc độ
│ │ ├── AudioManager.cs // Singleton phát BGM theo Map + SFX
│ │ ├── ScoreManager.cs // Tính điểm D8, lưu PlayerPrefs, phát Milestone SFX
│ │ ├── SpawnManager.cs // Sinh bẫy thông minh theo MapObstacleSet
│ │ └── TilemapManager.cs // Băng tải 2-Chunk cuộn vô tận
│ ├── Player/
│ │ └── PlayerControl.cs // Điều khiển Steve, OverlapBox, Crouch Hitbox, Reset state
│ ├── Obstacles/
│ │ └── Obstacle.cs // Trôi theo currentSpeed & tự hủy X < -15
│ └── Environment/
│ └── CameraColorChanger.cs // Lerp màu bầu trời mượt theo Biome
├── Prefabs/
│ ├── Player/
│ ├── Chunks/ // 4-7 Prefabs Tilemap Chunk
│ └── Obstacles/ // Prefabs Bẫy Đất, Hố sập, Chim bay
├── Audio/
│ ├── BGM/ // Nhạc nền 4 Biome
│ └── SFX/ // Jump, Die (Oof), Click, Milestone
├── Sprites/
├── Animations/
└── Scenes/
└── Gameplay.unity

text


---

## VI. HƯỚNG DẪN CHẠY GAME

### Cách 1: Chạy file Build (.exe)
1. Mở thư mục `Build/`.
2. Chạy file `Assighmen.exe`.
3. Chơi game ở độ phân giải 16:9 (Recommeded: 1920×1080).

### Cách 2: Chạy trong Unity Editor
1. Mở Unity Hub -> Add project từ thư mục nguồn.
2. Mở Scene: `Assets/Scenes/Gameplay.unity`.
3. Nhấn nút **Play** (`Ctrl + P`) trong Editor để trải nghiệm.

---
## VII. Video Gameplay   
https://drive.google.com/file/d/1kX7z37Jy_aRx6B0YWi0pf-3ygwSEyGJz/view?usp=drive_link


## 👤 VII. THÔNG TIN HỌC VIÊN NỘP BÀI

- NGUYỄN ANH TUẤN
- 114010125011 
- K25ISTG01
- 2/10/2026
