using System.Text;
using System.Text.Json;


namespace Shared.Common.Serialization
{
    public class Serializer : ISerializer
    {
        public T DeserializeObject<T>(string input)
        {
            return JsonSerializer.Deserialize<T>(input);
        }
        public T DeserializeObject<T>(byte[] input)
        {
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(input));
        }

        public string SerializeObject<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public byte[] SerializeObjectToByteArray<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(SerializeObject(obj));
        }

    }
}
