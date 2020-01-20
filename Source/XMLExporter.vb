''' <summary>
''' Export/import data from XML files
''' </summary>
Public Class XMLExporter
  ''' <summary>
  ''' Path to XML file
  ''' </summary>
  ''' <returns></returns>
  Private Property Path As String
  ''' <summary>
  ''' Constructor
  ''' </summary>
  Public Sub New()
    Path = Application.StartupPath + IO.Path.DirectorySeparatorChar + "saves.xml"
  End Sub
  ''' <summary>
  ''' Check if given patient is already in XML file
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Patient date</param>
  ''' <returns>True if exist / False if not</returns>
  Private Function CheckPatient(ByVal name As String, ByVal datte As String) As Boolean
    Dim root As XElement
    Try
      If Not IO.File.Exists(Path) Then
        Return False
      End If

      root = XElement.Load(Path)

      Dim patients As IEnumerable(Of XElement) =
        From el In root.<patient>
        Where el.@name = name And el.@date = datte
        Select el

      If patients.Count > 0 Then
        Return True
      Else
        Return False
      End If
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
    Return False
  End Function
  ''' <summary>
  ''' Saves patient data to XML file
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Patient date</param>
  ''' <param name="data">Data</param>
  Public Sub SaveData(ByVal name As String, ByVal datte As String, ByRef data As Dictionary(Of String, String))
    Dim root As XElement
    Dim patient = New XElement("patient")
    Dim element As XElement

    If name Is Nothing Then
      MsgBox("Név hiányzik")
      Exit Sub
    End If

    If datte Is Nothing Then
      MsgBox("Dátum hiányzik")
      Exit Sub
    End If

    If CheckPatient(name, datte) Then
      MsgBox("Már van rekord ezzel a névvel és dátummal")
      Exit Sub
    End If

    Try
      If Not IO.File.Exists(Path) Then
        root = <records></records>
      Else
        root = XElement.Load(Path)
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

      MsgBox("Sikeresen mentve")
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Load stored patients names and date
  ''' </summary>
  ''' <returns>Patient names and dates</returns>
  Public Function LoadPatients() As Dictionary(Of String, String)
    Dim results = New Dictionary(Of String, String)
    Dim root As XElement
    Try
      If Not IO.File.Exists(Path) Then
        Return results
      End If

      root = XElement.Load(Path)
      Dim patients As IEnumerable(Of XElement) =
        From el In root.<patient>
        Select el

      For Each element As XElement In patients
        results.Add(element.Attribute("name").Value, element.Attribute("date").Value)
      Next
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
    Return results
  End Function
  ''' <summary>
  ''' Load patient data from XML
  ''' </summary>
  ''' <param name="name">Patient name</param>
  ''' <param name="datte">Patient date</param>
  ''' <returns>Patient data</returns>
  Public Function LoadPatientData(ByVal name As String, ByVal datte As String) As Dictionary(Of String, String)
    Dim results = New Dictionary(Of String, String)
    Dim root As XElement
    Try
      If Not IO.File.Exists(Path) Then
        Return results
      End If

      root = XElement.Load(Path)
      Dim patients As IEnumerable(Of XElement) =
        From el In root.<patient>
        Where el.@name = name And el.@date = datte
        Select el

      For Each element As XElement In patients
        For Each subelem As XElement In element.Elements
          results.Add(subelem.Name.ToString, subelem.Value)
        Next
      Next
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
    Return results
  End Function
End Class
