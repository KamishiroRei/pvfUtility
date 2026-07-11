using PvfCode.ViewModels.DocumentFolder;

namespace PvfCode.MVVMServices;

public interface IDocumentGroupService
{
	void ShowContextMenu();

	void NextDocument(DocumentBase vm);

	void LastDocument(DocumentBase vm);
}
