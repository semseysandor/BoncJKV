''' <summary>
''' Class for error handling
''' </summary>
Public Class ErrorHandling
	''' <summary>
	''' General error handling procedure
	''' </summary>
	''' <param name="ex">Exception</param>
	Public Shared Sub General(ByRef ex As Exception)
		UI.ErrorBox(ex.Message, Main.AppName)
		ComponentManager.Logger.Critical(ex.Message + vbTab + ex.StackTrace)
	End Sub
End Class
