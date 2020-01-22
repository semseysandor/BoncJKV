''' <summary>
''' Main UI form
''' </summary>
Public Class Main
  ''' <summary>
  ''' Application Name
  ''' </summary>
  Public Const AppName = "BoncJKV"
  ''' <summary>
  ''' Transformer object
  ''' </summary>
  Private WithEvents transformer As WordTransformer
  ''' <summary>
  ''' UI worker
  ''' </summary>
  Private ui As UI = New UI(Me)
  ''' ''''''''''''''''''''''''''''''''''''''''''''''''''''' Main features '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
  ''' <summary>
  ''' Save data to disk
  ''' </summary>
  Private Sub SaveDataUI(sender As Object, e As EventArgs) Handles saveBtn.Click
    Dim datamng = New DataManager
    Dim xmlexp = New XMLExporter

    datamng.CollectData(dataInput.Controls)
    xmlexp.SaveData(nev.Text, datum.Text, datamng.GetData)
  End Sub
  ''' <summary>
  ''' Loads data from disk
  ''' </summary>
  ''' <param name="name">Patient name to load</param>
  ''' <param name="datte">Date of inspection</param>
  Friend Sub LoadDataUI(ByVal name As String, ByVal datte As String)
    ui.ResetScreen()
    ui.SetNameDate(name, datte)

    Dim datamng = New DataManager
    Dim xmlexporter = New XMLExporter
    datamng.LoadData(dataInput.Controls, xmlexporter.LoadData(name, datte))
  End Sub
  ''' <summary>
  ''' Exports data
  ''' </summary>
  Private Sub ExportWord(sender As Object, e As EventArgs) Handles export.Click
    Dim datamng = New DataManager
    transformer = New WordTransformer(False)
    Dim exporter = New WordExporter

    datamng.CollectData(dataInput.Controls)

    If Not transformer.ApplyRules(datamng.GetData) Then
      Exit Sub
    End If

    exporter.Open("bjk.docx")
    exporter.LoadData(transformer.GetContent)
    exporter.SaveAs(nev.Text + "_" + datum.Text + "_bjk.docx")
  End Sub
  ''' ''''''''''''''''''''''''''''''''''''''''''''''''''''' UI actions ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
  ''' <summary>
  ''' Initializes form
  ''' </summary>
  Friend Sub InitUI(sender As Object, e As EventArgs) Handles MyBase.Load, reset.Click
    ui.ResetScreen()
  End Sub
  ''' <summary>
  ''' Opens load dialog
  ''' </summary>
  Private Sub Loading(sender As Object, e As EventArgs) Handles loadButton.Click
    LoadForm.Show()
  End Sub
  ''' <summary>
  ''' Switch to the next tab
  ''' </summary>
  Private Sub NextTab(sender As Object, e As EventArgs) Handles Next1.Click, Next2.Click, Next3.Click
    ui.NextTab(sender)
  End Sub
  ''' <summary>
  ''' Reset a radio button set
  ''' </summary>
  Private Sub ResetRadio(sender As Object, e As EventArgs) Handles Button7.Click
    ui.ResetRadio(New Collection From {TryCast(sender, Button).Parent.Controls})
  End Sub
  ''' <summary>
  ''' UI action when a required field is missing
  ''' </summary>
  ''' <param name="fieldname">Missing field name</param>
  Private Sub FieldMissing(ByVal fieldname As String) Handles transformer.FieldMissing
    UI.Warning("Hiányzó adat: " + vbNewLine + vbNewLine + fieldname, AppName)
  End Sub
  ''' ''''''''''''''''''''''''''''''''''''''''''''''''''''' Enabling Controls '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableDecubitus(sender As Object, e As EventArgs) Handles decub_meret.TextChanged
    ui.EnableAssociatedControls(sender, New Collection From {decub_b, decub_j, decub_sac, decub_sark})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableAscites(sender As Object, e As EventArgs) Handles ascites.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {asc_l, asc_liter})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnablePacemaker(sender As Object, e As EventArgs) Handles pacemaker.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {pacemaker_serial, pacemaker_id})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableLagyulas(sender As Object, e As EventArgs) Handles agy_lagyulas.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {
                                agy_lagyulas_bal, agy_lagyulas_jobb, agy_lagyulas_front, agy_lagyulas_pariet,
                                agy_lagyulas_temp, agy_lagyulas_occ, agy_lagyulas_kisagy, agy_lagyulas_meret, agy_lagyulas_cm})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableVerzes(sender As Object, e As EventArgs) Handles agy_verzes.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {
                                agy_verzes_bal, agy_verzes_jobb, agy_verzes_front, agy_verzes_pariet,
                                agy_verzes_temp, agy_verzes_occ, agy_verzes_kisagy, agy_verzes_meret, agy_verzes_cm})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableAttet(sender As Object, e As EventArgs) Handles agy_attet.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {
                                agy_attet_bal, agy_attet_jobb, agy_attet_front, agy_attet_pariet, agy_attet_temp,
                                agy_attet_occ, agy_attet_kisagy, agy_attet_egy, agy_attet_tobb, agy_attet_meret, agy_attet_cm})
  End Sub
  ''' <summary>
  ''' Enables checkbox associated to this control
  ''' </summary>
  Private Sub EnableKoszoruSzuk(sender As Object, e As EventArgs) Handles koszoru_szuk.TextChanged
    ui.EnableAssociatedControls(sender, New Collection From {koszoru_szuk_jobb, koszoru_szuk_lad, koszoru_szuk_cx})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableStent(sender As Object, e As EventArgs) Handles koszoru_stent.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {koszoru_stent_jobb, koszoru_stent_lad, koszoru_stent_cx})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableThrombus(sender As Object, e As EventArgs) Handles koszoru_thromb.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {koszoru_thromb_jobb, koszoru_thromb_lad, koszoru_thromb_cx})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableInfRegi(sender As Object, e As EventArgs) Handles inf_regi.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {inf_regi_meret, inf_regi_cm, inf_regi_elul, inf_regi_hat, inf_regi_sept, inf_regi_oldal})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableInfUj(sender As Object, e As EventArgs) Handles inf_uj.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {inf_uj_meret, inf_uj_cm, inf_uj_elul, inf_uj_hat, inf_uj_sept, inf_uj_oldal})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableHaemo(sender As Object, e As EventArgs) Handles haemo.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {haemo_tomeg, haemo_g})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnablePneu(sender As Object, e As EventArgs) Handles pneu.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {pneu_ba, pneu_bf, pneu_ja, pneu_jf, pneu_jk, pneu_mko})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableTudoTumor(sender As Object, e As EventArgs) Handles tudo_tumor.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {tudo_tumor_ba, tudo_tumor_bf, tudo_tumor_ja, tudo_tumor_jf,
                                tudo_tumor_jk, tudo_tumor_meret, tudo_tumor_minden, tudo_tumor_mm})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableTudoAttet(sender As Object, e As EventArgs) Handles tudo_attet.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {tudo_attet_ba, tudo_attet_bf, tudo_attet_ja, tudo_attet_jf,
                                tudo_attet_jk, tudo_attet_meret, tudo_attet_minden, tudo_attet_mm})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableEmbolia(sender As Object, e As EventArgs) Handles tudo_emb.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {tudo_emb_bal, tudo_emb_elso, tudo_emb_jobb, tudo_emb_ket, tudo_emb_lovag, tudo_emb_tobb})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableHydro(sender As Object, e As EventArgs) Handles hydro.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {hydro_b, hydro_j, hydro_liter, hydro_menny, hydro_mko})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableMajAttet(sender As Object, e As EventArgs) Handles maj_attet.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {maj_attet_group, maj_attet_egy, maj_attet_meret, maj_attet_mm, maj_attet_tobb})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableEpeko(sender As Object, e As EventArgs) Handles epeko.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {epeko_darab, epeko_db, epeko_meret, epeko_mm})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableGyomorFekely(sender As Object, e As EventArgs) Handles gyomor_fekely.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {gyomor_fekely_group, gyomor_fekely_kis, gyomor_fekely_meret, gyomor_fekely_mm, gyomor_fekely_nagy})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableGyomorTumor(sender As Object, e As EventArgs) Handles gyomor_tumor.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {gyomor_tumor_group, gyomor_tumor_kis, gyomor_tumor_meret, gyomor_tumor_mm, gyomor_tumor_nagy})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableNyombelFekely(sender As Object, e As EventArgs) Handles nyombel_fekely.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {nyombel_fekely_meret, nyombel_fekely_mm})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableIleum(sender As Object, e As EventArgs) Handles ileum.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {ileum_cm, ileum_meret})
  End Sub
  ''' <summary>
  ''' Enables controls associated to this control
  ''' </summary>
  Private Sub EnableVastagbelTumor(sender As Object, e As EventArgs) Handles bel_tumor.CheckedChanged
    ui.EnableAssociatedControls(sender, New Collection From {bel_tumor_cm, bel_tumor_coec, bel_tumor_fel, bel_tumor_harant,
                                bel_tumor_le, bel_tumor_meret, bel_tumor_sig, bel_tumor_szuk, bel_tumor_vegbel})
  End Sub
End Class
