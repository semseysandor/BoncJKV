''' <summary>
''' Main UI form
''' </summary>
Public Class Main
  Private datamng As DataManager
  Private WithEvents transformer As WordTransformer
  Private exporter As Exporter
  ''' <summary>
  ''' Collects data from UI
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub CollectData(sender As Object, e As EventArgs) Handles gather.Click

    datamng = New DataManager
    For Each tabpage As TabPage In TabControl1.Controls
      datamng.CollectData(tabpage.Controls)
    Next

    datamng.PrintData()

    transformer = New WordTransformer
    transformer.ApplyRules(datamng.GetData)
    transformer.PrintContent()

  End Sub
  ''' <summary>
  ''' Exports data to word template
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub ExportWord(sender As Object, e As EventArgs) Handles export.Click
    exporter = New Exporter
    exporter.Open("bjk.docx")
    exporter.LoadData(transformer.GetContent)
  End Sub
  ''' <summary>
  ''' Enables textbox associated to this control
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub AscitesCheck(sender As Object, e As EventArgs) Handles ascites.CheckedChanged
    If ascites.Checked = True Then
      asc_liter.Enabled = True
      asc_l.Enabled = True
    Else
      asc_liter.Enabled = False
      asc_l.Enabled = False
    End If
  End Sub
  ''' <summary>
  ''' Enables textbox associated to this control
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub PacemakerCheck(sender As Object, e As EventArgs) Handles pacemaker.CheckedChanged
    If pacemaker.Checked = True Then
      pacemaker_serial.Enabled = True
      pacemaker_id.Enabled = True
    Else
      pacemaker_serial.Enabled = False
      pacemaker_id.Enabled = False
    End If
  End Sub
  ''' <summary>
  ''' UI action when a required field is missing
  ''' </summary>
  ''' <param name="fieldname"></param>
  Private Sub FieldMissing(ByVal fieldname As String) Handles transformer.FieldMissing
    'MsgBox("Hiányzó adat: " + fieldname)
  End Sub
  ''' <summary>
  ''' Switch to the next tab
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub NextTab(sender As Object, e As EventArgs) Handles Next1.Click, Button1.Click
    TabControl1.SelectedIndex = TryCast(sender, Button).Parent.TabIndex + 1
  End Sub
  ''' <summary>
  ''' Reset a radio button set
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub ResetRadio(sender As Object, e As EventArgs) Handles Button2.Click, Button6.Click, Button5.Click, Button4.Click, Button3.Click

    For Each ctrl As Control In TryCast(sender, Button).Parent.Controls
      If TypeOf ctrl Is RadioButton Then
        TryCast(ctrl, RadioButton).Checked = False
      End If
    Next

  End Sub
  ''' <summary>
  ''' Reset form controls
  ''' </summary>
  Private Sub ResetForm() Handles reset.Click
    ResetControls(TabControl1.Controls)
  End Sub
  ''' <summary>
  ''' Reset controls in a collection recursively
  ''' </summary>
  ''' <param name="ctrcoll"></param>
  Private Sub ResetControls(ctrcoll As Control.ControlCollection)

    For Each ctrl As Control In ctrcoll

      If TypeOf ctrl Is TextBox Then
        TryCast(ctrl, TextBox).Text = ""

      ElseIf TypeOf ctrl Is CheckBox Then
        TryCast(ctrl, CheckBox).Checked = False

      ElseIf TypeOf ctrl Is RadioButton Then
        TryCast(ctrl, RadioButton).Checked = False

      ElseIf TypeOf ctrl Is GroupBox Then
        ResetControls(ctrl.Controls)

      ElseIf TypeOf ctrl Is TabPage Then
        ResetControls(ctrl.Controls)

      End If

    Next
  End Sub
End Class
