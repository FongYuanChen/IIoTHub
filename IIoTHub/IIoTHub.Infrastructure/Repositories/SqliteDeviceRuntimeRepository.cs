using IIoTHub.Domain.Enums;
using IIoTHub.Domain.Interfaces.Repositories;
using IIoTHub.Domain.Models;
using Microsoft.Data.Sqlite;
using System.IO;

namespace IIoTHub.Infrastructure.Repositories
{
    public sealed class SqliteDeviceRuntimeRepository : IDeviceRuntimeRepository
    {
        private readonly string _connectionString
            = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deviceRuntime.db")}";

        public SqliteDeviceRuntimeRepository()
        {
            EnsureTable();
        }

        /// <summary>
        /// 確保 DeviceRuntimeRecords 資料表存在
        /// 若資料表不存在，自動建立
        /// </summary>
        private void EnsureTable()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                """
                CREATE TABLE IF NOT EXISTS DeviceRuntimeRecords (
                    DeviceId   TEXT NOT NULL,
                    RunStatus  INTEGER NOT NULL,
                    StartTime  TEXT NOT NULL,
                    EndTime    TEXT NOT NULL,
                    PRIMARY KEY (DeviceId, StartTime)
                );
                """;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 新增一筆設備運轉紀錄
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task AddAsync(DeviceRuntimeRecord record)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO DeviceRuntimeRecords
                (DeviceId, RunStatus, StartTime, EndTime)
                VALUES ($deviceId, $runStatus, $startTime, $endTime);
                """;
            command.Parameters.AddWithValue("$deviceId", record.DeviceId.ToString());
            command.Parameters.AddWithValue("$runStatus", (int)record.RunStatus);
            command.Parameters.AddWithValue("$startTime", record.StartTime);
            command.Parameters.AddWithValue("$endTime", record.EndTime);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 更新指定的設備運轉紀錄
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task UpdateAsync(DeviceRuntimeRecord record)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE DeviceRuntimeRecords
                SET EndTime = $endTime
                WHERE DeviceId = $deviceId AND StartTime = $startTime;
                """;
            command.Parameters.AddWithValue("$deviceId", record.DeviceId.ToString());
            command.Parameters.AddWithValue("$startTime", record.StartTime);
            command.Parameters.AddWithValue("$endTime", record.EndTime);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 取得指定設備在特定時間區間內的運轉紀錄
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<DeviceRuntimeRecord>> GetRecordsAsync(Guid deviceId, DateTime from, DateTime to)
        {
            var list = new List<DeviceRuntimeRecord>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT *
                FROM DeviceRuntimeRecords
                WHERE DeviceId = $deviceId
                  AND EndTime > $from
                  AND StartTime < $to
                ORDER BY StartTime;
                """;
            command.Parameters.AddWithValue("$deviceId", deviceId.ToString());
            command.Parameters.AddWithValue("$from", from);
            command.Parameters.AddWithValue("$to", to);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(ReadRecord(reader));
            }

            return list;
        }

        /// <summary>
        /// 取得指定設備最新的一筆運轉紀錄
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<DeviceRuntimeRecord> GetLatestRecordAsync(Guid deviceId)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT *
                FROM DeviceRuntimeRecords
                WHERE DeviceId = $deviceId
                ORDER BY StartTime DESC
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$deviceId", deviceId.ToString());

            using var reader = await command.ExecuteReaderAsync();
            if (!reader.Read())
                return null;
            return ReadRecord(reader);
        }

        /// <summary>
        /// 從 SQLite DataReader 讀取一筆資料並轉換為 DeviceRuntimeRecord
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static DeviceRuntimeRecord ReadRecord(SqliteDataReader reader)
        {
            return new DeviceRuntimeRecord(
                Guid.Parse(reader["DeviceId"].ToString()),
                (DeviceRunStatus)Convert.ToInt32(reader["RunStatus"]),
                DateTime.Parse(reader["StartTime"].ToString()),
                DateTime.Parse(reader["EndTime"].ToString())
            );
        }
    }
}
