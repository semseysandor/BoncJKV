Namespace Logger
	''' <summary>
	''' Base logger class
	''' </summary>
	Public MustInherit Class Logger
		Implements ILogger
		''' <summary>
		''' Actual logging level
		''' </summary>
		Public Property LogLevel As Integer = LogLevels.All
		''' <summary>
		''' Logs a critical event
		''' </summary>
		''' <param name="message">Message to log</param>
		''' <param name="context"></param>
		Public Sub Critical(message As String, Optional context As ArrayList = Nothing) Implements ILogger.Critical
			Log(LogLevels.Critical, message)
		End Sub
		''' <summary>
		''' Logs a warning
		''' </summary>
		''' <param name="message">Message to log</param>
		Public Sub Warning(message As String) Implements ILogger.Warning
			Log(LogLevels.Warning, message)
		End Sub
		''' <summary>
		''' Logs some information
		''' </summary>
		''' <param name="message">Message to log</param>
		Public Sub Info(message As String) Implements ILogger.Info
			Log(LogLevels.Info, message)
		End Sub
		''' <summary>
		''' Logs debug messages
		''' </summary>
		''' <param name="message">Message to log</param>
		Public Sub Debug(message As String) Implements ILogger.Debug
			Log(LogLevels.Debug, message)
		End Sub
		''' <summary>
		''' Logs a message at a given level
		''' </summary>
		''' <param name="level">Message level</param>
		''' <param name="message">Message to log</param>
		Protected MustOverride Sub Log(level As Integer, message As String) Implements ILogger.Log
		''' <summary>
		''' Returns the message level string
		''' </summary>
		''' <param name="msglevel">Message level</param>
		''' <returns>Message level string</returns>
		Protected Function MessageLevel(ByVal msglevel As Integer) As String
			Select Case msglevel
				Case LogLevels.Debug
					Return "DEBUG  "
				Case LogLevels.Info
					Return "INFO   "
				Case LogLevels.Warning
					Return "WARNING"
				Case LogLevels.Critical
					Return "ERROR  "
				Case Else
					Return ""
			End Select
		End Function
	End Class
End Namespace
