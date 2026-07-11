namespace PvfCode.Dot;

public class ResultData
{
	public bool IsError => !string.IsNullOrEmpty(Msg);

	public string Msg { get; set; }

	public int ErrorId { get; set; }
}
public class ResultData<T> : ResultData
{
	public T Data { get; set; }
}
public class ResultData<T, T2> : ResultData
{
	public T Data { get; set; }

	public T2 Data2 { get; set; }
}
public class ResultData<T, T2, T3> : ResultData
{
	public T Data { get; set; }

	public T2 Data2 { get; set; }

	public T3 Data3 { get; set; }
}
