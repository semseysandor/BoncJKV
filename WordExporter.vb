Imports Microsoft.Office.Interop
Imports System.IO.Path
''' <summary>
''' Exports content to Word document
''' </summary>
Public Class WordExporter
  Private Wordapp As Word.Application
  Private WordDoc As Word.Document
  ''' <summary>
  ''' Path of Application
  ''' </summary>
  ''' <returns>Path to the Application</returns>
  Private Property path As String
  Public Sub New()
    Wordapp = New Word.Application
    WordDoc = New Word.Document
    path = Application.StartupPath + DirectorySeparatorChar
  End Sub
  ''' <summary>
  ''' Opens a word document in current directory
  ''' </summary>
  ''' <param name="filename">Filename</param>
  Public Sub Open(ByVal filename As String)

    Try
      WordDoc = Wordapp.Documents.Open(path + filename)
      Wordapp.Visible = True

    Catch ex As Exception
      MsgBox(ex.Message)
    End Try

  End Sub
  ''' <summary>
  ''' Load word document content controls with data from input
  ''' </summary>
  ''' <param name="data">Data to save</param>
  Public Sub LoadData(ByRef data As Dictionary(Of String, String))

    Dim cc As Word.ContentControls

    Try
      For Each row As KeyValuePair(Of String, String) In data

        cc = WordDoc.SelectContentControlsByTag(row.Key)

        cc(1).Range.Text = row.Value
      Next
    Catch ex As Exception
      MsgBox(ex.Message)
    End Try

  End Sub
  ''' <summary>
  ''' Saves word doc as...
  ''' </summary>
  ''' <param name="filename">Filename</param>
  Public Sub SaveAs(ByVal filename As String)

    For Each character As Char In GetInvalidFileNameChars()
      filename = filename.Replace(character, "")
    Next

    Try
      Dim dir As String = path + DirectorySeparatorChar + "jkv"

      IO.Directory.CreateDirectory(dir)

      filename = dir + DirectorySeparatorChar + filename

      WordDoc.SaveAs2(filename.ToString)

    Catch ex As Exception
      MsgBox(ex.Message)
    End Try

  End Sub
End Class
