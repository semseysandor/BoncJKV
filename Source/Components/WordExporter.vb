Imports Microsoft.Office.Interop

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
  ''' Constructor
  ''' </summary>
  Public Sub New()
    WordApp = New Word.Application
    WordDoc = New Word.Document
  End Sub

  ''' <summary>
  ''' Opens a word document in current directory
  ''' </summary>
  ''' <param name="filename">File to open</param>
  Public Sub Open(ByVal filename As String)
    WordDoc = WordApp.Documents.Open(filename.ToString)
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
  ''' <param name="filename">File name</param>
  Public Sub SaveAs(ByVal filename As String)
    WordDoc.SaveAs2(filename.ToString)
  End Sub
End Class
