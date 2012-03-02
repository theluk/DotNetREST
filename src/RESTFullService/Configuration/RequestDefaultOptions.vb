Imports System.Configuration
Imports System.Web.Configuration

Public Class RequestDefaultOptions
    Inherits ConfigurationElementCollection

    Protected Overloads Overrides Function CreateNewElement() As System.Configuration.ConfigurationElement
        Return New RequestDefaultOptionElement
    End Function

    Protected Overrides Function GetElementKey(ByVal element As System.Configuration.ConfigurationElement) As Object
        Return CType(element, RequestDefaultOptionElement).name
    End Function

    Friend Function GetDictionary() As IDictionary(Of String, Object)
        Dim dict As New Dictionary(Of String, Object)
        For Each _item In Me.BaseGetAllKeys
            dict(_item.ToString) = CType(Me.BaseGet(_item), RequestDefaultOptionElement).Value
        Next
        Return dict
    End Function

End Class

Public Class RequestDefaultOptionElement
    Inherits ConfigurationElement

    <ConfigurationProperty("Name", IsKey:=True, IsRequired:=True)> _
    ReadOnly Property Name As String
        Get
            Return Me("Name")
        End Get
    End Property

    <ConfigurationProperty("Value", IsRequired:=True)> _
    ReadOnly Property Value As String
        Get
            Return Me("Value")
        End Get
    End Property

End Class