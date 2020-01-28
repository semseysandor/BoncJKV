﻿''' <summary>
''' Manages data from UI
''' </summary>
Public Class DataManager
  ''' <summary>
  ''' Inspection data
  ''' </summary>
  Private Data As Dictionary(Of String, String)
  ''' <summary>
  ''' Constructor
  ''' </summary>
  Public Sub New()
    Data = New Dictionary(Of String, String)
  End Sub
  ''' <summary>
  ''' Returns data
  ''' </summary>
  ''' <returns>data</returns>
  Public Function GetData() As Dictionary(Of String, String)
    Return Data
  End Function
  ''' <summary>
  ''' Prints data to the console
  ''' </summary>
  Public Sub PrintData()
    Console.WriteLine("UI DATA *******************************")
    For Each row As KeyValuePair(Of String, String) In Data
      Console.WriteLine(row.Key.ToString + vbTab + row.Value.ToString)
    Next
  End Sub
  ''' <summary>
  ''' Collects data from UI and puts in the dictionary
  ''' </summary>
  ''' <param name="root">Control collection to scan</param>
  Public Sub CollectData(ByRef root As Control.ControlCollection)
    Dim textbox As TextBox
    Dim chbox As CheckBox
    Dim radio As RadioButton

    For Each ctrl As Control In root
      Select Case ctrl.GetType
        Case GetType(TextBox)
          textbox = TryCast(ctrl, TextBox)
          If textbox.Enabled AndAlso textbox.Text <> String.Empty Then
            Data.Add(textbox.Tag.ToString, textbox.Text)
          End If
        Case GetType(CheckBox)
          chbox = TryCast(ctrl, CheckBox)
          If chbox.Enabled AndAlso chbox.Checked Then
            Data.Add(chbox.Tag.ToString, "TRUE")
          End If
        Case GetType(RadioButton)
          radio = TryCast(ctrl, RadioButton)
          If radio.Enabled AndAlso radio.Checked Then
            Data.Add(radio.Parent.Tag.ToString, radio.Tag.ToString)
          End If
        Case GetType(GroupBox), GetType(TabPage)
          CollectData(ctrl.Controls)
      End Select
    Next
  End Sub
  ''' <summary>
  ''' Loads data to the UI
  ''' </summary>
  ''' <param name="root">Control collection to load data to</param>
  ''' <param name="load">Actual data</param>
  Public Sub LoadData(ByRef root As Control.ControlCollection, ByVal load As Dictionary(Of String, String))
    Dim textbox As TextBox
    Dim chbox As CheckBox
    Dim radio As RadioButton

    For Each ctrl As Control In root
      Select Case ctrl.GetType
        Case GetType(TextBox)
          textbox = TryCast(ctrl, TextBox)
          If load.ContainsKey(textbox.Tag.ToString) Then
            textbox.Text = load.Item(textbox.Tag.ToString)
          End If
        Case GetType(CheckBox)
          chbox = TryCast(ctrl, CheckBox)
          If load.ContainsKey(chbox.Tag.ToString) Then
            chbox.Checked = True
            chbox.Enabled = True
          End If
        Case GetType(RadioButton)
          radio = TryCast(ctrl, RadioButton)
          If load.ContainsKey(radio.Parent.Tag.ToString) Then
            radio.Checked = True
            radio.Enabled = True
          End If
        Case GetType(GroupBox), GetType(TabPage)
          LoadData(ctrl.Controls, load)
      End Select
    Next
  End Sub
End Class