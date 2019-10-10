''' <summary>
''' Transform UI data to exportable format
''' </summary>
Public Class WordTransformer
  Private content As Dictionary(Of String, String)
  Private diag As String
  Public Sub New()
  End Sub
  ''' <summary>
  ''' Returns exportable content
  ''' </summary>
  ''' <returns></returns>
  Public Function GetContent() As Dictionary(Of String, String)
    Return content
  End Function
  ''' <summary>
  ''' Prints content to the console
  ''' </summary>
  Public Sub PrintContent()
    Console.WriteLine("Content DATA *******************************")
    For Each row As KeyValuePair(Of String, String) In content
      Console.WriteLine(row.Key.ToString + " " + row.Value.ToString)
    Next
    Console.WriteLine("diag: " + diag)
  End Sub
  ''' <summary>
  ''' Applies business rules to transform data
  ''' </summary>
  ''' <param name="data"></param>
  Public Sub ApplyRules(data As Dictionary(Of String, String))

    Dim name As String
    Dim value As String

    content = New Dictionary(Of String, String)
    diag = ""

    For Each row As KeyValuePair(Of String, String) In data
      name = row.Key
      value = row.Value

      If name = "hossz" OrElse name = "haj" _
        OrElse name = "kor" _
        OrElse name = "agy" OrElse name = "sziv" _
        OrElse name = "tudo" OrElse name = "maj" _
        OrElse name = "lep" OrElse name = "vese" _
        OrElse name = "fog" OrElse name = "zsir" Then
        content.Add(name, value)
        Continue For
      End If

      If name = "nem" Then

        content.Add("nem_1", value)
        content.Add("nem_2", value)
        Continue For
      End If

      If name = "test" Then
        content.Add(name, value)
        If value = "cachexiás" Then
          diag += "Cachexia., "
        End If
        Continue For
      End If

      If name = "decub" Then
        content.Add("decub", "A ")

        If data.ContainsKey("decub_sacralis") Then
          content.Item("decub") += "keresztcsont teruleteben, "
          diag += "Decubitus sacralis., "
        End If
        If data.ContainsKey("decub_sarkak") Then
          content.Item("decub") += "sarkakon, "
          diag += "Decubitus calcanei l. u., "
        End If
        If data.ContainsKey("decub_jobb_sarok") Then
          content.Item("decub") += "jobb sarkon, "
          diag += "Decubitus calcanei dextri., "
        End If
        If data.ContainsKey("decub_bal_sarok") Then
          content.Item("decub") += "bal sarkon, "
          diag += "Decubitus calcanei sinistri., "
        End If
        Dim length As Integer = content.Item("decub").Length
        If length > 2 Then
          content.Item("decub") = content.Item("decub").Remove(length - 2)
          content.Item("decub") += " "
        End If
        content.Item("decub") += value.ToString + " cm nagysagu felfekveses fekely lathato."

        Continue For
      End If

      If name = "amputacio" Then
        content.Add("amputacio", "")
        Select Case value
          Case "jobb_comb"
            content.Item("amputacio") = "jobb alsó végtag combszintben amputálva, egyebekben a "
            diag += "Status post amputationem femoris dextri., "

          Case "bal_comb"
            content.Item("amputacio") = "bal alsó végtag combszintben amputálva, egyebekben a "
            diag += "Status post amputationem femoris sinistri., "

          Case "combok"
            content.Item("amputacio") = "alsó végtagok combszintben amputálva, egyebekben a "
            diag += "Status post amputationem femoris l. u., "

          Case "jobb_labszar"
            content.Item("amputacio") = "jobb alsó végtag lábszárszintben amputálva, egyebekben a "
            diag += "Status post amputationem cruris dextri., "

          Case "bal_labszar"
            content.Item("amputacio") = "bal alsó végtag lábszárszintben amputálva, egyebekben a "
            diag += "Status post amputationem crusis sinistri., "

          Case "labszarak"
            content.Item("amputacio") = "alsó végtagok lábszárszintben amputálva, egyebekben a "
            diag += "Status post amputationem cruris l. u., "

        End Select
        Continue For

      End If

      If name = "asu_kp" Then
        content.Add("asu_kp_nyaki", "Az aorta mellkasi szakasza kp. tág, belfelszínén több lencsényi sárgásfehér plaque látható. ")
        content.Add("asu_kp_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér plaque látható. ")
        diag += "Arteriosclerosis universalis mediocris gradus., "
        Continue For
      End If

      If name = "asu_sulyos" Then
        content.Add("asu_sulyos_nyaki_1", "carotis-villák scleroticusak, egyebekben a ")
        content.Add("asu_sulyos_nyaki_2", "Az aorta mellkasi szakasza kp. tág, belfelszínén több lencsényi sárgásfehér meszes tapintatú plaque látható. ")
        content.Add("asu_sulyos_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér meszek tapintatú plaque látható. ")
        diag += "Arteriosclerosis universalis maioris gradus praecipue aortae et arteriarum coronariarum cordis., "
        Continue For
      End If

      If name = "ascites" Then
        content.Add("ascites", "A hasüregben ")
        content.Item("ascites") += data.Item("asc_liter").ToString + " liter szalmasárga folyadék található. "
        diag += "Ascites., "
        Continue For
      End If

      If name = "icterus" Then
        content.Add("icterus_1", "sárgás árnyalatú, ")
        content.Add("icterus_2", "sárgás árnyalatúak, ")
        diag += "Icterus universalis., "
        Continue For
      End If

      If name = "pacemaker" Then
        content.Add("pacemaker_kul", "Bal oldalon infraclavicularisan pacemaker telep található. ")
        content.Add("pacemaker_nyaki", "A jobb szívfélben pacemaker elektróda azonosítható. ")

        If data.ContainsKey("pacemaker_serial") Then
          diag += "Pacemaker. (" + data.Item("pacemaker_serial").ToString + "), "
        End If

        Continue For
      End If




    Next



  End Sub
End Class
