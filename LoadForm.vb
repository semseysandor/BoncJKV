''' <summary>
''' Load patient dialog
''' </summary>
Public Class LoadForm
  ''' <summary>
  ''' Loads selected patient data then closes form
  ''' </summary>
  ''' <param name="sender"></param>
  ''' <param name="e"></param>
  Private Sub LoadData(sender As Object, e As EventArgs) Handles LoadButton.Click

    Dim datte As String = saved.SelectedItem.ToString.Substring(0, 10)
    Dim name As String = saved.SelectedItem.ToString.Substring(11)

    Main.LoadDataUI(name, datte)

    Try
      Close()
    Catch ex As Exception
      MsgBox(ex.Message)
    End Try

  End Sub
End Class