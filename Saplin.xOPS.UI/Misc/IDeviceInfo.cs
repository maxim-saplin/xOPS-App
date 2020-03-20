using System;
namespace Saplin.xOPS.UI.Misc
{
    public interface IDeviceInfo
    {
        string GetModelName();
        string GetCPU();
        float GetRamSizeGb();
        bool IsChromeOs { get; }
    }
}
