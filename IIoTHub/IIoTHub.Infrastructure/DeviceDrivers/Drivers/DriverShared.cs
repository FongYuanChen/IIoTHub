namespace IIoTHub.Infrastructure.DeviceDrivers.Drivers
{
    public static class DriverShared
    {
        /// <summary>
        /// 取得指定參數的字串值。
        /// 若參數不存在，或值為 null / 空白字串，則回傳預設值。
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetString(IDictionary<string, string> parameters, string key, string defaultValue)
        {
            return parameters.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
                ? value
                : defaultValue;
        }

        /// <summary>
        /// 取得指定參數的整數值。
        /// 若參數不存在，或值無法解析為整數，則回傳預設值。
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetInt(IDictionary<string, string> parameters, string key, int defaultValue)
        {
            return parameters.TryGetValue(key, out var value) && int.TryParse(value, out var result)
                ? result
                : defaultValue;
        }

        /// <summary>
        /// 取得指定參數的列舉值。
        /// 若參數不存在，或值無法解析為指定的列舉型別，則回傳預設值。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TEnum GetEnum<TEnum>(IDictionary<string, string> parameters, string key, TEnum defaultValue) where TEnum : struct, Enum
        {
            return parameters.TryGetValue(key, out var value) && Enum.TryParse<TEnum>(value, out var result)
                ? result
                : defaultValue;
        }
    }
}
