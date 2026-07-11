using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public class ChatGPTDocumentVm : DocumentBase
{
	public string Keyword
	{
		get
		{
			return GetProperty(() => Keyword);
		}
		set
		{
			SetProperty<string>(() => Keyword, value);
		}
	}

	public ChatGPTDocumentVm(string documentPath = "chatGPT")
		: base(documentPath)
	{
		base.DocumentType = PvfFileDocumentType.chatGPT;
		base.Icon = Res.Instance.ChatGPTICON;
	}

	[Command]
	public void OnSend()
	{
		AppCore.ShowMsg("联网功能已移除");
	}

	public override void Dispose()
	{
	}
}
