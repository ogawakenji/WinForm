Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Text
Imports System.Windows.Forms
Imports System.Globalization
Imports System.Threading
Imports System.Runtime.InteropServices

'' UltraDateTimePicker
'' https://social.msdn.microsoft.com/Forums/windows/en-US/ee0c5fbe-8d18-4899-8df2-46535b570bb4/how-to-create-a-nullable-and-editable-datetimepicker?forum=winforms



Public Class BaseDateTimePicker
    Inherits DateTimePicker
    Implements ISupportInitialize

    Private _Format As DateTimePickerFormat
    Private _CustomFormat As String
    Private _readOnly As Boolean = False
    Private _visible As Boolean = True
    Private _tabStopWhenReadOnly As Boolean = False
    Private _textBox As TextBox
    Private initializing As Boolean = True
    Private _isNull As Boolean = False
    Public Property NullText As String


    Public Sub New()
        initTextBox()
        Me.Format = DateTimePickerFormat.Custom
        _Format = DateTimePickerFormat.Long
        If DesignMode Then
            setFormat()
        End If
    End Sub

    Public Sub BeginInit() Implements ISupportInitialize.BeginInit
        initializing = True
    End Sub

    Public Sub EndInit() Implements ISupportInitialize.EndInit
        Me.Value = DateTime.Today
        initializing = False
        If DesignMode Then
            Return
        End If

    End Sub

    Private Sub initTextBox()
        If DesignMode Then
            Return
        End If

        _textBox = New TextBox()
        _textBox.ReadOnly = True
        _textBox.Location = Me.Location
        _textBox.Size = Me.Size
        _textBox.Dock = Me.Dock
        _textBox.Anchor = Me.Anchor
        _textBox.RightToLeft = Me.RightToLeft
        _textBox.Font = Me.Font
        _textBox.TabStop = Me.TabStop
        _textBox.TabIndex = Me.TabIndex
        _textBox.Visible = Me.Visible
        _textBox.Parent = Me.Parent



    End Sub

    Private Sub setFormat()
        Me.CustomFormat = Nothing
        If _isNull Then
            Me.CustomFormat = String.Concat("'", _NullText, "'")
        Else
            Dim cultureInfo = Thread.CurrentThread.CurrentCulture
            Dim dTFormatInfo As DateTimeFormatInfo = cultureInfo.DateTimeFormat

            Select Case _Format
                Case DateTimePickerFormat.Long
                    Me.CustomFormat = dTFormatInfo.LongDatePattern

                Case DateTimePickerFormat.Short
                    Me.CustomFormat = dTFormatInfo.ShortDatePattern

                Case DateTimePickerFormat.Time
                    Me.CustomFormat = dTFormatInfo.FullDateTimePattern

                Case DateTimePickerFormat.Custom
                    Me.CustomFormat = Me._CustomFormat

            End Select

        End If

    End Sub

End Class
