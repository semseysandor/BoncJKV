''' <summary>
''' Export/import data from XML files
''' </summary>
Public Class XMLProcessor

  ''' <summary>
  ''' Path to XML file
  ''' </summary>
  Public Property FilePath As String

  ''' <summary>
  ''' Constructor
  ''' </summary>
  Public Sub New(Optional ByVal path As String = "")
    FilePath = path
  End Sub

  ''' <summary>
  ''' Check if given patient is already in XML file
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Inspection date</param>
  ''' <returns>True if exist / False if not</returns>
  Private Function CheckPatient(ByVal name As String, ByVal datte As String) As Boolean
    If Not IO.File.Exists(FilePath) Then
      Return False
    End If

    Dim root = XElement.Load(FilePath)

    Dim patients As IEnumerable(Of XElement) =
      From el In root.<patient>
      Where el.@name = name And el.@date = datte
      Select el

    If patients.Count > 0 Then
      Return True
    Else
      Return False
    End If

  End Function

  ''' <summary>
  ''' Saves patient data to XML file
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Inspection date</param>
  ''' <param name="data">Data</param>
  Public Sub SaveData(ByVal name As String, ByVal datte As String, ByRef data As Dictionary(Of String, String))
    If name = String.Empty Then
      AppUI.Warning("Név hiányzik", "Mentés")
      Exit Sub
    End If

    If datte = String.Empty Then
      AppUI.Warning("Dátum hiányzik", "Mentés")
      Exit Sub
    End If

    If CheckPatient(name, datte) Then
      If AppUI.Question("Már van rekord ezzel a névvel és dátummal, felülírjuk?", "Mentés") = DialogResult.Yes Then
        If Not DeletePatient(name, datte) Then
          AppUI.Warning("Mentés nem sikerült", "Mentés")
          Exit Sub
        End If
      Else
        Exit Sub
      End If
    End If

    Dim root As XElement
    Dim patient = New XElement("patient")
    Dim element As XElement

    If Not IO.File.Exists(FilePath) Then
      root = <records></records>
    Else
      root = XElement.Load(FilePath)
    End If

    patient.SetAttributeValue("name", name)
    patient.SetAttributeValue("date", datte)

    For Each row As KeyValuePair(Of String, String) In data
      element = New XElement(row.Key)
      element.Value = row.Value
      patient.Add(element)
    Next

    root.Add(patient)
    root.Save(FilePath)

    MsgBox("Sikeresen mentve")

  End Sub

  ''' <summary>
  ''' Load stored patients names and date
  ''' </summary>
  ''' <returns>Patient names and dates</returns>
  Public Function LoadPatients() As Dictionary(Of String, String)
    Dim results = New Dictionary(Of String, String)
    If Not IO.File.Exists(FilePath) Then
      Return results
    End If

    Dim root = XElement.Load(FilePath)
    Dim patients As IEnumerable(Of XElement) =
      From el In root.<patient>
      Select el

    For Each element As XElement In patients
      results.Add(element.Attribute("name").Value, element.Attribute("date").Value)
    Next

    Return results

  End Function

  ''' <summary>
  ''' Load patient data from XML
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Inspection date</param>
  ''' <returns>Patient data</returns>
  Public Function LoadData(ByVal name As String, ByVal datte As String) As Dictionary(Of String, String)
    Dim results = New Dictionary(Of String, String)

    If Not IO.File.Exists(FilePath) Then
      Return results
    End If

    Dim root = XElement.Load(FilePath)
    Dim patient As IEnumerable(Of XElement) =
      From el In root.<patient>
      Where el.@name = name And el.@date = datte
      Select el
      Take 1

    For Each element As XElement In patient
      For Each subelem As XElement In element.Elements
        results.Add(subelem.Name.ToString, subelem.Value)
      Next
    Next

    Return results

  End Function

  ''' <summary>
  ''' Deletes a patient
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Inspection date</param>
  ''' <returns></returns>
  Private Function DeletePatient(ByVal name As String, ByVal datte As String) As Boolean
    If Not IO.File.Exists(FilePath) Then
      Return False
    End If

    Dim root = XElement.Load(FilePath)
    Dim patient As IEnumerable(Of XElement) =
      From el In root.<patient>
      Where el.@name = name And el.@date = datte
      Select el
      Take 1

    patient.Remove
    root.Save(FilePath)

    Return True

  End Function
End Class
