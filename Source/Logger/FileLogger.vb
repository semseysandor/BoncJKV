Imports Microsoft.VisualBasic.FileIO
Namespace Logger
	''' <summary>
	''' Manage logging
	''' </summary>
	Public Class FileLogger
		Inherits Logger
		''' <summary>
		''' Log file path
		''' </summary>
		Public Shared Property LogFile As String
		''' <summary>
		''' Constructor
		''' </summary>
		''' <param name="path">Log file path</param>
		Public Sub New(Optional ByVal path As String = "")
			LogFile = path
		End Sub
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
					MessageBox.Show(ex.Message, "Logger", MessageBoxButtons.OK, MessageBoxIcon.Error)
				End Try
			End If
		End Sub
	End Class
End Namespace
