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
Imports System.Diagnostics

'' UltraDateTimePicker
'' https://social.msdn.microsoft.com/Forums/windows/en-US/ee0c5fbe-8d18-4899-8df2-46535b570bb4/how-to-create-a-nullable-and-editable-datetimepicker?forum=winforms
'' http://www.hivmr.com/db/cfjz8f9p9mj13mj9czjz1jscccp9cjpj


Public Class BaseDateTimePicker
    Inherits DateTimePicker
    Implements ISupportInitialize

    Private _Format As DateTimePickerFormat

    Private _CustomFormat As String

    Private _nullText As String = ""

    Private _readOnly As Boolean = False

    Private _visible As Boolean = True

    Private _tabStopWhenReadOnly As Boolean = False

    Private _textBox As TextBox

    Private initializing As Boolean = True

    Private _isNull As Boolean = False


    Public Sub New()
        initTextBox()
        MyBase.Format = DateTimePickerFormat.Custom
        _Format = DateTimePickerFormat.Short
        If DesignMode Then
            setFormat()
        End If
    End Sub

    Public Sub BeginInit() Implements ISupportInitialize.BeginInit
        initializing = True
    End Sub

    Public Sub EndInit() Implements ISupportInitialize.EndInit
        MyBase.Value = DateTime.Today
        initializing = False
        If DesignMode Then
            Return
        End If
        If Me.Parent.GetType = GetType(TableLayoutPanel) Then
            Dim cP As TableLayoutPanelCellPosition = DirectCast(Me.Parent, TableLayoutPanel).GetPositionFromControl(Me)
            DirectCast(Me.Parent, TableLayoutPanel).Controls.Add(_textBox, cP.Column, cP.Row)
            DirectCast(Me.Parent, TableLayoutPanel).SetColumnSpan(_textBox, DirectCast(Me.Parent, TableLayoutPanel).GetColumnSpan(Me))
            _textBox.Anchor = Me.Anchor
        ElseIf Me.Parent.GetType = GetType(FlowLayoutPanel) Then
            DirectCast(Me.Parent, FlowLayoutPanel).Controls.Add(_textBox)
            DirectCast(Me.Parent, FlowLayoutPanel).Controls.SetChildIndex(_textBox, DirectCast(Me.Parent, FlowLayoutPanel).Controls.IndexOf(Me))
            _textBox.Anchor = Me.Anchor
        Else
            _textBox.Parent = Me.Parent
            _textBox.Anchor = Me.Anchor
        End If

        Dim parent As Control = Me
        Dim foundLoadingParent As Boolean = False
        Do
            parent = parent.Parent
            If parent.GetType().IsSubclassOf(GetType(UserControl)) Then
                AddHandler DirectCast(parent, UserControl).Load, AddressOf BaseDateTimePicker_Load
                foundLoadingParent = True
            ElseIf parent.GetType().IsSubclassOf(GetType(Form))
                AddHandler DirectCast(parent, Form).Load, AddressOf BaseDateTimePicker_Load
                foundLoadingParent = True
            End If
        Loop While (Not foundLoadingParent)



    End Sub

    Private Sub BaseDateTimePicker_Load(sender As Object, e As EventArgs)
        setVisibility()
    End Sub

    '' summary
    '' Modified Value Propety now of type Object and uses MinDate to mark null values
    '' /summary

    Public Shadows Property Value() As [Object]

        Get

            'if (this.MinDate == base.Value) //Check to see if set to MinDate(null), return null or base.Value accordingly

            If _isNull Then

                Return Nothing
            Else

                Return MyBase.Value

            End If
        End Get

        Set(ByVal value As [Object])

            If value Is Nothing OrElse IsDBNull(value) Then
                'Check for null assignment

                If Not _isNull Then
                    'If not already null set to null and fire event

                    _isNull = True

                    Me.OnValueChanged(EventArgs.Empty)

                End If
            Else

                'Value is nto null

                If _isNull AndAlso MyBase.Value = DirectCast(value, DateTime) Then
                    'if null and value matches base.value take out of null and fire event
                    '(null-value needs a value changed even though base.Value did not change)

                    _isNull = False

                    Me.OnValueChanged(EventArgs.Empty)
                Else

                    'change to the new value(changed event fires from base class

                    _isNull = False

                    MyBase.Value = DirectCast(value, DateTime)

                End If
            End If

            setFormat()
            'refresh format

            _textBox.Text = Me.Text
        End Set
    End Property

    <Browsable(True)>
    <Category("Behavior")>
    <Description("Text shown When DateTime Is 'null'")>
    <DefaultValue("")>
    Public Property NullText() As String

        Get
            Return _nullText
        End Get

        Set(ByVal value As String)
            _nullText = value
        End Set
    End Property


    <Browsable(True)>
    <DefaultValue(DateTimePickerFormat.[Long]), TypeConverter(GetType([Enum]))>
    Public Shadows Property Format() As DateTimePickerFormat

        Get
            Return Me._Format
        End Get

        Set(ByVal value As DateTimePickerFormat)

            Me._Format = value

            Me.setFormat()
        End Set
    End Property

    Private Sub setFormat()
        MyBase.CustomFormat = Nothing
        If _isNull Then
            MyBase.CustomFormat = String.Concat("'", Me._nullText, "'")
        Else
            Dim cultureInfo = Thread.CurrentThread.CurrentCulture
            Dim dTFormatInfo As DateTimeFormatInfo = cultureInfo.DateTimeFormat

            Select Case _Format
                Case DateTimePickerFormat.Long
                    MyBase.CustomFormat = dTFormatInfo.LongDatePattern

                Case DateTimePickerFormat.Short
                    MyBase.CustomFormat = dTFormatInfo.ShortDatePattern

                Case DateTimePickerFormat.Time
                    MyBase.CustomFormat = dTFormatInfo.FullDateTimePattern

                Case DateTimePickerFormat.Custom
                    MyBase.CustomFormat = Me._CustomFormat

            End Select

        End If

    End Sub

    Public Shadows Property CustomFormat() As String

        Get
            Return _CustomFormat
        End Get

        Set(ByVal value As String)

            Me._CustomFormat = value

            Me.setFormat()
        End Set
    End Property

    <Browsable(True)>
    <Category("Behavior")>
    <Description("Displays Control As ReadOnly(Black On Gray) if 'true'")>
    <DefaultValue(False)>
    Public Property [ReadOnly]() As Boolean

        Get
            Return _readOnly
        End Get

        Set(ByVal value As Boolean)

            Me._readOnly = value

            setVisibility()
        End Set
    End Property

    <Category("Behavior")>
    <DefaultValue(False)>
    <Browsable(True)>
    <EditorBrowsable(EditorBrowsableState.Always)>
    Public Property TabstopWhenReadOnly() As Boolean

        Get
            Return _tabStopWhenReadOnly
        End Get

        Set(ByVal value As Boolean)

            _tabStopWhenReadOnly = value

            'TextBox is a Tabstop only if mimicing and DTP is a TabStop
            _textBox.TabStop = (_tabStopWhenReadOnly AndAlso Me.TabStop)
        End Set
    End Property

    Public Shadows Property TabStop() As Boolean

        Get
            Return MyBase.TabStop
        End Get

        Set(ByVal value As Boolean)

            MyBase.TabStop = value

            _textBox.TabStop = (_tabStopWhenReadOnly AndAlso MyBase.TabStop)
        End Set
    End Property

    '' summary
    '' Sets the Visible Property and then call the appropriate Display Function
    '' If in Design Mode then return(If we dont do this then the Control could Disappear )
    '' /summary

    Public Shadows Property Visible() As Boolean

        Get
            Return _visible
        End Get

        Set(ByVal value As Boolean)

            _visible = value

            setVisibility()
        End Set
    End Property

    Protected Overloads Overrides Sub WndProc(ByRef m As Message)

        If m.Msg = &H4E Then

            Dim nm As NMHDR = DirectCast(m.GetLParam(GetType(NMHDR)), NMHDR)

            If nm.Code = -746 OrElse nm.Code = -722 Then

                Me.Value = MyBase.Value
                'propagate change form base to UTDP
            End If
        End If

        MyBase.WndProc(m)
    End Sub

    <StructLayout(LayoutKind.Sequential)>
    Private Structure NMHDR

        Public HwndFrom As IntPtr

        Public IdFrom As Integer

        Public Code As Integer

    End Structure

    ''summary
    ''Sets UDTP Value to null when Delete or Backspace is pressed
    ''/summary
    ''param name=e/param

    Protected Overloads Overrides Sub OnKeyUp(ByVal e As KeyEventArgs)

        If e.KeyCode = Keys.Delete OrElse e.KeyCode = Keys.Back Then

            Me.Value = Nothing
        End If

        MyBase.OnKeyUp(e)
    End Sub

    '' summary
    '' When Null and a Number is pressed this method takes the UDTP out of Null Mode
    '' and resends the pressed key for timign reasons
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnKeyPress(ByVal e As KeyPressEventArgs)

        MyBase.OnKeyPress(e)

        If _isNull AndAlso [Char].IsDigit(e.KeyChar) Then

            Me.Value = MyBase.Value

            e.Handled = True

            SendKeys.Send("{Right}")

            SendKeys.Send(e.KeyChar.ToString())
        Else

            MyBase.OnKeyPress(e)

        End If
    End Sub


    '' summary
    '' Refreshes the Visibility of the Control if the Parent Changes(So the _TextBox get moved and Redrawn)
    '' If in Design Mode then return(If we dont do this then the Control could Disappear )
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnParentChanged(ByVal e As EventArgs)

        MyBase.OnParentChanged(e)

        If DesignMode OrElse initializing Then
            Exit Sub

        End If

        updateReadOnlyTextBoxParent()
        'update the _TextBox parent
        'Reset Visibilty for new parent
        setVisibility()
    End Sub

    '' summary
    '' Propagates Location to the _TextBox
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnLocationChanged(ByVal e As EventArgs)

        MyBase.OnLocationChanged(e)

        _textBox.Location = Me.Location

    End Sub

    '' summary
    '' Propagates Size to the _TextBox
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnSizeChanged(ByVal e As EventArgs)

        MyBase.OnSizeChanged(e)

        _textBox.Size = Me.Size
    End Sub

    '' summary
    '' Propagates Size to the _TextBox
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnResize(ByVal e As EventArgs)

        MyBase.OnResize(e)

        _textBox.Size = Me.Size
    End Sub

    '' summary
    '' Propagates Dock to the _TextBox
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnDockChanged(ByVal e As EventArgs)

        MyBase.OnDockChanged(e)

        _textBox.Dock = Me.Dock
    End Sub

    '' summary
    '' Propagates RightToLeft to the _TextBox
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnRightToLeftChanged(ByVal e As EventArgs)

        MyBase.OnRightToLeftChanged(e)

        _textBox.RightToLeft = Me.RightToLeft
    End Sub

    '' summary
    '' Propagates TabStop to the _TextBox if TabStopWhenReadOnly == true
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnTabStopChanged(ByVal e As EventArgs)

        MyBase.OnTabStopChanged(e)

        _textBox.TabStop = _tabStopWhenReadOnly AndAlso Me.TabStop
    End Sub

    '' summary
    '' Propagates TabIndex to the _TextBox
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnTabIndexChanged(ByVal e As EventArgs)

        MyBase.OnTabIndexChanged(e)

        _textBox.TabIndex = Me.TabIndex
    End Sub

    ''summary
    '' Propagates Font to the _TextBox
    '' /summary
    '' param name=e/param

    Protected Overloads Overrides Sub OnFontChanged(ByVal e As EventArgs)

        MyBase.OnFontChanged(e)

        _textBox.Font = Me.Font
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
    Private Sub setVisibility()
        If DesignMode OrElse initializing Then
            Return
        End If

        If Me._visible Then
            If Me._readOnly Then
                showTextBox()
            Else
                showDTP()
            End If
        Else
            showNone()
        End If

    End Sub

    Private Sub showTextBox()
        MyBase.Visible = False
        _textBox.Visible = True
        _textBox.TabStop = _tabStopWhenReadOnly AndAlso Me.TabStop

    End Sub

    Private Sub showDTP()
        MyBase.Visible = True
        _textBox.Visible = False
    End Sub

    Private Sub showNone()
        _textBox.Visible = False
        MyBase.Visible = False
    End Sub

    Private Sub updateReadOnlyTextBoxParent()

        If Me.Parent Is Nothing Then
            _textBox.Parent = Nothing
            Return
        End If

        If Not _textBox.Parent.Equals(Me.Parent) Then
            If Me.Parent.GetType() Is GetType(TableLayoutPanel) Then

                Dim cP As TableLayoutPanelCellPosition = DirectCast(Me.Parent, TableLayoutPanel).GetPositionFromControl(Me)

                DirectCast(Me.Parent, TableLayoutPanel).Controls.Add(_textBox, cP.Column, cP.Row)

                DirectCast(Me.Parent, TableLayoutPanel).SetColumnSpan(_textBox, DirectCast(Me.Parent, TableLayoutPanel).GetColumnSpan(Me))

                _textBox.Anchor = Me.Anchor

                'I added special logic here to handle positioning the _TextBox when the UDTP is in a FlowLayoutPanel

            ElseIf Me.Parent.[GetType]() Is GetType(FlowLayoutPanel) Then

                DirectCast(Me.Parent, FlowLayoutPanel).Controls.Add(_textBox)

                DirectCast(Me.Parent, FlowLayoutPanel).Controls.SetChildIndex(_textBox, DirectCast(Me.Parent, FlowLayoutPanel).Controls.IndexOf(Me))

                _textBox.Anchor = Me.Anchor
            Else

                'not a TableLayoutPanel or FlowLayoutPanel so just assign the parent

                _textBox.Parent = Me.Parent

                _textBox.Anchor = Me.Anchor

            End If

        End If
    End Sub

    Public Shadows Sub Show()
        Me.Visible = True
    End Sub

    Public Shadows Sub Hide()
        Me.Visible = False
    End Sub


End Class
