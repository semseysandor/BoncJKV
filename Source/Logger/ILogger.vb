Namespace Logger
	Public Interface ILogger
		Sub Critical(ByVal message As String, Optional ByVal context As ArrayList = Nothing)
		Sub Warning(ByVal message As String)
		Sub Info(ByVal message As String)
		Sub Debug(ByVal message As String)
		Sub Log(ByVal level As Integer, ByVal message As String)
	End Interface
End Namespace