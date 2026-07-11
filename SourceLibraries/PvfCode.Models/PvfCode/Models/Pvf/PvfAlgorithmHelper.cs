using System;

namespace PvfCode.Models.Pvf;

public static class PvfAlgorithmHelper
{
	private static readonly uint[] vt0kQfJZpk;

	static PvfAlgorithmHelper()
	{
		vt0kQfJZpk = new uint[256];
		uint num = 1u;
		for (uint num2 = 128u; num2 != 0; num2 /= 2)
		{
			uint num3 = (((num & 1) != 0) ? 3988292384u : 0u);
			num = (num >> 1) ^ num3;
			uint num4 = 0u;
			uint num5 = num2;
			uint num6 = num2 * 2;
			do
			{
				uint num7 = vt0kQfJZpk[num4] ^ num;
				vt0kQfJZpk[num5] = num7;
				uint num8 = num2 * 2;
				num5 += num8;
				num4 += num6;
			}
			while (num4 < 256);
		}
	}

	public static uint CreateBuffKey(byte[] sourceBytes, int trueLen, uint fileNameBytesHash)
	{
		uint num = ~fileNameBytesHash;
		for (int i = 0; i < trueLen; i += 4)
		{
			uint num2 = (sourceBytes[i] ^ num) & 0xFF;
			uint num3 = (num >> 8) ^ vt0kQfJZpk[num2];
			uint num4 = (num3 ^ sourceBytes[i + 1]) & 0xFF;
			uint num5 = (num3 >> 8) ^ vt0kQfJZpk[num4];
			uint num6 = (num5 ^ sourceBytes[i + 2]) & 0xFF;
			uint num7 = (num5 >> 8) ^ vt0kQfJZpk[num6];
			uint num8 = (num7 ^ sourceBytes[i + 3]) & 0xFF;
			num = (num7 >> 8) ^ vt0kQfJZpk[num8];
		}
		return ~num;
	}

	public static byte[] DecryptionPvf(byte[] sourceBytes, int len, uint checksum)
	{
		byte[] array = new byte[len];
		for (int i = 0; i < len; i += 4)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(uQfk0CB0DR(BitConverter.ToUInt32(sourceBytes, i) ^ 0x81A79011u ^ checksum, 6)), 0, array, i, 4);
		}
		return array;
	}

	private static uint uQfk0CB0DR(uint P_0, int P_1)
	{
		return (P_0 >> P_1) | (P_0 << 32 - P_1);
	}

	public static byte[] EncryptionPvf(byte[] sourceBytes, int len, uint checksum)
	{
		int num = (len + 3) & -4;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i += 4)
		{
			Buffer.BlockCopy(BitConverter.GetBytes(iwCksWjLwD(BitConverter.ToUInt32(sourceBytes, i), 6) ^ checksum ^ 0x81A79011u), 0, array, i, 4);
		}
		return array;
	}

	private static uint iwCksWjLwD(uint P_0, int P_1)
	{
		return (P_0 << P_1) | (P_0 >> 32 - P_1);
	}
}
