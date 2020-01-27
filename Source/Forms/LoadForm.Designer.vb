<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LoadForm
  Inherits System.Windows.Forms.Form

  'Form overrides dispose to clean up the component list.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Required by the Windows Form Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Windows Form Designer
  'It can be modified using the Windows Form Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LoadForm))
		Me.saved = New System.Windows.Forms.ListBox()
		Me.LoadButton = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'saved
		'
		Me.saved.FormattingEnabled = True
		Me.saved.Location = New System.Drawing.Point(9, 10)
		Me.saved.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
		Me.saved.Name = "saved"
		Me.saved.Size = New System.Drawing.Size(233, 121)
		Me.saved.TabIndex = 0
		'
		'LoadButton
		'
		Me.LoadButton.Location = New System.Drawing.Point(12, 141)
		Me.LoadButton.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
		Me.LoadButton.Name = "LoadButton"
		Me.LoadButton.Size = New System.Drawing.Size(228, 35)
		Me.LoadButton.TabIndex = 1
		Me.LoadButton.Text = "Betöltés"
		Me.LoadButton.UseVisualStyleBackColor = True
		'
		'LoadForm
		'
		Me.AcceptButton = Me.LoadButton
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(249, 180)
		Me.Controls.Add(Me.LoadButton)
		Me.Controls.Add(Me.saved)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "LoadForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "BoncJKV - Betöltés"
		Me.TopMost = True
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents saved As ListBox
	Friend WithEvents LoadButton As Button
End Class
