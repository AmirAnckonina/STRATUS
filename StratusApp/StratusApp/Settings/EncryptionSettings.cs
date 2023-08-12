namespace StratusApp.Settings
{
    public class EncryptionSettings
    {
        public EncryptionSettings(string encryptionKey, string fixedIV)
        {
            EncryptionKey = encryptionKey;
            FixedIV = fixedIV;
        }

        public EncryptionSettings() { }

        public string EncryptionKey { get; set; }
        public string FixedIV { get; set; }
    }
}
