namespace GameServer.Extensions
{
    public class PacketHandler : Attribute
    {
        public string Name { get; } = string.Empty;
        public PacketHandler() { }
        public PacketHandler(string name)
        {
            Name = name;
        }
    }
}
