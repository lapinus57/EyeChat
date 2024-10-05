Imports System.Globalization

Public Class StringToImageSourceConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is String Then
            Dim stringValue As String = CType(value, String)
            Dim image As New BitmapImage()
            image.BeginInit()
            image.UriSource = New Uri(stringValue, UriKind.RelativeOrAbsolute)
            image.EndInit()
            Return image
        End If
        Return Nothing
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
