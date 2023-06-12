Imports System.Globalization

Public Class ContainsStringToVisibilityConverter
    Implements IValueConverter

    Public Property TargetString As String

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim stringValue As String = TryCast(value, String)

        If Not String.IsNullOrEmpty(stringValue) AndAlso Not String.IsNullOrEmpty(TargetString) Then
            If stringValue.ToLower().Contains(TargetString.ToLower()) Then
                Return Visibility.Visible
            End If
        End If

        Return Visibility.Collapsed
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function
End Class
