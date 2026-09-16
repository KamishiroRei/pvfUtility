using System.Security.Cryptography;
using System.Text;

namespace Pvf110.Core;

/// <summary>Pvf110 重打包：明文逻辑流 → Builder 加密 Script.pvf + sk.dat。</summary>
public static class Pvf110Repack
{
    public static byte[] LcgEncryptKey(byte[] data, string label)
        => Pvf110Crypto.LcgDecryptKey(data, label);

    public static void EncryptChunkPrefix(byte[] stream, int index, byte[] key)
    {
        int off = index * Pvf110Crypto.ChunkStride;
        if (off + Pvf110Crypto.ChunkPrefix > stream.Length) return;
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = new byte[16];
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        using ICryptoTransform enc = aes.CreateEncryptor();
        byte[] encrypted = enc.TransformFinalBlock(stream, off, Pvf110Crypto.ChunkPrefix);
        Array.Copy(encrypted, 0, stream, off, Pvf110Crypto.ChunkPrefix);
    }

    /// <summary>
    /// 用 32 字节 chunk key 数组构建 sk.dat（AES 前缀 + RSA PKCS#1 v1.5 分块）。
    /// 使用 <see cref="Pvf110Crypto.ActiveKeySet"/>：即本次成功打开归档所用的密钥集
    /// （115 客户端为从 DFO.exe 现场派生），因此写出的 sk.dat 与原客户端同源、可被客户端解开。
    /// </summary>
    public static byte[] BuildSkDat(byte[][] chunkKeys)
        => BuildSkDat(chunkKeys, Pvf110Crypto.ActiveKeySet);

    /// <summary>用指定密钥集构建 sk.dat（AES 前缀 + RSA PKCS#1 v1.5 分块）。</summary>
    public static byte[] BuildSkDat(byte[][] chunkKeys, Pvf110Crypto.Pvf110KeySet keySet)
    {
        using MemoryStream metadataMs = new();
        foreach (byte[] k in chunkKeys)
        {
            if (k.Length != 0x20) throw new ArgumentException("chunk keys must be 32 bytes");
            metadataMs.Write(k, 0, k.Length);
        }
        byte[] metadata = metadataMs.ToArray();

        using Aes aes = Aes.Create();
        aes.Key = Convert.FromHexString(keySet.AesKeyHex);
        aes.IV = new byte[16];
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        int prefix = metadata.Length & ~(Pvf110Crypto.MetadataAlign - 1);
        using ICryptoTransform enc = aes.CreateEncryptor();
        byte[] encPrefix = enc.TransformFinalBlock(metadata, 0, prefix);
        byte[] encryptedMetadata = new byte[metadata.Length];
        Array.Copy(encPrefix, 0, encryptedMetadata, 0, prefix);
        Array.Copy(metadata, prefix, encryptedMetadata, prefix, metadata.Length - prefix);

        using RSA rsa = RSA.Create();
        rsa.ImportFromPem(keySet.BuilderPrivateKeyPem);
        byte[] publicParameters = rsa.ExportParameters(false).Modulus!;
        using RSA pub = RSA.Create();
        pub.ImportParameters(new RSAParameters { Modulus = publicParameters, Exponent = rsa.ExportParameters(false).Exponent });
        int mod = pub.KeySize / 8;
        int maxPlain = mod - 11;
        using MemoryStream outMs = new();
        for (int off = 0; off < encryptedMetadata.Length; off += maxPlain)
        {
            int len = Math.Min(maxPlain, encryptedMetadata.Length - off);
            byte[] block = new byte[len];
            Array.Copy(encryptedMetadata, off, block, 0, len);
            byte[] wrapped = pub.Encrypt(block, RSAEncryptionPadding.Pkcs1);
            outMs.Write(wrapped, 0, wrapped.Length);
        }
        return outMs.ToArray();
    }

    /// <summary>对明文逻辑流施加全部加密（原位）。body 按每组独立 mAIn LCG。</summary>
    public static void EncryptLogicalStream(byte[] stream, Pvf110Header header,
        byte[][] chunkKeys, IReadOnlyList<(int cumulative, int original)>? groupTable,
        bool hashPreserved)
    {
        int hashOff = Pvf110Crypto.HeaderSize + header.EntryCount * Pvf110Crypto.FileEntrySize;
        int nameOff = hashOff + header.HashTableSize;
        int groupOff = nameOff + header.NameTableSize;
        int bodyOff = groupOff + header.GroupCount * Pvf110Crypto.GroupEntrySize;

        EncryptSpan(stream, 0, Pvf110Crypto.HeaderSize, "header");
        if (!hashPreserved)
            EncryptSpan(stream, hashOff, nameOff - hashOff, "hash");
        EncryptSpan(stream, groupOff, bodyOff - groupOff, "group");

        if (groupTable is not null)
        {
            for (int i = 0; i < groupTable.Count; i++)
            {
                int prev = i > 0 ? groupTable[i - 1].cumulative : 0;
                int a = bodyOff + prev;
                int b = bodyOff + groupTable[i].cumulative;
                EncryptSpan(stream, a, b - a, "body");
            }
        }
        else
        {
            EncryptSpan(stream, bodyOff, stream.Length - bodyOff, "body");
        }

        for (int i = 0; i < chunkKeys.Length; i++)
            EncryptChunkPrefix(stream, i, chunkKeys[i]);
    }

    public static byte[] PackPvf(byte[] plaintext, Pvf110Header header, byte[][] chunkKeys,
        IReadOnlyList<(int cumulative, int original)>? groupTable, bool hashPreserved)
    {
        byte[] stream = (byte[])plaintext.Clone();
        EncryptLogicalStream(stream, header, chunkKeys, groupTable, hashPreserved);
        return stream;
    }

    private static void EncryptSpan(byte[] stream, int offset, int length, string keyLabel)
    {
        byte[] seg = new byte[length];
        Array.Copy(stream, offset, seg, 0, length);
        byte[] enc = LcgEncryptKey(seg, keyLabel);
        Array.Copy(enc, 0, stream, offset, length);
    }

    private const string PemPrivateKey = """
-----BEGIN PRIVATE KEY-----
MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAO5v9du54y/+lR6s
ukOqCXnIrG/0R2ZyDElgi+es1Qse8SAhZApIUcrLDH1HH5edI68a5V8KriIzNfSa
4BfqdqKcHdl8aN03Ge7pPk/6YFxOERB/qqi6fF87EveYXsJRtoi5A/6S7abYFZu3
JlfhdmsNe/lJeJp3X3Q21kPEnTC/AgMBAAECgYEArkJjRB/5+0SrhUyloPgvjrLr
27KDUEr/0cze0wYMeeE2Rs5AiAdMx9JYIWMOosb0onAcvLZYh6Q3xbo/FxuDcXEz
r6usyPnnlDqH/hDP04KD+qamBWmpspmiuOKZAo2E6Tmwp7oImODtfkpsgzafbVWE
IPhhVsNTWANgEHbRuuECQQD+E7TgB7nTTiyLZtUmad4HV0/BDZvKlmaBjDyElETX
/OmrmD0/pOWtd8vi1hCYkXyB0Q/CQavk6a4dHyl/X9+RAkEA8D3za7Ova4LH1W40
k7KR3lVp/r3+3a1j3WYdL058DQqYrv1VrWVVbFLOmxfa9f4yagfCcF0oPS2FXv0i
7KCDTwJAeDiW41KEiQl6ZlO4E78QQeT8ZdqslsVnp3DVyd3mXVTctEcK5CyASP5g
SOsFW0ur4Dkt/brZPS2dJ0ZxekXBIQJAVSTvr70oL8dTAa6kTbBdCFpnTZSSzogU
O7RkJY8AYjLeOC6q/eBLLTAP72G+Ba8IuRF09RPfHgeTHD5E8W5V+wJBAMflnAXz
Bd3qoYy1au5Pf27Bby3uMTQOLUzNL3rRYIPuvlY7cQCZq1jANNt81J6CPDrulCF4
JO0BPBshfjHt+NE=
-----END PRIVATE KEY-----
""";
}
