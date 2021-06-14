# $Id: ReadMe.txt 333 2011-12-28 10:19:07Z onuchin $

# Author: Valeriy Onuchin   29.12.2010


Two programs DoctorDisplay.exe, PatientDisplay.exe used for several purposes:
- provide video, audio communication between patient and physician(doctor)
- control patient position in "kreslo" during the proton therapy treatment: 
  o patient position is fixed before tomography scan. The reference image of 
    the patient is taken and saved to database.
  o patinet must be positioned before treatment at the same location as 
    at tomography scan. The difference between the current position and 
   reference one is displayed in red color.
- control and display patient's pulse
- control and display patient's breath for syncronization with proton accelerator.


Build instructions:
- to build release version just click on  build.bat
- to build debug version pass "debug" at the command line, for example, "build.bat debug"

After successfull build two programs will be produced:
Programs\Doctor\bin\Release | Debug\DoctorDisplay.exe   (must be run on the physician's machine)
Programs\Patient\bin\Release | Debug\PatientDisplay.exe (must be running on the kreslo's netbook)

Important note:
after each rebuild both programs (on the physician's machine and on the kreslo's netbook)
must be updated because theier multicast UDP communication port is changed. 

DoctorDisplay.exe can be started with the following command line arguments:
  -help                           
  -id Patient ID (patient's identification number in the database)                 
  -dsn ODBC DSN (default="SAMPLE") - identification of database source
  -pname "Patient's Full Name"
  -dname "Doctors's Name"
  -x Region Of Interest(ROI) X
  -y ROI Y
  -w ROI Width
  -f Reference Image file path, in case of working without database (locally)  
  -t Threshold - threshold (0 ... 255) above which the difference will be displayed (default = 50). 

 
Both programs can be configured via DoctorDisplay.exe.config, PatientDisplay.exe.config
The default configure settings are below (they can be changed in the future) 

<?xml version="1.0"?>
<configuration>
  <configSections>
  </configSections>
  <!-- DataBase settings. ODBC connection string  -->
  <connectionStrings>
    <add name="DoctorDisplay.Properties.Settings.ConnectionString"
      connectionString="Dsn=SAMPLE;dbq=P:\BasePTC.mdb;driverid=25;fil=MS Access;maxbuffersize=2048;pagetimeout=5"
      providerName="System.Data.Odbc" />
    <add name="PatientDisplay.Properties.Settings.ConnectionString"
      connectionString="Dsn=BasePTC" providerName="System.Data.Odbc" />
  </connectionStrings>
  <appSettings>
    <!-- network settings -->
    <!-- add key="EndPoint" value="234.9.9.9:5555" -->
    <add key="TcpPort" value="30043" />
    <!-- add key="PulsePort" value="30044" -->
    <add key="Local" value="false" />
    <add key="DataBasePath" value="P:\Head" />
    <add key="ActivateMicrophone" value="true" />

    <!-- video camera settings for PatientDisplay -->
    <add key="RoiX" value="0" />
    <add key="RoiY" value="0" />
    <add key="RoiWidth" value="0" />
    <add key="Threshold" value="50" />
    <add key="P.DShow.VideoCompressor" value="WMVideo9 Encoder DMO" />

    <!-- video camera settings for DoctorDisplay.  -->
    <add key="FlipVideo" value="1" />

    <!-- Serial/COM Port settings for reading pulse data on PatientDisplay--> 
    <add key="PortName" value="COM5" />
    <add key="BaudRate" value="115200" />
    <add key="DataBits" value="8" />
    <add key="Parity" value="0" />
    <add key="StopBits" value="0" />
    <add key="Handshake" value="0" />
    <add key="ClientSettingsProvider.ServiceUri" value="" />

    <!-- display errors with detailed Doctor Watson info -->
    <add key="ShowErrors" value="false" />
    <add key="UdpListenerErrorMsg" value="Произошла ошибка соединения. Проверьте сетевой разъём." />
  </appSettings>
  <startup>
    <supportedRuntime version="v2.0.50727" />
  </startup>
  <system.web>
    <membership defaultProvider="ClientAuthenticationMembershipProvider">
      <providers>
        <add name="ClientAuthenticationMembershipProvider" type="System.Web.ClientServices.Providers.ClientFormsAuthenticationMembershipProvider, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" serviceUri="" />
      </providers>
    </membership>
    <roleManager defaultProvider="ClientRoleProvider" enabled="true">
      <providers>
        <add name="ClientRoleProvider" type="System.Web.ClientServices.Providers.ClientRoleProvider, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" serviceUri="" cacheTimeout="86400" />
      </providers>
    </roleManager>
  </system.web>
</configuration>

