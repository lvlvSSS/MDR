using System.Net;

namespace MDR.Device.Api;

public class Connection
{
    public event MedicationOutput? NotifyMedicationOutput;

    protected IPEndPoint serverEndPoint;

    public Connection(IPEndPoint remoteEndPoint)
    {
        serverEndPoint = remoteEndPoint;
    }
}

public delegate void MessageEventNotifier(BaseMessageEvent messageEvent);

public delegate void MedicationOutput();

public delegate void MedicationInput();

public delegate void DeviceStateChanged();