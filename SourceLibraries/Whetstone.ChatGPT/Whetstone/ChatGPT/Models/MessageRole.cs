using System.Runtime.Serialization;

namespace Whetstone.ChatGPT.Models;

public enum MessageRole
{
	[EnumMember(Value = "system")]
	System,
	[EnumMember(Value = "user")]
	User,
	[EnumMember(Value = "assistant")]
	Assistant,
	[EnumMember(Value = "tool")]
	Tool
}
