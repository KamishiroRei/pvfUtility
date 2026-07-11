using System.Runtime.Serialization;

namespace Whetstone.ChatGPT.Models;

public enum CreatedImageFormat
{
	[EnumMember(Value = "url")]
	Url,
	[EnumMember(Value = "b64_json")]
	Base64
}
