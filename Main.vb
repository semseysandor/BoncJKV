''' <summary>
''' Main UI form
''' </summary>
Public Class Main
  ''' <summary>
  ''' Transformer object
  ''' </summary>
  Private WithEvents transformer As WordTransformer
  ''' <summary>
  ''' UI worker
  ''' </summary>
  Private ui As UI = New UI(Me)
  ''' ''''''''''''''''''''''''''''''''''''''''''''''''''''' Main features '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
  ''' <summary>
  ''' Save data to disk
  ''' </summary>
  Private Sub SaveDataUI(sender As Object, e As EventArgs) Handles saveBtn.Click
    Dim datamng = New DataManager
    Dim xmlexp = New XMLExporter

    datamng.CollectData(dataInput.Controls)
    xmlexp.SaveData(nev.Text, datum.Text, datamng.GetData)
  End Sub
  ''' <summary>
  ''' Loads data from disk
  ''' </summary>
  ''' <param name="name">Patient name to load</param>
  ''' <param name="datte">Date of inspection</param>
  Friend Sub LoadDataUI(ByVal name As String, ByVal datte As String)
    ui.ResetScreen()
    ui.SetNameDate(name, datte)

    Dim datamng = New DataManager
    datamng.LoadData(dataInput.Controls, (New XMLExporter).LoadPatientData(name, datte))
  End Sub
  ''' <summary>
  ''' Exports data
  ''' </summary>
  Private Sub ExportWord(sender As Object, e As EventArgs) Handles export.Click
    Dim datamng = New DataManager
    transformer = New WordTransformer(False)
    Dim exporter = New WordExporter

    datamng.CollectData(dataInput.Controls)

    transformer.ApplyRules(datamng.GetData)

    exporter.Open("bjk.docx")
    exporter.LoadData(transformer.GetContent)
    exporter.SaveAs(nev.Text + "_" + datum.Text + "_bjk.docx")
  End Sub
  ''' ''''''''''''''''''''''''''''''''''''''''''''''''''''' UI actions ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
  ''' <summary>
  ''' Initializes form
  ''' </summary>
  Friend Sub InitUI(sender As Object, e As EventArgs) Handles MyBase.Load, reset.Click
    ui.ResetScreen()
  End Sub
  ''' <summary>
  ''' Opens load dialog
  ''' </summary>
  Private Sub Loading(sender As Object, e As EventArgs) Handles loadButton.Click
    LoadForm.Show()
  End Sub
  ''' <summary>
  ''' Switch to the next tab
  ''' </summary>
  Private Sub NextTab(sender As Object, e As EventArgs) Handles Next1.Click, Next2.Click, Next3.Click
    ui.NextTab(sender)
  End Sub
  ''' <summary>
  ''' Reset a radio button set
  ''' </summary>
  Private Sub ResetRadio(sender As Object, e As EventArgs) Handles Button2.Click, Button6.Click, Button5.Click, Button4.Click, Button3.Click, Button7.Click, Button1.Click
    ui.ResetRadio(New Collection From {TryCast(sender, Button).Parent.Controls})
  End Sub
  ''' <summary>
  ''' UI action when a required field is missing
  ''' </summary>
  ''' <param name="fieldname">Missing field name</param>
  Private Sub FieldMissing(ByVal fieldname As String) Handles transformer.FieldMissing
    Console.WriteLine(fieldname)
    'MsgBox("Hiányzó adat: " + fieldname)
  End Sub
  ''' ''''''''''''''''''''''''''''''''''''''''''''''''''''' Enabling Controls '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableAscites(sender As Object, e As EventArgs) Handles ascites.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {asc_l, asc_liter})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnablePacemaker(sender As Object, e As EventArgs) Handles pacemaker.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {pacemaker_serial, pacemaker_id})
  End Sub
  ''' <summary>
  ''' Enables checkbox associated to this control
  ''' </summary>
  Private Sub EnableKoszoru(sender As Object, e As EventArgs) Handles koszoru_szuk.TextChanged
    ui.EnableAssociatedControls(sender, New Collection From {koszoru_jobbAC, koszoru_lad, koszoru_cx})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableStent(sender As Object, e As EventArgs) Handles stent.CheckedChanged, thrombus.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {stent_jobbAC, stent_lad, stent_cx})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableInf(sender As Object, e As EventArgs) Handles inf_regi.CheckedChanged, inf_friss.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {inf_meret, inf_elulso, inf_hatso, inf_septalis, inf_oldal})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableHaemo(sender As Object, e As EventArgs) Handles bill_haemo.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {haemo_g})
  End Sub
End Class
