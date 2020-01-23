''' <summary>
''' Class for error handling
''' </summary>
Public Class ErrorHandling
	''' <summary>
	''' Component Name
	''' </summary>
	Public Const ComponentName = "Error Handling"
	Private Shared Property errorhandler As ErrorHandling = New ErrorHandling
	Public Shared Property level As Integer
	''' <summary>
	''' General error handling procedure
	''' </summary>
	''' <param name="ex">Exception</param>
	Public Shared Sub General(ByRef ex As Exception, ByVal component As String)
		UI.ErrorBox(ex.Message, component)
		Dim log = New Logger(Logger.LOG_ALL)
		log.Critical(ex.Message + vbTab + ex.StackTrace, component)
	End Sub
	Public Shared Function singleton() As ErrorHandling
		Return errorhandler
	End Function
	Public Sub setLog(ByVal lvl As Integer)
		level = lvl
	End Sub
	Public Function getLog() As Integer
		Return level
	End Function
End Class
