namespace Southport.ResilientHttp
{
    public interface ISouthportConvert
    {
        string SerializeObject<T>(T data);
        T DeserializeObject<T>(string data);

    }
}