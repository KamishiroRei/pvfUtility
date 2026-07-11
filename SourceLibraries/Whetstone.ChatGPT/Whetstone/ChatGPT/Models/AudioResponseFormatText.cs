using System.Runtime.Serialization;

namespace Whetstone.ChatGPT.Models;

public enum AudioResponseFormatText
{
	[EnumMember(Value = "text")]
	Text,
	[EnumMember(Value = "srt")]
	SubRip,
	[EnumMember(Value = "vtt")]
	WebVtt
}
