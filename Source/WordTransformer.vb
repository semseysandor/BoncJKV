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
  ''' <summary>
  ''' Checks if required field is available
  ''' </summary>
  ''' <param name="key">Key to search</param>
  ''' <param name="data">The data that contains the key</param>
  ''' <returns>True if found, false if not</returns>
  Private Function CheckRequired(ByVal key As String, ByRef data As Dictionary(Of String, String)) As Boolean
    If data.ContainsKey(key) Then
      Return True
    Else
      RaiseEvent FieldMissing(key)
      Return False
    End If
  End Function
  ''' <summary>
  ''' Builds string expression to indicate location
  ''' </summary>
  ''' <param name="meret">Size</param>
  ''' <param name="oldal">Side</param>
  ''' <param name="lebeny">Organ</param>
  ''' <returns>Location and diagnose</returns>
  Private Function BrainLocationBuilder(ByVal meret As String, ByVal oldal As String, ByVal lebeny As String) As Dictionary(Of String, String)
    Dim result = New Dictionary(Of String, String) From {{"helyzet", ""}, {"diag", ""}}
    Select Case lebeny
      Case "frontalis", "parietalis", "temporalis", "occipitalis"
        result.Item("helyzet") = oldal + " " + lebeny + " lebenyben "
        Select Case oldal
          Case "jobb"
            result.Item("diag") = "lobi " + lebeny + " hemispherii dextri" + " cerebri."
          Case "bal"
            result.Item("diag") = "lobi " + lebeny + " hemispherii sinistri" + " cerebri."
        End Select
      Case "kisagy"
        result.Item("helyzet") = "kisagyi féltekében "
        result.Item("diag") = "cerebelli."
    End Select
    result.Item("helyzet") += meret + " cm kiterjedésű "
    Return result
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

        content.Item(key) += data.Item(key) + " cm nagyságú felfekvéses fekély látható. "
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
        content.Add("asu_sulyos_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér meszes tapintatú plaque látható. ")
        AddToDiag("Arteriosclerosis universalis maioris gradus praecipue aortae et arteriarum coronariarum cordis.")
      End If
      '########################################################################
      key = "ascites"
      If data.ContainsKey(key) Then
        content.Add("ascites", "A hasüregben ")
        If CheckRequired("asc_liter", data) Then
          content.Item("ascites") += data.Item("asc_liter") + " liter szalmasárga folyadék található. "
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
        If CheckRequired("pacemaker_serial", data) Then
          AddToDiag("Pacemaker. (" + data.Item("pacemaker_serial") + ")")
        ElseIf AbortOnMissing Then
          Exit Sub
        End If
        content.Add("pacemaker_kul", "Bal oldalon infraclavicularisan pacemaker telep található. ")
        content.Add("pacemaker_nyaki", "A jobb szívfélben pacemaker elektróda azonosítható. ")
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
    Dim text = ""
    Dim elvaltozas As Dictionary(Of String, String)
    Dim oldal As String
    Dim meret = ""
    Dim darab = ""
    Try
      '########################################################################
      key = "agy_allapot"
      If CheckRequired(key, data) Then
        Select Case data.Item(key)
          Case "normal"
            content.Add("agy_1", "Az agy tészta tapintatú, a tekervények és a barázdák kp. nagyságúak. ")
          Case "oedema"
            content.Add("agy_1", "A vizenyős agy tészta tapintatú, a tekervények kiszélesedtek, a barázdák sekélyek. ")
            AddToDiag("Oedema cerebri.")
        End Select
        content.Add("agy_2", "Az agytörzs és a kisagy eltérés nélkül.")
      ElseIf AbortOnMissing Then
        Exit Sub
      End If
      '########################################################################
      key = "agy_beek"
      If data.ContainsKey(key) Then
        content.Item("agy_2") = "Az agytörzs vizenyős, a kisagyi tonsillák körülárkoltak."
        AddToDiag("Oedema trunci cerebri et herniatio tonsillarum cerebelli.")
      End If
      '########################################################################
      key = "agy_stat_lac"
      If data.ContainsKey(key) Then
        content.Add("agy_stat_lac", "A basalis magvakban és a fehérállományban több gócban 1-2 mm nagyságú üregek találhatóak. ")
        AddToDiag("Status lacunaris cerebri.")
      End If
      '########################################################################
      key = "agy_lagyulas"
      If data.ContainsKey(key) Then
        If data.ContainsKey("agy_lagyulas_oldal") Then
          oldal = data.Item("agy_lagyulas_oldal")
        Else
          oldal = ""
        End If
        If CheckRequired("agy_lagyulas_lebeny", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If
        If CheckRequired("agy_lagyulas_meret", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If

        elvaltozas = BrainLocationBuilder(data.Item("agy_lagyulas_meret"), oldal, data.Item("agy_lagyulas_lebeny"))
        text = "A " + elvaltozas.Item("helyzet") + "lágyulás"
        AddToDiag("Emollitio " + elvaltozas.Item("diag"))
      End If
      '########################################################################
      key = "agy_verzes"
      If data.ContainsKey(key) Then
        If data.ContainsKey("agy_verzes_oldal") Then
          oldal = data.Item("agy_verzes_oldal")
        Else
          oldal = ""
        End If
        If CheckRequired("agy_verzes_lebeny", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If
        If CheckRequired("agy_verzes_meret", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If

        elvaltozas = BrainLocationBuilder(data.Item("agy_verzes_meret"), oldal, data.Item("agy_verzes_lebeny"))
        If text <> "" Then
          text += ", a " + elvaltozas.Item("helyzet") + "agyállományi vérzés"
        Else
          text = "A " + elvaltozas.Item("helyzet") + "agyállományi vérzés"
        End If
        AddToDiag("Apoplexia " + elvaltozas.Item("diag"))
      End If
      '########################################################################
      key = "agy_attet"
      If data.ContainsKey(key) Then
        If data.ContainsKey("agy_attet_oldal") Then
          oldal = data.Item("agy_attet_oldal")
        Else
          oldal = ""
        End If
        If CheckRequired("agy_attet_darab", data) Then
          darab = data.Item("agy_attet_darab")
        ElseIf AbortOnMissing Then
          Exit Sub
        End If
        If CheckRequired("agy_attet_meret", data) Then
          meret = data.Item("agy_attet_meret")
        ElseIf AbortOnMissing Then
          Exit Sub
        End If

        If data.ContainsKey("agy_attet_front") Then
          elvaltozas = BrainLocationBuilder(meret, oldal, "frontalis")
          If text <> "" Then
            text += ", a " + elvaltozas.Item("helyzet") + "daganatáttét"
          Else
            text = "A " + elvaltozas.Item("helyzet") + "daganatáttét"
          End If
          AddToDiag("Metastasis " + elvaltozas.Item("diag"))
        End If
        If data.ContainsKey("agy_attet_parietalis") Then
          elvaltozas = BrainLocationBuilder(meret, oldal, "parietalis")
          If text <> "" Then
            text += ", a " + elvaltozas.Item("helyzet") + "daganatáttét"
          Else
            text = "A " + elvaltozas.Item("helyzet") + "daganatáttét"
          End If
          AddToDiag("Metastasis " + elvaltozas.Item("diag"))
        End If
        If data.ContainsKey("agy_attet_temporalis") Then
          elvaltozas = BrainLocationBuilder(meret, oldal, "temporalis")
          If text <> "" Then
            text += ", a " + elvaltozas.Item("helyzet") + "daganatáttét"
          Else
            text = "A " + elvaltozas.Item("helyzet") + "daganatáttét"
          End If
          AddToDiag("Metastasis " + elvaltozas.Item("diag"))
        End If
        If data.ContainsKey("agy_attet_occ") Then
          elvaltozas = BrainLocationBuilder(meret, oldal, "occipitalis")
          If text <> "" Then
            text += ", a " + elvaltozas.Item("helyzet") + "daganatáttét"
          Else
            text = "A " + elvaltozas.Item("helyzet") + "daganatáttét"
          End If
          AddToDiag("Metastasis " + elvaltozas.Item("diag"))
        End If
        If data.ContainsKey("agy_attet_kisagy") Then
          elvaltozas = BrainLocationBuilder(meret, oldal, "kisagy")
          If text <> "" Then
            text += ", a " + elvaltozas.Item("helyzet") + "daganatáttét"
          Else
            text = "A " + elvaltozas.Item("helyzet") + "daganatáttét"
          End If
          AddToDiag("Metastasis " + elvaltozas.Item("diag"))
        End If
      End If
      If text <> "" Then
        content.Add("agy_elvaltozas", text + " figyelhető meg. ")
      End If

      If (data.ContainsKey("agy_lagyulas_lebeny") AndAlso data.Item("agy_lagyulas_lebeny") = "kisagy") OrElse
        (data.ContainsKey("agy_verzes_lebeny") AndAlso data.Item("agy_verzes_lebeny") = "kisagy") OrElse
        data.ContainsKey("agy_attet_kisagy") Then
        If data.ContainsKey("agy_beek") Then
          content.Item("agy_2") = "Az agytörzs vizenyős, a kisagyi tonsillák körülárkoltak egyebekben eltérés nélkül."
        Else
          content.Item("agy_2") = "Az agytörzs és a kisagy egyebekben eltérés nélkül."
        End If
      End If
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Applies rules regarding the heart
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Sub ApplyRulesHeart(data As Dictionary(Of String, String))
    Dim key As String
    Dim text As String
    Try
      '########################################################################
      key = "sziv_bal_kamra"
      If CheckRequired(key, data) AndAlso AbortOnMissing Then
        Exit Sub
      End If

      key = "sziv_jobb_kamra"
      If CheckRequired(key, data) AndAlso AbortOnMissing Then
        Exit Sub
      End If

      key = "sziv_allapot"
      text = "A "
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "konc"
            text = "körkörösen túltengett "
            AddToDiag("Hypertrophia concentrica ventriculi sinistri cordis.")
          Case "tagult"
            text = "tágult, túltengett "
            AddToDiag("Hypertrophia dilatativa ventriculi sinsitri cordis.")
        End Select
        content.Add("sziv_allapot_1", "A szív megnagyobbodott. ")
      End If
      text += "bal kamra fala " + data.Item("sziv_bal_kamra") + " mm, a "

      key = "sziv_cor_pulm"
      If data.ContainsKey(key) Then
        text = "tágult, túltengett "
        AddToDiag("Cor pulmonale chronicum.")
      End If
      text += "jobb kamra fala " + data.Item("sziv_jobb_kamra") + " mm vastag"

      key = "sziv_dcm"
      If data.ContainsKey(key) Then
        text += ", a kamrák fala elvékonyodott, lumenük extrém mértékben tágult"
        AddToDiag("Cardiomyopathia dilatativa.")
        If content.ContainsKey("sziv_allapot_1") Then
          content.Item("sziv_allapot_1") = "A szív kifejezetten megnagyobbodott."
        Else
          content.Add("sziv_allapot_1", "A szív kifejezetten megnagyobbodott. ")
        End If
      End If
      text += ". "
      content.Add("sziv_allapot_2", text)
      text = ""
      '########################################################################
      key = "sziv_iszb"
      If data.ContainsKey(key) Then
        content.Add("iszb", ", metszéslapján szürkésfehér rajzolat mutatkozik")
        AddToDiag("Cardyomyopathia ischaemica chronica.")
      End If
      '########################################################################
      key = "sziv_erek"
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "kp"
            content.Add("koszoru_allapot", ", scleroticusak")
          Case "sulyos"
            content.Add("koszoru_allapot", ", súlyosan meszesek, lumenük szűkületet mutat")
        End Select
      End If
      '########################################################################
      key = "sziv_szukulet"
      If data.ContainsKey(key) Then
        If CheckRequired("sziv_szuk_percent", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If

        Select Case data.Item(key)
          Case "jobb"
            content.Add("koszoru_szuk", "A jobb koszorúverőérben ")
          Case "lad"
            content.Add("koszoru_szuk", "A bal koszorúverőér elülső leszálló ágában ")
          Case "cx"
            content.Add("koszoru_szuk", "A bal koszorúverőér körbefutó ágában ")
        End Select
        content.Item("koszoru_szuk") += data.Item("sziv_szuk_percent") + " %-os lumenszűkület figyelhető meg. "
      End If
      '########################################################################
      key = "sziv_stent"
      If data.ContainsKey(key) Then
        content.Add("stent", "A ")
        text = "Implantatum (stent) "

        If data.ContainsKey("sziv_stent_jobb") Then
          content.Item("stent") += "jobb koszorúverőérben"
          text += "arteriae coronariae dextri cordis"
        End If

        If data.ContainsKey("sziv_stent_lad") Then
          If Not content.Item("stent").EndsWith(" ") Then
            content.Item("stent") += ", "
            text += ", "
          End If
          content.Item("stent") += "bal koszorúverőér elülső leszálló ágában"
          text += "rami interventricularis anterioris arteriae coronariae sinistri cordis"
        End If

        If data.ContainsKey("sziv_stent_cx") Then
          If Not content.Item("stent").EndsWith(" ") Then
            content.Item("stent") += ", "
            text += ", "
          End If
          content.Item("stent") += "bal koszorúverőér körbefutó ágában"
          text += "rami circumflexi arteriae coronariae sinistri cordis"
        End If
        content.Item("stent") += " stent implantatum található. "
        AddToDiag(text + ".")
        text = ""
      End If
      '########################################################################
      key = "sziv_thrombus"
      If data.ContainsKey(key) Then
        If CheckRequired("sziv_thrombus_poz", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If
        Select Case data.Item("sziv_thrombus_poz")
          Case "jobb"
            content.Add("thrombus", "A jobb koszorúverőérben friss vérrögös elzáródás figyelhető meg. ")
            AddToDiag("Thrombus recens arteriae coronariae dextri cordis.")
          Case "lad"
            content.Add("thrombus", "A bal koszorúverőér elülső leszálló ágában friss vérrögös elzáródás figyelhető meg. ")
            AddToDiag("Thrombus recens rami interventricularis anterioris arteriae coronariae sinistri cordis.")
          Case "cx"
            content.Add("thrombus", "A bal koszorúverőér körbefutó ágában friss vérrögös elzáródás figyelhető meg. ")
            AddToDiag("Thrombus recens rami circumflexi arteriae coronariae sinistri cordis.")
        End Select
      End If
      '########################################################################
      key = "sziv_inf_regi"
      If data.ContainsKey(key) Then
        If CheckRequired("sziv_inf_regi_meret", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If
        If CheckRequired("sziv_inf_regi_poz", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If

        content.Add("inf_regi", "A bal kamra ")
        Select Case data.Item("sziv_inf_regi_poz")
          Case "elulso"
            content.Item("inf_regi") += "elülső"
            AddToDiag("Infarctus obsoletus parietis anterioris ventriculi sinsitri cordis.")
          Case "hatso"
            content.Item("inf_regi") += "hátulsó"
            AddToDiag("Infarctus obsoletus parietis posterioris ventriculi sinsitri cordis.")
          Case "septalis"
            content.Item("inf_regi") += "sövényi"
            AddToDiag("Infarctus obsoletus parietis septalis ventriculi sinsitri cordis.")
          Case "oldalso"
            content.Item("inf_regi") += "oldalsó"
            AddToDiag("Infarctus obsoletus parietis lateralis ventriculi sinsitri cordis.")
        End Select
        content.Item("inf_regi") += " falában " + data.Item("sziv_inf_regi_meret") + " mm nagyságú "
        content.Item("inf_regi") += "szürkésfehér színű régi szívizomelhalás figyelhető meg. "
      End If
      '########################################################################
      key = "sziv_inf_uj"
      If data.ContainsKey(key) Then
        If CheckRequired("sziv_inf_uj_meret", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If
        If CheckRequired("sziv_inf_uj_poz", data) AndAlso AbortOnMissing Then
          Exit Sub
        End If

        content.Add("inf_uj", "A bal kamra")
        Select Case data.Item("sziv_inf_uj_poz")
          Case "elulso"
            content.Item("inf_uj") += "elülső"
            AddToDiag("Infarctus recens parietis anterioris ventriculi sinsitri cordis.")
          Case "hatso"
            content.Item("inf_uj") += "hátulsó"
            AddToDiag("Infarctus recens parietis posterioris ventriculi sinsitri cordis.")
          Case "septalis"
            content.Item("inf_uj") += "sövényi"
            AddToDiag("Infarctus recens parietis septalis ventriculi sinsitri cordis.")
          Case "oldalso"
            content.Item("inf_uj") += "oldalsó"
            AddToDiag("Infarctus recens parietis lateralis ventriculi sinsitri cordis.")
        End Select
        content.Item("inf_uj") += " falában " + data.Item("sziv_inf_uj_meret") + " mm nagyságú "
        content.Item("inf_uj") += "agyagsárga színű, helyenként vörhenyes szegéllyel bíró, heveny szívizomelhalás figyelhető meg. "
      End If
      '########################################################################
      key = "sziv_stenosis"
      If data.ContainsKey(key) Then
        content.Add(key, "z aortabillentyű meszes szűkületet mutat, egyebekben a")
        AddToDiag("Stenosis calcificans ostii aortae.")
      End If
      '########################################################################
      key = "sziv_mitralis"
      If data.ContainsKey(key) Then
        content.Add(key, " mitralis billentyű anulusa kifejezetten meszes, egyebekben a")
        AddToDiag("Calcificatio ostii atrioventricularis sinistri cordis.")
      End If
      '########################################################################
      key = "sziv_haemo"
      If data.ContainsKey(key) Then
        If CheckRequired("sziv_haemo_g", data) Then
          content.Add(key, "A szívburokban " + data.Item("haemo_g") + " g részben alvadt vér található. ")
          AddToDiag("Haemopericardium.")
        ElseIf AbortOnMissing Then
          Exit Sub
        End If
      End If
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
End Class
