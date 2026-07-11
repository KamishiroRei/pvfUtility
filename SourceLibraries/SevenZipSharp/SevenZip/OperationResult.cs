namespace SevenZip;

public enum OperationResult
{
	Ok,
	UnsupportedMethod,
	DataError,
	CrcError,
	Unavailable,
	UnexpectedEnd,
	DataAfterEnd,
	IsNotArc,
	HeadersError,
	WrongPassword
}
