using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameTimeX.Objects
{
    public class GTXComponent<TSelf> where TSelf : GTXComponent<TSelf>, new()
    {
        [JsonIgnore]
        public string RawValue { get; set; }


        public GTXComponent()
        {
            RawValue = string.Empty;
        }

        public GTXComponent(string rawValue) 
        {
            RawValue = rawValue;
        }

        public TSelf Dezerialize()
        {
            var instance = JsonSerializer.Deserialize<TSelf>(RawValue);

            if(instance is null)
                instance = new TSelf();

            return instance;
        }

        public string Serialize()
        {
            string rawValue = JsonSerializer.Serialize<TSelf>((TSelf)this);

            this.RawValue = rawValue;

            return rawValue;
        }
    }
}
