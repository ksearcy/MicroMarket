[1] Prereq
[1.1] .net Point of Service 1.12	[*]
[1.2] SQL Express 2012			[*]
[1.3] OpenDBDiff and Scripts		[*]
[1.4] Knas Restarter(v2.0-20100701)	[*]
[1.5] KMtronics Lock			[*]
[1.6] Remote Client			[*]

[2] Fingerprint				
[2.1] DigitalPersona(UrUSDK231RePackage)[*]

[3] Barcode
[3.1] Honeywell Barcode OPOS(1.13.4.13)	[*]
[3.2] Honeywell Config Util(4.5.20)	[*]
[3.3] Honeywell N600
[3.4] CodeCorp OPOS(4-1-0)		[*]
[3.5] CodeCorp Cortex Tools(2-5-26)	[*]

[4] Credit Card
[4.1] Magtek				[*]
[4.2] Magtek Magensa			[*]
[4.3] Nayax Tester			[*]

[5] Bill And Coin
[5.1] MEI Drivers			[*]
[5.2] MEI API				[*]
[5.3] Coinco USB			[*]
[5.4] iSocket				[*]

[6] Cameras
[6.1]iSpy Connect(6_5_8_0)		[*]

[7] Application
[7.1] App
[7.2] Sync Data
[7.3] ftp

[8] App Setup
[8.1] Customer and Location
  [LocationId]
    <add key="LocationId" value="1"></add>
  [LocationName]
    <add key="LocationName" value="Location Name"></add>
  [CustomerId]
    <add key="CustomerId" value="1"></add>
  [CustomerName]
    <add key="CustomerName" value="Customer Name"></add>
[8.2] Virtual Keyboard
  [Virtual Keyboard]
    <add key="EnableVirtualKeyboard" value="false"></add>
      [true] [false]
[8.3] Service
    <add key="deOROServiceUrl" value="http://209.159.152.234/deOROTestService/SyncDataService.svc"></add>
    <add key="deOROServiceAccessUserName" value="deoro1"></add>
    <add key="deOROServiceAccessPassword" value="deoro1"></add>
[8.4] Credit Card Processor
  [CreditCardProcessor]
    <add key="CreditCardProcessor" value="USAT"></add>
[8.5] App Settings
  [IdleTimeout] Set timeout for shopping cart
    <add key="IdleTimeout" value="180"></add>
  [RunMode]
    <add key="RunMode" value="debug"></add>
      [debug] [release]
  [AutologoffCountdownTimer] Auto logoff for user
    <add key="AutologoffCountdownTimer" value="5"></add>
  [SyncInterval] No longer in use
    <add key="SyncInterval" value="3600"></add>
  [BarcodeLogin] Login with barcode option
    <add key="BarcodeLogin" value="true"></add>
      [true] [false]
  [EnableCameraFeed]
    <add key="EnableCameraFeed" value="true"></add>
      [true] [false]
  [VideosPath]
    <add key="VideosPath" value="C:\Temp\Videos" />
  [CameraCycleInterval]
    <add key="CameraCycleInterval" value="20"></add>
  [Round] Rounding for bill and coin
    <add key="Round" value="0.05" />
  [Enable20WhenAmountIsGreaterOrEqualTo] Remove dispense option if cart is more than
    <add key="Enable20WhenAmountIsGreaterOrEqualTo" value="5" />
  [CommunicationType]
    <add key="CommunicationType" value="MDB" />
    [MDB] [MEI]
  [EnableCoin]
    <add key="EnableCoin" value="true" />
      [true] [false]
  [EnableBill]
    <add key="EnableBill" value="true" />
      [true] [false]
  [AutoCloseMessage] Auto close message popup
    <add key="AutoCloseMessage" value="60" />
  [EnableFingerprintAuthentication]
    <add key="EnableFingerprintAuthentication" value="true" />
      [true] [false]
  [EnableDiscounts]
    <add key="EnableDiscounts" value="true"></add>
      [true] [false]
  [DefaultToNumericKeyPad]
    <add key="DefaultToNumericKeyPad" value="true"></add>
      [true] [false]
  [EmailRequiredToRegister]
    <add key="EmailRequiredToRegister" value="false"></add>
      [true] [false]
  [FirstLastNameRequiredToRegister]
    <add key="FirstLastNameRequiredToRegister" value="true"></add>
      [true] [false]
  [EnableDispenseChange]
    <add key="EnableDispenseChange" value="true"></add>
      [true] [false]
  [NewUserCredit]
    <add key="NewUserCredit" value="0"></add>
  [DemoMode]
    <add key="DemoMode" value="false"></add>
      [true] [false]
[8.6] MDB Settings
  <add key="MDBVendor" value="E2C"></add>
    [Coinco] [E2C]
[8.7] USB Relay
  [KMtronic]
    <add key="KMtronic" value="COM7"></add>
  [KMtronicTimeout]
    <add key="KMtronicTimeout" value="5"></add>
[8.8] Mail
  <add key="SmtpServer" value="smtp.gmail.com" />
  <add key="SmtpPort" value="587"></add>
  <add key="EnableSSL" value="true"></add>
  <add key="SmtpUserName" value="deORO.micro.MARKET@gmail.com"></add>
  <add key="SmtpPassword" value="86@OO2841#!315"></add>
  <add key="FromMailAddress" value="deORO.micro.MARKET@gmail.com"></add>
  <add key="CCAddress" value="rgangavalliiiii@gmail.com"></add>
  <add key="SmtpAuthenticationMode" value="1"></add>
  <add key="ToAddress" value="rk638iii@yahoo.com"></add>
[8.9] Bill
  <add key="COMPort" value="COM6"></add>
  <add key="PUPMode" value="A"></add>
[8.10] Coin
  <add key="COMPort" value="COM1"></add>
[8.11] Card Reader
  <add key="CardReaderMake" value="MagTek"></add>
    [MagTek] [NayaxE2C] [NayaxMarshall] [CardKnox]
  <add key="DeviceName" value="MagTek Msr"></add>
    [MagTek Msr] Service Object name(use SoMgr in folder [4] Credit Car/[4.1] MagTek)
  <add key="SerialNumber" value="K3MTB1C402D"></add>
    [Magtek Serial Number from USAT]
  <add key="CardReaderType" value="1"></add>
    [1]
  <add key="CardType" value="C"></add>
    [C]
  <add key="Encoding" value="Hex"></add>
    [Hex] [ASCII]
  <add key="CardReaderCOMPort" value="9"></add>
[8.12] Card Processor
  <add key="CardProcessorCompanyName" value="USAT"></add>
    [USAT] [CardKnox] [Nayax]
  <add key="UserName" value="usat"></add> USAT username
    [usat]
  <add key="Password" value="JyCak+Q29w^9S29k9afR"></add> USAT password
  <add key="Url" value="https://ec.usatech.com:9443/soap/ec"></add>
    [https://ec.usatech.com:9443/soap/ec]
  <add key="HeartlandSecretApiKey" value="skapi_cert_MVuOAQAY7VQAKJu84gNielXs0n90xKD-fZu3s5PPzQ"></add>
    [skapi_cert_MVuOAQAY7VQAKJu84gNielXs0n90xKD-fZu3s5PPzQ]
  <add key="ZipCodeRequired" value="false"></add>
      [true] [false]    
  <add key="CardProcessorTimeout" value="30"></add>
  <add key="IP" value="192.168.0.117"></add>
  <add key="IPPort" value="9999"></add>
    [9999]
  <add key="CardKnoxKey" value="DeoroMarketsDEV_Test"></add>
    [DeoroMarketsDEV_Test]
  <add key="ApiVersion" value="4.5.4"></add>
[8.13] Barcode Reader
  <add key="BarcodeReaderDeviceMake" value="Honeywell"></add>
    [Honeywell] [Code]
  <add key="DeviceName" value="3310g"></add> Service Object SoMgr to check name
    [3310g] [Code]
[8.14] Camera Feed (rtsp streams)
  <add key="Camera1" value=""></add>
  <add key="Camera2" value=""></add>
  <add key="Camera3" value=""></add>
  <add key="Camera4" value=""></add>
[8.15] Refill Rewards
  <add key="EnableRefillRewards" value="false"></add>
  <add key="1-4.99" value="0.5"></add>
  <add key="5-9.99" value="1"></add>
  <add key="10-19.99" value="1.5"></add>
  <add key="20+" value="2"></add>
[8.16]
  <add key="COMPort" value="COM8"></add>
  <add key="HardwareId" value="FTDIBUS\COMPORT&amp;VID_0403&amp;PID_6001"></add>
[8.17] Dispense
  <add key="BillDispenser" value="false"></add>
    [true] [false]
  <add key="CoinHopper" value="false"></add>
    [true] [false]
  <add key="RecycleNote" value="1USD,5USD"></add>
    [1USD,5USD] [5USD,10USD]
[8.18] Connection String
  <add name="deOROEntities" connectionString="metadata=res://*/deOROModel.csdl|res://*/deOROModel.ssdl|res://*/deOROModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=.;initial catalog=deORO_Local;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
    [source=.;initial] [catalog=deORO_Local] [integrated security=True]
[8.19] Do not touch beyond this point. App will break

[9] SyncData
[9.1] Application
  [LocationId]
    <add key="LocationId" value="1"></add>
  [LocationName]
    <add key="LocationName" value="Location Name"></add>
  [CustomerId]
    <add key="CustomerId" value="1"></add>
  [CustomerName]
    <add key="CustomerName" value="Customer Name"></add>
  [deOROServiceUrl]
    <add key="deOROServiceUrl" value="http://209.159.152.234/deOROTestService/SyncDataService.svc"></add>
  [deOROServiceAccessUserName]
    <add key="deOROServiceAccessUserName" value="deoro16"></add>
  [deOROServiceAccessPassword]
    <add key="deOROServiceAccessPassword" value="deoro16"></add>
  [UserSharedAcrosssLocations]
    <add key="UserSharedAcrosssLocations" value="true"></add>
9.2 Connection String
    <add name="deOROEntities" connectionString="metadata=res://*/deOROModel.csdl|res://*/deOROModel.ssdl|res://*/deOROModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=.;initial catalog=deORO_Local;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />

Updater
[]Backup Local sql db
[]Backup App
[]Backup SyncData
[]Backup Isocket
[]FTP Download App
[]FTP Download Sync Data
[]App new keys
[]Import keys from backup
[]Run SyncData
 
