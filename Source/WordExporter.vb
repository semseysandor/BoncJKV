Imports Microsoft.Office.Interop
Imports System.IO.Path
''' <summary>
''' Exports content to Word document
''' </summary>
Public Class WordExporter
  ''' <summary>
  ''' Word application
  ''' </summary>
  Private WordApp As Word.Application
  ''' <summary>
  ''' Word document
  ''' </summary>
  Private WordDoc As Word.Document
  ''' <summary>
  ''' Path of Application
  ''' </summary>
  ''' <returns>Path to the Application</returns>
  Public Property FilePath As String
  ''' <summary>
  ''' Constructor
  ''' </summary>
  Public Sub New(Optional ByVal path As String = "")
    WordApp = New Word.Application
    WordDoc = New Word.Document
    FilePath = Helpers.ReplaceInvalidChars(path)
  End Sub
  ''' <summary>
  ''' Opens a word document in current directory
  ''' </summary>
  ''' <param name="filename">Filename</param>
  Public Sub Open(ByVal filename As String)
    filename = Helpers.ReplaceInvalidChars(filename)
    WordDoc = WordApp.Documents.Open(FilePath + filename)
    WordApp.Visible = True
  End Sub
  ''' <summary>
  ''' Load word document content controls with data from input
  ''' </summary>
  ''' <param name="data">Data to save</param>
  Public Sub LoadData(ByVal data As Dictionary(Of String, String))
    For Each cc As Word.ContentControl In WordDoc.ContentControls
      If data.ContainsKey(cc.Tag) Then
        cc.Range.Text = data.Item(cc.Tag)
        cc.Appearance = Word.WdContentControlAppearance.wdContentControlHidden
      Else
        cc.Delete(True)
      End If
    Next
  End Sub
  ''' <summary>
  ''' Saves word doc as...
  ''' </summary>
  ''' <param name="filename">Filename</param>
  Public Sub SaveAs(ByVal filename As String)
    filename = Helpers.ReplaceInvalidChars(filename)
    Dim directory = FilePath + "jkv"
    IO.Directory.CreateDirectory(directory)
    filename = directory + DirectorySeparatorChar + filename
    WordDoc.SaveAs2(filename.ToString)
  End Sub
End Class
