namespace GMTool.SqlModel;

public interface GMToolLogger
{
	void Debug(string msg);

	void Debug(object obj);

	void Warning(string msg);

	void Success(string msg);

	void Error(string msg);
}
