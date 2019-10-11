Imports Microsoft.Office.Interop
Imports System.IO.Path
''' <summary>
''' Exports content to Word document
''' </summary>
Public Class Exporter
  Private Wordapp As Word.Application
  Private WordDoc As Word.Document
  Public Sub New()
    Wordapp = New Word.Application
    WordDoc = New Word.Document
  End Sub
  ''' <summary>
  ''' Opens a word document in current directory
  ''' </summary>
  ''' <param name="file">filename</param>
  Public Sub Open(ByVal file As String)

    Dim path As String

    path = Application.StartupPath + DirectorySeparatorChar + file

    WordDoc = Wordapp.Documents.Open(path.ToString)
    Wordapp.Visible = True

  End Sub
  ''' <summary>
  ''' Load word document content controls with data from input
  ''' </summary>
  ''' <param name="data"></param>
  Public Sub LoadData(ByRef data As Dictionary(Of String, String))

    Dim cc As Word.ContentControls

    For Each row As KeyValuePair(Of String, String) In data
      Try
        cc = WordDoc.SelectContentControlsByTag(row.Key)

        cc(1).Range.Text = row.Value
      Catch ex As Exception
        MsgBox(ex.Message)
      End Try

    Next

  End Sub
  Public Sub SaveAs(ByVal filename As String)

    filename = Application.StartupPath + DirectorySeparatorChar + filename

    WordDoc.SaveAs2(filename.ToString)

  End Sub
End Class
