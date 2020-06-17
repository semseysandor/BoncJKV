''' <summary>
''' UI worker class
''' </summary>
Public Class UI
  ''' <summary>
  ''' Resets screen
  ''' </summary>
  Public Sub ResetScreen()
    ResetControls(App.dataInput.Controls)

    ComponentManager.Main.dataInput.SelectedIndex = 0

    ComponentManager.Main.datum.Text = Now.ToString("yyyy-MM-dd")

    ComponentManager.Main.nev.ResetText()
    ComponentManager.Main.nev.Select()
    ComponentManager.Main.nev.Select()
  End Sub
  ''' <summary>
  ''' Reset controls in a collection recursively
  ''' </summary>
  ''' <param name="ctrcoll">Reset controls in this collection</param>
  Private Sub ResetControls(ctrcoll As Control.ControlCollection)
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
  End Sub
  ''' <summary>
  ''' Enables associated controls
  ''' </summary>
  ''' <param name="mainCtrl">Main control</param>
  ''' <param name="assocCtrl">Associated controls</param>
  Public Sub EnableAssociatedControls(ByRef mainCtrl As Object, ByRef assocCtrl As Collection)
    Dim enable As Boolean = False

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
  End Sub
  ''' <summary>
  ''' Resets a group of radio controls
  ''' </summary>
  ''' <param name="controls">Group of controls</param>
  Public Sub ResetRadio(ByRef controls As Collection)
    For Each ctrl As Control In controls
      If TypeOf ctrl Is RadioButton Then
        TryCast(ctrl, RadioButton).Checked = False
      End If
    Next
  End Sub
  ''' <summary>
  ''' Switch to the next tab
  ''' </summary>
  Public Sub NextTab(sender As Object)
    ComponentManager.Main.dataInput.SelectedIndex = TryCast(sender, Button).Parent.TabIndex + 1
  End Sub
  ''' <summary>
  ''' Reset controls on this tab
  ''' </summary>
  Public Sub ResetTab(sender As Object)
    ResetControls(TryCast(sender, Button).Parent.Controls)
  End Sub
  ''' <summary>
  ''' Reset controls on this tab
  ''' </summary>
  Public Sub IcterusChange(sender As Object)
    If TryCast(sender, CheckBox).Checked Then
      ComponentManager.Main.BackColor = Color.Yellow
      For Each tab As TabPage In ComponentManager.Main.dataInput.TabPages
        tab.BackColor = Color.Yellow
      Next
    Else
      ComponentManager.Main.BackColor = Control.DefaultBackColor
      For Each tab As TabPage In ComponentManager.Main.dataInput.TabPages
        tab.BackColor = Control.DefaultBackColor
      Next
    End If
  End Sub
  ''' <summary>
  ''' Sets name and date on the main form
  ''' </summary>
  ''' <param name="name">Name of patient</param>
  ''' <param name="datte">Date of inspection</param>
  Public Sub SetNameDate(ByVal name As String, ByVal datte As String)
    ComponentManager.Main.nev.Text = name
    ComponentManager.Main.datum.Text = datte
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