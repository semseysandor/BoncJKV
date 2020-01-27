Namespace Logger
	Public MustInherit Class Logger
		Implements ILogger
		''' <summary>
		''' Component Name
		''' </summary>
		Public Const ComponentName = "Logger"
		''' <summary>
		''' Actual logging level
		''' </summary>
		Public Shared Property LogLevel As Integer = 0
		Public Sub Critical(message As String, Optional context As ArrayList = Nothing) Implements ILogger.Critical
			Log(LogLevels.Critical, message)
		End Sub

		Public Sub Warning(message As String) Implements ILogger.Warning
			Log(LogLevels.Warning, message)
		End Sub

		Public Sub Info(message As String) Implements ILogger.Info
			Log(LogLevels.Info, message)
		End Sub

		Public Sub Debug(message As String) Implements ILogger.Debug
			Log(LogLevels.Debug, message)
		End Sub
		Protected MustOverride Sub Log(level As Integer, message As String) Implements ILogger.Log
		''' <summary>
		''' Returns the message level string
		''' </summary>
		''' <param name="msglevel">Message level</param>
		''' <returns>Message level string</returns>
		Protected Function MessageLevel(ByVal msglevel As Integer) As String
			Select Case msglevel
				Case 0
					Return "INTERNAL"
				Case 1
					Return "INFO    "
				Case 2
					Return "ERROR   "
				Case Else
					Return ""
			End Select
		End Function
	End Class
End Namespace
