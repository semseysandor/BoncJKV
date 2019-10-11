''' <summary>
''' Main UI form
''' </summary>
Public Class Main
  Private datamng As DataManager
  Private WithEvents transformer As WordTransformer
  Private exporter As Exporter
  ''' <summary>
  ''' Initializes form
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub Loader(sender As Object, e As EventArgs) Handles MyBase.Load
    datum.Text = Now.ToShortDateString
    nev.Select()
  End Sub
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

    transformer = New WordTransformer(False)
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
  Private Sub NextTab(sender As Object, e As EventArgs) _
    Handles Next1.Click, Next2.Click, Next3.Click
    TabControl1.SelectedIndex = TryCast(sender, Button).Parent.TabIndex + 1
  End Sub
  ''' <summary>
  ''' Reset a radio button set
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub ResetRadio(sender As Object, e As EventArgs) _
    Handles Button2.Click, Button6.Click, Button5.Click,
    Button4.Click, Button3.Click, Button7.Click, Button1.Click

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

    TabControl1.SelectedIndex = 0

    datum.Text = Now.ToShortDateString

    nev.ResetText()
    nev.Select()

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
  ''' <summary>
  ''' Enables textbox associated to this control
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub AscitesCheck(sender As Object, e As EventArgs) Handles ascites.CheckedChanged
    If ascites.Checked Then
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
    If pacemaker.Checked Then
      pacemaker_serial.Enabled = True
      pacemaker_id.Enabled = True
    Else
      pacemaker_serial.Enabled = False
      pacemaker_id.Enabled = False
    End If
  End Sub
  ''' <summary>
  ''' Enables checkbox associated to this control
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub EnableKoszoru(sender As Object, e As EventArgs) Handles koszoru_szuk.TextChanged

    If koszoru_szuk.Text <> "" Then

      koszoru_jobbAC.Enabled = True
      koszoru_lad.Enabled = True
      koszoru_cx.Enabled = True

    Else

      koszoru_jobbAC.Enabled = False
      koszoru_lad.Enabled = False
      koszoru_cx.Enabled = False

    End If

  End Sub
  ''' <summary>
  ''' Enables checkbox associated to this control
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub EnableStent(sender As Object, e As EventArgs) Handles stent.CheckedChanged, thrombus.CheckedChanged

    If stent.Checked OrElse thrombus.Checked Then

      stent_jobbAC.Enabled = True
      stent_lad.Enabled = True
      stent_cx.Enabled = True

    Else

      stent_jobbAC.Enabled = False
      stent_lad.Enabled = False
      stent_cx.Enabled = False

    End If

  End Sub
  ''' <summary>
  ''' Enables checkbox associated to this control
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub EnableInf(sender As Object, e As EventArgs) Handles inf_regi.CheckedChanged, inf_friss.CheckedChanged

    If inf_regi.Checked OrElse inf_friss.Checked Then

      inf_meret.Enabled = True
      inf_elulso.Enabled = True
      inf_hatso.Enabled = True
      inf_septalis.Enabled = True
      inf_oldal.Enabled = True

    Else

      inf_meret.Enabled = False
      inf_elulso.Enabled = False
      inf_hatso.Enabled = False
      inf_septalis.Enabled = False
      inf_oldal.Enabled = False

    End If

  End Sub
  ''' <summary>
  ''' Enables textbox associated to this control
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub EnableHaemo(sender As Object, e As EventArgs) Handles bill_haemo.CheckedChanged

    If bill_haemo.Checked Then
      haemo_g.Enabled = True
    Else
      haemo_g.Enabled = False
    End If

  End Sub
End Class
