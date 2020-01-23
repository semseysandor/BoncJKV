﻿''' <summary>
''' Transform UI data to exportable format
''' Applies business rules to data
''' </summary>
Public Class WordTransformer
  ''' <summary>
  ''' Component Name
  ''' </summary>
  Public Const ComponentName = "Transformer"
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
      ErrorHandling.General(ex, ComponentName)
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
  Public Function ApplyRules(ByRef data As Dictionary(Of String, String)) As Boolean
    content = New Dictionary(Of String, String)

    If Not ApplyRulesGeneral(data) Then
      Return False
    End If
    If Not ApplyRulesBrain(data) Then
      Return False
    End If
    If Not ApplyRulesHeart(data) Then
      Return False
    End If
    If Not ApplyRulesLungs(data) Then
      Return False
    End If
    If Not ApplyRulesStomach(data) Then
      Return False
    End If
    If Not ApplyRulesKidney(data) Then
      Return False
    End If
    If Not ApplyRulesDeath(data) Then
      Return False
    End If

    Return True
  End Function
  ''' <summary>
  ''' Applies rules (general parts)
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Function ApplyRulesGeneral(ByRef data As Dictionary(Of String, String)) As Boolean
    Dim key As String
    Try
      '########################################################################
      For Each key In {"hossz", "haj", "kor", "agy", "sziv", "tudo", "maj", "lep", "vese", "fog", "zsir"}
        If CheckRequired(key, data) Then
          content.Add(key, data.Item(key))
        ElseIf AbortOnMissing Then
          Return False
        End If
      Next
      '########################################################################
      key = "nem"
      If CheckRequired(key, data) Then
        content.Add("nem_1", data.Item(key))
        content.Add("nem_2", data.Item(key))
      ElseIf AbortOnMissing Then
        Return False
      End If
      '########################################################################
      key = "test"
      If CheckRequired(key, data) Then
        content.Add(key, data.Item(key))
        If data.Item(key) = "cachexiás" Then
          AddToDiag("Cachexia.")
        End If
      ElseIf AbortOnMissing Then
        Return False
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
          Return False
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
          Return False
        End If
        content.Add("pacemaker_kul", "Bal oldalon infraclavicularisan pacemaker telep található. ")
        content.Add("pacemaker_nyaki", "A jobb szívfélben pacemaker elektróda azonosítható. ")
      End If
      Return True
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
    Return False
  End Function
  ''' <summary>
  ''' Applies rules regarding the brain
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Function ApplyRulesBrain(data As Dictionary(Of String, String)) As Boolean
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
        Return False
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
        If Not CheckRequired("agy_lagyulas_lebeny", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("agy_lagyulas_meret", data) AndAlso AbortOnMissing Then
          Return False
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
        If Not CheckRequired("agy_verzes_lebeny", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("agy_verzes_meret", data) AndAlso AbortOnMissing Then
          Return False
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
          Return False
        End If
        If CheckRequired("agy_attet_meret", data) Then
          meret = data.Item("agy_attet_meret")
        ElseIf AbortOnMissing Then
          Return False
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
      Return True
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
    Return False
  End Function
  ''' <summary>
  ''' Applies rules regarding the heart
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Function ApplyRulesHeart(data As Dictionary(Of String, String)) As Boolean
    Dim key As String
    Dim text As String
    Dim flag As Boolean
    Try
      '########################################################################
      key = "sziv_allapot"
      If Not CheckRequired("sziv_bal_kamra", data) AndAlso AbortOnMissing Then
        Return False
      End If

      If Not CheckRequired("sziv_jobb_kamra", data) AndAlso AbortOnMissing Then
        Return False
      End If

      text = "A "
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "konc"
            text += "körkörösen túltengett "
            AddToDiag("Hypertrophia concentrica ventriculi sinistri cordis.")
          Case "tagult"
            text += "tágult, túltengett "
            AddToDiag("Hypertrophia dilatativa ventriculi sinsitri cordis.")
        End Select
        content.Add("sziv_allapot_1", "A szív megnagyobbodott. ")
      End If
      text += "bal kamra fala " + data.Item("sziv_bal_kamra") + " mm, a "

      key = "sziv_cor_pulm"
      If data.ContainsKey(key) Then
        text += "tágult, túltengett "
        AddToDiag("Cor pulmonale chronicum.")
      End If
      text += "jobb kamra fala " + data.Item("sziv_jobb_kamra") + " mm vastag"

      key = "sziv_dcm"
      If data.ContainsKey(key) Then
        text += ", a kamrák fala elvékonyodott, lumenük extrém mértékben tágult"
        AddToDiag("Cardiomyopathia dilatativa.")
        If content.ContainsKey("sziv_allapot_1") Then
          content.Item("sziv_allapot_1") = "A szív kifejezetten megnagyobbodott. "
        Else
          content.Add("sziv_allapot_1", "A szív kifejezetten megnagyobbodott. ")
        End If
      End If
      text += ". "
      content.Add("sziv_allapot_2", text)
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
        If Not CheckRequired("sziv_szuk_percent", data) AndAlso AbortOnMissing Then
          Return False
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
        flag = False

        If data.ContainsKey("sziv_stent_jobb") Then
          content.Item("stent") += "jobb koszorúverőérben"
          text += "arteriae coronariae dextri cordis"
          flag = True
        End If

        If data.ContainsKey("sziv_stent_lad") Then
          If flag Then
            content.Item("stent") += ", "
            text += " et "
          End If
          content.Item("stent") += "bal koszorúverőér elülső leszálló ágában"
          text += "rami interventricularis anterioris arteriae coronariae sinistri cordis"
          flag = True
        End If

        If data.ContainsKey("sziv_stent_cx") Then
          If flag Then
            content.Item("stent") += ", "
            text += " et "
          End If
          content.Item("stent") += "bal koszorúverőér körbefutó ágában"
          text += "rami circumflexi arteriae coronariae sinistri cordis"
          flag = True
        End If
        content.Item("stent") += " stent implantatum található. "
        AddToDiag(text + ".")
      End If
      '########################################################################
      key = "sziv_thrombus"
      If data.ContainsKey(key) Then
        If Not CheckRequired("sziv_thrombus_poz", data) AndAlso AbortOnMissing Then
          Return False
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
        If Not CheckRequired("sziv_inf_regi_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("sziv_inf_regi_poz", data) AndAlso AbortOnMissing Then
          Return False
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
        If Not CheckRequired("sziv_inf_uj_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("sziv_inf_uj_poz", data) AndAlso AbortOnMissing Then
          Return False
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
          Return False
        End If
      End If
      '########################################################################
      key = "sziv_cabg"
      If data.ContainsKey(key) Then
        content.Add("sziv_cabg_kul", "a sternum felett régi hegvonal látható, a szegycsontban fémkapcsok figyelhetők meg, ")
        content.Add("sziv_cabg_nyaki_1", ", a szívburok lapszerint latapadva")
        content.Add("sziv_cabg_nyaki_2", "A koszorú-verőerekhez az aortából kiinduló bypass graftok csatlakoznak varratokkal, a graftok arterializálódtak, helyenkét szűkültek. ")
        AddToDiag("Status post CABG.")
      End If
      Return True
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
    Return False
  End Function
  ''' <summary>
  ''' Applies rules regarding the lungs
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Function ApplyRulesLungs(data As Dictionary(Of String, String)) As Boolean
    Dim key As String
    Dim text As String
    Dim flag As Boolean
    Try
      '########################################################################
      key = "tudo_anthra"
      If data.ContainsKey(key) Then
        content.Add("tudo_anthra", "A mellhártyákon szürkésfekete hálózatos rajzolat látható. ")
        AddToDiag("Anthracosis pulmonum.")
      End If
      '########################################################################
      key = "tudo_emphy"
      If data.ContainsKey(key) Then
        content.Add("tudo_emphy", "tágult ")
        AddToDiag("Emphysema pulmonum.")
      End If
      '########################################################################
      key = "tudo_oedema"
      If data.ContainsKey(key) Then
        content.Add("tudo_oedema", ", vizenyősek, főként az alsó lebenyek vérbővek, vörhenyesek, metszlapjukról nyomásra habos szilvalészerű folyadék ürül")
        AddToDiag("Oedema pulmonum.")
      End If
      '########################################################################
      key = "tudo_mindharom"
      If data.ContainsKey(key) Then
        If Not content.ContainsKey("tudo_anthra") Then
          content.Add("tudo_anthra", "A mellhártyákon szürkésfekete hálózatos rajzolat látható. ")
        End If
        If Not content.ContainsKey("tudo_emphy") Then
          content.Add("tudo_anthra", "tágult ")
        End If
        If Not content.ContainsKey("tudo_oedema") Then
          content.Add("tudo_anthra", ", vizenyősek, főként az alsó lebenyek vérbővek, vörhenyesek, metszlapjukról nyomásra habos szilvalészerű folyadék ürül")
        End If
        AddToDiag("Anthracosis, emphysema et oedema pulmonum.")
      End If
      '########################################################################
      key = "tudo_bronch"
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "chronic"
            content.Add("tudo_bronch_chron", "kifejezett")
            AddToDiag("Bronchitis chronica.")
          Case "acut"
            content.Add("tudo_bronch_acut", "purulens váladékot tartalmazó ")
            AddToDiag("Bronchitis chronica cum exacerbatinoe acuta.")
        End Select
      Else
        content.Add("tudo_bronch_chron", "enyhe")
      End If
      '########################################################################
      key = "tudo_pneu"
      If data.ContainsKey(key) Then

        If data.ContainsKey("tudo_pneu_mko") Then
          content.Add("tudo_pneu", "Mindkét tüdő alsó lebenye")
          text = "Bronchopenumonia loborum inferiorum pulmonum"
          flag = True
        Else
          content.Add("tudo_pneu", "A ")
          text = "Bronchopenumonia"
          flag = False
        End If

        If data.ContainsKey("tudo_pneu_j_a") Then
          If flag Then
            content.Item("tudo_pneu") += ", "
            text += " et "
          End If
          content.Item("tudo_pneu") += "jobb tüdő alsó lebenye"
          text += "lobi inferioris pulmonis dextri"
          flag = True
        End If

        If data.ContainsKey("tudo_pneu_j_k") Then
          If flag Then
            content.Item("tudo_pneu") += ", "
            text += " et "
          End If
          content.Item("tudo_pneu") += "jobb tüdő középső lebenye"
          text += "lobi medii pulmonis dextri"
          flag = True
        End If

        If data.ContainsKey("tudo_pneu_j_f") Then
          If flag Then
            content.Item("tudo_pneu") += ", "
            text += " et "
          End If
          content.Item("tudo_pneu") += "jobb tüdő felső lebenye"
          text += "lobi superioris pulmonis dextri"
          flag = True
        End If

        If data.ContainsKey("tudo_pneu_b_a") Then
          If flag Then
            content.Item("tudo_pneu") += ", "
            text += " et "
          End If
          content.Item("tudo_pneu") += "bal tüdő alsó lebenye"
          text += "lobi inferioris pulmonis sinistri"
          flag = True
        End If

        If data.ContainsKey("tudo_pneu_b_f") Then
          If flag Then
            content.Item("tudo_pneu") += ", "
            text += " et "
          End If
          content.Item("tudo_pneu") += "bal tüdő felső lebenye"
          text += "lobi suprioris pulmonis sinistri"
          flag = True
        End If
        content.Item("tudo_pneu") += " légtelen, tömött tapintatú, metszéslapján gennycsapok préselhetők. "
        AddToDiag(text + ".")
      End If
      '########################################################################
      key = "tudo_tumor"
      If data.ContainsKey(key) Then
        If data.ContainsKey("tudo_tumor_m") Then
          content.Add("tudo_tumor", "Az összes lebenyben")
          text = "Neoplasma malignum loborum omnium pulmonum"
          flag = True
        Else
          content.Add("tudo_tumor", "A ")
          text = "Neoplasma malignum"
          flag = False
        End If

        If data.ContainsKey("tudo_tumor_j_a") Then
          If flag Then
            content.Item("tudo_tumor") += ", "
            text += " et "
          End If
          content.Item("tudo_tumor") += "jobb alsó lebenyben"
          text += "lobi inferioris pulmonis dextri"
          flag = True
        End If

        If data.ContainsKey("tudo_tumor_j_k") Then
          If flag Then
            content.Item("tudo_tumor") += ", "
            text += " et "
          End If
          content.Item("tudo_tumor") += "jobb középső lebenyben"
          text += "lobi medii pulmonis dextri. "
          flag = True
        End If

        If data.ContainsKey("tudo_tumor_j_f") Then
          If flag Then
            content.Item("tudo_tumor") += ", "
            text += " et "
          End If
          content.Item("tudo_tumor") += "jobb felső lebenyben"
          text += "lobi superioris pulmonis dextri"
          flag = True
        End If

        If data.ContainsKey("tudo_tumor_b_a") Then
          If flag Then
            content.Item("tudo_tumor") += ", "
            text += " et "
          End If
          content.Item("tudo_tumor") += "bal alsó lebenyben"
          text += "lobi inferioris pulmonis sinistri"
          flag = True
        End If

        If data.ContainsKey("tudo_tumor_b_f") Then
          If flag Then
            content.Item("tudo_tumor") += ", "
            text += " et "
          End If
          content.Item("tudo_tumor") += "bal felső lebenyben"
          text += "lobi superioris pulmonis sinistri"
          flag = True
        End If

        If CheckRequired("tudo_tumor_meret", data) Then
          text += "szürkésfehér színű " + data.Item("tudo_tumor_meret") + " mm legnagyobb átmérőjű idegenszövet-szaporulat látható. "
        ElseIf AbortOnMissing Then
          Return False
        End If
        AddToDiag(text + ".")
      End If
      '########################################################################
      key = "tudo_attet"
      If data.ContainsKey(key) Then
        If data.ContainsKey("tudo_attet_m") Then
          content.Add("tudo_attet", "Az összes lebenyben")
          text = "Metastasis loborum omnium pulmonum"
          flag = True
        Else
          content.Add("tudo_attet", "A ")
          text = "Metastasis"
          flag = False
        End If

        If data.ContainsKey("tudo_attet_j_a") Then
          If flag Then
            content.Item("tudo_attet") += ", "
            text += " et "
          End If
          content.Item("tudo_attet") += "jobb alsó lebenyben"
          text += "lobi inferioris pulmonis dextri"
          flag = True
        End If

        If data.ContainsKey("tudo_attet_j_k") Then
          If flag Then
            content.Item("tudo_attet") += ", "
            text += " et "
          End If
          content.Item("tudo_attet") += "jobb középső lebenyben"
          text += "lobi medii pulmonis dextri. "
          flag = True
        End If

        If data.ContainsKey("tudo_attet_j_f") Then
          If flag Then
            content.Item("tudo_attet") += ", "
            text += " et "
          End If
          content.Item("tudo_attet") += "jobb felső lebenyben"
          text += "lobi superioris pulmonis dextri"
          flag = True
        End If

        If data.ContainsKey("tudo_attet_b_a") Then
          If flag Then
            content.Item("tudo_attet") += ", "
            text += " et "
          End If
          content.Item("tudo_attet") += "bal alsó lebenyben"
          text += "lobi inferioris pulmonis sinistri"
          flag = True
        End If

        If data.ContainsKey("tudo_attet_b_f") Then
          If flag Then
            content.Item("tudo_attet") += ", "
            text += " et "
          End If
          content.Item("tudo_attet") += "bal felső lebenyben"
          text += "lobi superioris pulmonis sinistri"
          flag = True
        End If

        If CheckRequired("tudo_attet_meret", data) Then
          text += "szürkésfehér színű " + data.Item("tudo_attet_meret") + " mm legnagyobb átmérőjű daganatáttét látható. "
        ElseIf AbortOnMissing Then
          Return False
        End If
        AddToDiag(text + ".")
      End If
      '########################################################################
      key = "tudo_embolia"
      If data.ContainsKey(key) Then

        If data.ContainsKey("tudo_embolia_lovag") Then
          content.Add("tudo_emb_lovag_1", ", oszlásában vérrögös elzáródás láható")
          content.Add("tudo_emb_lovag_2", ", egyebekben")
          AddToDiag("Thromboembolus bifurcationis trunci pulmonalis.")
        End If

        If data.ContainsKey("tudo_embolia_ket") Then
          content.Add("tudo_emb_ket", ", oszlása után a tüdőverőerek mindkét főágában masszív vérrögös elzáródás látható")
          If Not content.ContainsKey("tudo_emb_lovag_2") Then
            content.Add("tudo_emb_lovag_2", ", egyebekben")
          End If
          AddToDiag("Thromboembolus ramorum principalum arteriarum pulmonalum.")
        End If

        If data.ContainsKey("tudo_embolia_elso") Then
          If data.ContainsKey("tudo_embolia_b") Then
            content.Add("tudo_emb_elso_b", "bal arteria pulmonalis elsőrendű ágában vérrögös elzáródás látható, egyebekben ")
            AddToDiag("Thromboembolus rami principalis arteriae pulmonalis sinistri.")
          End If
          If data.ContainsKey("tudo_embolia_j") Then
            content.Add("tudo_emb_elso_j", "jobb arteria pulmonalis elsőrendű ágában vérrögös elzáródás látható, egyebekben ")
            AddToDiag("Thromboembolus rami principalis arteriae pulmonalis dextri.")
          End If
        End If

        If data.ContainsKey("tudo_embolia_tobb") Then
          If data.ContainsKey("tudo_embolia_b") Then
            AddToDiag("Thromboembolus rami ordinis II-III. arteriae pulmonalis sinistri.")
          End If
          If data.ContainsKey("tudo_embolia_j") Then
            AddToDiag("Thromboembolus rami ordinis II-III. arteriae pulmonalis dextri.")
          End If
          content.Add("tudo_emb_tobb", "a másod-, és harmadrendű ágaiban vérrögös elzáródás látható, egyebekben ")
        End If
      End If
      '########################################################################
      key = "tudo_hydro"
      If data.ContainsKey(key) Then
        If Not CheckRequired("tudo_hydro_liter", data) And AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("tudo_hydro_poz", data) And AbortOnMissing Then
          Return False
        End If

        content.Add("tudo_hydro", "A mellüregben ")

        Select Case data.Item("tudo_hydro_poz")
          Case "bal"
            content.Item("tudo_hydro") += "bal oldalt "
            AddToDiag("Hydrothorax l. s.")
          Case "jobb"
            content.Item("tudo_hydro") += "jobb oldalt "
            AddToDiag("Hydrothorax l. d.")
          Case "mko"
            content.Item("tudo_hydro") += "mindkét oldalt "
            AddToDiag("Hydrothorax l. u.")
        End Select
        content.Item("tudo_hydro") += data.Item("tudo_hydro_liter") + " liter szalmasárga folyadék látható. "
      End If
      Return True
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
    Return False
  End Function
  ''' <summary>
  ''' Applies rules regarding the stomach
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Function ApplyRulesStomach(data As Dictionary(Of String, String)) As Boolean
    Dim key As String
    Dim text As String
    Dim flag As Boolean
    Try
      '########################################################################
      key = "has_lep"
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "verbo"
            content.Add("lep", "A vérbő, vörhenyes lép megtartott szerkezetű. ")
            AddToDiag("Hyperaemia passiva lienis.")
          Case "puhult"
            content.Add("lep", "A vérbő, vörhenyes lép állománya ellágyult, metszlapjáról nagy mennyiségű kaparék nyerhető. ")
            AddToDiag("Splenitis septica acuta.")
          Case "nagy"
            content.Add("lep", "A vérbő, vörhenyes lép megnagyobbodott, állománya megtartott szerkezetű. ")
            AddToDiag("Splenomegalia.")
        End Select
      End If
      '########################################################################
      key = "has_maj"
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "verbo"
            content.Add("maj", "A máj vörhenyesbarna színű, állománya eltérés nélkül. ")
          Case "enyhe"
            content.Add("maj", "A máj vörhenyesbarna színű, állománya metszéslapon sárgásan zsírfényű. ")
            AddToDiag("Steatosis minoris gradus hepatis.")
          Case "zsir"
            content.Add("maj", "A máj megnagyobbodott, szélei lekerekítettek, állománya zsírosan átalakult. ")
            AddToDiag("Steatosis hepatis.")
          Case "szerecsen"
            content.Add("maj", "A vörhenyesbarna, vérbő máj metszlapon szerecsendió-rajzolatot mutat. ")
            AddToDiag("Hepar moschatum.")
          Case "cirr"
            content.Add("maj", "A máj zsugorodott, állománya apró göbös kötőszövetes átalakulást mutat. ")
            AddToDiag("Cirrhosis hepatis.")
        End Select
      End If
      '########################################################################
      key = "has_maj_attet"
      If data.ContainsKey(key) Then
        If Not CheckRequired("has_maj_attet_meret", data) And AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("has_maj_attet_db", data) And AbortOnMissing Then
          Return False
        End If
        content.Add("maj_attet", "A máj állományában ")
        Select Case data.Item("has_maj_attet_db")
          Case "egy"
            content.Item("maj_attet") += "egy"
            AddToDiag("Metastasis hepatis.")
          Case "tobb"
            content.Item("maj_attet") += "több"
            AddToDiag("Metastases multiplex hepatis.")
        End Select
        content.Item("maj_attet") += " db " + data.Item("has_maj_attet_meret")
        content.Item("maj_attet") += " mm legnagyobb kiterjedésű, szürkésfehér színű, környezetétől élesen elhatárolódó daganatáttét azonosítható. "
        AddToDiag("Cirrhosis hepatis.")
      End If
      '########################################################################
      key = "has_hasnyal"
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "ep"
            content.Add("hasnyal", "A hasnyálmirigy mirigyes, megtartott szerkezetű. ")
          Case "chronic"
            content.Add("hasnyal", "A hasnyálmirigy kiszélesedett, mirigyes állománya kifejezett. ")
            AddToDiag("Pancreatitis chronica.")
          Case "acut"
            content.Add("hasnyal", "A hasnyálmirigy állománya kiszélesedett, kiterjedten barnás-vörhenyes elszíneződést mutat," +
                          "nekrotikus, környezete vizenyős, a környező zsírszövetben sárgásfehér, ún. szappanképződés figyelhető meg. ")
            AddToDiag("Pancreatitis acuta.")
        End Select
      End If
      '########################################################################
      key = "has_epe"
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "megtartott"
            content.Add("epe", "Az epehólyag fala megtartott szerkezetű")
            If data.ContainsKey("has_epeko") Then
              If Not CheckRequired("has_epeko_meret", data) AndAlso AbortOnMissing Then
                Return False
              End If
              If Not CheckRequired("has_epeko_db", data) AndAlso AbortOnMissing Then
                Return False
              End If
              content.Item("epe") += ", lumenében " + data.Item("has_epeko_db") + " db, "
              content.Item("epe") += data.Item("has_epeko_meret") + " mm legnagyobb átmérőjű epekő azonosítható"
              AddToDiag("Cholecystolithiasis.")
            End If
            content.Item("epe") += ". "
          Case "eltavol"
            content.Add("epe", "Az epehólyagot korábban eltávolították. ")
            AddToDiag("Status post cholecystectomiam.")
        End Select
      End If
      '########################################################################
      key = "has_gyomor"
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "ep"
            content.Add("gyomor", "A gyomor fala, nyálkahártyája eltérés nélkül, redőzete megtartott. ")
          Case "erosio"
            content.Add("gyomor", "A gyomor nyálkahártyáján erosiók láthatók. ")
            AddToDiag("Erosiones ventriculi.")
          Case "fekely"
            content.Add("gyomor", "A gyomorban a ")
            If Not CheckRequired("has_gyomor_fekely_meret", data) AndAlso AbortOnMissing Then
              Return False
            End If
            If Not CheckRequired("has_gyomor_fekely_gorb", data) AndAlso AbortOnMissing Then
              Return False
            End If
            Select Case data.Item("has_gyomor_fekely_gorb")
              Case "kis"
                content.Item("gyomor") += "kisgörbület"
              Case "nagy"
                content.Item("gyomor") += "nagygörbület"
            End Select
            content.Item("gyomor") += " területén " + data.Item("has_gyomor_fekely_meret") + " mm legnagyobb átmérőjű fekély figyelhető meg. "
            AddToDiag("Ulcus ventriculi.")
        End Select
      End If
      '########################################################################
      key = "has_gyomor_tumor"
      If data.ContainsKey(key) Then
        content.Add("gyomor_tumor", "A gyomorban a ")
        If Not CheckRequired("has_gyomor_tumor_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("has_gyomor_tumor_gorb", data) AndAlso AbortOnMissing Then
          Return False
        End If
        Select Case data.Item("has_gyomor_tumor_gorb")
          Case "kis"
            content.Item("gyomor_tumor") += "kisgörbület"
          Case "nagy"
            content.Item("gyomor_tumor") += "nagygörbület"
        End Select
        content.Item("gyomor_tumor") += " területén " + data.Item("has_gyomor_tumor_meret") +
          " mm nagyságú szürkésfehér idegenszövet-szaporulat figyelhető meg.   "
        AddToDiag("Neoplasma malignum ventriculi.")
      End If
      '########################################################################
      key = "has_nyombel"
      If data.ContainsKey(key) Then
        Select Case data.Item(key)
          Case "ep"
            content.Add("nyombel", "A nyombél eltérés nélkül. ")
          Case "fekely"
            content.Add("nyombel", "A nyombél nyálkahártyáján ")
            If Not CheckRequired("has_nyombel_meret", data) AndAlso AbortOnMissing Then
              Return False
            End If
            content.Item("nyombel") += data.Item("has_nyombel_meret") + " mm legnagyobb átmérőjű fekély figyelhető meg. "
            AddToDiag("Ulcus duodeni.")
        End Select
      End If
      '########################################################################
      key = "has_ileum"
      If data.ContainsKey(key) Then
        If Not CheckRequired("has_ileum_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If
        content.Add("ileum", "Az ileum nyálkahártyája ")
        content.Item("ileum") += data.Item("has_ileum_meret") + " cm-es szakaszon vizenyős, felszínén sárgásfehér felrakódás mutatkozik. "
        AddToDiag("Ileitis pseudomembranacea.")
      End If
      '########################################################################
      key = "has_bel"
      If data.ContainsKey(key) Then
        content.Add("bel", "A vékonybelek között több területen heges kitapadások azonosíthatóak. ")
        AddToDiag("Adhaesinones intestini tenuis.")
      End If
      '########################################################################
      key = "has_vastagbel_divert"
      If data.ContainsKey(key) Then
        content.Add("vastagbel_divert", "A szigmabélben több területen a nyálkahártya zsákszerű kitüremkedése látható. ")
        AddToDiag("Divetriculosis sigmatos.")
      End If
      '########################################################################
      key = "has_vastagbel_col_is"
      If data.ContainsKey(key) Then
        content.Add("vastagbel_ischaem", "A vastagbél nyálkahártyája diffúzan vörhenyesbarna elszíneződést mutat. ")
        AddToDiag("Colitis ischaemica.")
      End If
      '########################################################################
      key = "has_vastagbel_col_alh"
      If data.ContainsKey(key) Then
        content.Add("vastagbel_alhartya", "A vastagbél nyálkahártyája diffúzan vizenyős, felszínén sárgásfehér felrakódás mutatkozik. ")
        AddToDiag("Colitis pseudomembranacea.")
      End If
      '########################################################################
      key = "has_vastagbel_tumor"
      If data.ContainsKey(key) Then
        content.Add("vastagbel_tumor", "A ")
        text = "Neoplasma malignum "
        flag = False

        If data.ContainsKey("has_vastagbel_tumor_le") Then
          content.Item("vastagbel_tumor") += "leszálló vastagbél"
          text += "colontos descendentis"
          flag = True
        End If

        If data.ContainsKey("has_vastagbel_tumor_fel") Then
          If flag Then
            content.Item("vastagbel_tumor") += ", "
            text += " et "
          End If
          content.Item("vastagbel_tumor") += "felszálló vastagbél"
          text += "colontos ascendentis"
          flag = True
        End If

        If data.ContainsKey("has_vastagbel_tumor_sigma") Then
          If flag Then
            content.Item("vastagbel_tumor") += ", "
            text += " et "
          End If
          content.Item("vastagbel_tumor") += "szigmabél"
          text += "sigmatos"
          flag = True
        End If

        If data.ContainsKey("has_vastagbel_tumor_harant") Then
          If flag Then
            content.Item("vastagbel_tumor") += ", "
            text += " et "
          End If
          content.Item("vastagbel_tumor") += "haránt vastagbél"
          text += "colontos transversi"
          flag = True
        End If

        If data.ContainsKey("has_vastagbel_tumor_coec") Then
          If flag Then
            content.Item("vastagbel_tumor") += ", "
            text += " et "
          End If
          content.Item("vastagbel_tumor") += "vakbél"
          text += "coeci"
          flag = True
        End If

        If data.ContainsKey("has_vastagbel_tumor_vegbel") Then
          If flag Then
            content.Item("vastagbel_tumor") += ", "
            text += " et "
          End If
          content.Item("vastagbel_tumor") += "végbél"
          text += "recti"
          flag = True
        End If

        If CheckRequired("has_vastagbel_tumor_meret", data) Then
          content.Item("vastagbel_tumor") += " területén " + data.Item("has_vastagbel_tumor_meret") + " cm-es szakaszon a nyálkahártyából kiinduló, "
        ElseIf AbortOnMissing Then
          Return False
        End If

        If data.ContainsKey("has_vastagbel_tumor_szukito") Then
          content.Item("vastagbel_tumor") += "a lumen jelentős szűkületét okozó, "
        End If

        content.Item("vastagbel_tumor") += "szürkésfehér színű, idegenszövet-szaporulat azonosítható. "
        AddToDiag(text + ".")
      End If
      Return True
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
    Return False
  End Function
  ''' <summary>
  ''' Applies rules regarding the kidney
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Function ApplyRulesKidney(data As Dictionary(Of String, String)) As Boolean
    Dim key As String
    Try
      '########################################################################
      key = "vese"
      If CheckRequired(key, data) Then
        Select Case data.Item(key)
          Case "sima"
            content.Add("vese", ", felszínük sima")
          Case "szemcses"
            content.Add("vese", ", felszínükön finom szemcsézettség ")
            If data.ContainsKey("vese_behuz") Then
              content.Item("vese") += "és számos behúzódás "
              AddToDiag("Nephritis interstitialis chronica l. u.")
            End If
            content.Item("vese") += "látható"
            AddToDiag("Nephrosclerosis arteriolosclerotica renum.")
        End Select
      ElseIf AbortOnMissing Then
        Return False
      End If
      '########################################################################
      key = "vese_tumor"
      If data.ContainsKey(key) Then
        If Not CheckRequired("vese_tumor_poz", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("vese_tumor_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If

        content.Add(key, "A ")
        Select Case data.Item("vese_tumor_poz")
          Case "bal"
            content.Item(key) += "bal"
            AddToDiag("Neoplasma malignum renis sinistri.")
          Case "jobb"
            content.Item(key) += "jobb"
            AddToDiag("Neoplasma malignum renis dextri.")
        End Select
        content.Item(key) += " vese állományában " + data.Item("vese_tumor_meret") + " mm nagyságú, "
        content.Item(key) += "kénsárga, helyenként vörhenyes idegenszövet-szaporulat azonosítható. "
      End If
      '########################################################################
      key = "veseko"
      If data.ContainsKey(key) Then
        If Not CheckRequired("veseko_poz", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("veseko_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If

        content.Add(key, "A ")
        Select Case data.Item("veseko_poz")
          Case "bal"
            content.Item(key) += "bal"
            AddToDiag("Nephrolithiasis sinistri.")
          Case "jobb"
            content.Item(key) += "jobb"
            AddToDiag("Nephrolithiasis dextri.")
        End Select
        content.Item(key) += " vesemedence területén " + data.Item("veseko_meret")
        content.Item(key) += " mm legnagyobb kiterjedésű vesekő azonosítható. "
      End If
      '########################################################################
      key = "vese_pyelo"
      If data.ContainsKey(key) Then
        If Not CheckRequired("vese_pyelo_poz", data) AndAlso AbortOnMissing Then
          Return False
        End If

        content.Add(key, "")
        Select Case data.Item("vese_pyelo_poz")
          Case "bal"
            content.Item(key) += "A bal"
            AddToDiag("Pyelonephritis acuta purulenta sinistri.")
          Case "jobb"
            content.Item(key) += "A jobb"
            AddToDiag("Pyelonephritis acuta purulenta dextri.")
          Case "mko"
            content.Item(key) += "Mindkét"
            AddToDiag("Pyelonephritis acuta purulenta l.u.")
        End Select
        content.Item(key) += " vesemedencében purulens váladék azonosítható, a vesék felszínén kicsiny abscessusok láthatók. "
      End If
      '########################################################################
      key = "holyag_kateter"
      If data.ContainsKey(key) Then
        content.Add("kateter", "A húgyhólyagban katéter található. ")
      End If
      '########################################################################
      key = "holyag_gyull"
      If data.ContainsKey(key) Then
        content.Add(key, "A húgyhólyag nyálkahártyája diffúzan vörhenyes, lumenében opálos vizelet azonosítható. ")
        AddToDiag("Urocytitis acuta.")
      End If
      '########################################################################
      key = "holyag_tumor"
      If data.ContainsKey(key) Then
        If Not CheckRequired("holyag_tumor_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If
        content.Add(key, "A húgyhólyag lumenében ")
        content.Item(key) += data.Item("holyag_tumor_meret") + " mm legnagyobb kiterjedésű, szürkésfehér-vörhenyes idegenszövet-szaporulat azonosítható. "
        AddToDiag("Neoplasma malignum vesicae urinariae.")
      End If
      '########################################################################
      If Not data.ContainsKey("holyag_gyull") AndAlso Not data.ContainsKey("holyag_tumor") Then
        content.Add("holyag", "és a húgyhólyag ")
      End If
      '########################################################################
      key = "meh_iud"
      If data.ContainsKey(key) Then
        content.Add("iud", "A méh üregében fogamzásgátló eszköz azonosítható. ")
      End If
      '########################################################################
      key = "meh_myoma"
      If data.ContainsKey(key) Then
        If Not CheckRequired("meh_myoma_darab", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("meh_myoma_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If

        content.Add(key, "A méh izmos falában ")
        content.Item(key) += data.Item("meh_myoma_darab") + " darab, " + data.Item("meh_myoma_meret")
        content.Item(key) += " mm nagyságú, szürkésfehér színű, örvényes szerkezetű, myomagöbnek imponáló képlet mutatkozik. "
        AddToDiag("Myomata uteri.")
      End If
      '########################################################################
      key = "meh_em"
      If data.ContainsKey(key) Then
        If Not CheckRequired("meh_em_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If

        content.Add(key, "A méh üregében ")
        content.Item(key) += data.Item("meh_em_meret") + " mm nagyságú polypoid képlet azonosítható. "
        AddToDiag("Polypus endometrialis uteri.")
      End If
      '########################################################################
      key = "meh_tumor"
      If data.ContainsKey(key) Then
        If Not CheckRequired("meh_tumor_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If

        content.Add(key, "A méh üregében a myometriumot is infiltráló szürkésfehér, helyenként vörhenyesbarna idegenszövet-szaporulat azonosítható. ")
        AddToDiag("Neoplasma malignum uteri.")
      End If
      '########################################################################
      key = "meh_cysta"
      If data.ContainsKey(key) Then
        If Not CheckRequired("meh_cysta_poz", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("meh_cysta_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If

        content.Add(key, "A ")
        Select Case data.Item("meh_cysta_poz")
          Case "bal"
            content.Item(key) += "bal"
            AddToDiag("Cysta ovarii sinistri.")
          Case "jobb"
            content.Item(key) += "jobb"
            AddToDiag("Cysta ovarii dextri.")
        End Select
        content.Item(key) += " petefészek állományában " + data.Item("meh_cysta_meret")
        content.Item(key) += " mm nagyságú, hártyás falú, víztiszta bennékű ciszta mutatkozik. "
      End If
      '########################################################################
      key = "prostata"
      If data.ContainsKey(key) Then
        content.Add(key, "A húgyhólyag lumene tágult, izomzata vaskos, a prostata megnagyobbodott, állománya göbös, körülírt kóros nem azonosítható. ")
        AddToDiag("Hyperplasia nodosa prostatae.")
      End If
      '########################################################################
      key = "scrotum"
      If data.ContainsKey(key) Then
        content.Add(key, "A scrotum megvastagodott, vizenyős. ")
        AddToDiag("Hydrokele testis.")
      End If
      '########################################################################
      key = "here_tumor"
      If data.ContainsKey(key) Then
        If Not CheckRequired("here_tumor_poz", data) AndAlso AbortOnMissing Then
          Return False
        End If
        If Not CheckRequired("here_tumor_meret", data) AndAlso AbortOnMissing Then
          Return False
        End If

        content.Add(key, "A ")
        Select Case data.Item("here_tumor_poz")
          Case "bal"
            content.Item(key) += "bal"
            AddToDiag("Neoplasma malignum testis sinistri.")
          Case "jobb"
            content.Item(key) += "jobb"
            AddToDiag("Neoplasma malignum testis dextri.")
        End Select
        content.Item(key) += " here állományában jól körülírt, " + data.Item("here_tumor_meret")
        content.Item(key) += " mm nagyságú, szürkésfehér színű, helyenként barnás-vörhenyes idegenszövet-szaporulat azonosítható. "
      End If
      Return True
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
    Return False
  End Function
  ''' <summary>
  ''' Applies rules regarding the death
  ''' </summary>
  ''' <param name="data">Data form UI</param>
  Private Function ApplyRulesDeath(data As Dictionary(Of String, String)) As Boolean
    Dim key As String
    Dim text = ""
    Dim flag = False
    Try
      '########################################################################
      key = "halal"
      If CheckRequired(key, data) Then
        content.Add(key, "")
        Select Case data.Item(key)
          Case "asu_iszb_sze"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
            content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
            content.Item(key) += "A halál okaként az arteria coronariák súlyos szűkülete és a szívizom idült ischaemiás "
            content.Item(key) += "elfajulása következtében kialakult szívelégtelenséget jelöltük meg"
          Case "asu_regi_sze"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
            content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
            content.Item(key) += "A bal kamrában régi szívizomelhalást figyelhettünk meg. A halál okaként az arteria "
            content.Item(key) += "coronariák súlyos szűkülete és a szívizom idült ischaemiás elfajulása következtében "
            content.Item(key) += "kialakult szívelégtelenséget jelöltük meg"
          Case "asu_heveny"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
            content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
            content.Item(key) += "A nagyfokú arteria coronaria sclerosis és szűkület heveny első fali/hátsó fali "
            content.Item(key) += "szívizomelhalást eredményezett, melyet a halál okaként jelöltünk meg"
          Case "asu_heveny_tamp"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
            content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
            content.Item(key) += "A nagyfokú arteria coronaria sclerosis és szűkület heveny első fali/hátsó fali "
            content.Item(key) += "szívizomelhalást eredményezett, itt a szabad rupturált, következményes heveny "
            content.Item(key) += "szívburki vérgyülemet okozva. A halál okaként a szívtamponádot jelöltük meg"
          Case "copd"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként idült obstruktív tüdőbetegséget "
            content.Item(key) += "állapítottunk meg, illetve következményes idült tüdőeredetű szívbetegséget figyelhettünk "
            content.Item(key) += "meg, melyet a tágult túltengett jobb szívfél és a belszervi pangás morfológiailag alátámasztott. "
            content.Item(key) += "A halál okaként a szívelégtelenséget jelöltük meg"
          Case "emphy"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként súlyos tüdőtágulatot állapítottunk meg, "
            content.Item(key) += "illetve következményes idült tüdőeredetű szívbetegséget figyelhettünk meg, melyet a "
            content.Item(key) += "tágult túltengett jobb szívfél és a belszervi pangás morfológiailag alátámasztott. "
            content.Item(key) += "A halál okaként a szívelégtelenséget jelöltük meg"
          Case "ht_hszb_sze"
            content.Item(key) += "anamnézisében magas vérnyomás betegség szerepel. A patológiai vizsgálata során magas "
            content.Item(key) += "vérnyomásos szívtúltengést állapítottunk meg, melyet a körkörösen/tágult súlyosan "
            content.Item(key) += "túltengett bal kamra morfológiailag alátámasztott. A halál okaként a szívelégtelenséget jelöltük meg"
          Case "htszb_sze"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként magas vérnyomásos szívtúltengést "
            content.Item(key) += "állapítottunk meg, melyet a körkörösen/tágult, súlyosan túltengett bal kamra "
            content.Item(key) += "morfológiailag alátámasztott. A halál okaként a szívelégtelenséget jelöltük meg."
          Case "asu_tudo"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
            content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
            content.Item(key) += "A halál közvetlen oka a pulmonalis főtörzs oszlásának lumenét teljesen elzáró, "
            content.Item(key) += "ún. lovagló thromboembolia / a tüdőverőerek elsőrendű ágainak masszív vérrögös elzáródása volt. "
            content.Item(key) += "Az embolia feltételezhetően bal/jobb alsóvégtagi mélyvénás rögösödésből származott. "
          Case "htszb_tudo"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként hypertensiv szívbetegséget állapítottunk meg. "
            content.Item(key) += "A halál közvetlen oka a pulmonalis főtörzs oszlásának lumenét teljesen elzáró, "
            content.Item(key) += "ún. lovagló thromboembolia / a tüdőverőerek elsőrendű ágainak masszív vérrögös elzáródása volt. "
            content.Item(key) += "Az embolia feltételezhetően bal/jobb alsóvégtagi mélyvénás rögösödésből származott."
          Case "ht_apo"
            content.Item(key) += "patológiai vizsgálata során alapbetegségként a bal/jobb agyféltekét/a kisagyat érintő "
            content.Item(key) += "roncsoló agyvérzést állapítottunk meg, szövődményként agyi nyomásfokozódás és a nyúltvelői "
            content.Item(key) += "légzési-keringési rendszer nyomás alá kerülése miatt következményes cardiorespiratoricus "
            content.Item(key) += "elégtelenség kialakulásával, melyek a halál közvetlen okaként megjelölt szívmegálláshoz vezettek."
          Case "etil"
            content.Item(key) += ""
        End Select
      ElseIf AbortOnMissing Then
        Return False
      End If
      '########################################################################
      key = "kis_asu"
      If data.ContainsKey(key) Then
        If flag Then
          text += ", "
        End If
        text += "az általános érelmeszesedés"
        flag = True
      End If
      '########################################################################
      key = "kis_ht"
      If data.ContainsKey(key) Then
        If flag Then
          text += ", "
        End If
        text += "a klinikailag jelzett magas vérnyomás betegség"
        flag = True
      End If
      '########################################################################
      key = "kis_htszb"
      If data.ContainsKey(key) Then
        If flag Then
          text += ", "
        End If
        text += "a magas vérnyomásos szívtúltengés"
        flag = True
      End If
      '########################################################################
      key = "kis_vese"
      If data.ContainsKey(key) Then
        If flag Then
          text += ", "
        End If
        text += ""
        flag = True
      End If
      '########################################################################
      key = "kis_copd"
      If data.ContainsKey(key) Then
        If flag Then
          text += ", "
        End If
        text += "az idült obstruktív tüdőbetegség"
        flag = True
      End If
      '########################################################################
      key = "kis_emphy"
      If data.ContainsKey(key) Then
        If flag Then
          text += ", "
        End If
        text += "a tüdőtágulat"
        flag = True
      End If
      '########################################################################
      key = "kis_diab"
      If data.ContainsKey(key) Then
        If flag Then
          text += ", "
        End If
        text += "az anamnézisben szereplő cukorbetegség"
        flag = True
      End If
      '########################################################################
      key = "kis_etil"
      If data.ContainsKey(key) Then
        If flag Then
          text += ", "
        End If
        text += ""
        flag = True
      End If

      If flag Then
        content.Add("kisero", "Kísérő betegségként ")
        content.Item("kisero") += text + " emelendő ki."
      End If
      Return True
    Catch ex As Exception
      ErrorHandling.General(ex, ComponentName)
    End Try
    Return False
  End Function
End Class
