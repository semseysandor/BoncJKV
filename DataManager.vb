''' <summary>
''' Manages data from UI
''' </summary>
Public Class DataManager
  Private data As Dictionary(Of String, String)
  Public Sub New()
    data = New Dictionary(Of String, String)
  End Sub
  ''' <summary>
  ''' Returns data
  ''' </summary>
  ''' <returns></returns>
  Public Function GetData() As Dictionary(Of String, String)
    Return data
  End Function
  ''' <summary>
  ''' Prints data to the console
  ''' </summary>
  Public Sub PrintData()
    Console.WriteLine("UI DATA *******************************")
    For Each row As KeyValuePair(Of String, String) In data
      Console.WriteLine(row.Key.ToString + vbTab + row.Value.ToString)
    Next
  End Sub
  ''' <summary>
  ''' Collects data from UI and puts in the dictionary
  ''' </summary>
  ''' <param name="coll"></param>
  Public Sub CollectData(ByRef coll As Control.ControlCollection)

    Dim textbox As TextBox
    Dim chbox As CheckBox
    Dim radio As RadioButton

    For Each ctrl As Control In coll

      If TypeOf ctrl Is TextBox Then

        textbox = TryCast(ctrl, TextBox)
        If textbox.Text <> "" AndAlso textbox.Enabled Then
          data.Add(textbox.Tag.ToString, textbox.Text)
        End If

      ElseIf TypeOf ctrl Is CheckBox Then

        chbox = TryCast(ctrl, CheckBox)
        If chbox.Checked AndAlso chbox.Enabled Then
          data.Add(chbox.Tag.ToString, "TRUE")
        End If

      ElseIf TypeOf ctrl Is RadioButton Then

        radio = TryCast(ctrl, RadioButton)
        If radio.Checked AndAlso radio.Enabled Then
          data.Add(radio.Parent.Tag.ToString, radio.Tag.ToString)
        End If

      ElseIf TypeOf ctrl Is GroupBox Then

        CollectData(ctrl.Controls)

      End If

    Next
  End Sub
End Class