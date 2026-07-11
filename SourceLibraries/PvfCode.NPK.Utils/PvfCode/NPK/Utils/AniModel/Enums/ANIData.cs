using System.Reflection;

namespace PvfCode.NPK.Utils.AniModel.Enums;

[Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
public enum ANIData
{
	LOOP = 0,
	SHADOW = 1,
	COORD = 3,
	IMAGE_RATE = 7,
	IMAGE_ROTATE = 8,
	RGBA = 9,
	INTERPOLATION = 10,
	GRAPHIC_EFFECT = 11,
	DELAY = 12,
	DAMAGE_TYPE = 13,
	DAMAGE_BOX = 14,
	ATTACK_BOX = 15,
	PLAY_SOUND = 16,
	PRELOAD = 17,
	SPECTRUM = 18,
	SET_FLAG = 23,
	FLIP_TYPE = 24,
	LOOP_START = 25,
	LOOP_END = 26,
	CLIP = 27,
	OPERATION = 28
}
