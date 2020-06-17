''' <summary>
''' Class for error handling
''' </summary>
Public Class ErrorHandling
	''' <summary>
	''' General error handling procedure
	''' </summary>
	''' <param name="ex">Exception</param>
	Public Shared Sub General(ByRef ex As Exception)
		Try
			UI.ErrorBox(ex.Message, App.AppName)
		Catch exUI As Exception
			MessageBox.Show(exUI.Message, "UI", MessageBoxButtons.OK, MessageBoxIcon.Error)
			Console.WriteLine("UI not working")
			Console.WriteLine(exUI.Message)
		End Try
		Try
			ComponentManager.Logger.Critical(ex.Message + vbTab + ex.StackTrace)
		Catch exLogger As Exception
			MessageBox.Show(exLogger.Message, "Logger", MessageBoxButtons.OK, MessageBoxIcon.Error)
			Console.WriteLine("Logger not working")
			Console.WriteLine(exLogger.Message)
		End Try
	End Sub
End Class
