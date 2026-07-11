using System.Runtime.Serialization;

namespace Whetstone.ChatGPT.Models;

public enum ModerationModels
{
	[EnumMember(Value = "text-moderation-latest")]
	Latest,
	[EnumMember(Value = "text-moderation-stable")]
	Stable
}
