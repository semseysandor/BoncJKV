Imports System.IO.Directory
Imports System.IO.Path
Imports Microsoft.VisualBasic.FileIO
''' <summary>
''' Manage logging
''' </summary>
Public Class Logger
	''' <summary>
	''' Component Name
	''' </summary>
	Public Const ComponentName = "Logger"
	''' <summary>
	''' Logging levels
	''' </summary>
	Public Const LOG_ALL As Integer = 0
	Public Const LOG_STATUS As Integer = 1
	Public Const LOG_CRITICAL As Integer = 2
	Public Const LOG_NONE As Integer = 3
	''' <summary>
	''' Message types
	''' </summary>
	Public Const MSG_INTERNAL As Integer = 0
	Public Const MSG_INFO As Integer = 1
	Public Const MSG_ERROR As Integer = 2
	''' <summary>
	''' Actual logging level
	''' </summary>
	Private Property logLevel As Integer
	''' <summary>
	''' Log file path
	''' </summary>
	Private Shared Property logFile As String
	Public Sub New(ByVal loglevel As Integer)
		Me.logLevel = loglevel
		Try
			logFile = GetCurrentDirectory() + DirectorySeparatorChar
			logFile += "log.txt"
		Catch ex As Exception
			ErrorHandling.General(ex, ComponentName)
		End Try
	End Sub
	''' <summary>
	''' Gets log level
	''' </summary>
	''' <returns>int	LogLevel</returns>
	Public Function GetlogLevel() As Integer
		Return logLevel
	End Function
	''' <summary>
	''' Sets log level
	''' </summary>
	''' <param name="logLevel">logging level</param>
	Public Sub SetlogLevel(logLevel As Integer)
		If Me.logLevel <> logLevel Then
			Me.logLevel = logLevel
		End If
	End Sub
	''' <summary>
	''' Returns the message level string
	''' </summary>
	''' <param name="msglevel">Message level</param>
	''' <returns>Message level string</returns>
	Private Function MessageLevel(ByVal msglevel As Integer) As String
		Select Case msglevel
			Case 0
				Return "INTERNAL"
			Case 1
				Return "INFO"
			Case 2
				Return "ERROR"
			Case Else
				Return ""
		End Select
	End Function
	''' <summary>
	''' Logs message
	''' </summary>
	''' <param name="msgLevel">Message Level</param>
	''' <param name="message">Message to log</param>
	Private Sub Log(ByVal msgLevel As Integer, ByVal message As String)
		If msgLevel >= Me.logLevel Then
			Try
				message = "[" + MessageLevel(msgLevel) + "] (" + DateTime.Now.ToString + ")" + vbTab + message + vbCr
				FileSystem.WriteAllText(logFile, message, True)
			Catch ex As Exception
				MessageBox.Show(ex.Message, ComponentName, MessageBoxButtons.OK, MessageBoxIcon.Error)
			End Try
		End If
	End Sub
	''' <summary>
	''' Logs an internal event
	''' </summary>
	''' <param name="message"></param>
	Public Sub Internal(ByVal message As String)
		Log(Logger.MSG_INTERNAL, message)
	End Sub
	''' <summary>
	''' Logs some info
	''' </summary>
	''' <param name="message"></param>
	Public Sub Info(ByVal message As String)
		Log(Logger.MSG_INFO, message)
	End Sub
	''' <summary>
	''' Logs a critical event
	''' </summary>
	''' <param name="message"></param>
	''' <param name="component"></param>
	Public Sub Critical(ByVal message As String, ByVal component As String)
		Log(Logger.MSG_ERROR, message + vbTab + "at: " + component)
	End Sub
End Class
