Imports System.Web.Configuration
Imports System.Configuration
Imports System.Web

Namespace Configuration

    Public Class RESTFullService
        Inherits ConfigurationSection

        Private _config As Global.System.Configuration.Configuration
        Private Shared _current As RESTFullService
        Private Shared _lockObject As New Object

        Public Shared ReadOnly Property Current As RESTFullService
            Get
                If _current Is Nothing Then
                    SyncLock (_lockObject)
                        If _current Is Nothing Then
                            Dim _cfg As Global.System.Configuration.Configuration = Nothing

                            Dim context As HttpContext = HttpContext.Current
                            If context Is Nothing Then
                                _cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
                            Else
                                _cfg = WebConfigurationManager.OpenWebConfiguration(context.Request.ApplicationPath)
                            End If
                            _current = CType(_cfg.GetSection("RESTFullService"), RESTFullService)
                            _current._config = _cfg
                        End If
                    End SyncLock
                End If
                Return _current
            End Get
        End Property

        <ConfigurationProperty("RequestDefaultOptions")> _
        ReadOnly Property RequestDefaultOptions As RequestDefaultOptions
            Get
                Return Me("RequestDefaultOptions")
            End Get
        End Property

        <ConfigurationProperty("RESTHandlers")> _
        ReadOnly Property RESTHandlers As RESTHandlerCollection
            Get
                Return Me("RESTHandlers")
            End Get
        End Property

    End Class

End Namespace