using System.Security.Cryptography;
using System.Text;

namespace ClientsideEncryption;

public class AesKey
{
    private static string Aeskey = "0256983015122023";
    public string Getkey() => Aeskey;
    public static string Key = "0256983015122023";
}

public class AESEncrytDecry
{
    public static string DecryptStringAES(string cipherText)
    {
        var obj = new AesKey();
        var strkey = obj.Getkey();
        var keybytes = Encoding.UTF8.GetBytes(strkey);
        var iv = Encoding.UTF8.GetBytes(strkey);
        var encrypted = Convert.FromBase64String(cipherText);
        return DecryptStringFromBytes(encrypted, keybytes, iv);
    }

    public static string EncryptStringAES(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
            return cipherText;

        var obj = new AesKey();
        var strkey = obj.Getkey();
        var keybytes = Encoding.UTF8.GetBytes(strkey);
        var iv = Encoding.UTF8.GetBytes(strkey);
        var encrypt = EncryptStringToBytes(cipherText, keybytes, iv);
        return Convert.ToBase64String(encrypt);
    }

    private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
    {
        if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException(nameof(cipherText));
        if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
        if (iv == null || iv.Length <= 0) throw new ArgumentNullException(nameof(iv));

        using var rijAlg = new RijndaelManaged();
        rijAlg.Mode = CipherMode.CBC;
        rijAlg.Padding = PaddingMode.PKCS7;
        rijAlg.FeedbackSize = 128;
        rijAlg.Key = key;
        rijAlg.IV = iv;

        var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
        try
        {
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        catch
        {
            return "keyError";
        }
    }

    private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
    {
        if (plainText == null || plainText.Length <= 0) throw new ArgumentNullException(nameof(plainText));
        if (key == null || key.Length <= 0) throw new ArgumentNullException(nameof(key));
        if (iv == null || iv.Length <= 0) throw new ArgumentNullException(nameof(iv));

        using var rijAlg = new RijndaelManaged();
        rijAlg.Mode = CipherMode.CBC;
        rijAlg.Padding = PaddingMode.PKCS7;
        rijAlg.FeedbackSize = 128;
        rijAlg.Key = key;
        rijAlg.IV = iv;

        var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return msEncrypt.ToArray();
    }
}
