using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutomationFramework.EntityFramework
{
    public static class JsonExtensions
    {
        public static T FromJson<T>(this string input) where T : class
        {
            return JsonSerializer.Deserialize<T>(input);
        }

        public static string ToJson<T>(this T input) where T : class
        {
            return JsonSerializer.Serialize<T>(input);
        }
    }
}
