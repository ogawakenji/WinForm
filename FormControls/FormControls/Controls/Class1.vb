Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Globalization
Imports System.Threading

'' summary
'' The following code is written from scratch, but was inspired by
'' the NullableDateTimePicker by Claudio Grazioli, http://www.grazioli.ch
''
'' This Extended DateTimePicker adds useful functionality to the DateTimePicker class espicially for Database Apllications
'' 1. I added the abilty to set the DTP to null with the following features
'' a. NullText Property that can be used to display Text of your choise when DTP is null
'' b. Ability to type dates or times into the DTP when set to null
'' c. BackSpace and Delete keystrokes set the DTP to null
'' 2. I added a ReadOnly Mode in which the UDTP hides itself and decorates a ReadOnly TextBox in its place
'' a. By Decorating a TextBox we get all black easily readable text.
'' b. The ReadOnly TextBox has automatic Clipboard access built in which is useful.
'' c. I added a TabStopWhenReadOnly Property that allows the ReadOnly Mode to mimic the UDTP Tabstop property or not;
'' If TabStopWhenReadOnly is true then the ReadOnly Mode's TabStop will be whatever the UDTP Tabstop Property is.
'' If TabStopWhenReadOnly is false then the the ReadOnly Mode's TabStop is false.
'' /summary

Partial Public Class UltraDateTimePicker
    Inherits DateTimePicker
    Implements ISupportInitialize

#Region "Member Variables Added To Allow Null Values"

    'Format and CustomForamt are shadowed since base.Format is always Custom
    'and base.CustomFormat is used in setFormat to show the intended _Format
    'You have to keep base.Format set to Custom to avoid superfluous ValueChanged
    'events from occuring.

    Private _Format As DateTimePickerFormat
    ' Variable to store 'Format'
    Private _CustomFormat As String
    'Variable to store 'CustomFormat'

    Private _nullText As String = ""
    'Variable to store null Display Text

#End Region

#Region "Member Variables Added To Enable ReadOnly Mode"

    Private _readOnly As Boolean = True ''''''''''''''''''''
    'Flag to denote the UDTP is in ReadOnly Mode
    Private _visible As Boolean = True
    'Overridden to show the proper Display for Readonly Mode
    Private _tabStopWhenReadOnly As Boolean = False
    'Variable to store whether or not the UDTP is a TabStop when in ReadOnly Mode
    Private _textBox As TextBox
    'TextBox Decorated when in ReadOnly Mode

#End Region

#Region "Constructor"

    '' summary
    '' Basic Constructer + ReadOnly _textBox initialization
    '' /summary
    Public Sub New()

        'InitializeComponent()

        initTextBox()

        MyBase.Format = DateTimePickerFormat.[Custom]

        _Format = DateTimePickerFormat.Short

        If DesignMode Then

            setFormat()

        End If
    End Sub

#End Region

#Region "ISupportInitialize Members"

    Private initializing As Boolean = True

    Public Sub BeginInit() Implements ISupportInitialize.BeginInit

        ' TODO: Add UserControl1.BeginInit implementation

        initializing = True
    End Sub

    Public Sub EndInit() Implements ISupportInitialize.EndInit

        MyBase.Value = DateTime.Today
        'Default the value to Today(makes me happy, but not necessary)
        initializing = False

        If DesignMode Then
            Exit Sub

        End If

        If Me.Parent.[GetType]() Is GetType(TableLayoutPanel) Then

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

        'I use the following block of code to walk up the parent-child
        'chain and find the first member that has a Load event that I can attach to
        'I set the visiblilty during this event so that Databinding will work correctly
        'otherwise the UDTP will fail to bind properly if its visibility is false during the
        'Load event.(Strange but true, has to do with hidden controls not binding for performance reasons)

        Dim parent As Control = Me

        Dim foundLoadingParent As Boolean = False

        Do

            'MessageBox.Show(parent.Name)

            parent = parent.Parent

            If parent.[GetType]().IsSubclassOf(GetType(UserControl)) Then

                AddHandler DirectCast(parent, UserControl).Load, AddressOf UltraDateTimePicker_Load

                foundLoadingParent = True

            ElseIf parent.[GetType]().IsSubclassOf(GetType(Form)) Then

                AddHandler DirectCast(parent, Form).Load, AddressOf UltraDateTimePicker_Load

                foundLoadingParent = True

            End If

        Loop While Not foundLoadingParent
    End Sub

    Private Sub UltraDateTimePicker_Load(ByVal sender As Object, ByVal e As EventArgs)

        setVisibility()
        'MessageBox.Show(_readOnly)
    End Sub

#End Region

#Region "Public Properties Modified/Added To Allow Null Values"

    Private _isNull As Boolean = False

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

    '' summary
    '' NullText property is used to access/change the Text shown when the UDTP is null
    '' /summary

#Region "DesignerModifiers"

#End Region

    <Browsable(True)>
    <Category("Behavior")>
    <Description("Text shown when DateTime Is 'null'")>
    <DefaultValue("")>
    Public Property NullText() As String

Get
    Return _nullText
    End Get

    Set(ByVal value As String)
_nullText = value
End Set
    End Property

    '' summary
    '' Modified Format Property stores the assigned Format and the propagates the change to base.CustomFormat
    '' /summary

#Region "DesignerModifiers"

#End Region

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
        'Resets the CustomFormat(bookkeeping)
        If _isNull Then
            'If null apply NullText to the UDTP

            MyBase.CustomFormat = [String].Concat("'", Me.NullText, "'")
        Else

'The Following is used to get a string representation ot the current UDTP Format

'And then set the CustomFormat to match the intended format

Dim cultureInfo As CultureInfo = Thread.CurrentThread.CurrentCulture

            Dim dTFormatInfo As DateTimeFormatInfo = cultureInfo.DateTimeFormat

            Select Case _Format

                Case DateTimePickerFormat.[Long]

                    MyBase.CustomFormat = dTFormatInfo.LongDatePattern

                    Exit Select

                Case DateTimePickerFormat.[Short]

                    MyBase.CustomFormat = dTFormatInfo.ShortDatePattern

                    Exit Select

                Case DateTimePickerFormat.Time

                    MyBase.CustomFormat = dTFormatInfo.ShortTimePattern

                    Exit Select

                Case DateTimePickerFormat.[Custom]

                    MyBase.CustomFormat = Me._CustomFormat

                    Exit Select

            End Select

        End If
    End Sub

    '' summary
    '' Modified CustomFormat Property stores the assigned CustomFormat when null, otherwise functions as normal
    '' /summary

    Public Shadows Property CustomFormat() As String

        Get
            Return _CustomFormat
        End Get

        Set(ByVal value As String)

            Me._CustomFormat = value

            Me.setFormat()
        End Set
    End Property

#End Region

#Region "Public Properties Modified/Added To Enable ReadOnly Mode"

    '' summary

    '' Sets the ReadOnly Property and then call the appropriate Display Function

    '' If in Design Mode then return(If we dont do this then the Control could Disappear )

    '' /summary

#Region "DesignerModifiers"

#End Region

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

    '' summary
    '' This useful property allows you to say wheher or not you want
    '' the TexBox to Mimic the DTP's TabStop value when in ReadOnly Mode.
    '' I personally found this useful on Data entry forms. The default is false,
    '' which means when in ReadOnly Mode you cannot tab into it so Tab will skip
    '' ReadOnly Pickers.
    '' /summary

#Region "DesignerModifiers"

#End Region

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

    '' summary
    '' Modified the TabStop Property to support the added TabStopWhenReadOnly property
    '' /summary

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

#End Region

#Region "OnXXXX() Modified To Allow Null Values"

    '' summary
    '' Used to change the UDTP.Value on Closeup(Without this code Closeup only changes the base.Value)
    '' /summary
    '' param name=e/param

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

#End Region

#Region "OnXXXX() Modified To Enable ReadOnly Mode"

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

#End Region

#Region "Private Methods Added To Enable ReadOnly Mode"

    '' summary
    '' Added to initialize the _textBox to the default values to match the DTP
    '' /summary

    Private Sub initTextBox()

        If DesignMode Then
            Exit Sub

        End If

        _textBox = New TextBox()

        _textBox.[ReadOnly] = True

        _textBox.Location = Me.Location

        _textBox.Size = Me.Size

        _textBox.Dock = Me.Dock

        _textBox.Anchor = Me.Anchor

        _textBox.RightToLeft = Me.RightToLeft

        _textBox.Font = Me.Font

        _textBox.TabStop = Me.TabStop

        _textBox.TabIndex = Me.TabIndex

        _textBox.Visible = False

        _textBox.Parent = Me.Parent

    End Sub

    Private Sub setVisibility()

        If DesignMode OrElse initializing Then
            Exit Sub
            'Dont actually change the visibility if in Design Mode
        End If

        If Me._visible Then

            If Me._readOnly Then

                showTextBox()
            Else
                'If Visible and Readonly Show TextBox

                showDTP()
                'If Visible and NOT ReadOnly Show DateTimePicker
            End If
        Else

            'If Not Visible Show Neither
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
            'If UTDP.Parent == null, set _textBox.Parent == null and return

            _textBox.Parent = Nothing

            Exit Sub
        End If

        If Not _textBox.Parent.Equals(Me.Parent) Then
            'If the Parents DO NOT already match

            'I Added Special logic here to handle positioning the _TextBox when the UDTP is in a TableLayoutPanel

            If Me.Parent.[GetType]() Is GetType(TableLayoutPanel) Then

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

#End Region

#Region "Public Methods Overriden To Enable ReadOnly Mode"

    Public Shadows Sub Show()

        Me.Visible = True
    End Sub

    Public Shadows Sub Hide()

        Me.Visible = False
    End Sub

#End Region

    'Public Sub InitializeComponent()
    ' Me.SuspendLayout()

    ' Me.ResumeLayout(False)
    'End Sub

End Class