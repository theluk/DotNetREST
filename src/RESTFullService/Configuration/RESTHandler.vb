Imports System.Configuration

Public Class RESTHandlerCollection
    Inherits ConfigurationElementCollection

    Protected Overloads Overrides Function CreateNewElement() As System.Configuration.ConfigurationElement
        Return New RESTHandlerElement
    End Function

    Protected Overrides Function GetElementKey(ByVal element As System.Configuration.ConfigurationElement) As Object
        Return CType(element, RESTHandlerElement).Name
    End Function

    Friend Function GetDictionary() As IDictionary(Of String, Type)
        Dim dict As New Dictionary(Of String, Type)
        For Each _item In Me.BaseGetAllKeys
            dict(_item.ToString) = CType(Me.BaseGet(_item), RESTHandlerElement).ReadType
        Next
        Return dict
    End Function

End Class

Public Class RESTHandlerElement
    Inherits ConfigurationElement

    <ConfigurationProperty("Name", IsKey:=True, IsRequired:=True)> _
    ReadOnly Property Name As String
        Get
            Return Me("Name")
        End Get
    End Property

    <ConfigurationProperty("Type", IsRequired:=True)> _
    ReadOnly Property Type As String
        Get
            Return Me("Type")
        End Get
    End Property

    Function ReadType() As Type
        Return System.Type.GetType(Me.Type)
    End Function


End Class