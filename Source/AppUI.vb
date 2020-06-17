''' <summary>
''' UI handling
''' </summary>
Public Class AppUI
  Inherits CoreUI

  ''' <summary>
  ''' Resets screen
  ''' </summary>
  Public Sub ResetScreen()
    ResetControls(App.dataInput.Controls)

    ComponentManager.Main.dataInput.SelectedIndex = 0

    ComponentManager.Main.datum.Text = Now.ToString("yyyy-MM-dd")

    ComponentManager.Main.nev.ResetText()
    ComponentManager.Main.nev.Select()
    ComponentManager.Main.nev.Select()
  End Sub

  ''' <summary>
  ''' Switch to the next tab
  ''' </summary>
  Public Sub NextTab(sender As Object)
    ComponentManager.Main.dataInput.SelectedIndex = TryCast(sender, Button).Parent.TabIndex + 1
  End Sub

  ''' <summary>
  ''' Reset controls on this tab
  ''' </summary>
  Public Sub IcterusChange(sender As Object)
    If TryCast(sender, CheckBox).Checked Then

      ComponentManager.Main.BackColor = Color.Yellow

      For Each tab As TabPage In ComponentManager.Main.dataInput.TabPages
        tab.BackColor = Color.Yellow
      Next

    Else
      ComponentManager.Main.BackColor = Control.DefaultBackColor

      For Each tab As TabPage In ComponentManager.Main.dataInput.TabPages
        tab.BackColor = Control.DefaultBackColor
      Next

    End If
  End Sub

  ''' <summary>
  ''' Sets name and date on the main form
  ''' </summary>
  ''' <param name="name">Name of patient</param>
  ''' <param name="datte">Date of inspection</param>
  Public Sub SetNameDate(ByVal name As String, ByVal datte As String)
    ComponentManager.Main.nev.Text = name
    ComponentManager.Main.datum.Text = datte
  End Sub
End Class