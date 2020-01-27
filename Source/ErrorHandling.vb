''' <summary>
''' Class for error handling
''' </summary>
Public Class ErrorHandling
	''' <summary>
	''' General error handling procedure
	''' </summary>
	''' <param name="ex">Exception</param>
	Public Shared Sub General(ByRef ex As Exception, ByVal component As String)
		UI.ErrorBox(ex.Message, component)
		'Logger.Logger.Singleton.Critical(ex.Message + vbTab + ex.StackTrace, component)
	End Sub
End Class
