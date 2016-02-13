<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
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

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.UltraDateTimePicker1 = New FormControls.UltraDateTimePicker()
        CType(Me.UltraDateTimePicker1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UltraDateTimePicker1
        '
        Me.UltraDateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.UltraDateTimePicker1.Location = New System.Drawing.Point(12, 12)
        Me.UltraDateTimePicker1.Name = "UltraDateTimePicker1"
        Me.UltraDateTimePicker1.ReadOnly = True
        Me.UltraDateTimePicker1.Size = New System.Drawing.Size(146, 22)
        Me.UltraDateTimePicker1.TabIndex = 0
        Me.UltraDateTimePicker1.TabstopWhenReadOnly = True
        Me.UltraDateTimePicker1.Value = New Date(2016, 2, 14, 0, 0, 0, 0)
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(282, 255)
        Me.Controls.Add(Me.UltraDateTimePicker1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.UltraDateTimePicker1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents UltraDateTimePicker1 As UltraDateTimePicker
End Class
