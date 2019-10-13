''' <summary>
''' Export/import data from XML files
''' </summary>
Public Class XMLExporter
  ''' <summary>
  ''' Path to XML file
  ''' </summary>
  ''' <returns></returns>
  Private Property path As String
  Public Sub New()
    path = Application.StartupPath + IO.Path.DirectorySeparatorChar + "saves.xml"
  End Sub
  ''' <summary>
  ''' Check if given patient is already in XML file
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Patient date</param>
  ''' <returns>True if exist / False if not</returns>
  Private Function CheckPatient(ByVal name As String, ByVal datte As String) As Boolean

    Dim root As XElement

    If Not IO.File.Exists(path) Then
      Return False
    Else
      root = XElement.Load(path)
    End If

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
  ''' Saves patient data to XML filem
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Patient date</param>
  ''' <param name="data">Data</param>
  Public Sub SaveData(ByVal name As String, ByVal datte As String, ByRef data As Dictionary(Of String, String))

    Dim root As XElement
    Dim patient As XElement = New XElement("patient")
    Dim element As XElement

    If CheckPatient(name, datte) Then
      MsgBox("Már van rekord ezzel a névvel és dátummal")
      Exit Sub
    End If

    If Not IO.File.Exists(path) Then
      root = <root></root>
    Else
      root = XElement.Load(path)
    End If

    patient.SetAttributeValue("name", name)
    patient.SetAttributeValue("date", datte)

    For Each row As KeyValuePair(Of String, String) In data
      element = New XElement(row.Key)
      element.Value = row.Value
      patient.Add(element)
    Next

    root.Add(patient)
    root.Save(Application.StartupPath + IO.Path.DirectorySeparatorChar + "saves.xml")

  End Sub
  ''' <summary>
  ''' Load stored patients names and date
  ''' </summary>
  ''' <returns>Patient names and dates</returns>
  Public Function LoadPatients() As Dictionary(Of String, String)

    Dim results As Dictionary(Of String, String) = New Dictionary(Of String, String)
    Dim root As XElement

    If Not IO.File.Exists(path) Then
      root = <root></root>
    Else
      root = XElement.Load(path)
    End If

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
  ''' <param name="datte">Patient date</param>
  ''' <returns>Patient data</returns>
  Public Function LoadPatientData(ByVal name As String, ByVal datte As String) As Dictionary(Of String, String)

    Dim results As Dictionary(Of String, String) = New Dictionary(Of String, String)
    Dim root As XElement

    If Not IO.File.Exists(path) Then
      root = <root></root>
    Else
      root = XElement.Load(path)
    End If

    Dim patients As IEnumerable(Of XElement) =
      From el In root.<patient>
      Where el.@name = name And el.@date = datte
      Select el

    For Each element As XElement In patients
      For Each subelem As XElement In element.Elements
        results.Add(subelem.Name.ToString, subelem.Value)
      Next
    Next

    Return results
  End Function
End Class
