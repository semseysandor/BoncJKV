Imports Microsoft.VisualBasic.FileIO
Namespace Logger
	''' <summary>
	''' Manage logging
	''' </summary>
	Public Class FileLogger
		Inherits Logger
		''' <summary>
		''' Singleton
		''' </summary>
		Private Shared instance As FileLogger = New FileLogger
		''' <summary>
		''' Log file path
		''' </summary>
		Public Shared Property LogFile As String
		Public Sub New()
		End Sub
		Public Shared Function Singleton() As FileLogger
			Return instance
		End Function
		''' <summary>
		''' Logs message
		''' </summary>
		''' <param name="msgLevel">Message Level</param>
		''' <param name="message">Message to log</param>
		Protected Overrides Sub Log(ByVal msgLevel As Integer, ByVal message As String)
			If LogFile Is Nothing Then
				Exit Sub
			End If
			If msgLevel >= LogLevel Then
				Try
					message = "[" + MessageLevel(msgLevel) + "] [" + DateTime.Now.ToString + "] " + message + vbCr
					FileSystem.WriteAllText(LogFile, message, True)
				Catch ex As Exception
					MessageBox.Show(ex.Message, ComponentName, MessageBoxButtons.OK, MessageBoxIcon.Error)
				End Try
			End If
		End Sub
	End Class
End Namespace
