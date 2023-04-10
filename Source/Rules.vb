''' <summary>
''' Transform UI data to exportable format
''' Applies business rules to data
''' </summary>
Public Class Rules

    ''' <summary>
    ''' Content ready to export
    ''' </summary>
    Private Content As Dictionary(Of String, String)

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
        Return Content
    End Function

    ''' <summary>
    ''' Prints content to the console
    ''' </summary>
    Public Sub PrintContent()
        Console.WriteLine("Content DATA *******************************")

        For Each row As KeyValuePair(Of String, String) In Content
            Console.WriteLine(row.Key.ToString + " " + row.Value.ToString)
        Next

    End Sub

    ''' <summary>
    ''' Adds a new diagnose to the diagnoses
    ''' </summary>
    ''' <param name="diag">Diagnose to add</param>
    Private Sub AddToDiag(ByVal diag As String)

        If Content.ContainsKey("diag") Then
            Content.Item("diag") += " " + diag
        Else
            Content.Add("diag", diag)
        End If

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
        End If

        RaiseEvent FieldMissing(key)

        Return False

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
        Content = New Dictionary(Of String, String)

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
        '########################################################################
        key = "benti"
        If data.ContainsKey(key) Then
            Content.Add(key, "Az anamnézist illetően a zárójelentésben foglaltakra utalunk.")
        End If
        '########################################################################
        key = "foto"
        If data.ContainsKey(key) Then
            Content.Add(key, " Fotódokumentáció készült.")
        End If
        '########################################################################
        For Each key In {"hossz", "haj", "kor", "agy", "sziv", "tudo", "maj", "lep", "vese", "fog", "zsir"}
            If CheckRequired(key, data) Then
                Content.Add(key, data.Item(key))
            ElseIf AbortOnMissing Then
                Return False
            End If
        Next
        '########################################################################
        key = "nem"
        If CheckRequired(key, data) Then
            Content.Add("nem_1", data.Item(key))
            Content.Add("nem_2", data.Item(key))
        ElseIf AbortOnMissing Then
            Return False
        End If
        '########################################################################
        key = "test"
        If CheckRequired(key, data) Then
            Content.Add(key, data.Item(key))
            If data.Item(key) = "cachexiás" Then
                AddToDiag("Cachexia.")
            End If
        ElseIf AbortOnMissing Then
            Return False
        End If
        '########################################################################
        key = "decub"
        If data.ContainsKey(key) Then
            Content.Add(key, "A ")

            If data.ContainsKey("decub_sacralis") Then
                Content.Item(key) += "keresztcsont területében, "
                AddToDiag("Decubitus sacralis.")
            End If
            If data.ContainsKey("decub_sarkak") Then
                Content.Item(key) += "sarkakon, "
                AddToDiag("Decubitus calcanei l. u.")
            End If
            If data.ContainsKey("decub_jobb_sarok") Then
                Content.Item(key) += "jobb sarkon, "
                AddToDiag("Decubitus calcanei dextri.")
            End If
            If data.ContainsKey("decub_bal_sarok") Then
                Content.Item(key) += "bal sarkon, "
                AddToDiag("Decubitus calcanei sinistri.")
            End If

            Dim length = Content.Item(key).Length
            If length > 2 Then
                Content.Item(key) = Content.Item(key).Remove(length - 2, 1)
            End If

            Content.Item(key) += data.Item(key) + " cm nagyságú felfekvéses fekély látható. "
        End If
        '########################################################################
        key = "amputacio"
        If data.ContainsKey(key) Then
            Content.Add(key, "")
            Select Case data.Item(key)
                Case "jobb_comb"
                    Content.Item(key) += "jobb alsó végtag combszintben"
                    AddToDiag("Status post amputationem femoris dextri.")
                Case "bal_comb"
                    Content.Item(key) += "bal alsó végtag combszintben"
                    AddToDiag("Status post amputationem femoris sinistri.")
                Case "combok"
                    Content.Item(key) += "alsó végtagok combszintben"
                    AddToDiag("Status post amputationem femoris l. u.")
                Case "jobb_labszar"
                    Content.Item(key) += "jobb alsó végtag lábszárszintben"
                    AddToDiag("Status post amputationem cruris dextri.")
                Case "bal_labszar"
                    Content.Item(key) += "bal alsó végtag lábszárszintben"
                    AddToDiag("Status post amputationem crusis sinistri.")
                Case "labszarak"
                    Content.Item(key) += "alsó végtagok lábszárszintben"
                    AddToDiag("Status post amputationem cruris l. u.")
            End Select
            Content.Item(key) += " amputálva, egyebekben a "
        End If
        '########################################################################
        key = "asu_kp"
        If data.ContainsKey(key) Then
            Content.Add("asu_kp_nyaki", "Az aorta mellkasi szakasza kp. tág, belfelszínén több lencsényi sárgásfehér plaque látható. ")
            Content.Add("asu_kp_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér plaque látható. ")
            AddToDiag("Arteriosclerosis universalis mediocris gradus.")
        End If
        '########################################################################
        key = "asu_sulyos"
        If data.ContainsKey(key) Then
            Content.Add("asu_sulyos_nyaki_1", "carotis-villák scleroticusak, egyebekben a ")
            Content.Add("asu_sulyos_nyaki_2", "Az aorta mellkasi szakasza kp. tág, belfelszínén több lencsényi sárgásfehér meszes tapintatú plaque látható. ")
            Content.Add("asu_sulyos_has", "Az aorta hasi szakasza kp. tág, belfelszínén több, forintosnyi sárgásfehér meszes tapintatú plaque látható. ")
            AddToDiag("Arteriosclerosis universalis maioris gradus praecipue aortae et arteriarum coronariarum cordis.")
        End If
        '########################################################################
        key = "ascites"
        If data.ContainsKey(key) Then
            Content.Add("ascites", "A hasüregben ")
            If CheckRequired("asc_liter", data) Then
                Content.Item("ascites") += data.Item("asc_liter") + " liter szalmasárga folyadék található. "
                AddToDiag("Ascites.")
            ElseIf AbortOnMissing Then
                Return False
            End If
        End If
        '########################################################################
        key = "icterus"
        If data.ContainsKey(key) Then
            Content.Add("icterus_1", "sárgás árnyalatú, ")
            Content.Add("icterus_2", "sárgás árnyalatúak, ")
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
            Content.Add("pacemaker_kul", "Bal oldalon infraclavicularisan pacemaker telep található. ")
            Content.Add("pacemaker_nyaki", "A jobb szívfélben pacemaker elektróda azonosítható. ")
        End If
        Return True
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
        Dim flag = False
        '########################################################################
        key = "agy_allapot"
        If CheckRequired(key, data) Then
            Select Case data.Item(key)
                Case "normal"
                    Content.Add("agy_1", "Az agy tészta tapintatú, a tekervények és a barázdák kp. nagyságúak. ")
                Case "oedema"
                    Content.Add("agy_1", "A vizenyős agy tészta tapintatú, a tekervények kiszélesedtek, a barázdák sekélyek. ")
                    AddToDiag("Oedema cerebri.")
            End Select
            Content.Add("agy_2", "Az agytörzs és a kisagy eltérés nélkül.")
        ElseIf AbortOnMissing Then
            Return False
        End If
        '########################################################################
        key = "agy_beek"
        If data.ContainsKey(key) Then
            Content.Item("agy_2") = "Az agytörzs vizenyős, a kisagyi tonsillák körülárkoltak."
            AddToDiag("Oedema trunci cerebri et herniatio tonsillarum cerebelli.")
        End If
        '########################################################################
        key = "agy_stat_lac"
        If data.ContainsKey(key) Then
            Content.Add("agy_stat_lac", "A basalis magvakban és a fehérállományban több gócban 1-2 mm nagyságú üregek találhatóak. ")
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
            flag = True
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
            If flag Then
                text += ", a "
            Else
                text = "A "
            End If
            text += elvaltozas.Item("helyzet") + "agyállományi vérzés"
            AddToDiag("Apoplexia " + elvaltozas.Item("diag"))
            flag = True
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
                If flag Then
                    text += ", a "
                Else
                    text = "A "
                End If
                text += elvaltozas.Item("helyzet") + "daganatáttét"
                AddToDiag("Metastasis " + elvaltozas.Item("diag"))
                flag = True
            End If
            If data.ContainsKey("agy_attet_parietalis") Then
                elvaltozas = BrainLocationBuilder(meret, oldal, "parietalis")
                If flag Then
                    text += ", a "
                Else
                    text = "A "
                End If
                text += elvaltozas.Item("helyzet") + "daganatáttét"
                AddToDiag("Metastasis " + elvaltozas.Item("diag"))
                flag = True
            End If
            If data.ContainsKey("agy_attet_temporalis") Then
                elvaltozas = BrainLocationBuilder(meret, oldal, "temporalis")
                If flag Then
                    text += ", a "
                Else
                    text = "A "
                End If
                text += elvaltozas.Item("helyzet") + "daganatáttét"
                AddToDiag("Metastasis " + elvaltozas.Item("diag"))
                flag = True
            End If
            If data.ContainsKey("agy_attet_occ") Then
                elvaltozas = BrainLocationBuilder(meret, oldal, "occipitalis")
                If flag Then
                    text += ", a "
                Else
                    text = "A "
                End If
                text += elvaltozas.Item("helyzet") + "daganatáttét"
                AddToDiag("Metastasis " + elvaltozas.Item("diag"))
                flag = True
            End If
            If data.ContainsKey("agy_attet_kisagy") Then
                elvaltozas = BrainLocationBuilder(meret, oldal, "kisagy")
                If flag Then
                    text += ", a "
                Else
                    text = "A "
                End If
                text += elvaltozas.Item("helyzet") + "daganatáttét"
                AddToDiag("Metastasis " + elvaltozas.Item("diag"))
                flag = True
            End If
        End If
        If flag Then
            Content.Add("agy_elvaltozas", text + " figyelhető meg. ")
        End If

        If (data.ContainsKey("agy_lagyulas_lebeny") AndAlso data.Item("agy_lagyulas_lebeny") = "kisagy") OrElse
        (data.ContainsKey("agy_verzes_lebeny") AndAlso data.Item("agy_verzes_lebeny") = "kisagy") OrElse
        data.ContainsKey("agy_attet_kisagy") Then
            If data.ContainsKey("agy_beek") Then
                Content.Item("agy_2") = "Az agytörzs vizenyős, a kisagyi tonsillák körülárkoltak egyebekben eltérés nélkül."
            Else
                Content.Item("agy_2") = "Az agytörzs és a kisagy egyebekben eltérés nélkül."
            End If
        End If
        Return True
    End Function

    ''' <summary>
    ''' Applies rules regarding the heart
    ''' </summary>
    ''' <param name="data">Data form UI</param>
    Private Function ApplyRulesHeart(data As Dictionary(Of String, String)) As Boolean
        Dim key As String
        Dim text As String
        Dim flag As Boolean
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
            Content.Add("sziv_allapot_1", "A szív megnagyobbodott. ")
        End If
        text += "bal kamra fala " + data.Item("sziv_bal_kamra") + " mm, a "
        '########################################################################
        key = "sziv_cor_pulm"
        If data.ContainsKey(key) Then
            text += "tágult, túltengett "
            AddToDiag("Cor pulmonale chronicum.")
        End If
        text += "jobb kamra fala " + data.Item("sziv_jobb_kamra") + " mm vastag"
        '########################################################################
        key = "sziv_dcm"
        If data.ContainsKey(key) Then
            text += ", a kamrák fala elvékonyodott, lumenük extrém mértékben tágult"
            AddToDiag("Cardiomyopathia dilatativa.")
            If Content.ContainsKey("sziv_allapot_1") Then
                Content.Item("sziv_allapot_1") = "A szív kifejezetten megnagyobbodott. "
            Else
                Content.Add("sziv_allapot_1", "A szív kifejezetten megnagyobbodott. ")
            End If
        End If
        text += ". "
        Content.Add("sziv_allapot_2", text)
        '########################################################################
        key = "sziv_iszb"
        If data.ContainsKey(key) Then
            Content.Add("iszb", ", metszéslapján szürkésfehér rajzolat mutatkozik")
            AddToDiag("Cardyomyopathia ischaemica chronica.")
        End If
        '########################################################################
        key = "sziv_erek"
        If data.ContainsKey(key) Then
            Select Case data.Item(key)
                Case "kp"
                    Content.Add("koszoru_allapot", ", scleroticusak")
                Case "sulyos"
                    Content.Add("koszoru_allapot", ", súlyosan meszesek, lumenük szűkületet mutat")
            End Select
        End If
        '########################################################################
        key = "sziv_szukulet"
        If data.ContainsKey(key) Then
            If Not CheckRequired("sziv_szuk_percent", data) AndAlso AbortOnMissing Then
                Return False
            End If

            Content.Add("koszoru_szuk", "A ")
            Select Case data.Item(key)
                Case "jobb"
                    Content.Item("koszoru_szuk") += "jobb koszorúverőérben "
                Case "lad"
                    Content.Item("koszoru_szuk") += "bal koszorúverőér elülső leszálló ágában "
                Case "cx"
                    Content.Item("koszoru_szuk") += "bal koszorúverőér körbefutó ágában "
            End Select
            Content.Item("koszoru_szuk") += data.Item("sziv_szuk_percent") + " %-os lumenszűkület figyelhető meg. "
        End If
        '########################################################################
        key = "sziv_stent"
        If data.ContainsKey(key) Then
            Content.Add("stent", "A ")
            text = "Implantatum (stent) "
            flag = False

            If data.ContainsKey("sziv_stent_jobb") Then
                Content.Item("stent") += "jobb koszorúverőérben"
                text += "arteriae coronariae dextri cordis"
                flag = True
            End If

            If data.ContainsKey("sziv_stent_lad") Then
                If flag Then
                    Content.Item("stent") += ", "
                    text += " et "
                End If
                Content.Item("stent") += "bal koszorúverőér elülső leszálló ágában"
                text += "rami interventricularis anterioris arteriae coronariae sinistri cordis"
                flag = True
            End If

            If data.ContainsKey("sziv_stent_cx") Then
                If flag Then
                    Content.Item("stent") += ", "
                    text += " et "
                End If
                Content.Item("stent") += "bal koszorúverőér körbefutó ágában"
                text += "rami circumflexi arteriae coronariae sinistri cordis"
                flag = True
            End If
            Content.Item("stent") += " stent implantatum található. "
            AddToDiag(text + ".")
        End If
        '########################################################################
        key = "sziv_thrombus"
        If data.ContainsKey(key) Then
            If Not CheckRequired("sziv_thrombus_poz", data) AndAlso AbortOnMissing Then
                Return False
            End If
            Content.Add("thrombus", "A ")
            Select Case data.Item("sziv_thrombus_poz")
                Case "jobb"
                    Content.Item("thrombus") += "jobb koszorúverőérben"
                    AddToDiag("Thrombus recens arteriae coronariae dextri cordis.")
                Case "lad"
                    Content.Item("thrombus") += "bal koszorúverőér elülső leszálló ágában"
                    AddToDiag("Thrombus recens rami interventricularis anterioris arteriae coronariae sinistri cordis.")
                Case "cx"
                    Content.Item("thrombus") += "bal koszorúverőér körbefutó ágában "
                    AddToDiag("Thrombus recens rami circumflexi arteriae coronariae sinistri cordis.")
            End Select
            Content.Item("thrombus") += " friss vérrögös elzáródás figyelhető meg. "
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

            Content.Add("inf_regi", "A bal kamra ")
            Select Case data.Item("sziv_inf_regi_poz")
                Case "elulso"
                    Content.Item("inf_regi") += "elülső"
                    AddToDiag("Infarctus obsoletus parietis anterioris ventriculi sinsitri cordis.")
                Case "hatso"
                    Content.Item("inf_regi") += "hátulsó"
                    AddToDiag("Infarctus obsoletus parietis posterioris ventriculi sinsitri cordis.")
                Case "septalis"
                    Content.Item("inf_regi") += "sövényi"
                    AddToDiag("Infarctus obsoletus parietis septalis ventriculi sinsitri cordis.")
                Case "oldalso"
                    Content.Item("inf_regi") += "oldalsó"
                    AddToDiag("Infarctus obsoletus parietis lateralis ventriculi sinsitri cordis.")
            End Select
            Content.Item("inf_regi") += " falában " + data.Item("sziv_inf_regi_meret") + " mm nagyságú "
            Content.Item("inf_regi") += "szürkésfehér színű régi szívizomelhalás figyelhető meg. "
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

            Content.Add("inf_uj", "A bal kamra")
            Select Case data.Item("sziv_inf_uj_poz")
                Case "elulso"
                    Content.Item("inf_uj") += "elülső"
                    AddToDiag("Infarctus recens parietis anterioris ventriculi sinsitri cordis.")
                Case "hatso"
                    Content.Item("inf_uj") += "hátulsó"
                    AddToDiag("Infarctus recens parietis posterioris ventriculi sinsitri cordis.")
                Case "septalis"
                    Content.Item("inf_uj") += "sövényi"
                    AddToDiag("Infarctus recens parietis septalis ventriculi sinsitri cordis.")
                Case "oldalso"
                    Content.Item("inf_uj") += "oldalsó"
                    AddToDiag("Infarctus recens parietis lateralis ventriculi sinsitri cordis.")
            End Select
            Content.Item("inf_uj") += " falában " + data.Item("sziv_inf_uj_meret") + " mm nagyságú "
            Content.Item("inf_uj") += "agyagsárga színű, helyenként vörhenyes szegéllyel bíró, heveny szívizomelhalás figyelhető meg. "
        End If
        '########################################################################
        key = "sziv_stenosis"
        If data.ContainsKey(key) Then
            Content.Add(key, "z aortabillentyű meszes szűkületet mutat, egyebekben a")
            AddToDiag("Stenosis calcificans ostii aortae.")
        End If
        '########################################################################
        key = "sziv_mitralis"
        If data.ContainsKey(key) Then
            Content.Add(key, " mitralis billentyű anulusa kifejezetten meszes, egyebekben a")
            AddToDiag("Calcificatio ostii atrioventricularis sinistri cordis.")
        End If
        '########################################################################
        key = "sziv_haemo"
        If data.ContainsKey(key) Then
            If CheckRequired("sziv_haemo_g", data) Then
                Content.Add(key, "A szívburokban " + data.Item("sziv_haemo_g") + " g részben alvadt vér található. ")
                AddToDiag("Haemopericardium.")
            ElseIf AbortOnMissing Then
                Return False
            End If
        End If
        '########################################################################
        key = "sziv_cabg"
        If data.ContainsKey(key) Then
            Content.Add("sziv_cabg_kul", "a sternum felett régi hegvonal látható, a szegycsontban fémkapcsok figyelhetők meg, ")
            Content.Add("sziv_cabg_nyaki_1", ", a szívburok lapszerint latapadva")
            Content.Add("sziv_cabg_nyaki_2", "A koszorú-verőerekhez az aortából kiinduló bypass graftok csatlakoznak varratokkal,")
            Content.Item("sziv_cabg_nyaki_2") += " a graftok arterializálódtak, helyenkét szűkültek. "
            AddToDiag("Status post CABG.")
        End If
        Return True
    End Function

    ''' <summary>
    ''' Applies rules regarding the lungs
    ''' </summary>
    ''' <param name="data">Data form UI</param>
    Private Function ApplyRulesLungs(data As Dictionary(Of String, String)) As Boolean
        Dim key As String
        Dim text As String
        Dim flag As Boolean
        '########################################################################
        key = "tudo_anthra"
        If data.ContainsKey(key) Then
            Content.Add("tudo_anthra", "A mellhártyákon szürkésfekete hálózatos rajzolat látható. ")
            AddToDiag("Anthracosis pulmonum.")
        End If
        '########################################################################
        key = "tudo_emphy"
        If data.ContainsKey(key) Then
            Content.Add("tudo_emphy", "tágult ")
            AddToDiag("Emphysema pulmonum.")
        End If
        '########################################################################
        key = "tudo_oedema"
        If data.ContainsKey(key) Then
            Content.Add("tudo_oedema", ", vizenyősek, főként az alsó lebenyek vérbővek, vörhenyesek,")
            Content.Item("tudo_oedema") += " metszlapjukról nyomásra habos szilvalészerű folyadék ürül"
            AddToDiag("Oedema pulmonum.")
        End If
        '########################################################################
        key = "tudo_mindharom"
        If data.ContainsKey(key) Then
            If Not Content.ContainsKey("tudo_anthra") Then
                Content.Add("tudo_anthra", "A mellhártyákon szürkésfekete hálózatos rajzolat látható. ")
            End If
            If Not Content.ContainsKey("tudo_emphy") Then
                Content.Add("tudo_emphy", "tágult ")
            End If
            If Not Content.ContainsKey("tudo_oedema") Then
                Content.Add("tudo_oedema", ", vizenyősek, főként az alsó lebenyek vérbővek, vörhenyesek,")
                Content.Item("tudo_oedema") += " metszlapjukról nyomásra habos szilvalészerű folyadék ürül"
            End If
            AddToDiag("Anthracosis, emphysema et oedema pulmonum.")
        End If
        '########################################################################
        key = "tudo_bronch"
        If data.ContainsKey(key) Then
            Select Case data.Item(key)
                Case "chronic"
                    Content.Add("tudo_bronch_chron", "kifejezett")
                    AddToDiag("Bronchitis chronica.")
                Case "acut"
                    Content.Add("tudo_bronch_acut", "purulens váladékot tartalmazó ")
                    AddToDiag("Bronchitis chronica cum exacerbatinoe acuta.")
            End Select
        Else
            Content.Add("tudo_bronch_chron", "enyhe")
        End If
        '########################################################################
        key = "tudo_pneu"
        If data.ContainsKey(key) Then
            flag = False
            Content.Add("tudo_pneu", "")
            text = "Bronchopenumonia"
            If data.ContainsKey("tudo_pneu_mko") Then
                Content.Item("tudo_pneu") += "Mindkét tüdő alsó lebenye"
                text += " loborum inferiorum pulmonum"
                flag = True
            Else
                Content.Item("tudo_pneu") += "A "
            End If

            If data.ContainsKey("tudo_pneu_j_a") Then
                If flag Then
                    Content.Item("tudo_pneu") += ", "
                    text += " et"
                End If
                Content.Item("tudo_pneu") += "jobb tüdő alsó lebenye"
                text += " lobi inferioris pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_pneu_j_k") Then
                If flag Then
                    Content.Item("tudo_pneu") += ", "
                    text += " et"
                End If
                Content.Item("tudo_pneu") += "jobb tüdő középső lebenye"
                text += " lobi medii pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_pneu_j_f") Then
                If flag Then
                    Content.Item("tudo_pneu") += ", "
                    text += " et"
                End If
                Content.Item("tudo_pneu") += "jobb tüdő felső lebenye"
                text += " lobi superioris pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_pneu_b_a") Then
                If flag Then
                    Content.Item("tudo_pneu") += ", "
                    text += " et"
                End If
                Content.Item("tudo_pneu") += "bal tüdő alsó lebenye"
                text += " lobi inferioris pulmonis sinistri"
                flag = True
            End If

            If data.ContainsKey("tudo_pneu_b_f") Then
                If flag Then
                    Content.Item("tudo_pneu") += ", "
                    text += " et"
                End If
                Content.Item("tudo_pneu") += "bal tüdő felső lebenye"
                text += " lobi suprioris pulmonis sinistri"
                flag = True
            End If
            Content.Item("tudo_pneu") += " légtelen, tömött tapintatú, metszéslapján gennycsapok préselhetők. "
            AddToDiag(text + ".")
        End If
        '########################################################################
        key = "tudo_tumor"
        If data.ContainsKey(key) Then
            flag = False
            Content.Add("tudo_tumor", "")
            text = "Neoplasma malignum"
            If data.ContainsKey("tudo_tumor_m") Then
                Content.Item("tudo_tumor") += "Az összes lebenyben"
                text += " loborum omnium pulmonum"
                flag = True
            Else
                Content.Item("tudo_tumor") += "A "
            End If

            If data.ContainsKey("tudo_tumor_j_a") Then
                If flag Then
                    Content.Item("tudo_tumor") += ", "
                    text += " et"
                End If
                Content.Item("tudo_tumor") += "jobb alsó lebenyben"
                text += " lobi inferioris pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_tumor_j_k") Then
                If flag Then
                    Content.Item("tudo_tumor") += ", "
                    text += " et"
                End If
                Content.Item("tudo_tumor") += "jobb középső lebenyben"
                text += " lobi medii pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_tumor_j_f") Then
                If flag Then
                    Content.Item("tudo_tumor") += ", "
                    text += " et"
                End If
                Content.Item("tudo_tumor") += "jobb felső lebenyben"
                text += " lobi superioris pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_tumor_b_a") Then
                If flag Then
                    Content.Item("tudo_tumor") += ", "
                    text += " et"
                End If
                Content.Item("tudo_tumor") += "bal alsó lebenyben"
                text += " lobi inferioris pulmonis sinistri"
                flag = True
            End If

            If data.ContainsKey("tudo_tumor_b_f") Then
                If flag Then
                    Content.Item("tudo_tumor") += ", "
                    text += " et"
                End If
                Content.Item("tudo_tumor") += "bal felső lebenyben"
                text += " lobi superioris pulmonis sinistri"
                flag = True
            End If

            If CheckRequired("tudo_tumor_meret", data) Then
                Content.Item("tudo_tumor") += "szürkésfehér színű " + data.Item("tudo_tumor_meret")
                Content.Item("tudo_tumor") += " mm legnagyobb átmérőjű idegenszövet-szaporulat látható. "
            ElseIf AbortOnMissing Then
                Return False
            End If
            AddToDiag(text + ".")
        End If
        '########################################################################
        key = "tudo_attet"
        If data.ContainsKey(key) Then
            flag = False
            Content.Add("tudo_attet", "Az összes lebenyben")
            text = "Metastasis"
            If data.ContainsKey("tudo_attet_m") Then
                Content.Item("tudo_attet") += "Az összes lebenyben"
                text = " loborum omnium pulmonum"
                flag = True
            Else
                Content.Item("tudo_attet") += "A "
            End If

            If data.ContainsKey("tudo_attet_j_a") Then
                If flag Then
                    Content.Item("tudo_attet") += ", "
                    text += " et"
                End If
                Content.Item("tudo_attet") += "jobb alsó lebenyben"
                text += " lobi inferioris pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_attet_j_k") Then
                If flag Then
                    Content.Item("tudo_attet") += ", "
                    text += " et"
                End If
                Content.Item("tudo_attet") += "jobb középső lebenyben"
                text += " lobi medii pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_attet_j_f") Then
                If flag Then
                    Content.Item("tudo_attet") += ", "
                    text += " et"
                End If
                Content.Item("tudo_attet") += "jobb felső lebenyben"
                text += " lobi superioris pulmonis dextri"
                flag = True
            End If

            If data.ContainsKey("tudo_attet_b_a") Then
                If flag Then
                    Content.Item("tudo_attet") += ", "
                    text += " et"
                End If
                Content.Item("tudo_attet") += "bal alsó lebenyben"
                text += " lobi inferioris pulmonis sinistri"
                flag = True
            End If

            If data.ContainsKey("tudo_attet_b_f") Then
                If flag Then
                    Content.Item("tudo_attet") += ", "
                    text += " et"
                End If
                Content.Item("tudo_attet") += "bal felső lebenyben"
                text += " lobi superioris pulmonis sinistri"
                flag = True
            End If

            If CheckRequired("tudo_attet_meret", data) Then
                Content.Item("tudo_attet") += "szürkésfehér színű " + data.Item("tudo_attet_meret")
                Content.Item("tudo_attet") += " mm legnagyobb átmérőjű daganatáttét látható. "
            ElseIf AbortOnMissing Then
                Return False
            End If
            AddToDiag(text + ".")
        End If
        '########################################################################
        key = "tudo_embolia"
        If data.ContainsKey(key) Then

            If data.ContainsKey("tudo_embolia_lovag") Then
                Content.Add("tudo_emb_lovag_1", ", oszlásában vérrögös elzáródás láható")
                Content.Add("tudo_emb_lovag_2", ", egyebekben")
                AddToDiag("Thromboembolus bifurcationis trunci pulmonalis.")
            End If

            If data.ContainsKey("tudo_embolia_ket") Then
                Content.Add("tudo_emb_ket", ", oszlása után a tüdőverőerek mindkét főágában masszív vérrögös elzáródás látható")
                If Not Content.ContainsKey("tudo_emb_lovag_2") Then
                    Content.Add("tudo_emb_lovag_2", ", egyebekben")
                End If
                AddToDiag("Thromboembolus ramorum principalum arteriarum pulmonalum.")
            End If

            If data.ContainsKey("tudo_embolia_elso") Then
                If data.ContainsKey("tudo_embolia_b") Then
                    Content.Add("tudo_emb_elso_b", "bal arteria pulmonalis elsőrendű ágában vérrögös elzáródás látható, egyebekben ")
                    AddToDiag("Thromboembolus rami principalis arteriae pulmonalis sinistri.")
                End If
                If data.ContainsKey("tudo_embolia_j") Then
                    Content.Add("tudo_emb_elso_j", "jobb arteria pulmonalis elsőrendű ágában vérrögös elzáródás látható, egyebekben ")
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
                Content.Add("tudo_emb_tobb", "a másod-, és harmadrendű ágaiban vérrögös elzáródás látható, egyebekben ")
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

            Content.Add("tudo_hydro", "A mellüregben ")

            Select Case data.Item("tudo_hydro_poz")
                Case "bal"
                    Content.Item("tudo_hydro") += "bal oldalt "
                    AddToDiag("Hydrothorax l. s.")
                Case "jobb"
                    Content.Item("tudo_hydro") += "jobb oldalt "
                    AddToDiag("Hydrothorax l. d.")
                Case "mko"
                    Content.Item("tudo_hydro") += "mindkét oldalt "
                    AddToDiag("Hydrothorax l. u.")
            End Select
            Content.Item("tudo_hydro") += data.Item("tudo_hydro_liter") + " liter szalmasárga folyadék látható. "
        End If
        Return True
    End Function

    ''' <summary>
    ''' Applies rules regarding the stomach
    ''' </summary>
    ''' <param name="data">Data form UI</param>
    Private Function ApplyRulesStomach(data As Dictionary(Of String, String)) As Boolean
        Dim key As String
        Dim text As String
        Dim flag As Boolean
        '########################################################################
        key = "has_lep"
        If data.ContainsKey(key) Then
            Content.Add(key, "A vérbő, vörhenyes lép ")
            Select Case data.Item(key)
                Case "verbo"
                    Content.Item(key) += "megtartott szerkezetű. "
                    AddToDiag("Hyperaemia passiva lienis.")
                Case "puhult"
                    Content.Item(key) += "állománya ellágyult, metszlapjáról nagy mennyiségű kaparék nyerhető. "
                    AddToDiag("Splenitis septica acuta.")
                Case "nagy"
                    Content.Item(key) += "megnagyobbodott, állománya megtartott szerkezetű. "
                    AddToDiag("Splenomegalia.")
            End Select
        End If
        '########################################################################
        key = "has_maj"
        If data.ContainsKey(key) Then
            Content.Add(key, "A ")
            Select Case data.Item(key)
                Case "verbo"
                    Content.Item(key) += "máj vörhenyesbarna színű, állománya eltérés nélkül. "
                Case "enyhe"
                    Content.Item(key) += "máj vörhenyesbarna színű, állománya metszéslapon sárgásan zsírfényű. "
                    AddToDiag("Steatosis minoris gradus hepatis.")
                Case "zsir"
                    Content.Item(key) += "máj megnagyobbodott, szélei lekerekítettek, állománya zsírosan átalakult. "
                    AddToDiag("Steatosis hepatis.")
                Case "szerecsen"
                    Content.Item(key) += "vörhenyesbarna, vérbő máj metszlapon szerecsendió-rajzolatot mutat. "
                    AddToDiag("Hepar moschatum.")
                Case "cirr"
                    Content.Item(key) += "máj zsugorodott, állománya apró göbös kötőszövetes átalakulást mutat. "
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
            Content.Add("maj_attet", "A máj állományában ")
            Select Case data.Item("has_maj_attet_db")
                Case "egy"
                    Content.Item("maj_attet") += "egy "
                    AddToDiag("Metastasis hepatis.")
                Case "tobb"
                    Content.Item("maj_attet") += "több "
                    AddToDiag("Metastases multiplex hepatis.")
            End Select
            Content.Item("maj_attet") += "db " + data.Item("has_maj_attet_meret")
            Content.Item("maj_attet") += " mm legnagyobb kiterjedésű, szürkésfehér színű,"
            Content.Item("maj_attet") += " környezetétől élesen elhatárolódó daganatáttét azonosítható. "
        End If
        '########################################################################
        key = "has_hasnyal"
        If data.ContainsKey(key) Then
            Content.Add("hasnyal", "A hasnyálmirigy ")
            Select Case data.Item(key)
                Case "ep"
                    Content.Item("hasnyal") += "mirigyes, megtartott szerkezetű. "
                Case "chronic"
                    Content.Item("hasnyal") += "kiszélesedett, mirigyes állománya kifejezett. "
                    AddToDiag("Pancreatitis chronica.")
                Case "acut"
                    Content.Item("hasnyal") += "állománya kiszélesedett, kiterjedten barnás-vörhenyes elszíneződést mutat, "
                    Content.Item("hasnyal") += "nekrotikus, környezete vizenyős, a környező zsírszövetben sárgásfehér,"
                    Content.Item("hasnyal") += " ún. szappanképződés figyelhető meg. "
                    AddToDiag("Pancreatitis acuta.")
            End Select
        End If
        '########################################################################
        key = "has_epe"
        If data.ContainsKey(key) Then
            Select Case data.Item(key)
                Case "megtartott"
                    Content.Add("epe", "Az epehólyag fala megtartott szerkezetű")
                    If data.ContainsKey("has_epeko") Then
                        If Not CheckRequired("has_epeko_meret", data) AndAlso AbortOnMissing Then
                            Return False
                        End If
                        If Not CheckRequired("has_epeko_db", data) AndAlso AbortOnMissing Then
                            Return False
                        End If
                        Content.Item("epe") += ", lumenében " + data.Item("has_epeko_db") + " db, "
                        Content.Item("epe") += data.Item("has_epeko_meret") + " mm legnagyobb átmérőjű epekő azonosítható"
                        AddToDiag("Cholecystolithiasis.")
                    End If
                    Content.Item("epe") += ". "
                Case "eltavol"
                    Content.Add("epe", "Az epehólyagot korábban eltávolították. ")
                    AddToDiag("Status post cholecystectomiam.")
            End Select
        End If
        '########################################################################
        key = "has_gyomor"
        If data.ContainsKey(key) Then
            Select Case data.Item(key)
                Case "ep"
                    Content.Add("gyomor", "A gyomor fala, nyálkahártyája eltérés nélkül, redőzete megtartott. ")
                Case "erosio"
                    Content.Add("gyomor", "A gyomor nyálkahártyáján erosiók láthatók. ")
                    AddToDiag("Erosiones ventriculi.")
                Case "fekely"
                    Content.Add("gyomor", "A gyomorban a ")
                    If Not CheckRequired("has_gyomor_fekely_meret", data) AndAlso AbortOnMissing Then
                        Return False
                    End If
                    If Not CheckRequired("has_gyomor_fekely_gorb", data) AndAlso AbortOnMissing Then
                        Return False
                    End If
                    Select Case data.Item("has_gyomor_fekely_gorb")
                        Case "kis"
                            Content.Item("gyomor") += "kisgörbület"
                        Case "nagy"
                            Content.Item("gyomor") += "nagygörbület"
                    End Select
                    Content.Item("gyomor") += " területén " + data.Item("has_gyomor_fekely_meret")
                    Content.Item("gyomor") += " mm legnagyobb átmérőjű fekély figyelhető meg. "
                    AddToDiag("Ulcus ventriculi.")
            End Select
        End If
        '########################################################################
        key = "has_gyomor_tumor"
        If data.ContainsKey(key) Then
            Content.Add("gyomor_tumor", "A gyomorban a ")
            If Not CheckRequired("has_gyomor_tumor_meret", data) AndAlso AbortOnMissing Then
                Return False
            End If
            If Not CheckRequired("has_gyomor_tumor_gorb", data) AndAlso AbortOnMissing Then
                Return False
            End If
            Select Case data.Item("has_gyomor_tumor_gorb")
                Case "kis"
                    Content.Item("gyomor_tumor") += "kisgörbület"
                Case "nagy"
                    Content.Item("gyomor_tumor") += "nagygörbület"
            End Select
            Content.Item("gyomor_tumor") += " területén " + data.Item("has_gyomor_tumor_meret")
            Content.Item("gyomor_tumor") += " mm nagyságú szürkésfehér idegenszövet-szaporulat figyelhető meg. "
            AddToDiag("Neoplasma malignum ventriculi.")
        End If
        '########################################################################
        key = "has_nyombel"
        If data.ContainsKey(key) Then
            Select Case data.Item(key)
                Case "ep"
                    Content.Add("nyombel", "A nyombél eltérés nélkül. ")
                Case "fekely"
                    Content.Add("nyombel", "A nyombél nyálkahártyáján ")
                    If Not CheckRequired("has_nyombel_meret", data) AndAlso AbortOnMissing Then
                        Return False
                    End If
                    Content.Item("nyombel") += data.Item("has_nyombel_meret") + " mm legnagyobb átmérőjű fekély figyelhető meg. "
                    AddToDiag("Ulcus duodeni.")
            End Select
        End If
        '########################################################################
        key = "has_ileum"
        If data.ContainsKey(key) Then
            If Not CheckRequired("has_ileum_meret", data) AndAlso AbortOnMissing Then
                Return False
            End If
            Content.Add("ileum", "Az ileum nyálkahártyája ")
            Content.Item("ileum") += data.Item("has_ileum_meret")
            Content.Item("ileum") += " cm-es szakaszon vizenyős, felszínén sárgásfehér felrakódás mutatkozik. "
            AddToDiag("Ileitis pseudomembranacea.")
        End If
        '########################################################################
        key = "has_bel"
        If data.ContainsKey(key) Then
            Content.Add("bel", "A vékonybelek között több területen heges kitapadások azonosíthatóak. ")
            AddToDiag("Adhaesinones intestini tenuis.")
        End If
        '########################################################################
        key = "vastagbel_divert"
        If data.ContainsKey(key) Then
            Content.Add(key, "A szigmabélben több területen a nyálkahártya zsákszerű kitüremkedése látható. ")
            AddToDiag("Divetriculosis sigmatos.")
        End If
        '########################################################################
        key = "vastagbel_col_is"
        If data.ContainsKey(key) Then
            Content.Add("vastagbel_ischaem", "A vastagbél nyálkahártyája diffúzan vörhenyesbarna elszíneződést mutat. ")
            AddToDiag("Colitis ischaemica.")
        End If
        '########################################################################
        key = "vastagbel_col_alh"
        If data.ContainsKey(key) Then
            Content.Add("vastagbel_alhartya", "A vastagbél nyálkahártyája diffúzan vizenyős, felszínén sárgásfehér felrakódás mutatkozik. ")
            AddToDiag("Colitis pseudomembranacea.")
        End If
        '########################################################################
        key = "vastagbel_tumor"
        If data.ContainsKey(key) Then
            Content.Add(key, "A ")
            text = "Neoplasma malignum"
            flag = False

            If data.ContainsKey("vastagbel_tumor_le") Then
                Content.Item(key) += "leszálló vastagbél"
                text += " colontos descendentis"
                flag = True
            End If

            If data.ContainsKey("vastagbel_tumor_fel") Then
                If flag Then
                    Content.Item(key) += ", "
                    text += " et"
                End If
                Content.Item(key) += "felszálló vastagbél"
                text += " colontos ascendentis"
                flag = True
            End If

            If data.ContainsKey("vastagbel_tumor_sigma") Then
                If flag Then
                    Content.Item(key) += ", "
                    text += " et"
                End If
                Content.Item(key) += "szigmabél"
                text += " sigmatos"
                flag = True
            End If

            If data.ContainsKey("vastagbel_tumor_harant") Then
                If flag Then
                    Content.Item(key) += ", "
                    text += " et"
                End If
                Content.Item(key) += "haránt vastagbél"
                text += " colontos transversi"
                flag = True
            End If

            If data.ContainsKey("vastagbel_tumor_coec") Then
                If flag Then
                    Content.Item(key) += ", "
                    text += " et"
                End If
                Content.Item(key) += "vakbél"
                text += " coeci"
                flag = True
            End If

            If data.ContainsKey("vastagbel_tumor_vegbel") Then
                If flag Then
                    Content.Item(key) += ", "
                    text += " et "
                End If
                Content.Item(key) += "végbél"
                text += "recti"
                flag = True
            End If

            If CheckRequired("vastagbel_tumor_meret", data) Then
                Content.Item(key) += " területén " + data.Item("vastagbel_tumor_meret")
                Content.Item(key) += " cm-es szakaszon a nyálkahártyából kiinduló, "
            ElseIf AbortOnMissing Then
                Return False
            End If

            If data.ContainsKey("vastagbel_tumor_szukito") Then
                Content.Item(key) += "a lumen jelentős szűkületét okozó, "
            End If

            Content.Item(key) += "szürkésfehér színű, idegenszövet-szaporulat azonosítható. "
            AddToDiag(text + ".")
        End If
        Return True
    End Function

    ''' <summary>
    ''' Applies rules regarding the kidney
    ''' </summary>
    ''' <param name="data">Data form UI</param>
    Private Function ApplyRulesKidney(data As Dictionary(Of String, String)) As Boolean
        Dim key As String
        '########################################################################
        key = "has_vese"
        If CheckRequired(key, data) Then
            Select Case data.Item(key)
                Case "sima"
                    Content.Add(key, ", felszínük sima")
                Case "szemcses"
                    Content.Add(key, ", felszínükön finom szemcsézettség ")
                    If data.ContainsKey("vese_behuz") Then
                        Content.Item(key) += "és számos behúzódás "
                        AddToDiag("Nephritis interstitialis chronica l. u.")
                    End If
                    Content.Item(key) += "látható"
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

            Content.Add(key, "A ")
            Select Case data.Item("vese_tumor_poz")
                Case "bal"
                    Content.Item(key) += "bal "
                    AddToDiag("Neoplasma malignum renis sinistri.")
                Case "jobb"
                    Content.Item(key) += "jobb "
                    AddToDiag("Neoplasma malignum renis dextri.")
            End Select
            Content.Item(key) += "vese állományában " + data.Item("vese_tumor_meret") + " mm nagyságú, "
            Content.Item(key) += "kénsárga, helyenként vörhenyes idegenszövet-szaporulat azonosítható. "
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

            Content.Add(key, "A ")
            Select Case data.Item("veseko_poz")
                Case "bal"
                    Content.Item(key) += "bal "
                    AddToDiag("Nephrolithiasis sinistri.")
                Case "jobb"
                    Content.Item(key) += "jobb "
                    AddToDiag("Nephrolithiasis dextri.")
            End Select
            Content.Item(key) += "vesemedence területén " + data.Item("veseko_meret")
            Content.Item(key) += " mm legnagyobb kiterjedésű vesekő azonosítható. "
        End If
        '########################################################################
        key = "vese_pyelo"
        If data.ContainsKey(key) Then
            If Not CheckRequired("vese_pyelo_poz", data) AndAlso AbortOnMissing Then
                Return False
            End If

            Content.Add(key, "")
            Select Case data.Item("vese_pyelo_poz")
                Case "bal"
                    Content.Item(key) += "A bal "
                    AddToDiag("Pyelonephritis acuta purulenta sinistri.")
                Case "jobb"
                    Content.Item(key) += "A jobb "
                    AddToDiag("Pyelonephritis acuta purulenta dextri.")
                Case "mko"
                    Content.Item(key) += "Mindkét "
                    AddToDiag("Pyelonephritis acuta purulenta l.u.")
            End Select
            Content.Item(key) += "vesemedencében purulens váladék azonosítható,"
            Content.Item(key) += " a vesék felszínén kicsiny abscessusok láthatók. "
        End If
        '########################################################################
        key = "holyag_kateter"
        If data.ContainsKey(key) Then
            Content.Add("kateter", "A húgyhólyagban katéter található. ")
        End If
        '########################################################################
        key = "holyag_gyull"
        If data.ContainsKey(key) Then
            Content.Add(key, "A húgyhólyag nyálkahártyája diffúzan vörhenyes,")
            Content.Item(key) += " lumenében opálos vizelet azonosítható. "
            AddToDiag("Urocytitis acuta.")
        End If
        '########################################################################
        key = "holyag_tumor"
        If data.ContainsKey(key) Then
            If Not CheckRequired("holyag_tumor_meret", data) AndAlso AbortOnMissing Then
                Return False
            End If
            Content.Add(key, "A húgyhólyag lumenében ")
            Content.Item(key) += data.Item("holyag_tumor_meret")
            Content.Item(key) += " mm legnagyobb kiterjedésű, szürkésfehér-vörhenyes"
            Content.Item(key) += " idegenszövet-szaporulat azonosítható. "
            AddToDiag("Neoplasma malignum vesicae urinariae.")
        End If
        '########################################################################
        If Not data.ContainsKey("holyag_gyull") AndAlso Not data.ContainsKey("holyag_tumor") Then
            Content.Add("holyag", "és a húgyhólyag ")
        End If
        '########################################################################
        key = "meh_iud"
        If data.ContainsKey(key) Then
            Content.Add("iud", "A méh üregében fogamzásgátló eszköz azonosítható. ")
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

            Content.Add(key, "A méh izmos falában ")
            Content.Item(key) += data.Item("meh_myoma_darab") + " darab, " + data.Item("meh_myoma_meret")
            Content.Item(key) += " mm nagyságú, szürkésfehér színű, örvényes szerkezetű,"
            Content.Item(key) += " myomagöbnek imponáló képlet mutatkozik. "
            AddToDiag("Myomata uteri.")
        End If
        '########################################################################
        key = "meh_em"
        If data.ContainsKey(key) Then
            If Not CheckRequired("meh_em_meret", data) AndAlso AbortOnMissing Then
                Return False
            End If

            Content.Add(key, "A méh üregében ")
            Content.Item(key) += data.Item("meh_em_meret") + " mm nagyságú polypoid képlet azonosítható. "
            AddToDiag("Polypus endometrialis uteri.")
        End If
        '########################################################################
        key = "meh_tumor"
        If data.ContainsKey(key) Then
            If Not CheckRequired("meh_tumor_meret", data) AndAlso AbortOnMissing Then
                Return False
            End If

            Content.Add(key, "A méh üregében a myometriumot is infiltráló szürkésfehér,")
            Content.Item(key) += " helyenként vörhenyesbarna idegenszövet-szaporulat azonosítható. "
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

            Content.Add(key, "A ")
            Select Case data.Item("meh_cysta_poz")
                Case "bal"
                    Content.Item(key) += "bal "
                    AddToDiag("Cysta ovarii sinistri.")
                Case "jobb"
                    Content.Item(key) += "jobb "
                    AddToDiag("Cysta ovarii dextri.")
            End Select
            Content.Item(key) += "petefészek állományában " + data.Item("meh_cysta_meret")
            Content.Item(key) += " mm nagyságú, hártyás falú, víztiszta bennékű ciszta mutatkozik. "
        End If
        '########################################################################
        key = "prostata"
        If data.ContainsKey(key) Then
            Content.Add(key, "A húgyhólyag lumene tágult, izomzata vaskos, a prostata megnagyobbodott,")
            Content.Item(key) += " állománya göbös, körülírt kóros nem azonosítható. "
            AddToDiag("Hyperplasia nodosa prostatae.")
        End If
        '########################################################################
        key = "scrotum"
        If data.ContainsKey(key) Then
            Content.Add(key, "A scrotum megvastagodott, vizenyős. ")
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

            Content.Add(key, "A ")
            Select Case data.Item("here_tumor_poz")
                Case "bal"
                    Content.Item(key) += "bal "
                    AddToDiag("Neoplasma malignum testis sinistri.")
                Case "jobb"
                    Content.Item(key) += "jobb "
                    AddToDiag("Neoplasma malignum testis dextri.")
            End Select
            Content.Item(key) += "here állományában jól körülírt, " + data.Item("here_tumor_meret")
            Content.Item(key) += " mm nagyságú, szürkésfehér színű, helyenként barnás-vörhenyes"
            Content.Item(key) += " idegenszövet-szaporulat azonosítható. "
        End If
        Return True
    End Function

    ''' <summary>
    ''' Applies rules regarding the death
    ''' </summary>
    ''' <param name="data">Data form UI</param>
    Private Function ApplyRulesDeath(data As Dictionary(Of String, String)) As Boolean
        Dim key As String
        Dim text = ""
        Dim flag = False
        '########################################################################
        key = "halal"
        If CheckRequired(key, data) Then
            Content.Add(key, "")
            Select Case data.Item(key)
                Case "asu_iszb_sze"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
                    Content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
                    Content.Item(key) += "A halál okaként az arteria coronariák súlyos szűkülete és a szívizom idült ischaemiás "
                    Content.Item(key) += "elfajulása következtében kialakult szívelégtelenséget jelöltük meg"
                Case "asu_regi_sze"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
                    Content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
                    Content.Item(key) += "A bal kamrában régi szívizomelhalást figyelhettünk meg. A halál okaként az arteria "
                    Content.Item(key) += "coronariák súlyos szűkülete és a szívizom idült ischaemiás elfajulása következtében "
                    Content.Item(key) += "kialakult szívelégtelenséget jelöltük meg"
                Case "asu_heveny"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
                    Content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
                    Content.Item(key) += "A nagyfokú arteria coronaria sclerosis és szűkület heveny első fali/hátsó fali "
                    Content.Item(key) += "szívizomelhalást eredményezett, melyet a halál okaként jelöltünk meg"
                Case "asu_heveny_tamp"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
                    Content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
                    Content.Item(key) += "A nagyfokú arteria coronaria sclerosis és szűkület heveny első fali/hátsó fali "
                    Content.Item(key) += "szívizomelhalást eredményezett, itt a szabad rupturált, következményes heveny "
                    Content.Item(key) += "szívburki vérgyülemet okozva. A halál okaként a szívtamponádot jelöltük meg"
                Case "copd"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként idült obstruktív tüdőbetegséget "
                    Content.Item(key) += "állapítottunk meg, illetve következményes idült tüdőeredetű szívbetegséget figyelhettünk "
                    Content.Item(key) += "meg, melyet a tágult túltengett jobb szívfél és a belszervi pangás morfológiailag alátámasztott. "
                    Content.Item(key) += "A halál okaként a szívelégtelenséget jelöltük meg"
                Case "emphy"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként súlyos tüdőtágulatot állapítottunk meg, "
                    Content.Item(key) += "illetve következményes idült tüdőeredetű szívbetegséget figyelhettünk meg, melyet a "
                    Content.Item(key) += "tágult túltengett jobb szívfél és a belszervi pangás morfológiailag alátámasztott. "
                    Content.Item(key) += "A halál okaként a szívelégtelenséget jelöltük meg"
                Case "ht_hszb_sze"
                    Content.Item(key) += "anamnézisében magas vérnyomás betegség szerepel. A patológiai vizsgálata során magas "
                    Content.Item(key) += "vérnyomásos szívtúltengést állapítottunk meg, melyet a körkörösen/tágult súlyosan "
                    Content.Item(key) += "túltengett bal kamra morfológiailag alátámasztott. A halál okaként a szívelégtelenséget jelöltük meg"
                Case "htszb_sze"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként magas vérnyomásos szívtúltengést "
                    Content.Item(key) += "állapítottunk meg, melyet a körkörösen/tágult, súlyosan túltengett bal kamra "
                    Content.Item(key) += "morfológiailag alátámasztott. A halál okaként a szívelégtelenséget jelöltük meg."
                Case "asu_tudo"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként az aortát és a szívkoszorú verőereket "
                    Content.Item(key) += "kifejezett mértékben érintő, súlyos, általános érelmeszesedést állapítottunk meg. "
                    Content.Item(key) += "A halál közvetlen oka a pulmonalis főtörzs oszlásának lumenét teljesen elzáró, "
                    Content.Item(key) += "ún. lovagló thromboembolia / a tüdőverőerek elsőrendű ágainak masszív vérrögös elzáródása volt. "
                    Content.Item(key) += "Az embolia feltételezhetően bal/jobb alsóvégtagi mélyvénás rögösödésből származott. "
                Case "htszb_tudo"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként hypertensiv szívbetegséget állapítottunk meg. "
                    Content.Item(key) += "A halál közvetlen oka a pulmonalis főtörzs oszlásának lumenét teljesen elzáró, "
                    Content.Item(key) += "ún. lovagló thromboembolia / a tüdőverőerek elsőrendű ágainak masszív vérrögös elzáródása volt. "
                    Content.Item(key) += "Az embolia feltételezhetően bal/jobb alsóvégtagi mélyvénás rögösödésből származott."
                Case "ht_apo"
                    Content.Item(key) += "patológiai vizsgálata során alapbetegségként a bal/jobb agyféltekét/a kisagyat érintő "
                    Content.Item(key) += "roncsoló agyvérzést állapítottunk meg, szövődményként agyi nyomásfokozódás és a nyúltvelői "
                    Content.Item(key) += "légzési-keringési rendszer nyomás alá kerülése miatt következményes cardiorespiratoricus "
                    Content.Item(key) += "elégtelenség kialakulásával, melyek a halál közvetlen okaként megjelölt szívmegálláshoz vezettek."
                Case "etil"
                    Content.Item(key) += ""
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
            Content.Add("kisero", "Kísérő betegségként ")
            Content.Item("kisero") += text + " emelendő ki."
        End If

        Return True

    End Function
End Class
