''' <summary>
''' Class for error handling
''' </summary>
Public Class ErrorHandling
	''' <summary>
	''' General error handling procedure
	''' </summary>
	''' <param name="ex">Exception</param>
	Public Shared Sub General(ByRef ex As Exception)
		MsgBox(ex.Message + vbNewLine + vbNewLine + ex.StackTrace)
	End Sub
End Class
