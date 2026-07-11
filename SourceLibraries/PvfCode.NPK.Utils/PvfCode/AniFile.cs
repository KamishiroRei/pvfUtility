using System.Collections.Generic;
using System.Text;
using DevExpress.Mvvm;
using PvfCode.NPK.Utils.AniModel;

namespace PvfCode;

public class AniFile : ViewModelBase
{
	public int Count => Items.Count;

	public List<FRAMEModel> Items
	{
		get
		{
			return GetProperty(() => Items);
		}
		set
		{
			SetProperty<List<FRAMEModel>>(() => Items, value);
			RaisePropertyChanged("Count");
		}
	}

	public bool LOOP { get; set; }

	public bool SHADOW { get; set; }

	public short? COORD { get; set; }

	public ushort? OPERATION { get; set; }

	public SPECTRUM? SPECTRUM { get; set; }

	public AniFile()
	{
		Items = new List<FRAMEModel>();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("#PVF_File\r\n\r\n");
		if (LOOP)
		{
			stringBuilder.AppendLine("[LOOP]\r\n\t1");
		}
		if (SHADOW)
		{
			stringBuilder.AppendLine("[SHADOW]\r\n\t1");
		}
		StringBuilder stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler;
		if (COORD.HasValue)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder2);
			handler.AppendLiteral("[COORD]\r\n\t");
			handler.AppendFormatted(COORD);
			stringBuilder3.AppendLine(ref handler);
		}
		if (OPERATION.HasValue)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(14, 1, stringBuilder2);
			handler.AppendLiteral("[OPERATION]\r\n\t");
			handler.AppendFormatted(OPERATION);
			stringBuilder4.AppendLine(ref handler);
		}
		if (SPECTRUM != null)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
			handler.AppendLiteral("[SPECTRUM]\r\n\t");
			handler.AppendFormatted(SPECTRUM.SPECTRUM_);
			stringBuilder5.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder6 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder2);
			handler.AppendLiteral("[SPECTRUM TERM]\t");
			handler.AppendFormatted(SPECTRUM.SPECTRUM_TERM);
			stringBuilder6.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder2);
			handler.AppendLiteral("[SPECTRUM LIFE TIME]\t");
			handler.AppendFormatted(SPECTRUM.SPECTRUM_LIFE_TIME);
			stringBuilder7.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder8 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
			handler.AppendLiteral("[SPECTRUM COLOR]\t");
			handler.AppendFormatted(SPECTRUM.SPECTRUM_COLOR.ToString());
			stringBuilder8.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder9 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
			handler.AppendLiteral("[SPECTRUM COLOR]\t");
			handler.AppendFormatted(SPECTRUM.SPECTRUM_COLOR.ToString());
			stringBuilder9.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder10 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
			handler.AppendLiteral("\t");
			handler.AppendFormatted(SPECTRUM.SPECTRUM_EFFECT);
			stringBuilder10.AppendLine(ref handler);
		}
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder11 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder2);
		handler.AppendLiteral("[FRAME MAX]\r\n\t");
		handler.AppendFormatted(Count);
		handler.AppendLiteral("\r\n");
		stringBuilder11.AppendLine(ref handler);
		foreach (FRAMEModel item in Items)
		{
			stringBuilder.AppendLine(item.ToString());
		}
		stringBuilder.AppendLine("");
		return stringBuilder.ToString();
	}
}
