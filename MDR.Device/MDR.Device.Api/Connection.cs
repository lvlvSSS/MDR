namespace MDR.Device.Api;

public class Connection
{
    public event MessageEvent? Message;
}

public delegate void MessageEvent();
