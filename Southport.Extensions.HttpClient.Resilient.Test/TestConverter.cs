using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Southport.Extensions.HttpClient.Resilient.Converters;

namespace Southport.Extensions.HttpClient.Resilient.Test
{
    class TestConverter : ISouthportConvert
    {
        public string SerializeObject<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public T DeserializeObject<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
