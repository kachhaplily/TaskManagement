using System.Text.Json.Serialization;

namespace TaskManagement.Model;



public class Enumdata
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskPriority
    {

        Low = -1,

        Medium = 0,

        High = 1
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]

    public enum TaskStatus
    {
        Todo,
        inProgress,
        Completed,


    }


}

