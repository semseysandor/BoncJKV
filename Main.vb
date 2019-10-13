''' <summary>
''' Main UI form
''' </summary>
Public Class Main
  Private WithEvents transformer As WordTransformer
  ''' <summary>
  ''' Initializes form
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub InitUI(sender As Object, e As EventArgs) Handles MyBase.Load
    datum.Text = Now.ToString("yyyy-MM-dd")
    nev.Select()
  End Sub
  ''' <summary>
  ''' Save data to disk
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub SaveDataUI(sender As Object, e As EventArgs) Handles saveBtn.Click

    Dim datamng = New DataManager
    Dim xmlexp As XMLExporter = New XMLExporter

    datamng.CollectData(TabControl1.Controls)

    xmlexp.SaveData(nev.Text, datum.Text, datamng.GetData)

  End Sub
  ''' <summary>
  ''' Opens load dialog
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub Loading(sender As Object, e As EventArgs) Handles loadButton.Click

    Dim xml As XMLExporter = New XMLExporter()

    LoadForm.Show()

    For Each row As KeyValuePair(Of String, String) In xml.LoadPatients
      LoadForm.saved.Items.Add(row.Value + " " + row.Key)
    Next

  End Sub
  ''' <summary>
  ''' Loads data from disk
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Patient date</param>
  Public Sub LoadDataUI(ByVal name As String, ByVal datte As String)

    Dim datamng As DataManager = New DataManager
    Dim xml As XMLExporter = New XMLExporter

    nev.Text = name
    datum.Text = datte
    datamng.LoadData(TabControl1.Controls, xml.LoadPatientData(name, datte))

  End Sub
  ''' <summary>
  ''' Exports data to word template
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub ExportWord(sender As Object, e As EventArgs) Handles export.Click

    Dim datamng = New DataManager
    transformer = New WordTransformer(False)
    Dim exporter = New WordExporter

    datamng.CollectData(TabControl1.Controls)

    transformer.ApplyRules(datamng.GetData)

    exporter.Open("bjk.docx")
    exporter.LoadData(transformer.GetContent)
    exporter.SaveAs(nev.Text + "_" + datum.Text + "_bjk.docx")

  End Sub

  ''' <summary>
  ''' UI action when a required field is missing
  ''' </summary>
  ''' <param name="fieldname">Missing field name</param>
  Private Sub FieldMissing(ByVal fieldname As String) Handles transformer.FieldMissing
    Console.WriteLine(fieldname)
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

    datum.Text = Now.ToString("yyyy-MM-dd")

    nev.ResetText()
    nev.Select()

  End Sub
  ''' <summary>
  ''' Reset controls in a collection recursively
  ''' </summary>
  ''' <param name="ctrcoll">Reset controls in collection</param>
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
