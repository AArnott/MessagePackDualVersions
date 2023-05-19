using System.Runtime.Serialization;

namespace App;

[DataContract]
public class DataObject
{
    [DataMember]
    public string Message { get; set; }
}
