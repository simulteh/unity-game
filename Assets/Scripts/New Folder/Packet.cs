public class Packet
{
    public string SourceAddress { get; set; }
    public string DestinationAddress { get; set; }
    public int TTL { get; set; }
    public string Data { get; set; } // Можно добавить поле для данных

    public Packet(string sourceAddress, string destinationAddress, int ttl, string data = "")
    {
        SourceAddress = sourceAddress;
        DestinationAddress = destinationAddress;
        TTL = ttl;
        Data = data;
    }

    public void DecrementTTL()
    {
        TTL--;
    }

    public override string ToString()
    {
        return $"Packet(Source: {SourceAddress}, Destination: {DestinationAddress}, TTL: {TTL}, Data: {Data})";
    }
}
