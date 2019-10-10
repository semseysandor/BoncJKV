﻿''' <summary>
''' Main UI form
''' </summary>
Public Class Main
  Private datamng As DataManager
  Private WithEvents transformer As WordTransformer
  Private exporter As Exporter
  ''' <summary>
  ''' Initializes components
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub Loader(sender As Object, e As EventArgs) Handles MyBase.Load


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
  Private Sub FieldMissing(ByVal fieldname As String) Handles transformer.FieldMissing
    'MsgBox("Hiányzó adat: " + fieldname)
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

  Private Sub NextTab(sender As Object, e As EventArgs) Handles Next1.Click, Button1.Click

    Dim btn As Button
    btn = TryCast(sender, Button)

    TabControl1.SelectedIndex = btn.Parent.TabIndex + 1

  End Sub


End Class
