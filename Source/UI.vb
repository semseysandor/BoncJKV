''' <summary>
''' UI worker class
''' </summary>
Public Class UI
  ''' <summary>
  ''' Component Name
  ''' </summary>
  Public Const ComponentName = "UI"
  ''' <summary>
  ''' Main form
  ''' </summary>
  ''' <returns></returns>
  Private Property main As Main
  ''' <summary>
  ''' Constructor
  ''' </summary>
  ''' <param name="main">Main form</param>
  Public Sub New(ByRef main As Main)
    Me.main = main
  End Sub
  ''' <summary>
  ''' Resets screen
  ''' </summary>
  Public Sub ResetScreen()
    Try
      ResetControls(main.dataInput.Controls)

      main.dataInput.SelectedIndex = 0

      main.datum.Text = Now.ToString("yyyy-MM-dd")

      main.nev.ResetText()
      main.nev.Select()
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
  End Sub
  ''' <summary>
  ''' Reset controls in a collection recursively
  ''' </summary>
  ''' <param name="ctrcoll">Reset controls in this collection</param>
  Private Sub ResetControls(ctrcoll As Control.ControlCollection)
    Try
      For Each ctrl As Control In ctrcoll
        Select Case ctrl.GetType
          Case GetType(TextBox)
            TryCast(ctrl, TextBox).ResetText()

          Case GetType(CheckBox)
            TryCast(ctrl, CheckBox).Checked = False

          Case GetType(RadioButton)
            TryCast(ctrl, RadioButton).Checked = False

          Case GetType(GroupBox), GetType(TabPage)
            ResetControls(ctrl.Controls)
        End Select
      Next
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
  End Sub
  ''' <summary>
  ''' Enables associated controls
  ''' </summary>
  ''' <param name="mainCtrl">Main control</param>
  ''' <param name="assocCtrl">Associated controls</param>
  Public Sub EnableAssociatedControls(ByRef mainCtrl As Object, ByRef assocCtrl As Collection)
    Dim enable As Boolean = False
    Try
      Select Case mainCtrl.GetType
        Case GetType(CheckBox)
          If TryCast(mainCtrl, CheckBox).Checked Then
            enable = True
          End If

        Case GetType(RadioButton)
          If TryCast(mainCtrl, RadioButton).Checked Then
            enable = True
          End If

        Case GetType(TextBox)
          If TryCast(mainCtrl, TextBox).Text <> String.Empty Then
            enable = True
          End If
      End Select

      For Each ctrl In assocCtrl
        TryCast(ctrl, Control).Enabled = enable
      Next
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
  End Sub
  ''' <summary>
  ''' Resets a group of radio controls
  ''' </summary>
  ''' <param name="controls">Group of controls</param>
  Public Sub ResetRadio(ByRef controls As Collection)
    Try
      For Each ctrl As Control In controls
        If TypeOf ctrl Is RadioButton Then
          TryCast(ctrl, RadioButton).Checked = False
        End If
      Next
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
  End Sub
  ''' <summary>
  ''' Switch to the next tab
  ''' </summary>
  Public Sub NextTab(sender As Object)
    main.dataInput.SelectedIndex = TryCast(sender, Button).Parent.TabIndex + 1
  End Sub
  ''' <summary>
  ''' Sets name and date on the main form
  ''' </summary>
  ''' <param name="name">Name of patient</param>
  ''' <param name="datte">Date of inspection</param>
  Public Sub SetNameDate(ByVal name As String, ByVal datte As String)
    main.nev.Text = name
    main.datum.Text = datte
  End Sub
  ''' <summary>
  ''' Displays a question box
  ''' </summary>
  ''' <param name="message">Message to display</param>
  ''' <returns>Button pressed by user</returns>
  Public Shared Function Question(ByVal message As String, ByVal title As String) As DialogResult
    Return MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
  End Function
  ''' <summary>
  ''' Displays a warning box
  ''' </summary>
  ''' <param name="message">Message to display</param>
  ''' <returns>Button pressed by user</returns>
  Public Shared Function Warning(ByVal message As String, ByVal title As String) As DialogResult
    Return MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
  End Function
  ''' <summary>
  ''' Displays an error box
  ''' </summary>
  ''' <param name="message">Message to display</param>
  ''' <returns>Button pressed by user</returns>
  Public Shared Function ErrorBox(ByVal message As String, ByVal title As String) As DialogResult
    Return MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
  End Function
End Class
