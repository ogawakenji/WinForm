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



Public Class BaseDateTimePicker
    Inherits DateTimePicker
    Implements ISupportInitialize

    Private _Format As DateTimePickerFormat
    Private _CustomFormat As String
    Private _nullText As String = ""




    Public Sub BeginInit() Implements ISupportInitialize.BeginInit
        Throw New NotImplementedException()
    End Sub

    Public Sub EndInit() Implements ISupportInitialize.EndInit
        Throw New NotImplementedException()
    End Sub
End Class
