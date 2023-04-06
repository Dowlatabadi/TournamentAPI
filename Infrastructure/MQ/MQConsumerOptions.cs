namespace Infrastructure.MQ;

public class MQConsumerOptions
{
    public int port { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string uri { get; set; }
    public string virtualhost { get; set; }
    public int heartbeat { get; set; }
    public string queuename { get; set; }

}
