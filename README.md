# <img width="32" height="32" src="https://github.com/user-attachments/assets/07d946a6-a868-4683-9d0d-1ec349290672" /> 工業物聯網樞紐 ![GitHub watchers](https://img.shields.io/github/watchers/FongYuanChen/IIoTHub) ![GitHub forks](https://img.shields.io/github/forks/FongYuanChen/IIoTHub) ![GitHub Repo stars](https://img.shields.io/github/stars/FongYuanChen/IIoTHub) ![GitHub last commit](https://img.shields.io/github/last-commit/FongYuanChen/IIoTHub) ![GitHub License](https://img.shields.io/github/license/FongYuanChen/IIoTHub)

**工廠設備監控與資料整合中樞！**

> 隨著工廠自動化程度提升，設備種類與數量持續的增加，不同設備、通訊方式與資料格式的整合，成為現場系統建置與維護上的一大挑戰。缺乏統一的管理核心，往往導致系統耦合度高、擴充困難，增加後續維運成本。

> 本專案以「可擴充的設備驅動器架構」為核心，打造一個工業物聯網樞紐，集中管理設備狀態、運行紀錄與即時資料。在此架構下，新增或調整設備僅需實作對應的驅動器模組，即可無縫整合至現有系統，使整體平台能隨著設備種類與工廠需求的變化持續擴充。


## 🎨 功能特色

- **設備監控總覽**：統一掌握工廠內多種設備的狀態、運行紀錄與即時資料，提供完整的監控視圖。
- **可擴充驅動器架構**：設備通訊邏輯以模組化驅動器實作，新增或調整設備僅需擴充對應的驅動器，無需修改核心系統。
- **多設備、多協議支援**：整合不同通訊協議與設備類型，靈活應對工廠需求。


## 🔗 數據來源

- **通訊協議支援**：支援 OPC UA 等多種通用工業協議。
- **專用SDK整合**：整合 Focas（Fanuc CNC）、FCSB1224W000（Mitsubishi CNC）等多種專用 SDK。
- **外部系統接入**：可接入 MES、ERP 或其他工廠資訊系統，擴展資料來源與管理能力。


## 🛠 核心技術

- **語言框架**：基於 C#、.NET 8、WPF，採 MVVM 架構設計。
- **依賴注入與服務管理**：使用 [Microsoft.Extensions.DependencyInjection](https://github.com/dotnet/runtime) 與 [Microsoft.Extensions.Hosting](https://github.com/dotnet/runtime) 管理服務生命週期與模組化架構。
- **設定管理與持久化**：使用 [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) 進行資料序列化與設定存取。
- **資料管理與持久化**：使用 [Microsoft.Data.Sqlite](https://github.com/dotnet/dotnet) 儲存歷史運行紀錄。
- **IPC 通訊架構**：採用 `Named Pipe` 作為 Driver Host IPC 機制進行跨進程通訊，可同時支援 x86 / x64 進程，以對應不同第三方位元需求。
- **應用單例啟動**：應用啟動時檢查已有實例，避免多個實例同時運行。
- **通知區圖示與操作**：使用 [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon) 建立通知區圖示及右鍵選單，提供快捷操作。


## 🧩 擴充驅動指南

1. 在 `IIoTHub.Infrastructure` 專案的 `Drivers` 資料夾建立新驅動器類別。
2. 根據設備類型實作對應介面：
   - 加工機驅動器：實作 `IMachineDriver`
   - 料倉驅動器：實作 `IMagazineDriver`
   - 機械手臂驅動器：實作 `IRobotDriver`
3. 驅動器參數設定
   - 使用 `[ParameterSetting]` 屬性標記驅動器中的相關參數屬性，這些屬性會在系統 UI 顯示給使用者設定。
   - 範例：
     ```csharp
     [ParameterSetting("快照變更間隔", "單位: 秒", "60")]
     public int SnapshotChangeInterval { get; set; }
     ```
   - 參數說明：
     1. 第一個字串：顯示名稱
     2. 第二個字串：說明文字
     3. 第三個字串：預設值

4. 範例驅動（DemoDriver)
   ```csharp
   public class DemoDriver : IMachineDriver
   {
       public string Name => "DEMO 機台專用驅動器";

       [ParameterSetting("快照變更間隔", "單位: 秒", "60")]
       public int SnapshotChangeInterval { get; set; } = 60;

       public MachineSnapshot GetSnapshot(DeviceSetting deviceSetting)
       {
           // 實作取得指定設備的快照
       }
   }


## 📄 API 文件

本專案提供 Swagger UI 作為 API 文件與測試工具。  
專案執行後，可透過 `http://localhost:5000/swagger/index.html` 存取 Swagger UI。

![Swagger UI Screenshot](https://github.com/user-attachments/assets/de5cc319-cbb9-4c9a-8199-1740816f73eb)


## 🖥️ 操作演示

https://github.com/user-attachments/assets/5c51bf94-e111-4b3c-8cd9-7b19fcc2e683


## 📜 授權條款

本專案採用 [MIT](https://github.com/FongYuanChen/IIoTHub/blob/main/LICENSE) 授權條款。歡迎自由使用、修改與分享！
