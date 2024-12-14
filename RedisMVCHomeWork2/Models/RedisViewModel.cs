using StackExchange.Redis;

namespace RedisMVCHomeWork2.Models
{
    public class RedisViewModel
    {
        public RedisValue[]? RedisValues { get; set; }
        public List<string> Messages { get; set; }
    }
}
