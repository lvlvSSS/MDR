using System.Net;
using MDR.Device.Api.Models;

namespace MDR.Device.Api;

public abstract class Connection
{
    public event SkuOutputStateChanged? NotifySkuOutputStateChanged;
    public event SkuOutputDone? NotifySkuOutputDone;
    public event MedicationInput? NotifyMedicationInput;
    public event QueryMedicationInfo MedicationInfoQueryCallback;
    public event DeviceStateChanged NotifyDeviceStateChanged;

    protected IPEndPoint remoteEndPoint { get; private set; }

    protected Connection(IPEndPoint ipEndPoint)
    {
        remoteEndPoint = ipEndPoint;
    }
}

/// <summary>
/// 出库状态通知
/// </summary>
public delegate void SkuOutputStateChanged();

/// <summary>
/// 出药完成通知
/// </summary>
public delegate void SkuOutputDone();

/// <summary>
/// 入药通知
/// </summary>
public delegate void MedicationInput();

/// <summary>
/// 设备状态通知
/// </summary>
public delegate void DeviceStateChanged();

/// <summary>
/// 查询药品信息通知
/// </summary>
public delegate void QueryMedicationInfo(Sku sku);