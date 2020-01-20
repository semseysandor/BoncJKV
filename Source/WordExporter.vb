Imports Microsoft.Office.Interop
Imports System.IO.Path
''' <summary>
''' Exports content to Word document
''' </summary>
Public Class WordExporter
  ''' <summary>
  ''' Word application
  ''' </summary>
  Private Wordapp As Word.Application
  ''' <summary>
  ''' Word document
  ''' </summary>
  Private WordDoc As Word.Document
  ''' <summary>
  ''' Path of Application
  ''' </summary>
  ''' <returns>Path to the Application</returns>
  Private Property Path As String
  ''' <summary>
  ''' Constructor
  ''' </summary>
  Public Sub New()
    Wordapp = New Word.Application
    WordDoc = New Word.Document
    Path = Application.StartupPath + DirectorySeparatorChar
  End Sub
  ''' <summary>
  ''' Opens a word document in current directory
  ''' </summary>
  ''' <param name="filename">Filename</param>
  Public Sub Open(ByVal filename As String)
    Try
      WordDoc = Wordapp.Documents.Open(Path + filename)
      Wordapp.Visible = True
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Load word document content controls with data from input
  ''' </summary>
  ''' <param name="data">Data to save</param>
  Public Sub LoadData(ByVal data As Dictionary(Of String, String))
    Dim allcc As Word.ContentControls
    Dim cc As Word.ContentControl
    Try
      allcc = WordDoc.ContentControls
      For Each cc In allcc

        If data.ContainsKey(cc.Tag) Then
          cc.Range.Text = data.Item(cc.Tag)
          cc.Appearance = Word.WdContentControlAppearance.wdContentControlHidden
        Else
          cc.Delete(True)
        End If

      Next
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Saves word doc as...
  ''' </summary>
  ''' <param name="filename">Filename</param>
  Public Sub SaveAs(ByVal filename As String)
    Dim directory As String = Path + "jkv"
    Try
      For Each character As Char In GetInvalidFileNameChars()
        filename = filename.Replace(character, "")
      Next
      IO.Directory.CreateDirectory(directory)
      filename = directory + DirectorySeparatorChar + filename
      WordDoc.SaveAs2(filename.ToString)
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
End Class
