using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

public class AppSettings
{
    public string Email { get; set; }
    public string Password { get; set; } // plain text in app memory
    public string Verbiage { get; set; }
    public string BaseUrl { get; set; }
    public bool Tracking { get; set; }

    private static readonly string EncryptionKey = "Your$ecretKey1234"; // Must be 16/24/32 characters

    public AppSettings()
    {
        try
        {
            DataTable dt = AccessHelper.ExecuteQuery("SELECT TOP 1 * FROM Settings");
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                Email = row["email"].ToString();
                Password = Decrypt(row["password"].ToString());
                Verbiage = row["verbiage"].ToString();
                BaseUrl = row["BaseUrl"].ToString();
                Tracking = Convert.ToBoolean(row["tracking"]);
            }
            else
            {
                // Optional: create default row if none exists
                AccessHelper.ExecuteNonQuery("INSERT INTO Settings (username, password, verbiage, BaseUrl, tracking) VALUES ('', '', '', '', 0)");
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
        }
    }

    public bool Save()
    {
        try
        {
            // Delete all previous rows to ensure single-entry settings
            AccessHelper.ExecuteNonQuery("DELETE FROM Settings");

            // Insert a new row with the updated settings
            AccessHelper.ExecuteNonQuery(
                "INSERT INTO Settings ([email], [password], [verbiage], [BaseUrl], [tracking]) VALUES (?, ?, ?, ?, ?)",
                ("email", Email),
                ("password", Encrypt(Password)),
                ("verbiage", Verbiage),
                ("BaseUrl", BaseUrl),
                ("tracking", Tracking ? 1 : 0)
            );

            return true;
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return false;
        }
    }

    private static string Encrypt(string plainText)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                byte[] ivBytes = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(16).Substring(0, 16));

                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(cipherBytes);
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return string.Empty;
        }
    }

    private static string Decrypt(string cipherText)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32).Substring(0, 32));
                byte[] ivBytes = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(16).Substring(0, 16));

                aes.Key = keyBytes;
                aes.IV = ivBytes;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }
        catch (Exception ex)
        {
            EventLogHelper.WriteErrorLog(ex);
            return string.Empty;
        }
    }
}
