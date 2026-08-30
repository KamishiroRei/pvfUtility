using System.Security.Cryptography;
using System.Text;

namespace Pvf110.Core;

/// <summary>
/// Pvf110 Script.pvf 外层 Builder 保护链 + LCG 段加密。
/// 只读核心：sk.dat RSA → metadata AES → 54 chunk keys → 区块 AES → 逻辑流 → LCG 段。
/// </summary>
public static class Pvf110Crypto
{
    public const uint Mask32 = 0xFFFFFFFF;
    public const uint LcgA = 0x000343FD;
    public const uint LcgInc = 0x00269EC3;   // header/hash/group/body
    public const uint NameInc = 0x00269EC9;   // name 表两流
    public const int ChunkStride = 0x00A00000;
    public const int ChunkPrefix = 0x00002800;
    public const int MetadataAlign = 0x100;
    public const int HeaderSize = 0x30;
    public const int FileEntrySize = 0x18;
    public const int GroupEntrySize = 8;

    private const string StaticAesKeyHex =
        "B90C9493DF1780FF03E5E3F8EE225B620260B96996114AEF5923938A2AA9B06E";

    private const string BuilderPrivateKeyPem = """
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

    /// <summary>额外的 Pvf110 客户端密钥集（与 pvf-tools 一致）。读取时自动尝试全部密钥集。</summary>
    private const string FallbackAesKeyHex =
        "25BA2FC5D473E61D9ACA862FF88D8B2784FD291E0CFCA2E4964BDC1D983E834A";

    private const string FallbackBuilderPrivateKeyPem = """
-----BEGIN PRIVATE KEY-----
MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAKlphypoks994mOC
y4+131PEMgkcs8Td2m8wdMlvMIeaIx2tA/38TiBdomKolknP2xJu9Ngv3co40Vwf
C6Zz3JhCBnLM9487GF4eNgC0eCpN7MUnCVm5oIz0Q2lxtF69NVXHmaCh9qVcx8Lg
WMvvppzFZL5bxGsll6KOBcP49verAgMBAAECgYBxC+4aKnXs53+uD5VubFp+Nyl8
Ux7Se4hLMfZ8hCnKEtzj7JQBY99uUY0OcJj44C67ejcAG9DQJGHwKvdHN4E3MsYy
60WsLQQN+lm3jTkZMj3vwSDV2PYXxzjJt1w4gJLsqyE2CzaDl8z2GEFfYdBiFREP
pQPVlmaRv+6gPUoOwQJBANH8yUChS9M2tTvoqRcmZMwoxvIG7HY0DkfDqmihu1r4
p290f9nn/Jy4Hpy431CqPKtMTg3U0fz6F8z5W2PWIkMCQQDOiK4+/huVfkR3A8iT
jSl+ihM9UYekVxy+Ppln9sBYCAM8gQDFLIVPLCaEQRIjHrnwKeEDhma7eb0zFDpu
bsJ5AkBuBcbkyB5UglufEh5WdNVnaw4iDaKjpE6/JzQIMoVZ+uKvsRiz8asX6hiQ
AQVw3GGBVy+ma5XCuL7ztMs/mhjFAkEAnvD6v4mIqtRAViL4Qe0ZqMMTqVXMAEH1
4caFAkgXn+eSrgZNFHB9qv88KP12usZTq6pe+vp8pvw6CUwg54mfaQJBAIYlfkBv
+yKXUscu9K6zil4uz+ka0hnTDWbxkZklFfV4NilVjiqmeE83XEv0P/b8eSuBHN4X
czWrX0+y0sX1JAU=
-----END PRIVATE KEY-----
""";

    private sealed record Pvf110KeySet(string AesKeyHex, string BuilderPrivateKeyPem);

    private static readonly Pvf110KeySet[] KnownKeySets =
    {
        new(StaticAesKeyHex, BuilderPrivateKeyPem),
        new(FallbackAesKeyHex, FallbackBuilderPrivateKeyPem),
    };

    private static readonly Dictionary<string, byte[]> KeyWords = new()
    {
        ["header"] = Encoding.Unicode.GetBytes("iNfO"),
        ["hash"] = Encoding.Unicode.GetBytes("HSrm"),
        ["group"] = Encoding.Unicode.GetBytes("Gidx"),
        ["body"] = Encoding.Unicode.GetBytes("mAIn"),
        ["utf8"] = Encoding.Unicode.GetBytes("stAs"),
        ["utf16"] = Encoding.Unicode.GetBytes("stWs"),
    };

    /// <summary>seed = w0*0x339E9711 + ((w1*0x393+w2)*0x393+w3)*0x393 (mod 2^32)。</summary>
    public static uint DeriveSeed(byte[] key8)
    {
        if (key8.Length < 8) throw new ArgumentException("key must be 8 bytes (4 UTF-16 words)", nameof(key8));
        uint w0 = BitConverter.ToUInt16(key8, 0);
        uint w1 = BitConverter.ToUInt16(key8, 2);
        uint w2 = BitConverter.ToUInt16(key8, 4);
        uint w3 = BitConverter.ToUInt16(key8, 6);
        return unchecked(w0 * 0x339E9711 + ((w1 * 0x393 + w2) * 0x393 + w3) * 0x393);
    }

    public static uint DeriveSeed(string asciiKey)
        => DeriveSeed(Encoding.Unicode.GetBytes(asciiKey));

    /// <summary>LCG 解密（XOR 自反，加密 == 解密）。</summary>
    public static byte[] LcgDecrypt(byte[] data, uint seed, uint inc)
    {
        byte[] outb = (byte[])data.Clone();
        int n = outb.Length;
        int end = (n >> 2) << 2;
        int i = 0;
        uint state = seed;
        while (i < end)
        {
            uint t1 = unchecked(state * LcgA + inc);
            state = unchecked(t1 * LcgA + inc);
            uint xorKey = unchecked((t1 & 0xFFFF0000) + ((state >> 16) & 0xFFFF));
            uint v = BitConverter.ToUInt32(outb, i) ^ xorKey;
            BitConverter.GetBytes(v).CopyTo(outb, i);
            i += 4;
        }
        int tail = n - end;
        if (tail > 0)
        {
            uint t1 = unchecked(state * LcgA + inc);
            uint t2 = unchecked(t1 * LcgA + inc);
            uint finalKey = unchecked((t1 & 0xFFFF0000) + ((t2 >> 16) & 0xFFFF));
            byte[] kb = BitConverter.GetBytes(finalKey);
            for (int k = 0; k < tail; k++) outb[end + k] ^= kb[k];
        }
        return outb;
    }

    public static byte[] LcgDecryptKey(byte[] data, string label)
        => LcgDecrypt(data, DeriveSeed(KeyWords[label]), LcgInc);

    public static byte[] LcgDecryptName(byte[] data, string utf8Or16)
        => LcgDecrypt(data, DeriveSeed(KeyWords[utf8Or16]), NameInc);

    /// <summary>sk.dat RSA PKCS#1 v1.5 私钥解包（用第一套密钥）。</summary>
    public static byte[] UnwrapSkDat(byte[] encrypted)
        => UnwrapSkDat(encrypted, KnownKeySets[0].BuilderPrivateKeyPem);

    /// <summary>sk.dat RSA PKCS#1 v1.5 私钥解包（指定 PEM）。</summary>
    public static byte[] UnwrapSkDat(byte[] encrypted, string pemKey)
    {
        using RSA rsa = RSA.Create();
        rsa.ImportFromPem(pemKey);
        int mod = rsa.KeySize / 8;
        if (encrypted.Length % mod != 0)
            throw new InvalidDataException("sk.dat is not a whole RSA block count");
        using MemoryStream ms = new();
        for (int off = 0; off < encrypted.Length; off += mod)
        {
            byte[] block = new byte[mod];
            Array.Copy(encrypted, off, block, 0, mod);
            byte[] plain = rsa.Decrypt(block, RSAEncryptionPadding.Pkcs1);
            ms.Write(plain, 0, plain.Length);
        }
        return ms.ToArray();
    }

    private static byte[] StaticAesKey()
        => Convert.FromHexString(StaticAesKeyHex);

    private static byte[] DecryptMetadata(byte[] metadata, string aesKeyHex)
    {
        int prefix = metadata.Length & ~(MetadataAlign - 1);
        if (prefix == 0) return (byte[])metadata.Clone();
        using Aes aes = Aes.Create();
        aes.Key = Convert.FromHexString(aesKeyHex);
        aes.IV = new byte[16];
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        using ICryptoTransform dec = aes.CreateDecryptor();
        byte[] decrypted = dec.TransformFinalBlock(metadata, 0, prefix);
        byte[] result = new byte[metadata.Length];
        Array.Copy(decrypted, 0, result, 0, prefix);
        Array.Copy(metadata, prefix, result, prefix, metadata.Length - prefix);
        return result;
    }

    /// <summary>metadata AES-256-CBC，zero IV，无 padding，只解前缀 N & ~0xFF（用第一套密钥）。</summary>
    public static byte[] DecryptMetadata(byte[] metadata)
        => DecryptMetadata(metadata, KnownKeySets[0].AesKeyHex);

    public static byte[][] ExtractChunkKeys(byte[] metadata)
    {
        int count = metadata.Length / 0x20;
        byte[][] keys = new byte[count][];
        for (int i = 0; i < count; i++)
        {
            keys[i] = new byte[0x20];
            Array.Copy(metadata, i * 0x20, keys[i], 0, 0x20);
        }
        return keys;
    }

    public static void DecryptChunkPrefix(byte[] stream, int index, byte[] key)
    {
        int off = index * ChunkStride;
        if (off + ChunkPrefix > stream.Length) return;
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = new byte[16];
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        using ICryptoTransform dec = aes.CreateDecryptor();
        byte[] decrypted = dec.TransformFinalBlock(stream, off, ChunkPrefix);
        Array.Copy(decrypted, 0, stream, off, ChunkPrefix);
    }

    /// <summary>完整重建逻辑流，自动尝试所有已知密钥集。</summary>
    public static byte[] OpenLogicalStream(byte[] skdat, byte[] pvfBytes)
    {
        var errors = new List<Exception>();
        foreach (var ks in KnownKeySets)
        {
            try
            {
                byte[] unwrapped = UnwrapSkDat(skdat, ks.BuilderPrivateKeyPem);
                byte[] metadata = DecryptMetadata(unwrapped, ks.AesKeyHex);
                byte[][] keys = ExtractChunkKeys(metadata);
                int chunkCount = (pvfBytes.Length + ChunkStride - 1) / ChunkStride;
                if (keys.Length < chunkCount) continue; // 密钥不对，尝试下一组
                byte[] stream = (byte[])pvfBytes.Clone();
                for (int i = 0; i < chunkCount; i++)
                    DecryptChunkPrefix(stream, i, keys[i]);
                byte[] header = new byte[HeaderSize];
                Array.Copy(stream, 0, header, 0, HeaderSize);
                byte[] headerPlain = LcgDecryptKey(header, "header");
                if (BitConverter.ToUInt32(headerPlain, 0) != 0x69706B6Eu) continue; // magic 不对
                Array.Copy(headerPlain, 0, stream, 0, HeaderSize);
                return stream;
            }
            catch (Exception ex) { errors.Add(ex); }
        }
        throw new AggregateException("Pvf110: all known key sets failed to decrypt sk.dat", errors);
    }

    public static Pvf110Header ParseHeader(byte[] stream)
    {
        if (BitConverter.ToUInt32(stream, 0) != 0x69706B6Eu)
            throw new InvalidDataException("header magic is not nkpi");
        Pvf110Header h = new()
        {
            Magic = BitConverter.ToUInt32(stream, 0),
            Guid = new byte[0x14],
            EntryCount = BitConverter.ToInt32(stream, 0x18),
            Padding = BitConverter.ToInt32(stream, 0x1C),
            BodySize = BitConverter.ToInt32(stream, 0x20),
            GroupCount = BitConverter.ToInt32(stream, 0x24),
            HashTableSize = BitConverter.ToInt32(stream, 0x28),
            NameTableSize = BitConverter.ToInt32(stream, 0x2C),
        };
        Array.Copy(stream, 4, h.Guid, 0, 0x14);
        return h;
    }

    public static Pvf110Layout ComputeLayout(Pvf110Header header, int fileSize)
    {
        int hashOffset = HeaderSize + header.EntryCount * FileEntrySize;
        int nameOffset = hashOffset + header.HashTableSize;
        int groupOffset = nameOffset + header.NameTableSize;
        int bodyOffset = groupOffset + header.GroupCount * GroupEntrySize;
        int endOffset = bodyOffset + header.BodySize;
        if (endOffset != fileSize)
            throw new InvalidDataException($"layout end {endOffset} != file size {fileSize}");
        return new Pvf110Layout
        {
            HashOffset = hashOffset,
            NameOffset = nameOffset,
            GroupOffset = groupOffset,
            BodyOffset = bodyOffset,
            EndOffset = endOffset,
        };
    }
}

public sealed class Pvf110Header
{
    public uint Magic;
    public byte[] Guid = Array.Empty<byte>();
    public int EntryCount;
    public int Padding;
    public int BodySize;
    public int GroupCount;
    public int HashTableSize;
    public int NameTableSize;
}

public sealed class Pvf110Layout
{
    public int HashOffset;
    public int NameOffset;
    public int GroupOffset;
    public int BodyOffset;
    public int EndOffset;
}
