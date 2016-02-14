Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim htmlUtil As New HtmlUtil
        Dim html As String = htmlUtil.GetHtml("http://stocks.finance.yahoo.co.jp/stocks/detail/?code=6178.t")
        Dim lastpos As Integer = html.IndexOf("</html>")
        If lastpos <> -1 Then
            html = html.Substring(0, lastpos + 7)
        End If

        Dim xdoc As XDocument = htmlUtil.ParseXml(html)
        Dim price = From q In xdoc.Descendants("td")
                    Where q.Attribute("class").Value = "stoksPrice"
                    Select q
        For Each r In price
            Me.Label1.Text = price.Value
            Exit For
        Next

        Dim ratio = From q In xdoc.Descendants("td")
                    Where q.Attribute("class").Value = "change"
                    Select q
        For Each r In ratio
            Me.Label2.Text = ratio.Value
            Exit For
        Next
    End Sub
End Class
