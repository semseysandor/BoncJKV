Namespace Logger
  ''' <summary>
  ''' Logger interface
  ''' </summary>
  Public Interface ILogger

    ''' <summary>
    ''' Logs a critical event
    ''' </summary>
    ''' <param name="message">Message to log</param>
    ''' <param name="context"></param>
    Sub Critical(ByVal message As String, Optional ByVal context As ArrayList = Nothing)

    ''' <summary>
    ''' Logs a warning
    ''' </summary>
    ''' <param name="message">Message to log</param>
    Sub Warning(ByVal message As String)

    ''' <summary>
    ''' Logs some information
    ''' </summary>
    ''' <param name="message">Message to log</param>
    Sub Info(ByVal message As String)

    ''' <summary>
    ''' Logs debug messages
    ''' </summary>
    ''' <param name="message">Message to log</param>
    Sub Debug(ByVal message As String)

    ''' <summary>
    ''' Logs a message at a given level
    ''' </summary>
    ''' <param name="level">Message level</param>
    ''' <param name="message">Message to log</param>
    Sub Log(ByVal level As Integer, ByVal message As String)

  End Interface
End Namespace