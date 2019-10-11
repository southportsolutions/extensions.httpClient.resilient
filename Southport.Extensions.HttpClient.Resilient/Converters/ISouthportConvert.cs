namespace Southport.Extensions.HttpClient.Resilient.Converters
{
    public interface ISouthportConvert
    {
        string SerializeObject<T>(T data);
        T DeserializeObject<T>(string data);

    }
}