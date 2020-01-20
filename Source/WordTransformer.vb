''' <summary>
''' Transform UI data to exportable format
''' Applies business rules to data
''' </summary>
Public Class WordTransformer
  ''' <summary>
  ''' Content ready to export
  ''' </summary>
  Private content As Dictionary(Of String, String)
  ''' <summary>
  ''' Whether to abort transformation if a required field is missing
  ''' </summary>
  ''' <returns></returns>
  Private Property AbortOnMissing As Boolean
  ''' <summary>
  ''' Event occurs when a field is missing
  ''' </summary>
  ''' <param name="fieldname"></param>
  Public Event FieldMissing(ByVal fieldname As String)
  ''' <summary>
  ''' Constructor
  ''' </summary>
  ''' <param name="abort">Abort if a required property missing</param>
  Public Sub New(ByVal abort As Boolean)
    AbortOnMissing = abort
  End Sub
  ''' <summary>
  ''' Returns exportable content
  ''' </summary>
  ''' <returns>exportable content</returns>
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
  ''' <param name="diag">Diagnose to add</param>
  Private Sub AddToDiag(ByVal diag As String)
    Try
      If content.ContainsKey("diag") Then
        content.Item("diag") += ", " + diag
      Else
        content.Add("diag", diag)
      End If
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  Private Function CheckRequired(ByVal key As String, ByRef data As Dictionary(Of String, String)) As Boolean
    If Not data.ContainsKey(key) Then
      RaiseEvent FieldMissing(key)
      Return False
    Else
      Return True
    End If
  End Function
  ''' <summary>
  ''' Applies business rules to transform data
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Public Sub ApplyRules(ByRef data As Dictionary(Of String, String))
    content = New Dictionary(Of String, String)

    ApplyRulesGeneral(data)
    ApplyRulesBrain(data)
    ApplyRulesHeart(data)
  End Sub
  ''' <summary>
  ''' Applies rules (general parts)
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Sub ApplyRulesGeneral(ByRef data As Dictionary(Of String, String))
    Dim key As String
    Try
      '########################################################################
      For Each key In {"hossz", "haj", "kor", "agy", "sziv", "tudo", "maj", "lep", "vese", "fog", "zsir"}
        If CheckRequired(key, data) Then
          content.Add(key, data.Item(key))
        ElseIf AbortOnMissing Then
          Exit Sub
        End If
      Next
      '########################################################################
      key = "nem"
      If CheckRequired(key, data) Then
        content.Add("nem_1", data.Item(key))
        content.Add("nem_2", data.Item(key))
      ElseIf AbortOnMissing Then
        Exit Sub
      End If
      '########################################################################
      key = "test"
      If CheckRequired(key, data) Then
        content.Add(key, data.Item(key))
        If data.Item(key) = "cachexiás" Then
          AddToDiag("Cachexia.")
        End If
      ElseIf AbortOnMissing Then
        Exit Sub
      End If
      '########################################################################
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

        Dim length = content.Item(key).Length
        If length > 2 Then
          content.Item(key) = content.Item(key).Remove(length - 2, 1)
        End If

        content.Item(key) += data.Item(key).ToString + " cm nagyságú felfekvéses fekély látható. "
      End If
      '########################################################################
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
      '########################################################################
      key = "asu_kp"
      If data.ContainsKey(key) Then
        content.Add("asu_kp_nyaki", "Az aorta mellkasi szakasza kp. tág, belfelszínén több lencsényi sárgásfehér plaque látható. ")
        content.Add("asu_kp_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér plaque látható. ")
        AddToDiag("Arteriosclerosis universalis mediocris gradus.")
      End If
      '########################################################################
      key = "asu_sulyos"
      If data.ContainsKey(key) Then
        content.Add("asu_sulyos_nyaki_1", "carotis-villák scleroticusak, egyebekben a ")
        content.Add("asu_sulyos_nyaki_2", "Az aorta mellkasi szakasza kp. tág, belfelszínén több lencsényi sárgásfehér meszes tapintatú plaque látható. ")
        content.Add("asu_sulyos_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér meszek tapintatú plaque látható. ")
        AddToDiag("Arteriosclerosis universalis maioris gradus praecipue aortae et arteriarum coronariarum cordis.")
      End If
      '########################################################################
      key = "ascites"
      If data.ContainsKey(key) Then
        content.Add("ascites", "A hasüregben ")
        If CheckRequired("asc_liter", data) Then
          content.Item("ascites") += data.Item("asc_liter").ToString + " liter szalmasárga folyadék található. "
          AddToDiag("Ascites.")
        ElseIf AbortOnMissing Then
          Exit Sub
        End If
      End If
      '########################################################################
      key = "icterus"
      If data.ContainsKey(key) Then
        content.Add("icterus_1", "sárgás árnyalatú, ")
        content.Add("icterus_2", "sárgás árnyalatúak, ")
        AddToDiag("Icterus universalis.")
      End If
      '########################################################################
      key = "pacemaker"
      If data.ContainsKey(key) Then
        content.Add("pacemaker_kul", "Bal oldalon infraclavicularisan pacemaker telep található. ")
        content.Add("pacemaker_nyaki", "A jobb szívfélben pacemaker elektróda azonosítható. ")
        If CheckRequired("pacemaker_serial", data) Then
          AddToDiag("Pacemaker. (" + data.Item("pacemaker_serial").ToString + ")")
        ElseIf AbortOnMissing Then
          Exit Sub
        End If
      End If
      '########################################################################
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Applies rules regarding the brain
  ''' </summary>
  ''' <param name="data">Data form UI</param>
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
      If AbortOnMissing Then
        Exit Sub
      End If

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
  ''' <param name="data">Data form UI</param>
  Private Sub ApplyRulesHeart(data As Dictionary(Of String, String))

    Dim key As String
    Dim text As String = ""

    '##########################################################################
    key = "sziv_allapot"

    Dim jobb_kamra As String = ""
    If data.ContainsKey("jobb_kamra") Then
      jobb_kamra = data.Item("jobb_kamra").ToString

    Else

      RaiseEvent FieldMissing("jobb kamra vastagság")
      If AbortOnMissing Then
        Exit Sub
      End If

    End If

    Dim bal_kamra As String = ""
    If data.ContainsKey("bal_kamra") Then
      bal_kamra = data.Item("bal_kamra").ToString

    Else

      RaiseEvent FieldMissing("bal kamra vastagság")
      If AbortOnMissing Then
        Exit Sub
      End If

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

    '##########################################################################
    key = "koszoru_kp"
    If data.ContainsKey(key) Then
      content.Add("koszoru_allapot", ", scleroticusak")
    End If

    '##########################################################################
    key = "koszoru_sulyos"
    If data.ContainsKey(key) Then

      If content.ContainsKey("koszoru_allapot") Then
        content.Item("koszoru_allapot") += ", súlyosan meszesek, lumenük szűkületet mutat"
      Else
        content.Add("koszoru_allapot", ", súlyosan meszesek, lumenük szűkületet mutat")
      End If

    End If

    '##########################################################################
    key = "koszoru_szuk"
    If data.ContainsKey(key) Then
      content.Add(key, "A ")

      If data.ContainsKey("koszoru_jobbAC") Then
        content.Item(key) += "jobb koszorúverőérben"
      End If

      If data.ContainsKey("koszoru_lad") Then

        If Not content.Item(key).EndsWith(" ") Then
          content.Item(key) += ", "
        End If

        content.Item(key) += "bal koszorúverőér elülső leszálló ágában"

      End If

      If data.ContainsKey("koszoru_cx") Then

        If Not content.Item(key).EndsWith(" ") Then
          content.Item(key) += ", "
        End If

        content.Item(key) += "bal koszorúverőér körbefutó ágában"

      End If

      content.Item(key) += " " + data.Item(key).ToString + " %-os lumenszűkület figyelhető meg. "

    End If

    '##########################################################################
    key = "stent"
    If data.ContainsKey(key) Then

      text = ""

      content.Add(key, "A ")

      If data.ContainsKey("stent_jobbAC") Then
        content.Item(key) += "jobb koszorúverőérben"
        text += "arteriae coronariae dextri cordis"
      End If

      If data.ContainsKey("stent_lad") Then

        If Not content.Item(key).EndsWith(" ") Then
          content.Item(key) += ", "
          text += ", "
        End If

        content.Item(key) += "bal koszorúverőér elülső leszálló ágában"
        text += "rami interventricularis anterioris arteriae coronariae sinistri cordis"

      End If

      If data.ContainsKey("stent_cx") Then

        If Not content.Item(key).EndsWith(" ") Then
          content.Item(key) += ", "
          text += ", "
        End If

        content.Item(key) += "bal koszorúverőér körbefutó ágában"
        text += "rami circumflexi arteriae coronariae sinistri cordis"

      End If

      Select Case data.Item(key)

        Case "stent"
          content.Item(key) += " stent implantatum található. "
          AddToDiag("Implantatum (stent) " + text + ".")
        Case "thrombus"
          content.Item(key) += " friss vérrögös elzáródás figyelhető meg. "
          AddToDiag("Thrombus recens " + text + ".")

      End Select

    End If

    '##########################################################################
    key = "infarktus"
    If data.ContainsKey(key) Then

      text = ""

      content.Add(key, "A ")

      If data.ContainsKey("inf_elulso") Then
        content.Item(key) += "bal kamra elülső falában"
        text += "parietis anterioris ventriculi sinsitri cordis"
      End If

      If data.ContainsKey("inf_hatso") Then
        If Not content.Item(key).EndsWith(" ") Then
          content.Item(key) += ", "
          text += ", "
        End If

        content.Item(key) += "bal kamra hátulsó falában"
        text += "parietis posterioris ventriculi sinsitri cordis"

      End If

      If data.ContainsKey("inf_septalis") Then

        If Not content.Item(key).EndsWith(" ") Then
          content.Item(key) += ", "
          text += ", "
        End If

        content.Item(key) += "bal kamra sövényi falában"
        text += "parietis septalis ventriculi sinsitri cordis"

      End If

      If data.ContainsKey("inf_oldal") Then

        If Not content.Item(key).EndsWith(" ") Then
          content.Item(key) += ", "
          text += ", "
        End If

        content.Item(key) += "bal kamra oldalsó falában"
        text += "parietis lateralis ventriculi sinsitri cordis"

      End If

      If data.ContainsKey("inf_meret") Then
        content.Item(key) += data.Item("inf_meret").ToString + " mm nagyságú "

      Else

        RaiseEvent FieldMissing("infarktus méret")
        If AbortOnMissing Then
          Exit Sub
        End If

      End If

      Select Case data.Item(key)

        Case "inf_regi"
          content.Item(key) += "szürkésfehér színű régi szívizomelhalás figyelhető meg. "
          AddToDiag("Infarctus obsoletus " + text + ".")

        Case "inf_friss"
          content.Item(key) += "agyagsárga színű, helyenként vörhenyes szegéllyel bíró, heveny szívizomelhalás figyelhető meg. "
          AddToDiag("Infarctus recens " + text + ".")

      End Select

    End If

    '##########################################################################
    key = "bill_sten"
    If data.ContainsKey(key) Then
      content.Add(key, "z aortabillentyű meszes szűkületet mutat, egyebekben a")
      AddToDiag("Stenosis calcificans ostii aortae.")
    End If

    '##########################################################################
    key = "bill_mitralis"
    If data.ContainsKey(key) Then
      content.Add(key, " mitralis billentyű anulusa kifejezetten meszese, egyebekben a")
      AddToDiag("Calcificatio ostii atrioventricularis sinistri cordis.")
    End If

    '##########################################################################
    key = "bill_haemo"
    If data.ContainsKey(key) Then

      If data.ContainsKey("haemo_g") Then

        content.Add(key, "A szívburokban ")
        content.Item(key) += data.Item("haemo_g").ToString + " g részben alvadt vér található. "
        AddToDiag("Haemopericardium.")

      Else

        RaiseEvent FieldMissing("alvadt vér mennyiség")
        If AbortOnMissing Then
          Exit Sub
        End If

      End If

    End If

  End Sub
End Class
