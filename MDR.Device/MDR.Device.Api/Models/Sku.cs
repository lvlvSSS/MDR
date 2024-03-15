using System.Xml.Serialization;

namespace MDR.Device.Api.Models;

public class Sku
{
    private sealed class KeyUniqueIdEqualityComparer : IEqualityComparer<Sku>
    {
        public bool Equals(Sku x, Sku y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Key == y.Key && x.UniqueId == y.UniqueId;
        }

        public int GetHashCode(Sku obj)
        {
            return HashCode.Combine(obj.Key, obj.UniqueId);
        }
    }

    public static IEqualityComparer<Sku> KeyUniqueIdComparer { get; } = new KeyUniqueIdEqualityComparer();

    public Sku(string key)
    {
        Key = key;
    }

    public Sku(string key, string? uniqueId, string? batchNumber, string? name, string? dosageForm,
        string? packagingUnit)
    {
        Key = key;
        UniqueId = uniqueId;
        BatchNumber = batchNumber;
        Name = name;
        DosageForm = dosageForm;
        PackagingUnit = packagingUnit;
    }

    /// <summary>
    /// 标识该类sku的唯一编号
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 该sku的过期时间
    /// </summary>
    public DateTime ExpireDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 标识该sku的唯一编号
    /// </summary>
    public string? UniqueId { get; set; }

    /// <summary>
    /// 批号
    /// </summary>
    public string? BatchNumber { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 规格
    /// </summary>
    public string? DosageForm { get; set; }

    /// <summary>
    /// 包装方式
    /// </summary>
    public string? PackagingUnit { get; set; }

    public override string ToString()
    {
        return
            $"{nameof(Key)}: {Key}, {nameof(ExpireDate)}: {ExpireDate}, {nameof(UniqueId)}: {UniqueId}, {nameof(BatchNumber)}: {BatchNumber}, {nameof(Name)}: {Name}, {nameof(DosageForm)}: {DosageForm}, {nameof(PackagingUnit)}: {PackagingUnit}";
    }
}