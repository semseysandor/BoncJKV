﻿''' <summary>
''' Load patient dialog
''' </summary>
Public Class LoadForm

  ''' <summary>
  ''' Initializes form
  ''' </summary>
  Private Sub Init(sender As Object, e As EventArgs) Handles MyBase.Load
    Try
      saved.Items.Clear()
      For Each row As KeyValuePair(Of String, String) In New XMLProcessor(ComponentManager.Main.SaveFilePath).LoadPatients
        saved.Items.Add(row.Value + " " + row.Key)
      Next
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub

  ''' <summary>
  ''' Loads selected patient data then closes form
  ''' </summary>
  Private Sub LoadData(sender As Object, e As EventArgs) Handles LoadButton.Click
    Try
      If Not IsNothing(saved.SelectedItem) Then
        App.LoadDataUI(saved.SelectedItem.ToString.Substring(11), saved.SelectedItem.ToString.Substring(0, 10))
      End If
      Close()
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
End Class
