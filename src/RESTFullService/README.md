Simple ASP.NET RESTFull Service
====

Setup 
=====

web.config
----------

    <configSections>
		<section name="RESTFullService" 
				 type="RESTFullService.Configuration.RESTFullService, RESTFullService"/>
    </configSections>

    <RESTFullService>
      <RESTHandlers>
        <add Name="MyModel" Type="MyProj.MyModelHandler, MyProj"/>
      </RESTHandlers>
    </RESTFullService>

