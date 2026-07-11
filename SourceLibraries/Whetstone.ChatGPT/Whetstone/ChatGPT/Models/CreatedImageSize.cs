using System.Runtime.Serialization;

namespace Whetstone.ChatGPT.Models;

public enum CreatedImageSize
{
	[EnumMember(Value = "256x256")]
	Size256,
	[EnumMember(Value = "512x512")]
	Size512,
	[EnumMember(Value = "1024x1024")]
	Size1024
}
