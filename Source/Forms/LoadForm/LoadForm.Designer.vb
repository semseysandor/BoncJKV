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
        Me.saved.ItemHeight = 16
        Me.saved.Location = New System.Drawing.Point(12, 12)
        Me.saved.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.saved.Name = "saved"
        Me.saved.Size = New System.Drawing.Size(297, 148)
        Me.saved.TabIndex = 0
        '
        'LoadButton
        '
        Me.LoadButton.Location = New System.Drawing.Point(16, 174)
        Me.LoadButton.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.LoadButton.Name = "LoadButton"
        Me.LoadButton.Size = New System.Drawing.Size(295, 43)
        Me.LoadButton.TabIndex = 1
        Me.LoadButton.Text = "Megnyitás"
        Me.LoadButton.UseVisualStyleBackColor = True
        '
        'LoadForm
        '
        Me.AcceptButton = Me.LoadButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(320, 222)
        Me.Controls.Add(Me.LoadButton)
        Me.Controls.Add(Me.saved)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "LoadForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "BoncJKV - Megnyitás"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents saved As ListBox
	Friend WithEvents LoadButton As Button
End Class
