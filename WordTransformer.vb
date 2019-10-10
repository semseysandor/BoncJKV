''' <summary>
''' Transform UI data to exportable format
''' </summary>
Public Class WordTransformer
  Private content As Dictionary(Of String, String)
  Public Event FieldMissing(ByVal fieldname As String)
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
  End Sub
  ''' <summary>
  ''' Adds a new diagnose to the diagnoses
  ''' </summary>
  ''' <param name="diag"></param>
  Private Sub AddToDiag(ByVal diag As String)
    If content.ContainsKey("diag") Then
      content.Item("diag") += ", " + diag
    Else
      content.Add("diag", diag)
    End If
  End Sub
  ''' <summary>
  ''' Applies business rules to transform data
  ''' </summary>
  ''' <param name="data"></param>
  Public Sub ApplyRules(data As Dictionary(Of String, String))
    content = New Dictionary(Of String, String)
    ApplyRulesGeneral(data)
    ApplyRulesBrain(data)
    ApplyRulesHeart(data)
  End Sub
  ''' <summary>
  ''' Applies rules (general parts)
  ''' </summary>
  ''' <param name="data"></param>
  Private Sub ApplyRulesGeneral(data As Dictionary(Of String, String))

    Dim key As String

    '##########################################################################
    For Each key In {"hossz", "haj", "kor", "agy", "sziv", "tudo", "maj", "lep", "vese", "fog", "zsir"}
      If data.ContainsKey(key) Then
        content.Add(key, data.Item(key))
      Else
        RaiseEvent FieldMissing(key)
        'Exit Sub
      End If
    Next

    '##########################################################################
    key = "nem"
    If data.ContainsKey(key) Then
      content.Add("nem_1", data.Item(key))
      content.Add("nem_2", data.Item(key))
    Else
      RaiseEvent FieldMissing(key)
      'Exit Sub
    End If

    '##########################################################################
    key = "test"
    If data.ContainsKey(key) Then
      content.Add(key, data.Item(key))
      If data.Item(key) = "cachexiás" Then
        AddToDiag("Cachexia.")
      End If
    Else
      RaiseEvent FieldMissing(key)
      'Exit Sub
    End If

    '##########################################################################
    key = "decub"
    If data.ContainsKey(key) Then
      content.Add(key, "A ")

      If data.ContainsKey("decub_sacralis") Then
        content.Item(key) += "keresztcsont területében, "
        AddToDiag("Decubitus sacralis.")
      End If

      If data.ContainsKey("decub_sarkak") Then
        content.Item(key) += "sarkakon, "
        AddToDiag("Decubitus calcanei l. u.")
      End If

      If data.ContainsKey("decub_jobb_sarok") Then
        content.Item(key) += "jobb sarkon, "
        AddToDiag("Decubitus calcanei dextri.")
      End If

      If data.ContainsKey("decub_bal_sarok") Then
        content.Item(key) += "bal sarkon, "
        AddToDiag("Decubitus calcanei sinistri.")
      End If

      Dim length As Integer = content.Item(key).Length
      If length > 2 Then
        content.Item(key) = content.Item(key).Remove(length - 2, 1)
      End If
      content.Item(key) += data.Item(key).ToString + " cm nagyságú felfekvéses fekély látható. "
    End If

    '##########################################################################
    key = "amputacio"
    If data.ContainsKey(key) Then

      Select Case data.Item(key)

        Case "jobb_comb"
          content.Add(key, "jobb alsó végtag combszintben amputálva, egyebekben a ")
          AddToDiag("Status post amputationem femoris dextri.")

        Case "bal_comb"
          content.Add(key, "bal alsó végtag combszintben amputálva, egyebekben a ")
          AddToDiag("Status post amputationem femoris sinistri.")

        Case "combok"
          content.Add(key, "alsó végtagok combszintben amputálva, egyebekben a ")
          AddToDiag("Status post amputationem femoris l. u.")

        Case "jobb_labszar"
          content.Add(key, "jobb alsó végtag lábszárszintben amputálva, egyebekben a ")
          AddToDiag("Status post amputationem cruris dextri.")

        Case "bal_labszar"
          content.Add(key, "bal alsó végtag lábszárszintben amputálva, egyebekben a ")
          AddToDiag("Status post amputationem crusis sinistri.")

        Case "labszarak"
          content.Add(key, "alsó végtagok lábszárszintben amputálva, egyebekben a ")
          AddToDiag("Status post amputationem cruris l. u.")

      End Select
    End If

    '##########################################################################
    key = "asu_kp"
    If data.ContainsKey(key) Then
      content.Add("asu_kp_nyaki", "Az aorta mellkasi szakasza kp. tág, belfelszínén több lencsényi sárgásfehér plaque látható. ")
      content.Add("asu_kp_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér plaque látható. ")
      AddToDiag("Arteriosclerosis universalis mediocris gradus.")
    End If

    '##########################################################################
    key = "asu_sulyos"
    If data.ContainsKey(key) Then
      content.Add("asu_sulyos_nyaki_1", "carotis-villák scleroticusak, egyebekben a ")
      content.Add("asu_sulyos_nyaki_2", "Az aorta mellkasi szakasza kp. tág, belfelszínén több lencsényi sárgásfehér meszes tapintatú plaque látható. ")
      content.Add("asu_sulyos_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér meszek tapintatú plaque látható. ")
      AddToDiag("Arteriosclerosis universalis maioris gradus praecipue aortae et arteriarum coronariarum cordis.")
    End If

    '##########################################################################
    key = "ascites"
    If data.ContainsKey(key) Then
      content.Add("ascites", "A hasüregben ")
      If data.ContainsKey("asc_liter") Then
        content.Item("ascites") += data.Item("asc_liter").ToString + " liter szalmasárga folyadék található. "
        AddToDiag("Ascites.")
      Else
        RaiseEvent FieldMissing("Ascites liter")
        'Exit Sub
      End If
    End If

    '##########################################################################
    key = "icterus"
    If data.ContainsKey(key) Then
      content.Add("icterus_1", "sárgás árnyalatú, ")
      content.Add("icterus_2", "sárgás árnyalatúak, ")
      AddToDiag("Icterus universalis.")
    End If

    '##########################################################################
    key = "pacemaker"
    If data.ContainsKey(key) Then
      content.Add("pacemaker_kul", "Bal oldalon infraclavicularisan pacemaker telep található. ")
      content.Add("pacemaker_nyaki", "A jobb szívfélben pacemaker elektróda azonosítható. ")

      If data.ContainsKey("pacemaker_serial") Then
        AddToDiag("Pacemaker. (" + data.Item("pacemaker_serial").ToString + ")")
      Else
        RaiseEvent FieldMissing("pacemaker sorozatszám")
        'Exit Sub
      End If
    End If

  End Sub
  ''' <summary>
  ''' Applies rules regarding the brain
  ''' </summary>
  ''' <param name="data"></param>
  Private Sub ApplyRulesBrain(data As Dictionary(Of String, String))

    Dim key As String

    '##########################################################################
    key = "agy_allapot"
    If data.ContainsKey(key) Then

      Select Case data.Item(key)

        Case "normal"
          content.Add("agy_1", "Az agy tészta tapintatú, a tekervények és a barázdák kp. nagyságúak. ")
          content.Add("agy_2", "Az agytörzs és a kisagy eltérés nélkül.")
        Case "oedema"
          content.Add("agy_1", "A vizenyős agy tészta tapintatú, a tekervények kiszélesedtek, a barázdák sekélyek. ")
          content.Add("agy_2", "Az agytörzs és a kisagy eltérés nélkül.")
          AddToDiag("Oedema cerebri.")
      End Select
    Else
      RaiseEvent FieldMissing("agy állapota")
      'Exit Sub

    End If

    '##########################################################################
    key = "agy_beek"
    If data.ContainsKey(key) Then
      content.Item("agy_2") = "Az agytörzs vizenyős, a kisagyi tonsillák körülárkoltak."
      AddToDiag("Oedema trunci cerebri et herniatio tonsillarum cerebelli.")
    End If

    '##########################################################################
    key = "sat_lac"
    If data.ContainsKey(key) Then
      content.Add("agy_sat_lac", "A basalis magvakban és a fehérállományban több gócban 1-2mm nagyságú üregek találhatók. ")
      AddToDiag("Staus lacunaris cerebri.")
    End If

    '##########################################################################
    key = "agy_elvaltozas"
    If data.ContainsKey(key) Then

      Dim diag_elv As String = ""
      Dim elv As String = ""
      Dim oldal As String = ""
      Dim diag_oldal As String = ""
      Dim lebeny As String = ""
      Dim diag_lebeny As String = ""
      Dim meret As String = ""

      Select Case data.Item(key)

        Case "verzes"
          elv = "agyállományi vérzés "
          diag_elv = "Apoplexia "

        Case "lagyulas"
          elv = "lágyulás "
          diag_elv = "Emollitio "

        Case "attet"
          elv = "daganatáttét "
          diag_elv = "Metastasis "

      End Select

      If data.ContainsKey("agy_oldal") Then

        Select Case data.Item("agy_oldal")

          Case "jobb"
            oldal = "jobb "
            diag_oldal = " hemispherii dextri"

          Case "bal"
            oldal = "bal "
            diag_oldal = " hemispherii sinistri"

        End Select

      End If

      If data.ContainsKey("agy_lebeny") Then

        Select Case data.Item("agy_lebeny")

          Case "frontalis", "parietalis", "temporalis", "occipitalis"
            lebeny = oldal + data.Item("agy_lebeny") + " lebenyben "
            diag_lebeny = "lobi " + data.Item("agy_lebeny") + diag_oldal + " cerebri."

          Case "kisagy"
            lebeny = "kisagyi féltekében "
            diag_lebeny = "cerebelli."

            If data.ContainsKey("agy_beek") Then
              content.Item("agy_2") = "Az agytörzs vizenyős, a kisagyi tonsillák körülárkoltak egyebekben eltérés nélkül."
            Else
              content.Item("agy_2") = "Az agytörzs és a kisagy egyebekben eltérés nélkül."
            End If

        End Select

      End If

      If data.ContainsKey("agy_elv_meret") Then
        meret = data.Item("agy_elv_meret") + " cm kiterjedésű "
      End If

      content.Add("agy_elvaltozas", "A " + lebeny + meret + elv + "figyelhető meg. ")
      AddToDiag(diag_elv + diag_lebeny)

    End If
  End Sub
  ''' <summary>
  ''' Applies rules regarding the heart
  ''' </summary>
  ''' <param name="data"></param>
  Private Sub ApplyRulesHeart(data As Dictionary(Of String, String))
    Dim key As String

    '##########################################################################
    key = "sziv_allapot"

    Dim jobb_kamra As String = ""
    Dim bal_kamra As String = ""
    Dim text As String = ""

    If data.ContainsKey("jobb_kamra") Then
      jobb_kamra = data.Item("jobb_kamra").ToString
    Else
      RaiseEvent FieldMissing("jobb kamra vastagság")
      'Exit Sub
    End If

    If data.ContainsKey("bal_kamra") Then
      bal_kamra = data.Item("bal_kamra").ToString
    Else
      RaiseEvent FieldMissing("bal kamra vastagság")
      'Exit Sub
    End If

    If data.ContainsKey(key) Then

      Select Case data.Item(key)
        Case "konc"
          text = "A szív megnagyobbodott. A körkörösen túltengett bal kamra fala "
          text += bal_kamra + " mm, a jobb kamra fala " + jobb_kamra + " mm vastag. "
          AddToDiag("Hypertrophia concentrica ventriculi sinistri cordis.")

        Case "tagult"
          text = "A szív megnagyobbodott. A tágult, túltengett bal kamra fala "
          text += bal_kamra + " mm, a jobb kamra fala " + jobb_kamra + " mm vastag. "
          AddToDiag("Hypertrophia dilatativa ventriculi sinsitri cordis.")

        Case "cor_pulm"
          text = "A bal kamra fala " + bal_kamra + " mm, a tágult, túltengett jobb kamra fala " + jobb_kamra + " mm vastag. "
          AddToDiag("Cor pulmonale chronicum.")

        Case "dcm"
          text = "A szív kifejezetten megnagyobbodott. A bal kamra fala "
          text += bal_kamra + " mm, a jobb kamra fala " + jobb_kamra
          text += " mm vastag, a kamrák fala elvékonyodott, lumenük extrém mértékben tágult. "
          AddToDiag("Cardiomyopathia dilatativa.")

        Case "iszb"
          text = "A bal kamra fala " + bal_kamra + " mm, a jobb kamra fala " + jobb_kamra + " mm vastag. "
          content.Add("iszb", ", metszéslapján szürkésfehér rajzolat mutatkozik")
          AddToDiag("Cardyomyopathia ischaemica chronica.")

      End Select
      content.Add("sziv_allapot", text)
    Else
      text = "A bal kamra fala " + bal_kamra + " mm, a jobb kamra fala " + jobb_kamra + " mm vastag. "
      content.Add("sziv_allapot", text)
    End If

  End Sub
End Class
