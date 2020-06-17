Imports BoncJKV.Logger
''' <summary>
''' Main UI form
''' </summary>
Public Class App
  ''' <summary>
  ''' Application Name
  ''' </summary>
  Public Const AppName = "BoncJKV"
  ''' <summary>
  ''' Transformer object
  ''' </summary>
  Private WithEvents Transformer As Rules
  ''' <summary>
  ''' Application Path
  ''' </summary>
  Private Path As String = Application.StartupPath + IO.Path.DirectorySeparatorChar
  Public SaveFilePath As String = "saves.xml"
  Public LogFilePath As String = "log.txt"
  ''' ''''''''''''''''''''''''''''''''''''''''''''''''''''' Main features '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
  ''' <summary>
  ''' Initializes form
  ''' </summary>
  Friend Sub InitUI(sender As Object, e As EventArgs) Handles MyBase.Load, menu_new.Click, toolstrip_new.Click
    Try
      ComponentManager.Main = Me
      ComponentManager.Logger = New FileLogger(LogFilePath)
      ComponentManager.UI = New UI

      ComponentManager.UI.ResetScreen()
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Save data to disk
  ''' </summary>
  Private Sub SaveDataUI(sender As Object, e As EventArgs) Handles menu_save.Click, toolstrip_save.Click
    Try
      Dim datamng = New DataManager
      Dim xmlexp = New XMLExporter(SaveFilePath)

      datamng.CollectData(dataInput.Controls)
      xmlexp.SaveData(nev.Text, datum.Text, datamng.GetData)
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Loads data from disk
  ''' </summary>
  ''' <param name="name">Patient name to load</param>
  ''' <param name="datte">Date of inspection</param>
  Friend Sub LoadDataUI(ByVal name As String, ByVal datte As String)
    Try
      ComponentManager.UI.ResetScreen()
      ComponentManager.UI.SetNameDate(name, datte)

      Dim datamng = New DataManager
      Dim xmlexporter = New XMLExporter(SaveFilePath)
      datamng.LoadData(xmlexporter.LoadData(name, datte), dataInput.Controls)
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Exports data
  ''' </summary>
  Private Sub ExportWord(sender As Object, e As EventArgs) Handles menu_export.Click, toolstrip_export.Click
    Try
      Dim datamng = New DataManager
      Transformer = New Rules(True)
      Dim exporter = New WordExporter()

      datamng.CollectData(dataInput.Controls)

      If Not Transformer.ApplyRules(datamng.GetData) Then
        Exit Sub
      End If

      exporter.Open(Path + "bjk.docx")
      exporter.LoadData(Transformer.GetContent)
      IO.Directory.CreateDirectory(Path + "jkv")
      exporter.SaveAs(Path + "jkv" + IO.Path.DirectorySeparatorChar + nev.Text + "_" + datum.Text + "_bjk.docx")
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Close application
  ''' </summary>
  Private Sub CloseApp(sender As Object, e As EventArgs) Handles menu_exit.Click
    Application.Exit()
  End Sub
  ''' <summary>
  ''' Show about
  ''' </summary>
  Private Sub ShowAbout(sender As Object, e As EventArgs) Handles menu_about.Click
    About.Show()
  End Sub
  ''' ''''''''''''''''''''''''''''''''''''''''''''''''''''' UI actions ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
  ''' <summary>
  ''' Opens load dialog
  ''' </summary>
  Private Sub Loading(sender As Object, e As EventArgs) Handles menu_open.Click, toolstrip_open.Click
    Try
      LoadForm.Show()
    Catch ex As Exception
      ErrorHandling.General(ex)
    End Try
  End Sub
  ''' <summary>
  ''' Switch to the next tab
  ''' </summary>
  Private Sub NextTab(sender As Object, e As EventArgs) Handles next_1.Click, next_2.Click, next_3.Click, next_4.Click, next_5.Click, next_6.Click
    ComponentManager.UI.NextTab(sender)
  End Sub
  ''' <summary>
  ''' Reset tab
  ''' </summary>
  Private Sub ResetTab(sender As Object, e As EventArgs) Handles reset_1.Click, reset_2.Click, reset_3.Click, reset_4.Click, reset_5.Click, reset_6.Click, reset_7.Click
    ComponentManager.UI.ResetTab(sender)
  End Sub
  ''' <summary>
  ''' UI action when a required field is missing
  ''' </summary>
  ''' <param name="fieldname">Missing field name</param>
  Private Sub FieldMissing(ByVal fieldname As String) Handles Transformer.FieldMissing
    UI.Warning("Hiányzó adat: " + vbNewLine + vbNewLine + fieldname, AppName)
  End Sub
  ''' <summary>
  ''' Change form background
  ''' </summary>
  Private Sub IcterusChange(sender As Object, e As EventArgs) Handles icterus.CheckedChanged
    ComponentManager.UI.IcterusChange(sender)
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableControl(sender As Object, e As EventArgs) Handles here_tumor.CheckedChanged, decub_meret.TextChanged,
    ascites.CheckedChanged, pacemaker.CheckedChanged, agy_lagyulas.CheckedChanged, agy_verzes.CheckedChanged,
    agy_attet.CheckedChanged, koszoru_szuk.TextChanged, koszoru_stent.CheckedChanged, koszoru_thromb.CheckedChanged,
    inf_regi.CheckedChanged, inf_uj.CheckedChanged, haemo.CheckedChanged, pneu.CheckedChanged,
    tudo_tumor.CheckedChanged, tudo_attet.CheckedChanged, tudo_emb.CheckedChanged, hydro.CheckedChanged,
    maj_attet.CheckedChanged, epeko.CheckedChanged, gyomor_fekely.CheckedChanged, gyomor_tumor.CheckedChanged,
    nyombel_fekely.CheckedChanged, ileum.CheckedChanged, bel_tumor.CheckedChanged, vese_tumor.CheckedChanged,
    veseko.CheckedChanged, pyelo.CheckedChanged, holyag_tumor.CheckedChanged, pete.CheckedChanged,
    meh_myoma.CheckedChanged, meh_em.CheckedChanged, meh_tumor.CheckedChanged, here_tumor.CheckedChanged

    Dim controls As Collection
    If sender.Equals(decub_meret) Then
      controls = New Collection From {decub_b, decub_j, decub_sac, decub_sark}
    ElseIf sender.Equals(ascites) Then
      controls = New Collection From {asc_l, asc_liter}
    ElseIf sender.Equals(pacemaker) Then
      controls = New Collection From {pacemaker_serial, pacemaker_id}
    ElseIf sender.Equals(agy_lagyulas) Then
      controls = New Collection From {agy_lagyulas_bal, agy_lagyulas_jobb, agy_lagyulas_front, agy_lagyulas_pariet,
                                agy_lagyulas_temp, agy_lagyulas_occ, agy_lagyulas_kisagy, agy_lagyulas_meret, agy_lagyulas_cm}
    ElseIf sender.Equals(agy_verzes) Then
      controls = New Collection From {agy_verzes_bal, agy_verzes_jobb, agy_verzes_front, agy_verzes_pariet,
                                agy_verzes_temp, agy_verzes_occ, agy_verzes_kisagy, agy_verzes_meret, agy_verzes_cm}
    ElseIf sender.Equals(agy_attet) Then
      controls = New Collection From {agy_attet_bal, agy_attet_jobb, agy_attet_front, agy_attet_pariet, agy_attet_temp,
                                agy_attet_occ, agy_attet_kisagy, agy_attet_egy, agy_attet_tobb, agy_attet_meret, agy_attet_cm}
    ElseIf sender.Equals(koszoru_szuk) Then
      controls = New Collection From {koszoru_szuk_jobb, koszoru_szuk_lad, koszoru_szuk_cx}
    ElseIf sender.Equals(koszoru_stent) Then
      controls = New Collection From {koszoru_stent_jobb, koszoru_stent_lad, koszoru_stent_cx}
    ElseIf sender.Equals(koszoru_thromb) Then
      controls = New Collection From {koszoru_thromb_jobb, koszoru_thromb_lad, koszoru_thromb_cx}
    ElseIf sender.Equals(inf_regi) Then
      controls = New Collection From {inf_regi_meret, inf_regi_cm, inf_regi_elul, inf_regi_hat, inf_regi_sept, inf_regi_oldal}
    ElseIf sender.Equals(inf_uj) Then
      controls = New Collection From {inf_uj_meret, inf_uj_cm, inf_uj_elul, inf_uj_hat, inf_uj_sept, inf_uj_oldal}
    ElseIf sender.Equals(haemo) Then
      controls = New Collection From {haemo_tomeg, haemo_g}
    ElseIf sender.Equals(pneu) Then
      controls = New Collection From {pneu_ba, pneu_bf, pneu_ja, pneu_jf, pneu_jk, pneu_mko}
    ElseIf sender.Equals(tudo_tumor) Then
      controls = New Collection From {tudo_tumor_ba, tudo_tumor_bf, tudo_tumor_ja, tudo_tumor_jf,
                                tudo_tumor_jk, tudo_tumor_meret, tudo_tumor_minden, tudo_tumor_mm}
    ElseIf sender.Equals(tudo_attet) Then
      controls = New Collection From {tudo_attet_ba, tudo_attet_bf, tudo_attet_ja, tudo_attet_jf,
                                tudo_attet_jk, tudo_attet_meret, tudo_attet_minden, tudo_attet_mm}
    ElseIf sender.Equals(tudo_emb) Then
      controls = New Collection From {tudo_emb_bal, tudo_emb_elso, tudo_emb_jobb, tudo_emb_ket, tudo_emb_lovag, tudo_emb_tobb}
    ElseIf sender.Equals(hydro) Then
      controls = New Collection From {hydro_b, hydro_j, hydro_liter, hydro_menny, hydro_mko}
    ElseIf sender.Equals(maj_attet) Then
      controls = New Collection From {maj_attet_group, maj_attet_egy, maj_attet_meret, maj_attet_mm, maj_attet_tobb}
    ElseIf sender.Equals(epeko) Then
      controls = New Collection From {epeko_darab, epeko_db, epeko_meret, epeko_mm}
    ElseIf sender.Equals(gyomor_fekely) Then
      controls = New Collection From {gyomor_fekely_group, gyomor_fekely_kis, gyomor_fekely_meret, gyomor_fekely_mm, gyomor_fekely_nagy}
    ElseIf sender.Equals(gyomor_tumor) Then
      controls = New Collection From {gyomor_tumor_group, gyomor_tumor_kis, gyomor_tumor_meret, gyomor_tumor_mm, gyomor_tumor_nagy}
    ElseIf sender.Equals(nyombel_fekely) Then
      controls = New Collection From {nyombel_fekely_meret, nyombel_fekely_mm}
    ElseIf sender.Equals(ileum) Then
      controls = New Collection From {ileum_cm, ileum_meret}
    ElseIf sender.Equals(bel_tumor) Then
      controls = New Collection From {bel_tumor_cm, bel_tumor_coec, bel_tumor_fel, bel_tumor_harant,
                                bel_tumor_le, bel_tumor_meret, bel_tumor_sig, bel_tumor_szuk, bel_tumor_vegbel}
    ElseIf sender.Equals(vese_tumor) Then
      controls = New Collection From {vese_tumor_b, vese_tumor_grp, vese_tumor_j, vese_tumor_meret, vese_tumor_mm}
    ElseIf sender.Equals(veseko) Then
      controls = New Collection From {veseko_b, veseko_grp, veseko_j, veseko_meret, veseko_mm}
    ElseIf sender.Equals(pyelo) Then
      controls = New Collection From {pyelo_b, pyelo_grp, pyelo_j, pyelo_mko}
    ElseIf sender.Equals(holyag_tumor) Then
      controls = New Collection From {holyag_tumor_meret, holyag_tumor_mm}
    ElseIf sender.Equals(pete) Then
      controls = New Collection From {pete_b, pete_j, pete_meret, pete_mm}
    ElseIf sender.Equals(meh_myoma) Then
      controls = New Collection From {meh_myoma_darab, meh_myoma_db, meh_myoma_meret, meh_myoma_mm}
    ElseIf sender.Equals(meh_em) Then
      controls = New Collection From {meh_em_meret, meh_em_mm}
    ElseIf sender.Equals(meh_tumor) Then
      controls = New Collection From {meh_tumor_meret, meh_tumor_mm}
    ElseIf sender.Equals(here_tumor) Then
      controls = New Collection From {here_tumor_b, here_tumor_j, here_tumor_meret, here_tumor_mm}
    Else
      controls = New Collection From {}
    End If
    ComponentManager.UI.EnableAssociatedControls(sender, controls)
  End Sub
End Class
