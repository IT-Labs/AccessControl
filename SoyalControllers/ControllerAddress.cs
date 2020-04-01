namespace SoyalControllers
{
    public interface IControllerAddress
    {
        string IpAddress {get; set;}
        int Port {get; set;}
        int NodeId {get; set;}
    }

    public class ControllerAddress : IControllerAddress
    {
        public string IpAddress {get; set;}
        public int Port {get; set;}
        public int NodeId  {get; set;}
    }
}