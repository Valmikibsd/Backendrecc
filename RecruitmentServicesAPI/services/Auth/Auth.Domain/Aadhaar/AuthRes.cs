using System.Xml.Serialization;

namespace RecruitmentPortal.Core.Aadhaar;

public class AuthRes
{
    [XmlElement("DeviceId")]
    public string? deviceId { get; set; }

    [XmlElement("UID")]
    public string? uid { get; set; }

    [XmlElement("SubAUAtransId")]
    public string? subAUAtransId { get; set; }

    [XmlElement("Ret")]
    public string? ret { get; set; }

    [XmlElement("ResponseMsg")]
    public string? responseMsg { get; set; }

    [XmlElement("ResponseCode")]
    public string? responseCode { get; set; }

    [XmlElement("ResponseTs")]
    public string? responseTs { get; set; }

    [XmlElement("SubAUAcode")]
    public string? subAUAcode { get; set; }

    //public Ranks ranks{ get; set; }

    [XmlElement("OtpErrorCode")]
    public string? otpErrorCode { get; set; }

    [XmlElement("OtpRet")]
    public string? otpRet { get; set; }
}
