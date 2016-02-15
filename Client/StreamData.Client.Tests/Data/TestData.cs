namespace StreamData.Client.Tests.Data
{
    public static class TestData
    {
        public static string ORIGINAL_DATA =
            @"[
{""title"":""Value 0"",""price"":60,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 1"",""price"":58,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 2"",""price"":44,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 3"",""price"":40,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 4"",""price"":78,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 5"",""price"":37,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 6"",""price"":10,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 7"",""price"":69,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 8"",""price"":62,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 9"",""price"":22,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 10"",""price"":96,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 11"",""price"":28,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 12"",""price"":77,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 13"",""price"":15,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""},
{""title"":""Value 14"",""price"":22,""param1"":""value1"",""param2"":""value2"",""param3"":""value3"",""param4"":""value4"",""param5"":""value5"",""param6"":""value6"",""param7"":""value7"",""param8"":""value8""}]";

        public static string PATCH_1 = @"[{""op"":""replace"",""path"":""/0/price"",""value"":95}]";
        public static string PATCH_2 = @"[{""op"":""replace"",""path"":""/4/price"",""value"":0},{""op"":""replace"",""path"":""/12/price"",""value"":13},{""op"":""replace"",""path"":""/14/price"",""value"":27}]";
        public static string PATCH_3 = @"[{""op"":""replace"",""path"":""/1/price"",""value"":80},{""op"":""replace"",""path"":""/3/price"",""value"":59},{""op"":""replace"",""path"":""/8/price"",""value"":75}]";
 
    }
}
