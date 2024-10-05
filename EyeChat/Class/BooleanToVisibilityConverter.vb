Imports System.Globalization

Public Class BooleanToVisibilityConverter
    Implements IValueConverter





    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim boolValue As Boolean = False
        If Boolean.TryParse(value.ToString(), boolValue) Then
            Return If(boolValue, Visibility.Visible, Visibility.Collapsed)
        End If
        Return Visibility.Collapsed
    End Function

    'Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
    'Dim boolValue As Boolean = False
    'If Boolean.TryParse(value.ToString(), boolValue) Then
    'If boolValue = False Then
    'Return Visibility.Hidden
    'Else
    'Return Visibility.Hidden
    'End If
    'End If
    'Return Visibility.Collapsed
    'End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class